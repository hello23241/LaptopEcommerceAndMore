using System.Text.Json;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Services
{
    public class CompareService : ICompareService
    {
        private const string CompareSessionKey = "CompareProductIds";
        private const int MaxCompareItems = 4;

        public Task<IReadOnlyList<int>> GetComparedProductIdsAsync(HttpContext httpContext)
        {
            var list = GetIds(httpContext);
            return Task.FromResult<IReadOnlyList<int>>(list);
        }

        public Task<int> GetCompareCountAsync(HttpContext httpContext)
        {
            var count = GetIds(httpContext).Count;
            return Task.FromResult(count);
        }

        public Task<bool> AddProductAsync(HttpContext httpContext, int productId)
        {
            var ids = GetIds(httpContext);
            if (ids.Contains(productId))
            {
                return Task.FromResult(true);
            }

            if (ids.Count >= MaxCompareItems)
            {
                return Task.FromResult(false);
            }

            ids.Add(productId);
            SaveIds(httpContext, ids);
            return Task.FromResult(true);
        }

        public Task RemoveProductAsync(HttpContext httpContext, int productId)
        {
            var ids = GetIds(httpContext);
            if (ids.Remove(productId))
            {
                SaveIds(httpContext, ids);
            }

            return Task.CompletedTask;
        }

        public Task ClearAsync(HttpContext httpContext)
        {
            SaveIds(httpContext, new List<int>());
            return Task.CompletedTask;
        }

        private static List<int> GetIds(HttpContext httpContext)
        {
            var json = httpContext.Session.GetString(CompareSessionKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<int>();
            }

            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }

        private static void SaveIds(HttpContext httpContext, List<int> ids)
        {
            var json = JsonSerializer.Serialize(ids);
            httpContext.Session.SetString(CompareSessionKey, json);
        }
    }
}

