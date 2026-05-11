using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data; // Đảm bảo đúng namespace chứa DbContext của bạn
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;

        public DataService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Products ---
        public async Task<List<Products>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductDetails)
                .ToListAsync();
        }

        public async Task<List<Products>> GetProductsPageAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetProductCountAsync() => await _context.Products.CountAsync();

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task AddProductAsync(Products product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Products product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // --- Categories ---
        public async Task<List<Categories>> GetAllCategoriesAsync() => await _context.Categories.ToListAsync();
        public async Task<Categories> GetCategoryByIdAsync(int id) => await _context.Categories.FindAsync(id);

        public async Task AddCategoryAsync(Categories category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Categories category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        // --- Brands ---
        public async Task<List<Brands>> GetAllBrandsAsync() => await _context.Brands.ToListAsync();
        public async Task<Brands> GetBrandByIdAsync(int id) => await _context.Brands.FindAsync(id);

        public async Task AddBrandAsync(Brands brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBrandAsync(Brands brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        // --- Users ---
        public async Task<List<Users>> GetAllUsersAsync() => await _context.Users.ToListAsync();
        public async Task<Users> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);
        public async Task<Users> GetUserByUserNameAsync(string userName) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Users user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateLoginAsync(string userName, string password)
        {
            return await _context.Users.AnyAsync(u => u.Username == userName && u.PasswordHash == password);
        }
    }
}
