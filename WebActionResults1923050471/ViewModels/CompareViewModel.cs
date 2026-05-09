using WebActionResults1923050471.Models;
using System.Collections.Generic;

namespace WebActionResults1923050471.ViewModels
{
    public class CompareViewModel
    {
        // Danh sách các sản phẩm để hiển thị trên bảng so sánh
        public List<Products> Products { get; set; }

        // Constructor để khởi tạo list tránh lỗi null
        public CompareViewModel(List<Products> products)
        {
            Products = products ?? new List<Products>();
        }

        // Constructor mặc định (nếu cần)
        public CompareViewModel()
        {
            Products = new List<Products>();
        }
    }
}