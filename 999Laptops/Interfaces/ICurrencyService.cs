using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToVndRateAsync();
    }
}