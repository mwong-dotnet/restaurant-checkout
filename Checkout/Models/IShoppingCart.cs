using Checkout.Models;
using System.Collections.Generic;

namespace Checkout
{
    public interface IShoppingCart
    {
        void Add(IOrderLine item);
        void Remove(string menuItemName);
        List<IOrderLine> GetOrderLines();
        IOrderLine GetItem(string name);
        decimal GetTotalPrice();
        int GetUniqueItemsCount();
        int GetTotalItemsCount();
    }
}
