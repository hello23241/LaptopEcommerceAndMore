using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Services
{
    public class InMemoryDataService : IDataService
    {
        private static List<Product> products = new();
        private static List<Category> categories = new();
        private static List<Supplier> suppliers = new();
        private static List<Account> accounts = new();
        private static int productId = 1;
        private static int categoryId = 1;
        private static int supplierId = 1;
        private static int accountId = 1;

        public InMemoryDataService()
        {
            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            if (categories.Count == 0)
            {
                categories.Add(new Category { Id = categoryId++, Name = "Electronics", Description = "Electronic devices" });
                categories.Add(new Category { Id = categoryId++, Name = "Books", Description = "Books and publications" });
                categories.Add(new Category { Id = categoryId++, Name = "Clothing", Description = "Clothing items" });

                suppliers.Add(new Supplier { Id = supplierId++, Name = "Supplier A", ContactInfo = "contact@suppliera.com", Address = "123 Street A" });
                suppliers.Add(new Supplier { Id = supplierId++, Name = "Supplier B", ContactInfo = "contact@supplierb.com", Address = "456 Street B" });
                suppliers.Add(new Supplier { Id = supplierId++, Name = "Supplier C", ContactInfo = "contact@supplierc.com", Address = "789 Street C" });

                products.Add(new Product { Id = productId++, Name = "Laptop Pro 15", Description = "High-performance laptop", Price = 1499, Quantity = 10, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "Ultrabook Air", Description = "Lightweight productivity laptop", Price = 1199, Quantity = 12, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "Gaming Beast", Description = "High-refresh gaming laptop", Price = 1899, Quantity = 7, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "Creator Studio", Description = "Content creation workstation", Price = 2099, Quantity = 5, CategoryId = 1, SupplierId = 2 });
                products.Add(new Product { Id = productId++, Name = "Business Elite", Description = "Secure business laptop", Price = 1399, Quantity = 8, CategoryId = 1, SupplierId = 2 });
                products.Add(new Product { Id = productId++, Name = "Student Starter", Description = "Affordable everyday laptop", Price = 699, Quantity = 18, CategoryId = 1, SupplierId = 3 });
                products.Add(new Product { Id = productId++, Name = "Convertible Flex", Description = "2-in-1 touchscreen laptop", Price = 999, Quantity = 9, CategoryId = 1, SupplierId = 3 });
                products.Add(new Product { Id = productId++, Name = "Travel Mate", Description = "Compact travel laptop", Price = 849, Quantity = 14, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "Office Essential", Description = "Reliable office laptop", Price = 899, Quantity = 16, CategoryId = 1, SupplierId = 2 });
                products.Add(new Product { Id = productId++, Name = "Premium OLED", Description = "Vivid OLED display laptop", Price = 1599, Quantity = 6, CategoryId = 1, SupplierId = 3 });
                products.Add(new Product { Id = productId++, Name = "Budget Boost", Description = "Entry-level performance", Price = 599, Quantity = 20, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "Performance Max", Description = "Balanced power and battery", Price = 1299, Quantity = 11, CategoryId = 1, SupplierId = 2 });

                // Initialize sample accounts
                accounts.Add(new Account { Id = accountId++, UserName = "admin", FullName = "Administrator", Password = "admin", Email = "admin@example.com", Phone = "0123456789", Status = 1 });
                accounts.Add(new Account { Id = accountId++, UserName = "user1", FullName = "John Doe", Password = "user123", Email = "user1@example.com", Phone = "0987654321", Status = 1 });
                accounts.Add(new Account { Id = accountId++, UserName = "user2", FullName = "Jane Smith", Password = "user456", Email = "user2@example.com", Phone = "0918273645", Status = 1 });
            }
        }

        // Products
        public Task<List<Product>> GetAllProductsAsync() => Task.FromResult(products);
        public Task<List<Product>> GetProductsPageAsync(int pageNumber, int pageSize)
        {
            var items = products
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(items);
        }

        public Task<int> GetProductCountAsync() => Task.FromResult(products.Count);
        public Task<Product> GetProductByIdAsync(int id) => Task.FromResult(products.FirstOrDefault(p => p.Id == id));

        public Task AddProductAsync(Product product)
        {
            product.Id = productId++;
            products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateProductAsync(Product product)
        {
            var existing = products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Quantity = product.Quantity;
                existing.CategoryId = product.CategoryId;
                existing.SupplierId = product.SupplierId;
            }
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                products.Remove(product);
            return Task.CompletedTask;
        }

        // Categories
        public Task<List<Category>> GetAllCategoriesAsync() => Task.FromResult(categories);
        public Task<Category> GetCategoryByIdAsync(int id) => Task.FromResult(categories.FirstOrDefault(c => c.Id == id));

        public Task AddCategoryAsync(Category category)
        {
            category.Id = categoryId++;
            categories.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateCategoryAsync(Category category)
        {
            var existing = categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
            }
            return Task.CompletedTask;
        }

        public Task DeleteCategoryAsync(int id)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
                categories.Remove(category);
            return Task.CompletedTask;
        }

        // Suppliers
        public Task<List<Supplier>> GetAllSuppliersAsync() => Task.FromResult(suppliers);
        public Task<Supplier> GetSupplierByIdAsync(int id) => Task.FromResult(suppliers.FirstOrDefault(s => s.Id == id));

        public Task AddSupplierAsync(Supplier supplier)
        {
            supplier.Id = supplierId++;
            suppliers.Add(supplier);
            return Task.CompletedTask;
        }

        public Task UpdateSupplierAsync(Supplier supplier)
        {
            var existing = suppliers.FirstOrDefault(s => s.Id == supplier.Id);
            if (existing != null)
            {
                existing.Name = supplier.Name;
                existing.ContactInfo = supplier.ContactInfo;
                existing.Address = supplier.Address;
            }
            return Task.CompletedTask;
        }

        public Task DeleteSupplierAsync(int id)
        {
            var supplier = suppliers.FirstOrDefault(s => s.Id == id);
            if (supplier != null)
                suppliers.Remove(supplier);
            return Task.CompletedTask;
        }

        // Accounts
        public Task<List<Account>> GetAllAccountsAsync() => Task.FromResult(accounts);
        public Task<Account> GetAccountByIdAsync(int id) => Task.FromResult(accounts.FirstOrDefault(a => a.Id == id));
        public Task<Account> GetAccountByUserNameAsync(string userName) => Task.FromResult(accounts.FirstOrDefault(a => a.UserName == userName));

        public Task AddAccountAsync(Account account)
        {
            account.Id = accountId++;
            account.CreatedDate = DateTime.Now;
            accounts.Add(account);
            return Task.CompletedTask;
        }

        public Task UpdateAccountAsync(Account account)
        {
            var existing = accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existing != null)
            {
                existing.UserName = account.UserName;
                existing.FullName = account.FullName;
                existing.Password = account.Password;
                existing.Email = account.Email;
                existing.Phone = account.Phone;
                existing.Birthday = account.Birthday;
                existing.Status = account.Status;
                existing.Notes = account.Notes;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAccountAsync(int id)
        {
            var account = accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
                accounts.Remove(account);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateLoginAsync(string userName, string password)
        {
            var account = accounts.FirstOrDefault(a => a.UserName == userName && a.Password == password && a.Status == 1);
            return Task.FromResult(account != null);
        }
    }
}
