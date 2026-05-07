using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Interfaces
{
    public interface IDataService
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);

        List<Category> GetAllCategories();
        Category GetCategoryById(int id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);

        List<Supplier> GetAllSuppliers();
        Supplier GetSupplierById(int id);
        void AddSupplier(Supplier supplier);
        void UpdateSupplier(Supplier supplier);
        void DeleteSupplier(int id);

        List<Account> GetAllAccounts();
        Account GetAccountById(int id);
        Account GetAccountByUserName(string userName);
        void AddAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(int id);
        bool ValidateLogin(string userName, string password);
    }
}
