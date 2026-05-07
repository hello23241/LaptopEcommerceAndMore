namespace WebActionResults1923050471.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
