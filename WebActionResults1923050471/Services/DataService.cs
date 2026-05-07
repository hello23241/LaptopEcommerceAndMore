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

                products.Add(new Product { Id = productId++, Name = "Laptop", Description = "High-performance laptop", Price = 1200, Quantity = 10, CategoryId = 1, SupplierId = 1 });
                products.Add(new Product { Id = productId++, Name = "C# Programming", Description = "Learn C# from basics", Price = 35, Quantity = 50, CategoryId = 2, SupplierId = 2 });
                products.Add(new Product { Id = productId++, Name = "T-Shirt", Description = "Comfortable cotton t-shirt", Price = 25, Quantity = 100, CategoryId = 3, SupplierId = 3 });

                // Initialize sample accounts
                accounts.Add(new Account { Id = accountId++, UserName = "admin", FullName = "Administrator", Password = "nimda", Email = "admin@example.com", Phone = "0123456789", Status = 1 });
                accounts.Add(new Account { Id = accountId++, UserName = "user1", FullName = "John Doe", Password = "user123", Email = "user1@example.com", Phone = "0987654321", Status = 1 });
                accounts.Add(new Account { Id = accountId++, UserName = "user2", FullName = "Jane Smith", Password = "user456", Email = "user2@example.com", Phone = "0918273645", Status = 1 });
            }
        }

        // Products
        public List<Product> GetAllProducts() => products;
        public Product GetProductById(int id) => products.FirstOrDefault(p => p.Id == id);

        public void AddProduct(Product product)
        {
            product.Id = productId++;
            products.Add(product);
        }

        public void UpdateProduct(Product product)
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
        }

        public void DeleteProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                products.Remove(product);
        }

        // Categories
        public List<Category> GetAllCategories() => categories;
        public Category GetCategoryById(int id) => categories.FirstOrDefault(c => c.Id == id);

        public void AddCategory(Category category)
        {
            category.Id = categoryId++;
            categories.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            var existing = categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
            }
        }

        public void DeleteCategory(int id)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
                categories.Remove(category);
        }

        // Suppliers
        public List<Supplier> GetAllSuppliers() => suppliers;
        public Supplier GetSupplierById(int id) => suppliers.FirstOrDefault(s => s.Id == id);

        public void AddSupplier(Supplier supplier)
        {
            supplier.Id = supplierId++;
            suppliers.Add(supplier);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            var existing = suppliers.FirstOrDefault(s => s.Id == supplier.Id);
            if (existing != null)
            {
                existing.Name = supplier.Name;
                existing.ContactInfo = supplier.ContactInfo;
                existing.Address = supplier.Address;
            }
        }

        public void DeleteSupplier(int id)
        {
            var supplier = suppliers.FirstOrDefault(s => s.Id == id);
            if (supplier != null)
                suppliers.Remove(supplier);
        }

        // Accounts
        public List<Account> GetAllAccounts() => accounts;
        public Account GetAccountById(int id) => accounts.FirstOrDefault(a => a.Id == id);
        public Account GetAccountByUserName(string userName) => accounts.FirstOrDefault(a => a.UserName == userName);

        public void AddAccount(Account account)
        {
            account.Id = accountId++;
            account.CreatedDate = DateTime.Now;
            accounts.Add(account);
        }

        public void UpdateAccount(Account account)
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
        }

        public void DeleteAccount(int id)
        {
            var account = accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
                accounts.Remove(account);
        }

        public bool ValidateLogin(string userName, string password)
        {
            var account = accounts.FirstOrDefault(a => a.UserName == userName && a.Password == password && a.Status == 1);
            return account != null;
        }
    }
}
