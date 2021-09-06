﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Controllers
{
    [Localization]
    public partial class PageController : BasePageController
    {
        #region Pages


        public ActionResult DomainPage()
        {

            //for 301 redirect (the old pages is query string)
            if (Request.QueryString["CategoryID"] != null)
            {
                return HttpNotFound();
            }
            return View(CurrentPage);
        }

        public ActionResult LanguagePage()
        {
            return View(CurrentPage);
        }

        public ActionResult MagazineArticlePage()
        {
            var parent = _db.MagazinePages.FirstOrDefault(x => x.ID == CurrentPage.ParentID);
            ViewBag.ParentTitle = parent.Title;
            ViewBag.OtherArticles = _db.MagazineArticlePages.Where(x => x.Visible && x.ID != CurrentPage.ID && x.ParentID == parent.ID).OrderBy(r => r.Order).ToList();
            return View(CurrentPage);
        }

        public ActionResult MagazinePage()
        {
            ViewBag.Items = _db.MagazineArticlePages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID && r.Visible).OrderBy(r => r.Order).ToList();
            ViewBag.OtherMagazines = _db.MagazinePages.Where(x => x.DomainID == CurrentSettings.ID && x.Visible && x.ID != CurrentPage.ID).OrderBy(r => r.Order).ToList();
            return View(CurrentPage);
        }

        public ActionResult MagazinesPage(int? page)
        {
            var Items = _db.MagazinePages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID && r.Visible).OrderBy(r => r.Order);

            //Paging
            Pagination paging = new Pagination();
            paging.PageTotal = Items.Count();
            paging.PageItems = 6;
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();

            return View(CurrentPage);
        }

        public ActionResult ContentPage()
        {
            return View(CurrentPage);
        }

        public ActionResult SiteMapPage(string xml)
        {
            if (string.IsNullOrEmpty(xml) || xml != "1")
            {
                ViewBag.SiteMapData = RP.GetSiteMapFormated();
                return View(CurrentPage);
            }
            else
            {
                HttpContext.Response.Clear();
                return Content(RP.GetSiteMapFormatedXML(), "text/xml");
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FormPage()
        {
            if (CurrentPage.XML1 == null) ViewBag.Items = new List<FormField>();
            else ViewBag.Items = CurrentPage.GetDataFromXML1<FormField>().OrderBy(r => r.FormFieldOrder).ToList();

            return View(CurrentPage);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormPage(string InvisibleCaptchaValue, FormCollection formCollection)
        {
            if (!CaptchaController.IsInvisibleCaptchaValid(InvisibleCaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Captcha error.");
                return Content("Error: Captcha", "text/html");
            }

            var recaptcha = Request.Form["g-recaptcha-response"];
            if (!CaptchaController.IsGoogleReCaptchaValid(recaptcha))
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { errors = RP.T("Google.ReCaptcha.Invalid", "Please verify that you are not a robot.") });
            }

            Contact c = new Contact();

            List<FormField> FormFields = CurrentPage.GetDataFromXML1<FormField>().OrderBy(r => r.FormFieldOrder).ToList();
            List<FormRool> FormRools = CurrentPage.GetDataFromXML2<FormRool>().OrderBy(r => r.FormRoolOrder).ToList();

            //get name, email and phone
            foreach (Uco.Models.FormField item in FormFields.OrderBy(r => r.FormFieldOrder))
            {
                if (string.IsNullOrEmpty(Request["form_item_" + CurrentPage.ID + "_" + item.FormFieldID.ToString()])) continue;
                string t = Request["form_item_" + CurrentPage.ID + "_" + item.FormFieldID.ToString()];

                if (item.FormFieldType == FormField.FormFildType.Name && string.IsNullOrEmpty(c.ContactName)) c.ContactName = t;
                else if (item.FormFieldType == FormField.FormFildType.PhoneNumber && string.IsNullOrEmpty(c.ContactPhone)) c.ContactPhone = t;
                else if (item.FormFieldType == FormField.FormFildType.EmailAddress && string.IsNullOrEmpty(c.ContactEmail)) c.ContactEmail = t;
                else c.ContactData = c.ContactData + item.FormFieldTitle + ": " + t + "<br />";
            }

            string SendTo = string.Empty;

            //process rools
            foreach (Uco.Models.FormRool item in FormRools.OrderBy(r => r.FormRoolOrder))
            {
                bool MatchingRoolAnd = true;
                bool MatchingRoolOr = false;
                bool MatchingRun = false;

                if (!string.IsNullOrEmpty(item.FormRoolItem1) && item.FormRoolItem1 != "null")
                {
                    MatchingRun = true;
                    FormField field = FormFields.FirstOrDefault(r => r.FormFieldTitle.Trim() == item.FormRoolItem1.Trim());
                    if (field == null) MatchingRoolAnd = false;
                    else
                    {
                        string FormRoolValue1 = Request["form_item_" + CurrentPage.ID + "_" + field.FormFieldID.ToString()];
                        if (!("," + item.FormRoolValue1 + ",").Contains("," + FormRoolValue1 + ",")) MatchingRoolAnd = false;
                        else if (item.FormRoolValue1 != FormRoolValue1) MatchingRoolAnd = false;
                        else MatchingRoolOr = true;
                    }
                }

                if (!string.IsNullOrEmpty(item.FormRoolItem2) && item.FormRoolItem2 != "null")
                {
                    MatchingRun = true;
                    FormField field = FormFields.FirstOrDefault(r => r.FormFieldTitle.Trim() == item.FormRoolItem2.Trim());
                    if (field == null) MatchingRoolAnd = false;
                    else
                    {
                        string FormRoolValue2 = Request["form_item_" + CurrentPage.ID + "_" + field.FormFieldID.ToString()];
                        if (!("," + item.FormRoolValue2 + ",").Contains("," + FormRoolValue2 + ",")) MatchingRoolAnd = false;
                        if (item.FormRoolValue2 != FormRoolValue2) MatchingRoolAnd = false;
                        else MatchingRoolOr = true;
                    }
                }

                if (!string.IsNullOrEmpty(item.FormRoolItem3) && item.FormRoolItem3 != "null")
                {
                    MatchingRun = true;
                    FormField field = FormFields.FirstOrDefault(r => r.FormFieldTitle.Trim() == item.FormRoolItem3.Trim());
                    if (field == null) MatchingRoolAnd = false;
                    {
                        string FormRoolValue3 = Request["form_item_" + CurrentPage.ID + "_" + field.FormFieldID.ToString()];
                        if (!("," + item.FormRoolValue3 + ",").Contains("," + FormRoolValue3 + ",")) MatchingRoolAnd = false;
                        if (item.FormRoolValue3 != FormRoolValue3) MatchingRoolAnd = false;
                        else MatchingRoolOr = true;
                    }
                }

                if ((MatchingRun && item.FormRoolAnd && MatchingRoolAnd) || (MatchingRun && !item.FormRoolAnd && MatchingRoolOr))
                {
                    c.RoleDefault = item.FormRoolRole;
                    c.Rool = item.FormRoolTitle;
                    SendTo = item.FormRoolEmail;
                    break;
                }
            }
            if (string.IsNullOrEmpty(c.RoleDefault)) c.RoleDefault = "Admin";

            //get other data
            c.ContactReferal = SF.GetCookie("Referal");
            c.ContactUrl = "<a target='_blank' href='http://" + Request.ServerVariables["HTTP_HOST"] + Url.Content(CurrentPage.Url) + "'>" + CurrentPage.Title + "</a>";
            c.ContactDate = DateTime.Now;
			
			_db.Contacts.Add(c);
            _db.SaveChanges();

            if (string.IsNullOrEmpty(SendTo)) SendTo = RP.GetCurrentSettings().AdminEmail;

            foreach (string item in SendTo.Split(','))
            {
                if (!SF.isEmail(item.Trim())) continue;

                _db.OutEmails.Add(new OutEmail
                {
                    MailTo = item.Trim(),
                    Subject =  Uco.Models.Resources.SystemModels.ContactMailSubject
                        .Replace("{ID}", c.ID.ToString())
                        .Replace("{HTTP_HOST}", Request.ServerVariables["HTTP_HOST"]),

                    Body = "<div style='text-align: right; direction: rtl;'>" +
                        Uco.Models.Resources.SystemModels.ContactMailBody
						.Replace("{ID}", c.ID.ToString())
                        .Replace("{HTTP_HOST}", Request.ServerVariables["HTTP_HOST"])
						.Replace("{ContactName}", c.ContactName)
                        .Replace("{ContactEmail}", c.ContactEmail)
                        .Replace("{ContactPhone}", c.ContactPhone)
                        .Replace("{ContactData}", c.ContactData)
                        .Replace("{ContactReferal}", c.ContactReferal)
                        .Replace("{ContactUrl}", c.ContactUrl) + "</div>",
                    TimesSent = 0,
                    LastTry = DateTime.Now
                });
            }

            _db.SaveChanges();

            if (!string.IsNullOrEmpty(CurrentPage.Text3) && !string.IsNullOrEmpty(c.ContactEmail) && SF.isEmail(c.ContactEmail))
            {
                _db.OutEmails.Add(new OutEmail
                {
                    MailTo = c.ContactEmail.Trim(),
                    Subject = Uco.Models.Resources.SystemModels.ContactMailSubject
                        .Replace("{HTTP_HOST}", Request.ServerVariables["HTTP_HOST"]),
                    Body = "<div style='text-align: right; direction: rtl;'>" +
                        CurrentPage.Text3
                        .Replace("{HTTP_HOST}", Request.ServerVariables["HTTP_HOST"])
                        .Replace("{ContactName}", c.ContactName)
                        .Replace("{ContactEmail}", c.ContactEmail)
                        .Replace("{ContactPhone}", c.ContactPhone)
                        .Replace("{ContactData}", c.ContactData)
                        .Replace("{ContactReferal}", c.ContactReferal)
                        .Replace("{ContactUrl}", c.ContactUrl) + "</div>",
                    TimesSent = 0,
                    LastTry = DateTime.Now
                });
            }

            return Content(((FormPage)CurrentPage).Text2, "text/html");
        }

        public ActionResult ArticleListPage(int? page)
        {
            var Item = _db.ArticleListPages.Find(CurrentPage.ID);
            var Items = _db.ArticlePages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order);
            var ArticleListPageItems = _db.ArticleListPages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order);

            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = Items.Count();
            paging.PageItems = 6;
            if (Item.PageTemplate.Contains("Table"))
            {
                 paging.PageItems = 12;
            }
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();

            ViewBag.ArticleListPageItems = ArticleListPageItems.ToList();

            return View(CurrentPage);
        }

        public ActionResult ArticlePage()
        {
            List<ArticlePage> relatedArticles;
            var thisArticle = (ArticlePage)CurrentPage;
            if (!string.IsNullOrEmpty(thisArticle.RelatedArticles))
            {
                string[] lines = thisArticle.RelatedArticles.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                ViewBag.relatedArticle = _db.ArticlePages.Where(item => lines.Contains(item.Title)).OrderBy(item => Guid.NewGuid()).Take(4).ToList();
            }
            return View(CurrentPage);
        }

        public ActionResult GalleryListPage(int? page)
        {
            var Items = _db.GalleryPages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID).ToList();
            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = Items.Count();
            paging.PageItems = 12;
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();

            return View(CurrentPage);
        }

        public ActionResult GalleryPage()
        {
            return View(CurrentPage);
        }

        public ActionResult NewsListPage(int? page)
        {
            var Items = _db.NewsPages.Where(r => r.DomainID == CurrentSettings.ID && r.ParentID == CurrentPage.ID).ToList();
            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = Items.Count();
            paging.PageItems = 6;
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();

            return View(CurrentPage);
        }

        public ActionResult NewsPage()
        {
            return View(CurrentPage);
        }

        public ActionResult RedirectPage()
        {
            return Redirect(((RedirectPage)CurrentPage).RedirectTo);
        }

        #endregion


    }
}
