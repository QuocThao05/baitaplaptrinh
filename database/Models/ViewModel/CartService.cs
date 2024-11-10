using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace database.Models.ViewModel
{
    public class CartService
    {
        private readonly HttpSessionStateBase session ;
        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

     

        public Cart GetCart()
        {
            Cart cart = (Cart)session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                session["Cart"] = cart;
            }
            return cart;
        }
        public void ClearCart()
        {
            session["Cart"] = null; 
            GetCart().Clear();  
        }
    }
}