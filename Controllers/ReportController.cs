using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBFinanceLinq;
using CZDailyFinance.MvcExtensions;
using System.Text;
using System.IO;

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

        public IQueryable<PurchasedProducts> GetPurchasedProductsList(string startDate, string endDate)
        {
            var purchasedProducts = context.PurchasedProducts.AsQueryable();
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                purchasedProducts = context.PurchasedProducts.Where(q => q.PurchasedDate > Convert.ToDateTime(startDate) && q.PurchasedDueDate < Convert.ToDateTime(endDate));
            }
            return purchasedProducts;
        }

        public ActionResult GetList(string startDate, string endDate)
        {
            var purchasedProducts = GetPurchasedProductsList(startDate, endDate);
            ViewBag.TotalReturnAmount = purchasedProducts.Sum(p => p.ReturnAmount);
            return View(purchasedProducts);
        }

        private string GetCsvString(string startDate, string endDate)
        {
            StringBuilder csvString = new StringBuilder();
            csvString.Append("PurchasedProId,ProductName,PurchasedDate,PurchasedDueDate,PurchasedDays,PurchasedAmount,ReturnAmount");
            var query = GetPurchasedProductsList(startDate, endDate);
            foreach(var item in query)
            {
                csvString.Append(Environment.NewLine);
                csvString.Append(item.PurchasedProId).Append(",");
                csvString.Append(item.ProductName).Append(",");
                csvString.Append(item.PurchasedDate).Append(",");
                csvString.Append(item.PurchasedDueDate).Append(",");
                csvString.Append(item.PurchasedDays).Append(",");
                csvString.Append(item.PurchasedAmount).Append(",");
                csvString.Append(item.ReturnAmount);
            }
            return csvString.ToString();
        }

        public string SaveTempFile(string fileName,string content)
        {
            
        }
        public ActionResult ExportClick(string startDate, string endDate)
        {
            string csvString = GetCsvString(startDate,endDate);

            //cloud method
            string fileName = "FinanceCloud.csv";
            string relativeFilePathName = SaveTempFile(fileName,csvString);


            //string exportFileName = "Export" + DateTime.Now.ToString("yyyyMMddHHmmss"); 
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //context.Response.Charset = "UTF-8";
            //context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //context.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            //context.Response.ContentType = "txt/csv";
            //context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            //context.Response.Write(csvString);
            //context.Response.AppendHeader("content-disposition", "attachment; filename=" + HttpUtility.UrlEncode(exportFileName + ".csv", System.Text.Encoding.UTF8).Replace("+", "%20"));
            ////context.Response.OutputStream.Write(fileData, 0, fileData.Length);    
            //context.Response.Flush();
            //context.Response.End();

            //System.IO.File.WriteAllText("D://FinanceReport.csv", csvString);
            //return new OKJsonResult();//可以用

          // return File(csvString,"text/csv","FinanceReport.csv");

          // return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", "FinanceReport.csv"); //the return action is FileContentResult好像没用


        }

        [HttpGet]
        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/FileDownload"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }

       
    }
}
