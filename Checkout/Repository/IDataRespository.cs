namespace Checkout
{
    public interface IDataRespository
    {
        decimal GetPriceByFoodCategory(FoodCategoryEnum category);
    }
}
