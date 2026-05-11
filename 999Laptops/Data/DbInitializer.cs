using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.Data;
using Microsoft.EntityFrameworkCore;

namespace LaptopEcommerceAndMore.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Đảm bảo Database đã được tạo
            context.Database.EnsureCreated();

            // Nếu đã có sản phẩm thì không chạy Seed để tránh trùng lặp
            if (context.Products.Any())
            {
                return;
            }

            // 1. Thêm Brands (Thương hiệu)
            var brands = new List<Brands>
            {
                new Brands { BrandName = "ASUS", BrandLogo = "asus_logo.png", ContactInfo = "support@asus.com", Address = "Taiwan" },
                new Brands { BrandName = "Dell", BrandLogo = "dell_logo.png", ContactInfo = "contact@dell.com", Address = "USA" },
                new Brands { BrandName = "Logitech", BrandLogo = "logitech_logo.png", ContactInfo = "sales@logitech.com", Address = "Switzerland" },
                new Brands { BrandName = "Razer", BrandLogo = "razer_logo.png", ContactInfo = "info@razer.com", Address = "USA" },
                new Brands { BrandName = "Apple", BrandLogo = "apple_logo.png", ContactInfo = "support@apple.com", Address = "USA" }
            };
            context.Brands.AddRange(brands);
            context.SaveChanges();

            // 2. Thêm Categories (Danh mục)
            var catLaptop = new Categories { CategoryName = "Laptops", Slug = "laptops", Icon = "fa-laptop" };
            var catAccessory = new Categories { CategoryName = "Accessories", Slug = "accessories", Icon = "fa-mouse" };
            context.Categories.AddRange(catLaptop, catAccessory);
            context.SaveChanges();

            var catGaming = new Categories { CategoryName = "Gaming Laptops", Slug = "gaming-laptops", ParentID = catLaptop.CategoryId };
            var catOffice = new Categories { CategoryName = "Office Laptops", Slug = "office-laptops", ParentID = catLaptop.CategoryId };
            var catMouse = new Categories { CategoryName = "Mouse & Keyboard", Slug = "mouse-keyboard", ParentID = catAccessory.CategoryId };
            context.Categories.AddRange(catGaming, catOffice, catMouse);
            context.SaveChanges();

            // 3. Thêm Products (3 Laptop, 2 Phụ kiện)
            var p1 = new Products
            {
                ProductName = "ROG Zephyrus G14",
                BrandID = brands[0].BrandId,
                CategoryID = catGaming.CategoryId,
                BasePrice = 1500,
                StockQuantity = 10,
                ProductImage = "g14.jpg",
                Status = "Available",
                Description = "Quái vật gaming nhỏ gọn."
            };
            var p2 = new Products
            {
                ProductName = "Dell XPS 13",
                BrandID = brands[1].BrandId,
                CategoryID = catOffice.CategoryId,
                BasePrice = 1200,
                StockQuantity = 15,
                ProductImage = "xps13.jpg",
                Status = "Available",
                Description = "Đẳng cấp văn phòng."
            };
            var p3 = new Products
            {
                ProductName = "MacBook Pro M3",
                BrandID = brands[4].BrandId,
                CategoryID = catOffice.CategoryId,
                BasePrice = 2000,
                StockQuantity = 5,
                ProductImage = "mbp.jpg",
                Status = "Available",
                Description = "Sức mạnh từ chip M3."
            };
            var p4 = new Products
            {
                ProductName = "Logitech G Pro X",
                BrandID = brands[2].BrandId,
                CategoryID = catMouse.CategoryId,
                BasePrice = 150,
                StockQuantity = 50,
                ProductImage = "mouse.jpg",
                Status = "Available",
                Description = "Chuột gaming không dây."
            };
            var p5 = new Products
            {
                ProductName = "Razer BlackWidow",
                BrandID = brands[3].BrandId,
                CategoryID = catMouse.CategoryId,
                BasePrice = 120,
                StockQuantity = 30,
                ProductImage = "keyboard.jpg",
                Status = "Available",
                Description = "Bàn phím cơ huyền thoại."
            };

            context.Products.AddRange(p1, p2, p3, p4, p5);
            context.SaveChanges();

            // 4. Thêm ProductDetails (Chỉ dành cho 3 Laptop)
            context.ProductDetails.AddRange(
                new ProductDetails
                {
                    ProductID = p1.ProductID,
                    CPU = "AMD Ryzen 9 8945HS",
                    RAM = "16GB DDR5",
                    GPU = "NVIDIA RTX 4060",
                    Display = "14 inch OLED 3K",
                    Storage = "1TB SSD",
                    Battery = "76Wh",
                    Weight = "1.5kg",
                    OS = "Windows 11"
                },
                new ProductDetails
                {
                    ProductID = p2.ProductID,
                    CPU = "Intel Core i7-1365U",
                    RAM = "16GB LPDDR5",
                    GPU = "Intel Iris Xe",
                    Display = "13.4 inch FHD+",
                    Storage = "512GB SSD",
                    Battery = "51Wh",
                    Weight = "1.2kg",
                    OS = "Windows 11"
                },
                new ProductDetails
                {
                    ProductID = p3.ProductID,
                    CPU = "Apple M3 Chip",
                    RAM = "8GB Unified",
                    GPU = "10-Core GPU",
                    Display = "14.2 inch Liquid Retina",
                    Storage = "512GB SSD",
                    Battery = "70Wh",
                    Weight = "1.6kg",
                    OS = "macOS"
                }
            );
            context.SaveChanges();
        }
    }
}