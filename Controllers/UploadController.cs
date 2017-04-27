using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace CZDailyFinance.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            foreach(string upload in Request.Files)
            {
                if(Request.Files[upload].FileName!="")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/UploadFiles/";
                    string filename = Path.GetFileName(Request.Files[upload].FileName);
                    Request.Files[upload].SaveAs(Path.Combine(path, filename));
                }
            }
            return View("Upload");
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult Documents()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/UploadFiles/"));
            System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
            List<string> items = new List<string>();
            foreach (var file in fileNames)
            {
                items.Add(file.Name);
            }
            return View(items);
        }

        public FileResult Download(string ImageName)
        {
            var FileVirtualPath = "~/UploadFiles/" + ImageName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public ActionResult BasicJqueryFileUpload()
        {
            return View();
        }

      

    }
}
