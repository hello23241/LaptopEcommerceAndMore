using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Interfaces
{
    public interface IDataService
    {
        Task<List<Products>> GetAllProductsAsync();
        Task<List<Products>> GetProductsPageAsync(int pageNumber, int pageSize);
        Task<int> GetProductCountAsync();
        Task<Products> GetProductByIdAsync(int id);
        Task AddProductAsync(Products product);
        Task UpdateProductAsync(Products product);
        Task DeleteProductAsync(int id);

        Task<List<Categories>> GetAllCategoriesAsync();
        Task<Categories> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Categories category);
        Task UpdateCategoryAsync(Categories category);
        Task DeleteCategoryAsync(int id);

        Task<List<Brands>> GetAllBrandsAsync();
        Task<Brands> GetBrandByIdAsync(int id);
        Task AddBrandAsync(Brands brand);
        Task UpdateBrandAsync(Brands brand);
        Task DeleteBrandAsync(int id);

        Task<List<Users>> GetAllUsersAsync();
        Task<Users> GetUserByIdAsync(int id);
        Task<Users> GetUserByUserNameAsync(string userName);
        Task AddUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(int id);
        Task<bool> ValidateLoginAsync(string userName, string password);
    }
}
