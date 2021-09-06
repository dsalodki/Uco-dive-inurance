using CrmMigration.NewDB;
using CrmMigration.OldDB;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrmMigration
{  
    class Program
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string CleanUrl(string UrlToClean)
        {
            if (UrlToClean == null) return "";
            UrlToClean = UrlToClean.Trim();
            UrlToClean = UrlToClean.Replace(" ", "-");
            UrlToClean = UrlToClean.Replace(".", "-");
            UrlToClean = UrlToClean.Replace(",", "-");
            UrlToClean = UrlToClean.Replace("\"", "-");
            UrlToClean = UrlToClean.Replace("'", "-");
            UrlToClean = UrlToClean.Replace(",", "-");
            UrlToClean = UrlToClean.Replace("/", "-");
            UrlToClean = UrlToClean.Replace("\\", "-");
            UrlToClean = UrlToClean.Replace("!", "-");
            UrlToClean = UrlToClean.Replace("@", "-");
            UrlToClean = UrlToClean.Replace("#", "-");
            UrlToClean = UrlToClean.Replace("%", "-");
            UrlToClean = UrlToClean.Replace("^", "-");
            UrlToClean = UrlToClean.Replace("&", "-");
            UrlToClean = UrlToClean.Replace("*", "-");
            UrlToClean = UrlToClean.Replace("(", "-");
            UrlToClean = UrlToClean.Replace(")", "-");
            UrlToClean = UrlToClean.Replace("-", "-");
            UrlToClean = UrlToClean.Replace("+", "-");
            UrlToClean = UrlToClean.Replace("=", "-");
            UrlToClean = UrlToClean.Replace("?", "-");
            UrlToClean = UrlToClean.Replace("<", "-");
            UrlToClean = UrlToClean.Replace(">", "-");
            UrlToClean = UrlToClean.Replace("₪", "-");
            UrlToClean = UrlToClean.Replace("|", "-");

            UrlToClean = UrlToClean.Replace("-----", "-");
            UrlToClean = UrlToClean.Replace("----", "-");
            UrlToClean = UrlToClean.Replace("---", "-");
            UrlToClean = UrlToClean.Replace("--", "-");

            return UrlToClean;
        }


        

        static void Main(string[] args)
        {
            Console.WriteLine("Start app");
            var _oldDBconnection = new CrmMigration.OldDB.OldDB();
            var _newDBconnection = new CrmMigration.NewDB.NewDB();
            _oldDBconnection.Configuration.AutoDetectChangesEnabled = false;
            _oldDBconnection.Configuration.ValidateOnSaveEnabled = false;
            _newDBconnection.Configuration.AutoDetectChangesEnabled = false;
            _newDBconnection.Configuration.ValidateOnSaveEnabled = false;

            //Loop For each caterory (including templates) + oldID            
            Console.Write("Do you want add category in new BD ? Write Y or N: ");
            var key = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (key == 'Y' || key == 'y')
            {
                try
                {
                    var oldCategoryList = _oldDBconnection.dbsCategories.AsNoTracking();
                    var allAbstractPagesEntityList = _newDBconnection.AbstractPages;

                    foreach (var oldCategory in oldCategoryList)
                    {
                        var has = _newDBconnection.AbstractPages.FirstOrDefault(x => x.MigrationOldID.HasValue && x.MigrationOldID == oldCategory.CategoryID && x.RouteUrl == "arl");
                        if (has != null)
                        {
                            Console.WriteLine("Category with Migration ID = {0} already exist in new DB.", oldCategory.CategoryID);
                            continue;
                        }
                        var newCategoryAbstract = new AbstractPages();

                        newCategoryAbstract.MigrationOldID = oldCategory.CategoryID;
                        newCategoryAbstract.Title = oldCategory.CategoryTitle;
                        newCategoryAbstract.ShortDescription = oldCategory.CategoryShortDesc; //always null
                        newCategoryAbstract.Layout = "_LayoutIdive.cshtml";

                        switch (oldCategory.TemplateID)
                        {
                            case 370: newCategoryAbstract.PageTemplate = ""; break;                                    //370 No Template
                            case 371: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //371Content - List Display                                           
                            case 372: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //372	Content - Gallery Display
                            case 373: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //373	Content - Lexicon Display
                            case 374: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //374	Content - Images Gallery
                            case 375: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //375	Behavior - Forms
                            case 377: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //377	Categories - List View
                            case 378: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //378	Categories - Gallery View
                            case 379: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //379	Behavior - Direct Link
                            case 380: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //380	Behavior - Forum
                            case 381: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //381	Behavior - Forum group
                            case 382: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //382	Content - Portal Display
                            case 495: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //495	Custom homepage
                            case 497: newCategoryAbstract.PageTemplate = "ArticleListPageTable.cshtml"; break;         //497	Content - News
                            case 552: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //552	Content - Events
                            case 642: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;          //642	Content - FAQ
                            case 3724: newCategoryAbstract.PageTemplate = ""; break;                                   //3724	Competitions
                            case 8497: newCategoryAbstract.PageTemplate = "ArticleListPageList.cshtml"; break;         //8497	Content - Ads Board
                        }
                        //newCategoryAbstract.PageTemplate = "ArticleListPageList";               //or other template
                        newCategoryAbstract.Text = oldCategory.CategoryDesc;
                        //oldCategory.CategoryLevel
                        newCategoryAbstract.Order = oldCategory.OrderKey ?? 0;
                        //oldCategory.IsLocked
                        newCategoryAbstract.Visible = oldCategory.IsActive ?? true;
                        newCategoryAbstract.ShowInMenu = oldCategory.IsActive ?? true;
                        newCategoryAbstract.ShowInSitemap = oldCategory.IsActive ?? true;
                        newCategoryAbstract.ShowInAdminMenu = oldCategory.IsActive ?? true;
                        //oldCategory.IsDivisionCat
                        //oldCategory.HideChilds
                        //oldCategory.CategoryURL
                        //oldCategory.NewWin                                                    //open in new win if have CategoryURL
                        //oldCategory.MoreTitle
                        //oldCategory.EmailTitle
                        newCategoryAbstract.LanguageCode = "he-IL";
                        //oldCategory.PageSize
                        //oldCategory.EnableShoppingChart
                        //oldCategory.EnableResponses
                        //oldCategory.FormID
                        //oldCategory.GUIID
                        //oldCategory.SystemPage
                        //oldCategory.EnableTicker
                        //oldCategory.EnableQuickSurvey
                        //oldCategory.IsRootCategory
                        if (!string.IsNullOrEmpty(oldCategory.CategoryImgName))
                        {
                            newCategoryAbstract.Pic = "/uploads/dbsCategories/" + oldCategory.CategoryImgName;                  //add path
                        }

                        newCategoryAbstract.DomainID = 1;
                        newCategoryAbstract.ParentID = oldCategory.ParentID <= 1 ? 1 : 0;
                        if (allAbstractPagesEntityList.Select(x => x.SeoUrlName.Contains(oldCategory.CategoryTitle)).Any())
                            newCategoryAbstract.SeoUrlName = CleanUrl(oldCategory.CategoryTitle + "-" + RandomString(5));
                        else
                            newCategoryAbstract.SeoUrlName = CleanUrl(oldCategory.CategoryTitle);

                        newCategoryAbstract.Discriminator = "ArticleListPage";
                        newCategoryAbstract.RouteUrl = "arl";
                        newCategoryAbstract.CreateTime = DateTime.Now;
                        newCategoryAbstract.ChangeTime = DateTime.Now;

                        _newDBconnection.AbstractPages.Add(newCategoryAbstract);
                        _newDBconnection.SaveChanges();
                        Console.WriteLine("Category successfully added with migration ID = {0}", oldCategory.CategoryID);

                    }
                    var newCategoryList = _newDBconnection.AbstractPages.Where(x => x.MigrationOldID != null && x.Discriminator == "ArticleListPage").ToList();
                    foreach (var oldCategory in oldCategoryList)
                    {
                        if (oldCategory.ParentID > 1)
                        {
                            var oldParent = oldCategoryList.FirstOrDefault(x => x.CategoryID == oldCategory.ParentID);
                            if (oldParent != null)
                            {
                                var newCategory = newCategoryList.FirstOrDefault(x => x.MigrationOldID == oldCategory.CategoryID);

                                if (newCategory != null)
                                {
                                    var newParent = newCategoryList.FirstOrDefault(x => x.MigrationOldID == oldParent.CategoryID);
                                    if (newParent != null)
                                    {
                                        newCategory.ParentID = newParent.ID;
                                        _newDBconnection.Entry(newCategory).State = System.Data.Entity.EntityState.Modified;
                                        _newDBconnection.SaveChanges();
                                        Console.WriteLine("Parent ID successfully added to categories with MigrationID = {0}", newCategory.MigrationOldID);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Categories. Error Message: {0}, Inner Exception Message: {1}", e.Message, e.InnerException.InnerException.Message);
                }
            }
            Console.Write("Do you want add articles in new BD ? Write Y or N: ");
            var key2 = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (key2 == 'Y' || key2 == 'y')
            {
                //Loop For each articles+ category mapping + oldID
                try
                {
                    var oldArticleList = _oldDBconnection.dbsArticles.AsNoTracking();
                    var allAbstractPagesEntityList = _newDBconnection.AbstractPages.AsNoTracking();
                    foreach (var oldArticle in oldArticleList)
                    {
                        var has = _newDBconnection.AbstractPages.FirstOrDefault(x => x.MigrationOldID.HasValue && x.MigrationOldID == oldArticle.ArticleID && x.RouteUrl == "a");
                        if (has != null)
                        {
                            Console.WriteLine("Article with Migration ID = {0} already exist in new DB", oldArticle.ArticleID);
                            continue;
                        }
                        var newArticleAbstract = new AbstractPages();

                        newArticleAbstract.MigrationOldID = oldArticle.ArticleID;
                        newArticleAbstract.Title = oldArticle.Title;
                        newArticleAbstract.ShortDescription = oldArticle.Summary;
                        newArticleAbstract.Text = oldArticle.Body;
                        newArticleAbstract.Layout = "_LayoutIdive.cshtml";
                        newArticleAbstract.PageTemplate = "";
                        newArticleAbstract.Discriminator = "ArticlePage";
                        newArticleAbstract.Order = oldArticle.OrderKey ?? 0;

                        newArticleAbstract.Visible = oldArticle.Status == 2 ? true : false;
                        newArticleAbstract.ShowInMenu = oldArticle.Status == 2 ? true : false;
                        newArticleAbstract.ShowInSitemap = oldArticle.Status == 2 ? true : false;
                        newArticleAbstract.ShowInAdminMenu = oldArticle.Status == 2 ? true : false;

                        newArticleAbstract.ShopPrice = oldArticle.SalePrice;
                        newArticleAbstract.ShopOldPrice = oldArticle.MarketPrice;

                        if (!string.IsNullOrEmpty(oldArticle.FullImg))
                        {
                            newArticleAbstract.Pic = "/uploads/dbsArticles/" + oldArticle.FullImg;                  //add path
                            newArticleAbstract.MainImg = "/uploads/dbsArticles/" + oldArticle.MainImg;
                        }
                        newArticleAbstract.LanguageCode = "he-IL";
                        newArticleAbstract.RouteUrl = "a";
                        newArticleAbstract.Visible = true;
                        newArticleAbstract.DomainID = 1;
                        //newArticleAbstract.ShopAddShippingPrice = oldArticle.ShippingFee;

                        newArticleAbstract.CreateTime = DateTime.Now;
                        newArticleAbstract.ChangeTime = oldArticle.LastUpdated ?? DateTime.Now;

                        if (allAbstractPagesEntityList.Select(x => x.SeoUrlName.Contains(oldArticle.Title)).Any())
                            newArticleAbstract.SeoUrlName = CleanUrl(oldArticle.Title + "-" + RandomString(5));
                        else
                            newArticleAbstract.SeoUrlName = CleanUrl(oldArticle.Title);

                        var abstractPage =  allAbstractPagesEntityList.FirstOrDefault(x => x.MigrationOldID == oldArticle.CategoryID); //oldArticle.CategoryID ?? 0;//  
                        if(abstractPage != null)
                        {
                            newArticleAbstract.ParentID = abstractPage.ID;
                        }


                        _newDBconnection.AbstractPages.Add(newArticleAbstract);
                        _newDBconnection.SaveChanges();
                        Console.WriteLine("Article successfully added with migration ID = {0}", oldArticle.ArticleID);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Articles. Error Message: {0}, Inner Exception Message: {1}", e.Message, e.InnerException.InnerException.Message);
                }
            }       
            
            Console.WriteLine("End of application. Press any key");
            Console.ReadLine();        
        }


        //add only MainPage by MigrationID
        static void Main2(string[] args)
        {
            Console.WriteLine("Start app");
            var _oldDBconnection = new CrmMigration.OldDB.OldDB();
            var _newDBconnection = new CrmMigration.NewDB.NewDB();
            _oldDBconnection.Configuration.AutoDetectChangesEnabled = false;
            _oldDBconnection.Configuration.ValidateOnSaveEnabled = false;
            _newDBconnection.Configuration.AutoDetectChangesEnabled = true;
            _newDBconnection.Configuration.ValidateOnSaveEnabled = false;

            //Loop For each articles+ category mapping + oldID
            try
            {
                var oldArticleList = _oldDBconnection.dbsArticles.AsNoTracking().ToList();
                foreach (var oldArticle in oldArticleList)
                {
                    var newArticle = _newDBconnection.AbstractPages.Where(x => x.Discriminator == "ArticlePage" && x.MigrationOldID == oldArticle.ArticleID).FirstOrDefault();
                    if (newArticle != null)
                    {
                        if (!string.IsNullOrEmpty(oldArticle.MainImg))
                        {
                            newArticle.MainImg = "/uploads/dbsArticles/" + oldArticle.MainImg;
                        }
                        else
                        {
                            newArticle.MainImg = "";
                        }
                            _newDBconnection.Entry(newArticle).State = System.Data.Entity.EntityState.Modified;
                            _newDBconnection.SaveChanges();
                            Console.WriteLine("Article successfully changes with migration ID = {0}, MainImg = {1}", oldArticle.ArticleID, newArticle.MainImg);
                        
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Articles. Error Message: {0}, Inner Exception Message: {1}", e.Message, e.InnerException.InnerException.Message);
            }
            Console.WriteLine("End of application. Press any key");
            Console.ReadLine();
        }

    }
}