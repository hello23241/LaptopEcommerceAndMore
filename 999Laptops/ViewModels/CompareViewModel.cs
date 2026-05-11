using LaptopEcommerceAndMore.Models;
using System.Collections.Generic;

namespace LaptopEcommerceAndMore.ViewModels
{
    public class CompareViewModel
    {
        // Danh sách các s?n ph?m d? hi?n th? trên b?ng so sánh
        public List<Products> Products { get; set; }

        // Constructor d? kh?i t?o list tránh l?i null
        public CompareViewModel(List<Products> products)
        {
            Products = products ?? new List<Products>();
        }

        // Constructor m?c d?nh (n?u c?n)
        public CompareViewModel()
        {
            Products = new List<Products>();
        }
    }
}
