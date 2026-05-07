using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Interfaces
{
    public interface IDataService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsPageAsync(int pageNumber, int pageSize);
        Task<int> GetProductCountAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        Task<List<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);

        Task<List<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task<Account> GetAccountByUserNameAsync(string userName);
        Task AddAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(int id);
        Task<bool> ValidateLoginAsync(string userName, string password);
    }
}
