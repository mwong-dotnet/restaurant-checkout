using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkout.Models
{
    public class ShoppingCart : IShoppingCart
    {
        private readonly List<IOrderLine> _items = new List<IOrderLine>();

        public void Add(IOrderLine item) => _items.Add(item);

        public void Remove(string name)
        {
            var item = GetItem(name);

            if (item is null) throw new ArgumentException("Cannot remove an item that is not in cart");
            _items.Remove(item);
        }

        public List<IOrderLine> GetOrderLines() => _items;

        public IOrderLine GetItem(string name) => _items.SingleOrDefault(o => o.MenuItem.Name == name);

        public decimal GetTotalPrice() => _items.Sum(o => o.Quantity * o.MenuItem.Price);

        public int GetUniqueItemsCount() => _items.Count;

        public int GetTotalItemsCount() => _items.Sum(o => o.Quantity);
    }
}
