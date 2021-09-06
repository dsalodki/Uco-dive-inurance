//using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uco.Infrastructure;
using Uco.Models;

namespace Uco.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,DomainAdmin")]
    public class ShopProductImportController : BaseAdminController
    {
        public ActionResult Import()
        {
            return View();
        }

        public ActionResult ImportSave(IEnumerable<HttpPostedFileBase> attachments)
        {
            string FolderPath = Server.MapPath("~/App_Data/cache/images/" + CurrentSettings.ID + "/");
            foreach (var file in attachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(FolderPath, fileName);
                file.SaveAs(physicalPath);
            }
            return Content("");
        }

        public ActionResult ImportRemove(string[] fileNames)
        {
            string FolderPath = Server.MapPath("~/App_Data/cache/images/" + CurrentSettings.ID + "/");
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(FolderPath, fileName);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            return Content("");
        }

        //[HttpPost]
        //public ActionResult Import(string FileName, int? row, string AllowErrors)
        //{
        //    int rowToUpdate = 100000;
        //    if (row != null) rowToUpdate = (int)row;
        //    if (AllowErrors == "on") rowToUpdate = 1;

        //    string FinalError = string.Empty;
        //    if (string.IsNullOrEmpty(FileName))
        //    {
        //        ModelState.AddModelError("", "קובץ חובה");
        //        return View();
        //    }

        //    if (!FileName.EndsWith(".xls") && !FileName.EndsWith(".xlsx") && !FileName.EndsWith(".csv"))
        //    {
        //        FinalError = FinalError + "מותר לעלות קבצים מסוג : xls, xlsx, csv";
        //        ViewBag.FinalError = FinalError + "תקן שגיאות לפני יבוא<br />";
        //        return View();
        //    }

        //    string FilePath = (Server.MapPath("~/App_Data/cache/images/" + CurrentSettings.ID + "/" + FileName));

        //    if (!System.IO.File.Exists(FilePath))
        //    {
        //        ModelState.AddModelError("", "קובץ לא נמצא בשרת");
        //        return View();
        //    }

        //    //check if have excel sheet משרות
        //    //var excel = new ExcelQueryFactory(FilePath);

        //    //List<ShopProductImport> l = excel.Worksheet<ShopProductImport>(0).ToList();

        //    //check list before insert
        //    int i = 1;
        //    //foreach (ShopProductImport item in l)
        //    //{
        //    //    i = i + 1;
        //    //    //data validation
        //    //    if (string.IsNullOrEmpty(item.ShopProductTitle))
        //    //    {
        //    //        FinalError = FinalError + "שורה " + i + " ShopProductTitle חובה<br />";
        //    //    }
        //    //}
        //    if (!string.IsNullOrEmpty(FinalError))
        //    {
        //        try
        //        {
        //            System.IO.File.Delete(FilePath);
        //        }
        //        catch
        //        {
        //            FinalError = FinalError + "לא ניתן למחוק קובץ<br />";
        //        }
        //        ViewBag.FinalError = FinalError + "תקן שגיאות לפני יבוא<br />";
        //        return View();
        //    }

        //    //Delete old
        //    _db.Database.ExecuteSqlCommand("Delete from AbstractPages WHERE RouteUrl = 'p' AND ShopImported = 'false'");

        //    //for speed
        //    _db.Configuration.AutoDetectChangesEnabled = false;
        //    _db.Configuration.ValidateOnSaveEnabled = false;

        //    //Get ShopUrl list
        //    List<string> UrlList = _db.AbstractPages.Select(r => r.SeoUrlName).ToList();

        //    //start insert list
        //    i = 1;
        //    int toUpdate = 0;
        //    foreach (ShopProductImport item in l)
        //    {
        //        i = i + 1;
        //        ShopProductPage newItem = new ShopProductPage();

        //        newItem.ShopProductTitle = ConvertToString(item.ShopProductTitle);
        //        newItem.ShopShortDescription = ConvertToString(item.ShopShortDescription);
        //        newItem.ShopText = ConvertToString(item.ShopText);
        //        newItem.ShopShowInMainPage = ConvertToBool(item.ShopShowInMainPage);
        //        newItem.ShopShowInBest = ConvertToBool(item.ShopShowInBest);
        //        newItem.ShopIsVisibleOnMirror = ConvertToBool(item.ShopIsVisibleOnMirror);
        //        newItem.ShopShowInSale = ConvertToBool(item.ShopShowInSale);
        //        newItem.ShopPrice = ConvertToDecimal(item.ShopPrice);
        //        newItem.ShopOldPrice = ConvertToDecimal(item.ShopOldPrice);
        //        newItem.ShopAddShippingPrice = ConvertToDecimal(item.ShopAddShippingPrice);
        //        newItem.ShopAddShippingPriceEach = ConvertToBool(item.ShopAddShippingPriceEach);
        //        newItem.ShopBrand = ConvertToString(item.ShopBrand);
        //        newItem.ShopTags = ConvertToString(item.ShopTags);
        //        newItem.ShopRelatedProducts = ConvertToString(item.ShopRelatedProducts);
        //        newItem.ShopVideoGallery = ConvertToString(item.ShopVideoGallery);
        //        newItem.ShopPtoductID = ConvertToString(item.ShopPtoductID);

        //        //Categorys
        //        string CategoryIDsList = GetCategoryIDsList(item.ShopCategoryNames);
        //        string SearchType1IDsList = GetSearchCategoryIDsList(item.ShopSearchType1Names, "Type1");
        //        string SearchType2IDsList = GetSearchCategoryIDsList(item.ShopSearchType2Names, "Type2");
        //        string SearchType3IDsList = GetSearchCategoryIDsList(item.SearchType3Names, "Type3");

        //        newItem.ShopCategoryIDs = SF.SetTreeViewItemPageModelIDs(CategoryIDsList);
        //        newItem.ShopCategoryNames = SF.SetTreeViewItemPageModelNames(CategoryIDsList);
        //        newItem.ShopSearchType1IDs = SF.SetTreeViewItemModelIDs(SearchType1IDsList);
        //        newItem.ShopSearchType1Names = SF.SetTreeViewItemModelNames(SearchType1IDsList);
        //        newItem.ShopSearchType2IDs = SF.SetTreeViewItemModelIDs(SearchType2IDsList);
        //        newItem.ShopSearchType2Names = SF.SetTreeViewItemModelNames(SearchType2IDsList);
        //        newItem.SearchType3IDs = SF.SetTreeViewItemModelIDs(SearchType3IDsList);
        //        newItem.SearchType3Names = SF.SetTreeViewItemModelNames(SearchType3IDsList);

        //        //image
        //        if (string.IsNullOrEmpty(item.ShopPtoductID))
        //        {
        //            newItem.ShopPic = "/Content/DesignFiles/default.png";
        //        }
        //        else
        //        {
        //            if (System.IO.File.Exists(Server.MapPath("~/App_Data/Import/" + item.ShopPtoductID + ".jpg")))
        //            {
        //                try
        //                {
        //                    System.IO.File.Copy(Server.MapPath("~/App_Data/Import/" + item.ShopPtoductID + ".jpg"), Server.MapPath("~/Content/UserFiles/Products/" + item.ShopPtoductID + ".jpg"));
        //                    newItem.ShopPic = "/Content/UserFiles/Products/" + item.ShopPtoductID + ".jpg";
        //                }
        //                catch
        //                {
        //                    newItem.ShopPic = "/Content/DesignFiles/default.png";
        //                }
        //            }
        //            else newItem.ShopPic = "/Content/DesignFiles/default.png";
        //        }

        //        //SEO
        //        newItem.ShopSeoH1 = ConvertToString(item.ShopSeoH1);
        //        newItem.ShopSeoTitle = ConvertToString(item.ShopSeoTitle);
        //        newItem.ShopSeoDescription = ConvertToString(item.ShopSeoDescription);
        //        newItem.ShopSeoKywords = ConvertToString(item.ShopSeoKywords);
        //        newItem.ShopSeoInLinkName = ConvertToString(item.ShopSeoInLinkName);

        //        //ShopUrl
        //        newItem.SeoUrlName = SF.CleanUrl(SF.ClearDoubleSpace(newItem.ShopProductTitle));
        //        string SeoUrlName = newItem.SeoUrlName;

        //        int si = 2;
        //        while (UrlList.Contains(SeoUrlName))
        //        {
        //            SeoUrlName = newItem.SeoUrlName + "(" + si + ")";
        //            si = si + 1;
        //        }
        //        UrlList.Add(SeoUrlName);
        //        newItem.SeoUrlName = SeoUrlName;

        //        //ShopOther
        //        newItem.ShowInSitemap = true;
        //        newItem.Visible = false;
        //        newItem.ShopImported = false;
        //        newItem.ShowInMenu = true;
        //        newItem.ShowInAdminMenu = true;
        //        newItem.CreateTime = DateTime.Now;
        //        newItem.ChangeTime = DateTime.Now;
        //        newItem.ShopDomainID = CurrentSettings.ID;

        //        CustumMenuItem cmi = SF.GetDisplayMenuRepository().FirstOrDefault(r => r.RouteUrl == "pal");
        //        if (cmi == null) return Content("Create ProductAll page");
        //        newItem.ShopParentID = cmi.ID;
        //        newItem.ShopDomainID = CurrentSettings.ID;

        //        _db.ShopProductPages.Add(newItem);

        //        toUpdate = toUpdate + 1;

        //        if (toUpdate >= rowToUpdate)
        //        {
        //            toUpdate = 0;
        //            try
        //            {
        //                _db.SaveChanges();
        //            }
        //            catch (DbEntityValidationException dbEx)
        //            {
        //                foreach (var validationErrors in dbEx.EntityValidationErrors)
        //                {
        //                    foreach (var validationError in validationErrors.ValidationErrors)
        //                    {
        //                        FinalError = FinalError + "<br />שורה:" + i + "+-" + rowToUpdate + " Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
        //                    }
        //                }
        //            }
        //            catch (DbUpdateException objErr)
        //            {
        //                FinalError = FinalError + "<br />שורה:" + i + "+-" + rowToUpdate + " " + objErr.Message + ". " + objErr.InnerException.Message;
        //            }
        //            catch (Exception objErr)
        //            {
        //                FinalError = FinalError + "<br />שורה:" + i + "+-" + rowToUpdate + " " + GetErrorText(objErr);
        //            }
        //        }
        //    }

        //    try
        //    {
        //        _db.SaveChanges();
        //    }
        //    catch (Exception objErr)
        //    {
        //        FinalError = FinalError + GetErrorText(objErr);
        //    }

        //    if (string.IsNullOrEmpty(FinalError) || AllowErrors == "on")
        //    {
        //        _db.Database.ExecuteSqlCommand("Update AbstractPages Set Visible = 'true' WHERE RouteUrl = 'p' AND ShopImported = 'false'");
        //        _db.Database.ExecuteSqlCommand("Update AbstractPages Set ShopImported = 'true' WHERE RouteUrl = 'p' AND ShopImported = 'false'");
        //    }
        //    else
        //    {
        //        _db.Database.ExecuteSqlCommand("Delete from AbstractPages WHERE RouteUrl = 'p' AND ShopImported = 'false'");
        //    }

        //    try
        //    {
        //        System.IO.File.Delete(FilePath);
        //    }
        //    catch
        //    {
        //        FinalError = FinalError + "לא ניתן למחוק קובץ<br />";
        //    }

        //    ViewBag.FinalError = FinalError + "<br />סיום";
        //    return View();
        //}

        public ActionResult Export()
        {
            var items = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID).ToList();
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

            writer.Write("ID,");
            writer.Write("ShopProductTitle,");
            writer.Write("ShopShortDescription,");
            writer.Write("ShopText,");
            writer.Write("ShopShowInBest,");
            writer.Write("ShopIsVisibleOnMirror,");
            writer.Write("ShopShowInSale,");
            writer.Write("ShopPrice,");
            writer.Write("ShopOldPrice,");
            writer.Write("ShopAddShippingPrice,");
            writer.Write("ShopAddShippingPriceEach,");
            writer.Write("ShopBrand,");
            writer.Write("ShopRelatedProducts,");
            writer.Write("ShopPtoductID,");
            writer.Write("ShopCategoryIDs,");
            writer.Write("ShopCategoryNames,");
            writer.Write("ShopSeoH1,");
            writer.Write("ShopSeoTitle,");
            writer.Write("ShopSeoDescription,");
            writer.Write("ShopSeoKywords,");
            writer.Write("ShopSeoInLinkName,");
            writer.Write("SeoUrlName,");
            writer.Write("ShowInSitemap,");
            writer.Write("Visible,");
            writer.Write("ShowInMenu,");
            writer.Write("ShowInAdminMenu,");
            writer.Write("CreateTime,");
            writer.Write("ChangeTime,");
            writer.WriteLine();
            foreach (ShopProductPage item in items)
            {
                writer.Write(item.ID); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.Title)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.ShortDescription)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.Text)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopShowInBest); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopIsVisibleOnMirror); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopShowInSale); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopPrice); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopOldPrice); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopAddShippingPrice); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopAddShippingPriceEach); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.ShopBrand)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.ShopRelatedProducts)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.ShopSKU)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShopCategoryIDs); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.ShopCategoryNames)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.SeoH1)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.SeoTitle)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.SeoDescription)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.SeoKywords)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(ConvertToCSVString(item.SeoInLinkName)); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.SeoUrlName); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShowInSitemap); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.Visible); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShowInMenu); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ShowInAdminMenu); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.CreateTime); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.ChangeTime); writer.Write("\""); writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "application/csv", "Products.csv");
        }

        private bool ConvertToBool(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            value = value.ToLower();
            if (value == "*") return true;
            if (value == "true") return true;
            if (value == "כ") return true;
            if (value == "כן") return true;

            return false;
        }

        private string ConvertToString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Trim();
        }

        private string ConvertToCSVString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Trim().Replace("\"","'");
        }

        private DateTime? ConvertToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTime temp = DateTime.Now;
            if (DateTime.TryParse(value, out temp)) return temp;
            else return null;
        }

        private DateTime? ConvertToDateTime(string date, string time)
        {
            if (string.IsNullOrEmpty(date)) return null;

            DateTime temp = DateTime.Now;
            string d = date + " " + time;
            if (DateTime.TryParse(d, out temp)) return temp;
            else return null;
        }

        private decimal ConvertToDecimal(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            decimal temp = 0;
            if (decimal.TryParse(value, out temp)) return temp;
            else return 0;
        }

        private int ConvertToInt(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            int temp = 0;
            if (int.TryParse(value, out temp)) return temp;
            else return 0;
        }

        private string GetErrorText(Exception objErr)
        {
            string FinalError = string.Empty;
            if (objErr is DbEntityValidationException)
            {
                DbEntityValidationException dbEx = (DbEntityValidationException)objErr;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string error = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                        FinalError = FinalError + error + "<br />";
                    }
                }
            }
            else if (objErr is HttpException)
            {
                FinalError = FinalError + "חיבור לשרת נותק<br />";
            }
            else
            {
                FinalError = FinalError + objErr.Message + " " + objErr.Data + "<br />";
            }
            return FinalError;
        }

        private string GetCategoryIDsList(string CategoryNames)
        {
            string ReturnValue = ",";

            if (string.IsNullOrEmpty(CategoryNames)) return ReturnValue;
            List<ShopCategoryPage> l = _db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID).ToList();
            foreach (string item in CategoryNames.Split(','))
            {
                if (string.IsNullOrEmpty(item)) continue;
                string TempItem = item.Trim();
                if (string.IsNullOrEmpty(TempItem)) continue;

                ShopCategoryPage cp = l.FirstOrDefault(r => r.Title == TempItem);
                if (cp == null) continue;
                else ReturnValue = ReturnValue + cp.ID + ",";
            }

            return ReturnValue;
        }

        private string GetSearchCategoryIDsList(string SearchCategoryNames, string Segment)
        {
            //string ReturnValue = ",";
            //if (string.IsNullOrEmpty(SearchCategoryNames) || string.IsNullOrEmpty(Segment)) return ReturnValue;
            //foreach (string item in SearchCategoryNames.Split(','))
            //{
            //    if (string.IsNullOrEmpty(item)) continue;
            //    string TempItem = item.Trim();
            //    if (string.IsNullOrEmpty(TempItem)) continue;

            //    SearchModel cp = SF.GetSearchModelRepository().FirstOrDefault(r => r.ShopProductTitle == TempItem && r.SearchSegmentName == Segment);
            //    if (cp == null) continue;
            //    else ReturnValue = ReturnValue + cp.ID + ",";
            //}

            //return Segment + "|" + ReturnValue;
            return string.Empty;
        }
    }
}
