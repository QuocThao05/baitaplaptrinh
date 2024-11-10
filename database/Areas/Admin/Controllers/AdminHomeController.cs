using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using database.Models;
using database.Models.ViewModel;
using PagedList;
using System.Net;
namespace database.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: Admin/Home
        private MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                    p.ProductDecription.Contains(searchTerm) ||
                                    p.Category.CategoryName.Contains(searchTerm));
            }
            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.FuturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();
            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);

            return View(model);
        }
       
        
        public ActionResult ProductDetails(int? id, int? quantity, int? page )
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }
            // lấy tất cả các sản phẩm cùng danh mục 
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID!= pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            // đoạn code liên quan tới phân trang 
            // lấy số trang hiện tại(mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;// số sản phẩm mỗi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize); 
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);
            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            return View(model);
        }
    }

}