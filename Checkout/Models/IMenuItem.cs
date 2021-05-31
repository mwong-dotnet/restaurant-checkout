namespace Checkout
{
    public interface IMenuItem
    {
        string Name { get; set; }
        FoodCategoryEnum Category { get; set; }
        decimal Price { get; set; }
    }
}
