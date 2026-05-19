using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.Data;
using Microsoft.EntityFrameworkCore;

namespace LaptopEcommerceAndMore.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Apply migrations and ensure database exists
            context.Database.Migrate();

            // Exit if data already exists
            if (context.Products.Any()) return;

            // 1. Seed Brands (Based on your Brand Model)
            var asus = new Brands { BrandName = "ASUS", BrandLogo = "asus_logo.png", ContactInfo = "support@asus.com", Address = "Taiwan" };
            var dell = new Brands { BrandName = "Dell", BrandLogo = "dell_logo.png", ContactInfo = "contact@dell.com", Address = "USA" };
            var logi = new Brands { BrandName = "Logitech", BrandLogo = "logitech_logo.png", ContactInfo = "sales@logitech.com", Address = "Switzerland" };
            var razer = new Brands { BrandName = "Razer", BrandLogo = "razer_logo.png", ContactInfo = "info@razer.com", Address = "USA" };
            var apple = new Brands { BrandName = "Apple", BrandLogo = "apple_logo.png", ContactInfo = "support@apple.com", Address = "USA" };

            context.Brands.AddRange(asus, dell, logi, razer, apple);

            // 2. Seed Categories (Including the required Description field)
            var catLaptop = new Categories
            {
                CategoryName = "Laptops",
                Slug = "laptops",
                Icon = "fa-laptop",
                Description = "High-performance portable computers"
            };
            var catAccessory = new Categories
            {
                CategoryName = "Accessories",
                Slug = "accessories",
                Icon = "fa-mouse",
                Description = "Essential computer peripherals"
            };

            context.Categories.AddRange(catLaptop, catAccessory);

            // Save to ensure Parent Categories have IDs
            context.SaveChanges();

            var catGaming = new Categories
            {
                CategoryName = "Gaming Laptops",
                Slug = "gaming-laptops",
                ParentCategory = catLaptop,
                Description = "Laptops designed for high-end gaming",
                Icon = "fa-gamepad"
            };
            var catOffice = new Categories
            {
                CategoryName = "Office Laptops",
                Slug = "office-laptops",
                ParentCategory = catLaptop,
                Description = "Reliable laptops for daily work",
                Icon = "fa-briefcase"
            };
            var catMouse = new Categories
            {
                CategoryName = "Mouse & Keyboard",
                Slug = "mouse-keyboard",
                ParentCategory = catAccessory,
                Description = "Input devices for your setup",
                Icon = "fa-keyboard"
            };

            context.Categories.AddRange(catGaming, catOffice, catMouse);
            context.SaveChanges();

            // 3. Seed Products
            var p1 = new Products
            {
                ProductName = "ROG Zephyrus G14",
                Brand = asus,
                Category = catGaming,
                BasePrice = 1500,
                StockQuantity = 10,
                ProductImage = "g14.jpg",
                Status = "Available",
                Description = "Powerful 14-inch gaming laptop."
            };
            var p2 = new Products
            {
                ProductName = "Dell XPS 13",
                Brand = dell,
                Category = catOffice,
                BasePrice = 1200,
                StockQuantity = 15,
                ProductImage = "xps13.jpg",
                Status = "Available",
                Description = "Premium ultrabook for professionals."
            };
            var p3 = new Products
            {
                ProductName = "MacBook Pro M3",
                Brand = apple,
                Category = catOffice,
                BasePrice = 2000,
                StockQuantity = 5,
                ProductImage = "mbp.jpg",
                Status = "Available",
                Description = "Next-generation Apple Silicon performance."
            };
            var p4 = new Products
            {
                ProductName = "Logitech G Pro X",
                Brand = logi,
                Category = catMouse,
                BasePrice = 150,
                StockQuantity = 50,
                ProductImage = "mouse.jpg",
                Status = "Available",
                Description = "Pro-grade wireless gaming mouse."
            };

            context.Products.AddRange(p1, p2, p3, p4);
            context.SaveChanges();

            // 4. Seed ProductDetails (1:1 Relationship)
            var details = new List<ProductDetails>
            {
                new ProductDetails
                {
                    Product = p1,
                    CPU = "AMD Ryzen 9", RAM = "16GB DDR5", GPU = "RTX 4060",
                    Display = "14 inch OLED", Storage = "1TB SSD", Battery = "76Wh",
                    Weight = "1.5kg", OS = "Windows 11", Extra = "Backlit Keyboard"
                },
                new ProductDetails
                {
                    Product = p2,
                    CPU = "Intel Core i7", RAM = "16GB LPDDR5", GPU = "Intel Iris Xe",
                    Display = "13.4 inch FHD+", Storage = "512GB SSD", Battery = "51Wh",
                    Weight = "1.2kg", OS = "Windows 11", Extra = "InfinityEdge Display"
                },
                new ProductDetails
                {
                    Product = p3,
                    CPU = "Apple M3 Chip", RAM = "8GB Unified", GPU = "10-Core GPU",
                    Display = "14.2 inch Liquid Retina", Storage = "512GB SSD", Battery = "70Wh",
                    Weight = "1.6kg", OS = "macOS", Extra = "Touch ID"
                }
            };

            context.ProductDetails.AddRange(details);
            context.SaveChanges();
        }
    }
}