using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Models;
namespace Uco.Migrate
{
    public static class MigrateLib
    {
        public static string Migrate()
        {

            var dataContext = new oldDataContext();
            var oldList = dataContext.dbsCategories.ToList();
            var newList = LS.CurrentEntityContext.ArticleListPages.ToList();

            var uniqSeoList = LS.CurrentEntityContext.AbstractPages
                .Select(x=>new {x.ID,x.SeoUrlName}).ToList().Select(x => new KeyValuePair<int, string>(x.ID, x.SeoUrlName)).ToList();

            LS.CurrentEntityContext.Configuration.ValidateOnSaveEnabled = false;
            StringBuilder _log = new StringBuilder();
            //fix old seo urls
            bool fixSeo = false;
            if (fixSeo)
            {
                foreach (var articlListPage in newList)
                {
                    
                    string oldUrl = articlListPage.SeoUrlName;
                    if (articlListPage.SeoUrlName == null)
                    {
                        articlListPage.SeoUrlName = articlListPage.Title;
                    }
                    if (articlListPage.SeoUrlName == null)
                    {
                        continue;
                    }
                    articlListPage.SeoUrlName = SF.CleanUrl(articlListPage.SeoUrlName.ToLower());
                    while (uniqSeoList.Any(x => x.Value != null 
                        && x.Value.ToLower() == articlListPage.SeoUrlName 
                        && x.Key != articlListPage.ID))
                    {
                        articlListPage.SeoUrlName += "-" + articlListPage.ID;
                    }
                    if (oldUrl != articlListPage.SeoUrlName || articlListPage.PageTemplate != "ArticleListPageTable.cshtml")
                    {
                        articlListPage.PageTemplate = "ArticleListPageTable.cshtml";
                        LS.CurrentEntityContext.SaveChanges();
                        _log.AppendLine(" old: " + oldUrl);
                        _log.AppendLine(" new: " + articlListPage.SeoUrlName);
                    }
                }
            }
            foreach (var oldCategory in oldList)
            {
                if (false && !newList.Any(x => x.Text3 == oldCategory.CategoryID.ToString())
                   // || oldCategory.CategoryID == 511
                  //   || oldCategory.CategoryID == 512
                   //  || oldCategory.CategoryID == 513
                    //&& oldCategory.ParentID == 36
                    )
                {
                    _log.AppendLine(oldCategory.CategoryID + ": " + oldCategory.CategoryTitle
                        + " url: " + SF.CleanUrl(oldCategory.CategoryTitle));
                    int parent = 0;
                    parent = newList.Where(x => x.Text3 == oldCategory.ParentID.ToString())
                        .Select(x => x.ID).DefaultIfEmpty(0).FirstOrDefault();
                    //create new 
                    var newArticleListPage = new ArticleListPage();
                    newArticleListPage.MigrationOldID = oldCategory.CategoryID;
                    newArticleListPage.ChangeTime = DateTime.Now;
                    newArticleListPage.CreateTime = DateTime.Now;
                    newArticleListPage.LanguageCode = "he-IL";
                    newArticleListPage.Layout = "_Layout.cshtml";
                    newArticleListPage.DomainID = 1;
                    newArticleListPage.PageTemplate = "ArticleListPageTable.cshtml";
                    newArticleListPage.Order = oldCategory.OrderKey.HasValue ? oldCategory.OrderKey .Value : 999;
                    newArticleListPage.ParentID = parent;
                    newArticleListPage.PermissionsUpdateChildPages = true;
                    newArticleListPage.RouteUrl = "arl";
                    newArticleListPage.Pic = oldCategory.CategoryImgName;
          
                    newArticleListPage.Visible = oldCategory.IsActive.HasValue ? oldCategory.IsActive.Value : true;
                    newArticleListPage.Title = oldCategory.CategoryTitle;
                    newArticleListPage.Text = oldCategory.CategoryDesc;
                    newArticleListPage.Text3 = oldCategory.CategoryID.ToString();
                    newArticleListPage.ShortDescription = oldCategory.CategoryShortDesc;
                  
           
                    string url  = SF.CleanUrl(newArticleListPage.Title.ToLower());
                    newArticleListPage.SeoUrlName = url;
                    int i = 1;
                    while (uniqSeoList.Any(x => x.Value != null
                        && x.Value.ToLower() == url 
                        ))
                    {
                        url = newArticleListPage.SeoUrlName + "-" + i.ToString();
                        i++;
                    }
                    newArticleListPage.SeoUrlName = url;
                    newArticleListPage.ShowInAdminMenu = true;
                    newArticleListPage.ShowInMenu = true;
                    newArticleListPage.ShowInSitemap = true;
          
                    LS.CurrentEntityContext.ArticleListPages.Add(newArticleListPage);
                    uniqSeoList.Add( new KeyValuePair<int,string>( newArticleListPage.ID,newArticleListPage.SeoUrlName ));
                }
            }
            LS.CurrentEntityContext.SaveChanges();
            // 2771 - all to parent in new db parent id for all aricl list pages (categories)

           
            //import articles
            var oldArticles = dataContext.dbsArticles.ToList();
            var newArticles = LS.CurrentEntityContext.ArticlePages.ToList();

            //fix old seo articlesurls
            bool fixSeoArticle = false;
            if (fixSeoArticle)
            {
                foreach (var articlPage in newArticles)
                {

                    string oldUrl = articlPage.SeoUrlName;
                    if (articlPage.SeoUrlName == null)
                    {
                        articlPage.SeoUrlName = articlPage.Title;
                    }
                    if (articlPage.SeoUrlName == null)
                    {
                        continue;
                    }
                    articlPage.SeoUrlName = SF.CleanUrl(articlPage.SeoUrlName.ToLower());
                    while (uniqSeoList.Any(x => x.Value != null
                        && x.Value.ToLower() == articlPage.SeoUrlName
                        && x.Key != articlPage.ID))
                    {
                        articlPage.SeoUrlName += "-" + articlPage.ID;
                    }
                    if (oldUrl != articlPage.SeoUrlName )
                    {
                       
                        LS.CurrentEntityContext.SaveChanges();
                        uniqSeoList.Add(new KeyValuePair<int, string>(articlPage.ID, articlPage.SeoUrlName));
                        _log.AppendLine(" old: " + oldUrl);
                        _log.AppendLine(" new: " + articlPage.SeoUrlName);
                    }
                }
            }

            newList = LS.CurrentEntityContext.ArticleListPages.ToList();
            foreach (var oldArticle in oldArticles)
            {
                if (!newArticles.Any(x => x.Text3 == oldArticle.ArticleID.ToString())

                    && (
                    oldArticle.CategoryID == 511
                    || oldArticle.CategoryID == 512
                    || oldArticle.CategoryID == 513
                    )
                 )
                {
                    int parent = 0;
                    parent = newList.Where(x => x.Text3 == oldArticle.CategoryID.ToString())
                        .Select(x => x.ID).DefaultIfEmpty(0).FirstOrDefault();
                    _log.AppendLine(oldArticle.CategoryID + ": " + oldArticle.Title
                        + " url: " + SF.CleanUrl(oldArticle.Title));
                    //create new 
                    var newArticlePage = new ArticlePage();
                    newArticlePage.MigrationOldID = oldArticle.CategoryID;
                    newArticlePage.ChangeTime = DateTime.Now;
                    newArticlePage.CreateTime = DateTime.Now;
                    newArticlePage.LanguageCode = "he-IL";
                    newArticlePage.Layout = "_Layout.cshtml";
                    newArticlePage.DomainID = 1;
                    //newArticlePage.PageTemplate = "ArticleListPageTable.cshtml";
                    newArticlePage.Order = oldArticle.OrderKey.HasValue ? oldArticle.OrderKey.Value : 999;
                    newArticlePage.ParentID = parent;
                    newArticlePage.PermissionsUpdateChildPages = true;
                    newArticlePage.RouteUrl = "a";
                    newArticlePage.Pic = oldArticle.FullImg;
                    if(string.IsNullOrEmpty(newArticlePage.Pic))
                    {
                        newArticlePage.Pic = oldArticle.MainImg;
                    }
                    if (string.IsNullOrEmpty(newArticlePage.Pic))
                    {
                        newArticlePage.Pic = oldArticle.MainImgSmall;
                    }
                    newArticlePage.Visible = oldArticle.Status.HasValue ? oldArticle.Status.Value == 2 : false;
                    newArticlePage.Title = oldArticle.Title;
                    newArticlePage.Text = oldArticle.Body;
                    newArticlePage.Text3 = oldArticle.ArticleID.ToString();
                    newArticlePage.ShortDescription = oldArticle.Summary;


                    string url = SF.CleanUrl(newArticlePage.Title.ToLower());
                    newArticlePage.SeoUrlName = url;
                    int i = 1;
                    while (uniqSeoList.Any(x => x.Value != null
                        && x.Value.ToLower() == url
                        ))
                    {
                        url = newArticlePage.SeoUrlName + "-" + i.ToString();
                        i++;
                    }
                    newArticlePage.SeoUrlName = url;
                    newArticlePage.ShowInAdminMenu = true;
                    newArticlePage.ShowInMenu = true;
                    newArticlePage.ShowInSitemap = true;

                    LS.CurrentEntityContext.ArticlePages.Add(newArticlePage);
                    uniqSeoList.Add(new KeyValuePair<int, string>(newArticlePage.ID, newArticlePage.SeoUrlName));
                }
            }
            LS.CurrentEntityContext.SaveChanges();


            return _log.ToString();
        }
    }
}