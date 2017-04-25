using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CZDailyFinance.Common;
using DBFinanceLinq;
using CZDailyFinance.MvcExtensions;


namespace CZDailyFinance.Controllers
{
    public class ProductSummaryController : Controller
    {
        //
        // GET: /ProductSummary/
        DBFinanceDataContext context = new DBFinanceDataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            PurchasedProducts product = new PurchasedProducts();
            return View(product);
        }

        public ActionResult EditProduct(int id)
        {
            PurchasedProducts product = new PurchasedProducts();
            product = context.PurchasedProducts.FirstOrDefault(q => q.PurchasedProId == id);
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(PurchasedProducts productModel)
        {
            PurchasedProducts product = new PurchasedProducts();
            if (productModel.PurchasedProId > 0)
            {
                product = context.PurchasedProducts.FirstOrDefault(q => q.PurchasedProId == productModel.PurchasedProId);
                product.ProductName = productModel.ProductName;
                product.PurchasedDate = Convert.ToDateTime(productModel.PurchasedDate);
                product.PurchasedDueDate = Convert.ToDateTime(productModel.PurchasedDueDate);
                product.PurchasedDays = productModel.PurchasedDays;
                product.ReturnAmount = productModel.ReturnAmount;
                product.ReturnRate = productModel.ReturnRate;
                product.ManagementRate = productModel.ManagementRate;
                product.PurchasedAmount = productModel.PurchasedAmount;
                product.UpdatedBy = "System";
                product.UpdatedDate = DateTime.Today;
                context.SubmitChanges();
            }
            return new OKJsonResult();
        }

        [HttpPost]
        public JsonResult AddProduct(PurchasedProducts productModel)
        {
            PurchasedProducts product = new PurchasedProducts();
            product.ProductName = productModel.ProductName;
            product.PurchasedDate = Convert.ToDateTime(productModel.PurchasedDate);
            product.PurchasedDueDate = Convert.ToDateTime(productModel.PurchasedDueDate);
            product.PurchasedDays = productModel.PurchasedDays;
            product.ReturnAmount = productModel.ReturnAmount;
            product.ReturnRate = productModel.ReturnRate;
            product.ManagementRate = productModel.ManagementRate;
            product.PurchasedAmount = productModel.PurchasedAmount;
            product.CreatedBy = "System";
            product.CreatedDate = DateTime.Today;
            context.PurchasedProducts.InsertOnSubmit(product);
            context.SubmitChanges();
            return new OKJsonResult();
        }

        [HttpPost]
        public JsonResult DeletePurchasedProduct(int purchasedProductId)
        {
            var product = context.PurchasedProducts.FirstOrDefault(q => q.PurchasedProId == purchasedProductId);
            if (null != product)
            {
                context.PurchasedProducts.DeleteOnSubmit(product);
                context.SubmitChanges();
            }
            return new OKJsonResult();
        }

        [HttpPost]
        public JsonResult DeleteMultiPurchasedProducts(string ids)
        {
            List<PurchasedProducts> products = new List<PurchasedProducts>();
            string[] purchasedProductIds=ids.Split(',');
            foreach(var id in purchasedProductIds)
            {
                products.Add(context.PurchasedProducts.FirstOrDefault(q => q.PurchasedProId ==Convert.ToInt32 (id)));
            }
            context.PurchasedProducts.DeleteAllOnSubmit(products);
            context.SubmitChanges();
            return new OKJsonResult();
        }

      
        public JsonResult List(bool isExpired)
        {
            IQueryable<PurchasedProducts> productList = null;
            if (isExpired)
            {
                productList = context.PurchasedProducts.Where(q =>  q.PurchasedDueDate < DateTime.Today).OrderByDescending(q=>q.PurchasedDate);
            }
            else
            {
                productList = context.PurchasedProducts.Where(q => q.PurchasedDueDate == null || q.PurchasedDueDate >= DateTime.Today).OrderByDescending(q => q.PurchasedDate);
            }
            DataTablesParser<PurchasedProducts> parser = new DataTablesParser<PurchasedProducts>(Request, productList);
            FormatedList<PurchasedProducts> list = parser.Parse();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
