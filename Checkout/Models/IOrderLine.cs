namespace Checkout.Models
{
    public interface IOrderLine
    {
        IMenuItem MenuItem { get; set; }
        int Quantity { get; set; }
        decimal Price { get; set; }
    }
}
