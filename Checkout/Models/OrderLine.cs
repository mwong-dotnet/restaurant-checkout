namespace Checkout.Models
{
    public class OrderLine : IOrderLine
    {
        public IMenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get => Quantity * MenuItem.Price; set => Price = value; }
    }
}
