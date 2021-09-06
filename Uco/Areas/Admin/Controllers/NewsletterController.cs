using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data;
using System.IO;
using System.Text;
using Uco.Infrastructure;
using Uco.Infrastructure.Tasks;
using Uco.Infrastructure.Logger;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class NewsletterController : BaseAdminController
    {
        private readonly object _lock = "sendEmailLock";

        public ActionResult Index()
        {
            ViewBag.MessageRed = TempData["MessageRed"];
            ViewBag.MessageYellow = TempData["MessageYellow"];
            ViewBag.MessageGreen = TempData["MessageGreen"];
            return View();
        }
        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Newsletter> items = _db.Newsletters.Where(r => (CurrentUser.RoleDefault == "Admin" ? true : (r.RoleDefault == CurrentUser.RoleDefault)));
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(Newsletter item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.Newsletters.Remove(_db.Newsletters.First(r => (CurrentUser.RoleDefault == "Admin" ? true : (r.RoleDefault == CurrentUser.RoleDefault)) && r.ID == item.ID));
                _db.SaveChanges();
                CleanCache.CleanOutputCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Details(int ID)
        {
            return View(_db.Newsletters.First(r => (CurrentUser.RoleDefault == "Admin" ? true : (r.RoleDefault == CurrentUser.RoleDefault)) && r.ID == ID));
        }

        public ActionResult CSVExport()
        {
            var items = _db.Newsletters.Where(r => (CurrentUser.RoleDefault == "Admin" ? true : (r.RoleDefault == CurrentUser.RoleDefault))).ToList();
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write("ID,");
            writer.Write("ShopDate,");
            writer.Write("Name,");
            writer.Write("ShopEmail,");
            writer.Write("Role,");
            writer.Write("Data");
            writer.WriteLine();
            foreach (Newsletter item in items)
            {
                writer.Write(item.ID); writer.Write(","); writer.Write("\"");
                writer.Write(item.NewsletterDate); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.NewsletterName); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.NewsletterEmail); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.RoleDefault); writer.Write("\""); writer.Write(","); writer.Write("\"");
                writer.Write(item.NewsletterData); writer.Write("\""); writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "application/csv", "Newsletters.csv");
        }

        public ActionResult Send()
        {
            if (Session["NewsletterSend"] == null)
            {
                OutEmail outEmail = new OutEmail();
                outEmail.NewsletterAccountGroups = NewsletterAccountGroups();
                return View(outEmail);
            }
            else
            {

                return View((OutEmail)Session["NewsletterSend"]);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Send(OutEmail model)
        {
         DateTime FromDate = (!string.IsNullOrEmpty(model.DateFrom)) ? DateTime.Parse(model.DateFrom) : DateTime.MinValue;
         DateTime ToDate = (!string.IsNullOrEmpty(model.DateTo)) ? DateTime.Parse(model.DateTo) : DateTime.MaxValue;

            if (ModelState.IsValid)
            {
                SendMail sendEmail = new SendMail();

                if (model.MailTo == "" || model.MailTo == null)
                {
                    List<string> MailRecipients = new List<string>();
                    foreach (Newsletter item in _db.Newsletters)
                    {
                        MailRecipients.Add(item.NewsletterEmail);
                    }

                    //  if (model.NewsletterAccountGroupsSelected == null || model.NewsletterAccountGroupsSelected.Count < 1 && model.DateFrom == DateTime.MinValue && model.DateTo == DateTime.MaxValue)
                    if (model.NewsletterAccountGroupsSelected == null && FromDate == DateTime.MinValue && ToDate == DateTime.MaxValue)

                    {
                        foreach (string item2 in MailRecipients)
                        {
                            OutEmail outEmail = new OutEmail()
                            {
                                MailTo = item2.Trim(),
                                Subject = model.Subject,
                                Body = model.Body,
                                TimesSent = 0,
                                LastTry = DateTime.Now

                            };

                            _db.OutEmails.Add(outEmail);
                            lock (_lock)
                            {
                                sendEmail.SendEmail(outEmail);
                            }
                        }
                    }
                    else if (model.NewsletterAccountGroupsSelected == null || model.NewsletterAccountGroupsSelected.Count < 1 && FromDate != DateTime.MinValue || ToDate != DateTime.MaxValue)
                    {
                        foreach (Newsletter newsletter in _db.Newsletters)
                        {
                            if (FromDate.CompareTo(newsletter.NewsletterDate) <= 0)
                            {
                                OutEmail outEmail = new OutEmail()
                                {
                                    MailTo = newsletter.NewsletterEmail.Trim(),
                                    Subject = model.Subject,
                                    Body = model.Body,
                                    TimesSent = 0,
                                    LastTry = DateTime.Now

                                };
                                if (ToDate != DateTime.MaxValue)
                                {
                                    if (ToDate.CompareTo(newsletter.NewsletterDate) < 0)
                                    {
                                        outEmail = new OutEmail();
                                    }
                                }
                                if (!String.IsNullOrEmpty(outEmail.MailTo))
                                {
                                    _db.OutEmails.Add(outEmail);
                                    lock (_lock)
                                    {
                                        sendEmail.SendEmail(outEmail); // ====== Actually sends the email ===============================================================================
                                        Logger.Information("Sending email for group '" + newsletter.NewsletterData + "', email address: " + newsletter.NewsletterEmail);
                                    }
                                }
                            
                            }
                            else if (model.DateTo.CompareTo(newsletter.NewsletterDate) >= 0)
                            {
                                OutEmail outEmail = new OutEmail()
                                {
                                    MailTo = newsletter.NewsletterEmail.Trim(),
                                    Subject = model.Subject,
                                    Body = model.Body,
                                    TimesSent = 0,
                                    LastTry = DateTime.Now

                                };
                                if (FromDate != DateTime.MinValue)
                                {
                                    if (FromDate.CompareTo(newsletter.NewsletterDate) > 0)
                                    {
                                        outEmail = new OutEmail();
                                    }
                                }
                                if (!String.IsNullOrEmpty(outEmail.MailTo))
                                {
                                    _db.OutEmails.Add(outEmail);
                                    lock (_lock)
                                    {
                                        sendEmail.SendEmail(outEmail); // ====== Actually sends the email ===============================================================================
                                        Logger.Information("Sending email for group '" + newsletter.NewsletterData + "', email address: " + newsletter.NewsletterEmail);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Newsletter newsletter in _db.Newsletters)
                        {
                            if (model.NewsletterAccountGroupsSelected.Count(x => x == newsletter.NewsletterData) > 0)
                            {
                                OutEmail outEmail = new OutEmail()
                                {
                                    MailTo = newsletter.NewsletterEmail.Trim(),
                                    Subject = model.Subject,
                                    Body = model.Body,
                                    TimesSent = 0,
                                    LastTry = DateTime.Now

                                };
                                if (FromDate != DateTime.MinValue)
                                {
                                    if (FromDate.CompareTo(newsletter.NewsletterDate) > 0)
                                    {
                                        outEmail = new OutEmail();
                                    }
                                }
                                if (ToDate != DateTime.MaxValue)
                                {
                                    if (ToDate.CompareTo(newsletter.NewsletterDate) < 0)
                                    {
                                        outEmail = new OutEmail();
                                    }
                                }
                                if (!String.IsNullOrEmpty(outEmail.MailTo))
                                {
                                    _db.OutEmails.Add(outEmail);
                                    lock (_lock)
                                    {
                                        sendEmail.SendEmail(outEmail); // ====== Actually sends the email ===============================================================================
                                        Logger.Information("Sending email for group '" + newsletter.NewsletterData + "', email address: " + newsletter.NewsletterEmail);
                                    }
                                }
                            }
                        }
                    }
                    _db.SaveChanges();

                    // =======================================================================================
                    //sendEmail.SendEmail()
                    // ========================================================================================

                    model.NewsletterAccountGroups = NewsletterAccountGroups();

                    Session["NewsletterSend"] = model;
                    ViewBag.Message = LocalizationHelpers.GetLocalResource("~/Areas/Admin/Views/Newsletter/Send.cshtml", "SentToAll");
                    return View(model);
                }
                else
                {
                    _db.OutEmails.Add(new OutEmail
                    {
                        MailTo = model.MailTo,
                        Subject = model.Subject,
                        Body = model.Body,
                        TimesSent = 0,
                        LastTry = DateTime.Now
                    });
                    _db.SaveChanges();

                    model.NewsletterAccountGroups = NewsletterAccountGroups();

                    Session["NewsletterSend"] = model;
                    ViewBag.Message = LocalizationHelpers.GetLocalResource("~/Areas/Admin/Views/Newsletter/Send.cshtml", "SentTo") + model.MailTo;
                    return View(model);
                }
            }
            else
            {
                model.NewsletterAccountGroups = NewsletterAccountGroups();
                return View(model);
            }
        }

        private List<string> NewsletterAccountGroups()
        {
            string groupNotDefined = "שם קבוצה לא הוגדר";
            List<string> newsletterAccountGroups = new List<string>();
            foreach (Newsletter newsletter in _db.Newsletters)
            {
                if (newsletterAccountGroups.Count(x => x == newsletter.NewsletterData) < 1)
                {
                    if (!string.IsNullOrEmpty(newsletter.NewsletterData))
                        newsletterAccountGroups.Add(newsletter.NewsletterData);
                    else
                        if (newsletterAccountGroups.Count(x => x == groupNotDefined) < 1)
                            newsletterAccountGroups.Add(groupNotDefined);
                }
            }
            return newsletterAccountGroups;
        }
    }
}
