using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LaptopEcommerceAndMore.Interfaces; // Gọi đến file Interface đã có sẵn trong dự án của bạn

namespace LaptopEcommerceAndMore.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private decimal _cachedRate = 25400m;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToVndRateAsync()
        {
            // Kiểm tra cache 4 tiếng để tránh spam API ngoài
            if ((DateTime.Now - _lastUpdateTime).TotalHours < 4)
            {
                return _cachedRate;
            }

            try
            {
                string url = "https://open.er-api.com/v6/latest/USD";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (var jsonDoc = JsonDocument.Parse(jsonString))
                    {
                        if (jsonDoc.RootElement.TryGetProperty("rates", out var rates) &&
                            rates.TryGetProperty("VND", out var vndRate))
                        {
                            _cachedRate = (decimal)vndRate.GetDouble();
                            _lastUpdateTime = DateTime.Now;
                        }
                    }
                }
            }
            catch
            {
                // Nếu sập mạng hoặc lỗi kết nối bên thứ 3, dùng tỷ giá backup
            }

            return _cachedRate;
        }
    }
}