namespace Checkout
{
    public class MenuItem : IMenuItem
    {
        public string Name { get; set; }
        public FoodCategoryEnum Category { get; set; }
        public decimal Price { get; set; }
    }
}
