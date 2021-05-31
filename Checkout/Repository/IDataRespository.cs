namespace Checkout
{
    public interface IDataRespository
    {
        decimal GetPrice(FoodCategoryEnum category);
    }
}
