namespace WebActionResults1923050471.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public int Status { get; set; } = 1; // 1: Active, 0: Inactive
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
