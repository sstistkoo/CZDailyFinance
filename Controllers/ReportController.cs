using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBFinanceLinq;

namespace CZDailyFinance.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        DBFinanceDataContext context = new DBFinanceDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(string startDate, string endDate)
        {
            var purchasedProducts = context.PurchasedProducts.AsQueryable();
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                purchasedProducts = context.PurchasedProducts.Where(q => q.PurchasedDate > Convert.ToDateTime(startDate) && q.PurchasedDueDate < Convert.ToDateTime(endDate));
            }
            ViewBag.TotalReturnAmount = purchasedProducts.Sum(p => p.ReturnAmount);
            return View(purchasedProducts);
        }

       
    }
}
