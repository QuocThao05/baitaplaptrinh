using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace database.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        public void AddItem(CartItem cartItem)
        {
            CartItem existingItem = items.FirstOrDefault(i => i.ProductID == cartItem.ProductID);
            if (existingItem == null)
            {
                items.Add(cartItem);

                return; 
            }
            existingItem.Quantity += cartItem.Quantity;
        }
        public void RemoveItem(int productId) 
        {
            items.RemoveAll(i => i.ProductID == productId);
        }
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }
        public void Clear()
        {
            items.Clear();
        }
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if(item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}
