namespace WebActionResults1923050471.Interfaces
{
    public interface ICompareService
    {
        Task<IReadOnlyList<int>> GetComparedProductIdsAsync(HttpContext httpContext);
        Task<int> GetCompareCountAsync(HttpContext httpContext);
        Task<bool> AddProductAsync(HttpContext httpContext, int productId);
        Task RemoveProductAsync(HttpContext httpContext, int productId);
        Task ClearAsync(HttpContext httpContext);
    }
}
