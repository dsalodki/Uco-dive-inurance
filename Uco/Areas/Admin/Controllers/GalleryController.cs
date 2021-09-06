﻿using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Uco.Infrastructure;
using Uco.Models;
using Uco.Infrastructure.Livecycle;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public partial class GalleryController : BaseAdminController
    {
        public ActionResult Save(IEnumerable<HttpPostedFileBase> attachments, string name)
        {
            string UserFolder = string.Format("~/Content/UserFiles/{0}", name);
            string UserFolderPath = Server.MapPath(UserFolder);
            CreateUserFolder(UserFolderPath);
            foreach (var file in attachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(UserFolderPath, fileName);
                file.SaveAs(physicalPath);
            }
            return Content("");
        }

        public ActionResult Remove(string[] fileNames, string name)
        {
            string UserFolder = string.Format("~/Content/UserFiles/{0}", name);
            string UserFolderPath = Server.MapPath(UserFolder);
            CreateUserFolder(UserFolderPath);
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(UserFolderPath, fileName);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            return Content("");
        }

        public ActionResult RemoveFile(string fullName, string name)
        {
            if (name.StartsWith("/")) name = name.Remove(0, 1);
            string folder = "/image?img=%2fcontent%2fuserfiles%2f" + Server.UrlEncode(name + "/");
            folder = folder.ToLower();
            fullName = fullName.ToLower();
            if (!fullName.Contains(folder))
                throw new HttpException(500, "error");
            string file = fullName.Replace(folder, "");
            if (!file.Contains("&w="))
                throw new HttpException(500, "error");
            file = file.Remove(file.IndexOf("&w="));
            string UserFolder = string.Format("~/Content/UserFiles/{0}", name);
            string UserFolderPath = Server.MapPath(UserFolder);
            CreateUserFolder(UserFolderPath);
            file = Server.UrlDecode(file);
            var physicalPath = Path.Combine(UserFolderPath, file);
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
                return Content("");
            }
            else throw new HttpException(500, "error");
        }

        private void CreateUserFolder(string UserFolderPath)
        {
            if (Directory.Exists(UserFolderPath)) return;
            DirectoryInfo di = Directory.CreateDirectory(UserFolderPath);
        }

        public ActionResult _UpdateGalleryDitails(string folder)
        {
            string UserFolder = string.Format("~/Content/UserFiles/{0}/", folder);

            string GalleryPath = Server.MapPath(UserFolder);
            List<ImageGalleryItem> allImages = SF.GalleryFile2List(GalleryPath, 120, 120, false, false);
            foreach (ImageGalleryItem item in allImages)
            {
                string order = Request["gallery_div_order_" + Url.Encode(item.FileName)];
                string title = Request["gallery_div_title_" + Url.Encode(item.FileName)];
                string imageAuthor = Request["gallery_div_author_" + Url.Encode(item.FileName)];
                string imageDescription = Request["gallery_div_desc_" + Url.Encode(item.FileName)];

                item.Title = title;
                item.Author = imageAuthor;
                item.Description = imageDescription;

                if (!string.IsNullOrEmpty(order))
                {
                    int tempInt = 0;
                    int.TryParse(order, out tempInt);
                    if (tempInt != 0) item.Order = tempInt;
                }
            }

            bool done = SF.GalleryList2File(allImages, GalleryPath);

            return Content((done ? "Done" : "Error"));
        }
    }
}