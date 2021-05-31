using Checkout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Checkout
{
    public class CheckoutService
    {
        private readonly IDataRespository _respository;
        private readonly IShoppingCart _cart;

        public CheckoutService(IDataRespository respository, IShoppingCart cart)
        {
            _respository = respository;
            _cart = cart;
        }

        private decimal GetPriceByFoodCategory(FoodCategoryEnum category) => _respository.GetPriceByFoodCategory(category);

        public void AddToCart(IMenuItem menuItem)
        {
            var orderLine = GetAddedItem(menuItem);

            if (orderLine is null)
            {
                menuItem.Price = GetPriceByFoodCategory(menuItem.Category);
                _cart.Add(new OrderLine() { MenuItem = menuItem, Quantity = 1 });
            }
            else
            {
                orderLine.Quantity += 1;
            }
        }

        public void RemoveFromCart(IMenuItem menuItem) => _cart.Remove(menuItem.Name);

        public void UpdateQuantity(IMenuItem menuItem, int quantity)
        {
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Please specify a quantity greater than 0");

            if (quantity > 0)
            {
                var orderLine = GetAddedItem(menuItem);
                if (orderLine is null) throw new ArgumentException("Cannot update an item that is not in cart");

                orderLine.Quantity = quantity;
            }
            else
            {
                RemoveFromCart(menuItem);
            }
        }

        public int GetUniqueItemsCount() => _cart.GetUniqueItemsCount();

        public int GetTotalItems() => _cart.GetTotalItemsCount();

        public decimal CalculateTotalBill() => _cart.GetTotalPrice();

        public IOrderLine GetAddedItem(IMenuItem menuItem) => _cart.GetItem(menuItem.Name);
    }
}
