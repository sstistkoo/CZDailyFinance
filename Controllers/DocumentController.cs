using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CZDailyFinance.Models;
using System.IO;
using DBFinanceLinq;
using CZDailyFinance.Common;
using Microsoft.Win32;


namespace CZDailyFinance.Controllers
{
    public class DocumentController : Controller
    {
        DBFinanceDataContext context = new DBFinanceDataContext();
        //
        // GET: /Document/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        public JsonResult List()
        {
            IQueryable<DocFile> docfiles = context.DocFile;
            DataTablesParser<DocFile> parser=new DataTablesParser<DocFile>(Request,docfiles);
            FormatedList<DocFile> list=parser.Parse();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Upload(int? productId=0)
        {
            var statuses = new List<FilesStatus>();
          
            if(Request.Files!=null&&Request.Files.Count>0)
            {
                for(int i=0;i<Request.Files.Count;i++)
                {
                    var file = Request.Files[0];
                    string fileName = Path.GetFileName(file.FileName);
                    string folerName = "ClaimId";
                    string uploadFoler = Server.MapPath("/Content/File/Document/") + folerName;
                    if(!Directory.Exists(uploadFoler))
                    {
                        Directory.CreateDirectory(uploadFoler);
                    }
                    var filePath = Path.Combine(uploadFoler, fileName);
                    if(System.IO.File.Exists(filePath))
                    {
                        int k = 1;
                        do
                        {
                            fileName = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), k, Path.GetExtension(fileName));
                            filePath = Path.Combine(uploadFoler, fileName);
                            k++;
                        }
                        while (System.IO.File.Exists(filePath));
                    }
                    file.SaveAs(filePath);
                    var fileUrl = Path.Combine(folerName, fileName);

                    DocFile docFile = new DocFile();
                    docFile.ProductId = 1;
                    docFile.FileName = fileName;
                    docFile.FileUrl = fileUrl;
                    docFile.FileSuffix = Path.GetExtension(fileName);

                    if(file.ContentLength<1024*1024)
                    {
                        docFile.FileSize = (file.ContentLength / 1024f).ToString("0.00") + "KB";
                    }
                    else
                    {
                        docFile.FileSize = ((file.ContentLength / 1024f) / 1024f).ToString("0.00") + "MB";
                    }
                    docFile.CreaterId = 1;
                    docFile.UpdatedOn = docFile.CreatedOn = DateTime.UtcNow;
                    docFile.UpdatedBy = docFile.CreatedBy = "System";
                    context.DocFile.InsertOnSubmit(docFile);
                    context.SubmitChanges();
                    FilesStatus fileStatus = new FilesStatus(file.FileName, file.ContentLength);
                    statuses.Add(fileStatus);
                }
            }
            return Json(statuses);
        }

        public ActionResult Download(int id)
        {
            var docFile = context.DocFile.FirstOrDefault(q => q.DocFileId == id);
            if(docFile!=null)
            {
                var filePath = Path.Combine(Server.MapPath("/Content/File/Document/"), docFile.FileUrl);
                if (System.IO.File.Exists(filePath))
                {
                    string contentType = "text/plain";
                    RegistryKey rg = null;//干什么用的？
                    try
                    {
                        rg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(docFile.FileSuffix.ToLower());
                        object obj = rg.GetValue("Content Type");
                        if(obj!=null)
                        {
                            contentType = obj.ToString();
                        }
                    }
                    catch
                    {
                        contentType = "text/plain";
                    }
                    finally
                    {
                        rg.Close();
                    }
                    return File(filePath, contentType, docFile.FileName);
                }
            }
            return Content("File Not Found");
        }
       
    }
}
