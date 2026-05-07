using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Models
{
    public record ProductDetailsViewModel(Product Product, IReadOnlyList<ProductDetailTab> Tabs);
    public record ProductDetailTab(string Title, string Content);
}
