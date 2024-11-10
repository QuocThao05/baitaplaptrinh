using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using database.Models;
using database.Models.ViewModel;

namespace database.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private CartService GetCartService()
        {
            return new CartService(Session);
        }
        public ActionResult Index()
        {
            Cart cart = GetCartService().GetCart();
            return View(cart);
        }
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            CartService cartService = GetCartService();
            Product product = db.Products.Find(id);
            if (product != null)
            {
                CartItem cartItem = new CartItem();
                cartItem.ProductID = product.ProductID;
                cartItem.ProductImage = product.ProductImage;
                cartItem.ProductName = product.ProductName;
                cartItem.UnitPrice = product.ProductPrice;
                cartItem.Quantity = quantity;
                // Add Cart item to list 
                cartService.GetCart().AddItem(cartItem);
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}