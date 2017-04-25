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
            csvString.Append("产品号,产品,成立日,到期日,天数,购买金额,收益");
            var query = GetPurchasedProductsList(startDate, endDate);
            decimal? totalReturnAmount = 0;
            //csv body
            foreach (var item in query)
            {
                csvString.AppendLine();
                csvString.Append(item.PurchasedProId).Append(",");
                csvString.Append(ConvertToCsvString(item.ProductName)).Append(",");
                csvString.Append(string.Format("{0}", item.PurchasedDate.HasValue ? ConvertToCsvString(item.PurchasedDate.Value.ToShortDateString()) : string.Empty)).Append(",");
                csvString.Append(string.Format("{0}", item.PurchasedDueDate.HasValue ? ConvertToCsvString(item.PurchasedDueDate.Value.ToShortDateString()) : string.Empty)).Append(",");
                csvString.Append(item.PurchasedDays).Append(",");
                csvString.Append(string.Format("{0}", item.PurchasedAmount.HasValue ? ConvertToCsvString(item.PurchasedAmount.Value.ToString("C2")) : string.Empty)).Append(",");
                csvString.Append(string.Format("{0}", item.ReturnAmount.HasValue ? ConvertToCsvString(item.ReturnAmount.Value.ToString("C2")) : string.Empty));
                if (item.ReturnAmount != null)
                {
                    totalReturnAmount += item.ReturnAmount;
                }
            }

            //footer
            csvString.AppendLine();
            csvString.Append(",").Append(",").Append(",").Append(",").Append(",").Append(",");
            csvString.Append(string.Format("{0}", ConvertToCsvString(totalReturnAmount.HasValue ? totalReturnAmount.Value.ToString("C2") : string.Empty)));

            return csvString.ToString();
        }

        public ActionResult ExportClick(string startDate, string endDate)
        {
            string csvString = GetCsvString(startDate, endDate);

            //cloud method
            string fileName = "FinanceCloud.csv";
            string filePathName = SaveTempFile(fileName, csvString);
            return new OKJsonResult(filePathName);

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
        }

        public string SaveTempFile(string fileName, string contents)
        {
            string _tempFolder;
            _tempFolder = Server.MapPath("/temp");
            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);
            string fullName = Path.Combine(_tempFolder, fileName);
            if (System.IO.File.Exists(fullName))
                System.IO.File.Delete(fullName);
            try
            {
                /***Issue:The CSV File with chinese will appear incorrect (下载的CSV文件中中文会出现乱码)***/
                //UnicodeEncoding uniEncoding = new UnicodeEncoding();
                //byte[] buffer = uniEncoding.GetBytes(contents);
                //using(FileStream fileStream=new FileStream(fullName,FileMode.Create))
                //{
                //    fileStream.Write(buffer, 0, uniEncoding.GetByteCount(contents));                  
                //    fileStream.Close();
                //}
                /***Issue:The CSV File with chinese will appear garbled (下载的CSV文件中中文会出现乱码)***/

                /***Solution:The CSV File with chinese will appear incorrect (下载的CSV文件中中文会出现乱码)***/
                byte[] buffer = Encoding.Default.GetBytes(contents);
                using (FileStream fileStream = new FileStream(fullName, FileMode.Create))
                {
                    fileStream.Write(buffer, 0, Encoding.Default.GetByteCount(contents));
                    fileStream.Close();
                }
                /***Solution:The CSV File with chinese will appear garbled (下载的CSV文件中中文会出现乱码)***/


                return string.Format("{0}/{1}", "/temp", fileName);
            }
            catch
            {
                if (System.IO.File.Exists(fullName))
                    System.IO.File.Delete(fullName);
            }
            return null;
        }

        public static string ConvertToCsvString(string source)
        {
            return string.IsNullOrEmpty(source) ? source : "\"" + source.Replace("\"", "\"\"") + "\"";//Just replace" as \"; Eg:"Hello"=>"\"Hello\""
        }



    }
}
