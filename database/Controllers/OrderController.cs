using database.Models.ViewModel;
using database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace database.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private CartService GetCartService()
        {
            return new CartService(Session);
        }
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        //Get: Order/Checkout
        public ActionResult Checkout()
        {
            //kiểm tra giỏ hàng trong section
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            Cart cart = GetCartService().GetCart();
            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xác thực ng dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới đănh nhập 
            string usrName = null;
            if(Session["UserName"] != null)
            {
                usrName = Session["UserName"].ToString().Trim();
            }
            if (usrName == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //lấy thông tin từ khách hàng từ CSDL, nếu chưa thì chueyenr hướng tới trang đăng nhập
            //nếu rồi thì lấy địa chỉ của khách hànng và gáng vào ShippingAddress của checkoutVM
            Customer customer = db.Customers.SingleOrDefault(c => c.Username == usrName);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");

            }
            CheckoutVM model = new CheckoutVM
            { //tạo dữ liệu hiển thị cho checkoutVm
                CartItems = cart.Items.ToList(),//Lấy danh sách các sản phẩm trong giỏ 
                TotalAmount = cart.Items.Sum(item => item.TotalPrice), // Tổng giá trị các mặt hàng trong giỏ
                OrderDate = DateTime.Now,//mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress,//Lấy địa chỉ mặc định từ bẳng customer
                CustomerID = customer.CustomerID,//lấy mã khách từ bảng Customer
                UserName = customer.Username
            };
            return View(model);
        }
        //POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                Cart cart = GetCartService().GetCart();
                //Nếu giỏ hàng rổng chueyern tới trang Home
                if (cart == null) { return RedirectToAction("Index", "Home"); }
                //Nếu ng dùng ch đăng nhập chuyển đén trang đăng nhập
                string usrName = null;
                if (Session["UserName"] != null)
                {
                    usrName = Session["UserName"].ToString().Trim();
                }
                User user = db.Users.SingleOrDefault(u => u.Username.Trim() == usrName);
                if (user == null) { return RedirectToAction("Login", "Account"); }
                //nếu khách hàng k khớp với tên đn  sẽ chueyern đến trang đăng nhập(login)
                Customer customer = db.Customers.SingleOrDefault(c => c.Username.Trim() == usrName);
                if (customer == null) { return RedirectToAction("Login", "Account"); }
                //Nếu ng dùng chọn thanh toán bằn Paypal, sẽ chuyển đến trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction("PaymentWithPaypal", "Paypal", model);
                }
                //Thiết lập PaypalStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //Tạo đơn hàng và chi tiết đơn hàng liên quan
                Order order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymenMethod = model.PaymentMethod,
                    DeliveryMethod = model.ShippingMethod,
                    AddressDelivery = model.ShippingAddress,
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL 
                db.Orders.Add(order);
                db.SaveChanges();
                //Xóa giỏ hàng sau khi đặt hàng thành công  
                GetCartService().ClearCart();
                Session["Cart"] = null;
                //Chuyển đến trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }
    }
}