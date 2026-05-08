using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Services
{
    public class InMemoryDataService : IDataService
    {
        private static List<Products> products = new();
        private static List<Categories> categories = new();
        private static List<Brands> brands = new();
        private static List<Users> users = new();
        private static int productId = 1;
        private static int categoryId = 1;
        private static int brandId = 1;
        private static int userId = 1;

        public InMemoryDataService()
        {
            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            if (categories.Count == 0)
            {
                categories.Add(new Categories { CategoryID = categoryId++, CategoryName = "Laptops", Description = "Laptop devices" });
                categories.Add(new Categories { CategoryID = categoryId++, CategoryName = "Accessories", Description = "Laptop accessories" });

                brands.Add(new Brands { BrandID = brandId++, BrandName = "TechCore", BrandLogo = "techcore.png" });
                brands.Add(new Brands { BrandID = brandId++, BrandName = "NovaGear", BrandLogo = "novagear.png" });

                products.Add(new Products
                {
                    ProductID = productId++,
                    ProductName = "Laptop Pro 15",
                    BasePrice = 1499,
                    StockQuantity = 10,
                    CategoryID = 1,
                    BrandID = 1,
                    ProductImage = "laptop-pro-15.png",
                    Status = "Active",
                    ProductDetails = new ProductDetails
                    {
                        CPU = "Intel Core i7",
                        RAM = "16GB DDR5",
                        GPU = "Intel Iris Xe",
                        Display = "15.6\" FHD",
                        Storage = "512GB NVMe SSD",
                        Battery = "10 hours",
                        Weight = "1.7 kg",
                        OS = "Windows 11"
                    }
                });
                products.Add(new Products
                {
                    ProductID = productId++,
                    ProductName = "Ultrabook Air",
                    BasePrice = 1199,
                    StockQuantity = 12,
                    CategoryID = 1,
                    BrandID = 2,
                    ProductImage = "ultrabook-air.png",
                    Status = "Active",
                    ProductDetails = new ProductDetails
                    {
                        CPU = "Intel Core i5",
                        RAM = "8GB DDR5",
                        GPU = "Intel UHD",
                        Display = "14\" FHD",
                        Storage = "256GB NVMe SSD",
                        Battery = "12 hours",
                        Weight = "1.2 kg",
                        OS = "Windows 11"
                    }
                });
                products.Add(new Products
                {
                    ProductID = productId++,
                    ProductName = "Wireless Mouse",
                    BasePrice = 39,
                    StockQuantity = 40,
                    CategoryID = 2,
                    BrandID = 1,
                    ProductImage = "wireless-mouse.png",
                    Status = "Active"
                });
                products.Add(new Products
                {
                    ProductID = productId++,
                    ProductName = "USB-C Hub",
                    BasePrice = 59,
                    StockQuantity = 30,
                    CategoryID = 2,
                    BrandID = 2,
                    ProductImage = "usb-c-hub.png",
                    Status = "Active"
                });

                users.Add(new Users
                {
                    UserID = userId++,
                    Username = "admin",
                    PasswordHash = "admin",
                    FullName = "Administrator",
                    Email = "admin@example.com",
                    PhoneNumber = "0123456789",
                    Role = "Admin"
                });
                users.Add(new Users
                {
                    UserID = userId++,
                    Username = "user1",
                    PasswordHash = "user123",
                    FullName = "John Doe",
                    Email = "user1@example.com",
                    PhoneNumber = "0987654321",
                    Role = "Customer"
                });
            }
        }

        // Products
        public Task<List<Products>> GetAllProductsAsync() => Task.FromResult(products);
        public Task<List<Products>> GetProductsPageAsync(int pageNumber, int pageSize)
        {
            var items = products
                .OrderBy(p => p.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(items);
        }

        public Task<int> GetProductCountAsync() => Task.FromResult(products.Count);
        public Task<Products> GetProductByIdAsync(int id) => Task.FromResult(products.FirstOrDefault(p => p.ProductID == id));

        public Task AddProductAsync(Products product)
        {
            product.ProductID = productId++;
            products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateProductAsync(Products product)
        {
            var existing = products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existing != null)
            {
                existing.ProductName = product.ProductName;
                existing.BasePrice = product.BasePrice;
                existing.StockQuantity = product.StockQuantity;
                existing.CategoryID = product.CategoryID;
                existing.BrandID = product.BrandID;
                existing.ProductImage = product.ProductImage;
                existing.Status = product.Status;
            }
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductID == id);
            if (product != null)
                products.Remove(product);
            return Task.CompletedTask;
        }

        // Categories
        public Task<List<Categories>> GetAllCategoriesAsync() => Task.FromResult(categories);
        public Task<Categories> GetCategoryByIdAsync(int id) => Task.FromResult(categories.FirstOrDefault(c => c.CategoryID == id));

        public Task AddCategoryAsync(Categories category)
        {
            category.CategoryID = categoryId++;
            categories.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateCategoryAsync(Categories category)
        {
            var existing = categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);
            if (existing != null)
            {
                existing.CategoryName = category.CategoryName;
                existing.Description = category.Description;
                existing.Icon = category.Icon;
                existing.Slug = category.Slug;
                existing.ParentID = category.ParentID;
            }
            return Task.CompletedTask;
        }

        public Task DeleteCategoryAsync(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryID == id);
            if (category != null)
                categories.Remove(category);
            return Task.CompletedTask;
        }

        // Suppliers
        public Task<List<Brands>> GetAllBrandsAsync() => Task.FromResult(brands);
        public Task<Brands> GetBrandByIdAsync(int id) => Task.FromResult(brands.FirstOrDefault(b => b.BrandID == id));

        public Task AddBrandAsync(Brands brand)
        {
            brand.BrandID = brandId++;
            brands.Add(brand);
            return Task.CompletedTask;
        }

        public Task UpdateBrandAsync(Brands brand)
        {
            var existing = brands.FirstOrDefault(b => b.BrandID == brand.BrandID);
            if (existing != null)
            {
                existing.BrandName = brand.BrandName;
                existing.BrandLogo = brand.BrandLogo;
            }
            return Task.CompletedTask;
        }

        public Task DeleteBrandAsync(int id)
        {
            var brand = brands.FirstOrDefault(b => b.BrandID == id);
            if (brand != null)
                brands.Remove(brand);
            return Task.CompletedTask;
        }

        // Users
        public Task<List<Users>> GetAllUsersAsync() => Task.FromResult(users);
        public Task<Users> GetUserByIdAsync(int id) => Task.FromResult(users.FirstOrDefault(a => a.UserID == id));
        public Task<Users> GetUserByUserNameAsync(string userName) => Task.FromResult(users.FirstOrDefault(a => a.Username == userName));

        public Task AddUserAsync(Users user)
        {
            user.UserID = userId++;
            user.CreatedAt = DateTime.UtcNow;
            users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateUserAsync(Users user)
        {
            var existing = users.FirstOrDefault(a => a.UserID == user.UserID);
            if (existing != null)
            {
                existing.Username = user.Username;
                existing.FullName = user.FullName;
                existing.PasswordHash = user.PasswordHash;
                existing.Email = user.Email;
                existing.PhoneNumber = user.PhoneNumber;
                existing.Address = user.Address;
                existing.Role = user.Role;
            }
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(int id)
        {
            var user = users.FirstOrDefault(a => a.UserID == id);
            if (user != null)
                users.Remove(user);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateLoginAsync(string userName, string password)
        {
            var user = users.FirstOrDefault(a => a.Username == userName && a.PasswordHash == password);
            return Task.FromResult(user != null);
        }
    }
}
