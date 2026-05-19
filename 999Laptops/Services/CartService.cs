using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.ViewModels;

namespace LaptopEcommerceAndMore.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "ShoppingCart_";

        // Inject ApplicationDbContext vào Service để làm việc trực tiếp với SQL Server
        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ĐÃ SỬA: Đọc dữ liệu trực tiếp từ Database Azure theo UserID ---
        public async Task<ShoppingCart> GetCartAsync(HttpContext httpContext, int accountId)
        {
            // 1. Tạo một đối tượng ShoppingCart rỗng để chuẩn bị chứa dữ liệu trả về giao diện
            var shoppingCart = new ShoppingCart
            {
                UserID = accountId,
                Items = new List<CartItem>(),
                TotalItems = 0,
                TotalPrice = 0m
            };

            // Nếu chưa đăng nhập (accountId == 0), trả về giỏ hàng trống ngay lập tức
            if (accountId <= 0) return shoppingCart;

            // 2. Truy vấn dữ liệu từ bảng Cart thật trong database, kèm thông tin bảng Products liên kết
            var dbCartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserID == accountId)
                .ToListAsync();

            // 3. Đổ (Map) dữ liệu từ thực thể bảng Cart trong database sang danh sách CartItem của ViewModel
            foreach (var dbItem in dbCartItems)
            {
                if (dbItem.Product != null)
                {
                    var cartItem = new CartItem
                    {
                        ProductId = dbItem.ProductID,
                        ProductName = dbItem.Product.ProductName,
                        ProductImage = dbItem.Product.ProductImage,
                        Price = dbItem.Product.BasePrice,
                        Quantity = dbItem.Quantity,
                        Subtotal = dbItem.Product.BasePrice * dbItem.Quantity
                    };
                    shoppingCart.Items.Add(cartItem);
                }
            }

            // 4. Tính toán tổng số lượng và tổng tiền thực tế từ DB để hiển thị ở trang Cart
            shoppingCart.TotalItems = shoppingCart.Items.Sum(x => x.Quantity);
            shoppingCart.TotalPrice = shoppingCart.Items.Sum(x => x.Subtotal);

            return shoppingCart;
        }

        // --- HANDLER: Các hàm xử lý Session phụ trợ cũ giữ nguyên cấu trúc để tránh lỗi biên dịch hệ thống ---
        public async Task AddToCartAsync(HttpContext httpContext, int accountId, CartItem item)
        {
            var cart = await GetCartAsync(httpContext, accountId);
            await AddItemAsync(cart, item);
            await UpdateCartAsync(httpContext, accountId, cart);
        }

        public async Task RemoveFromCartAsync(HttpContext httpContext, int accountId, int productId)
        {
            var cart = await GetCartAsync(httpContext, accountId);
            await RemoveItemAsync(cart, productId);
            await UpdateCartAsync(httpContext, accountId, cart);
        }

        public Task UpdateCartAsync(HttpContext httpContext, int accountId, ShoppingCart cart)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(key, cartJson);
            return Task.CompletedTask;
        }

        public Task ClearCartAsync(HttpContext httpContext, int accountId)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            session.Remove(key);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(ShoppingCart cart, CartItem item)
        {
            item.Subtotal = item.Price * item.Quantity;
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                existingItem.Subtotal = existingItem.Price * existingItem.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(ShoppingCart cart, int productId)
        {
            cart.Items.RemoveAll(x => x.ProductId == productId);
            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            return Task.CompletedTask;
        }

        public Task UpdateQuantityAsync(ShoppingCart cart, int productId, int quantity)
        {
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.RemoveAll(x => x.ProductId == productId);
                }
                else
                {
                    item.Quantity = quantity;
                    item.Subtotal = item.Price * item.Quantity;
                }
            }

            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            return Task.CompletedTask;
        }

        public Task ClearAsync(ShoppingCart cart)
        {
            cart.Items.Clear();
            cart.TotalItems = 0;
            cart.TotalPrice = 0m;
            return Task.CompletedTask;
        }
    }
}