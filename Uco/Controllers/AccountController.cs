using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult LogOn(string returnUrl)
        {
            if (TempData["Error"] != null) { ModelState.AddModelError("", TempData["Error"] as string); }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl, string InvisibleCaptchaValue)
        {
            if (!CaptchaController.IsInvisibleCaptchaValid(InvisibleCaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Captcha error.");
                return View();
            }

            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("DomainPage", "Page", new { name = "root" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", LocalizationHelpers.GetLocalResource("~/Views/Account/LogOn.cshtml", "UsernameIncorrect"));
                }
            }

            return View(model);
        }

        public ActionResult SendPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendPassword(string Model, string InvisibleCaptchaValue, string CaptchaValue)
        {
            if (!CaptchaController.IsCaptchaValid(CaptchaValue) || !CaptchaController.IsInvisibleCaptchaValid(InvisibleCaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Captcha error.");
                return View();
            }

            if (ModelState.IsValid)
            {
                User u = _db.Users.FirstOrDefault(r => r.Email == Model);
                if (u == null) ModelState.AddModelError("", LocalizationHelpers.GetLocalResource("~/Views/Account/SendPassword.cshtml", "UsernameIncorrect"));
                else
                {
                    _db.OutEmails.Add(new OutEmail() { MailTo = Model, Subject = LocalizationHelpers.GetLocalResource("~/Views/Account/SendPassword.cshtml", "EmailTitle"), Body = LocalizationHelpers.GetLocalResource("~/Views/Account/SendPassword.cshtml", "EmailBody").Replace("{0}", u.UserName).Replace("{1}", u.Password) });
                    _db.SaveChanges();

                    return RedirectToAction("SendPasswordSuccess");
                }
            }
            return View();
        }

        public ActionResult SendPasswordSuccess()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("DomainPage", "Page", new { name = "root" });
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model, string CaptchaValue, string InvisibleCaptchaValue)
        {
            if (!CaptchaController.IsInvisibleCaptchaValid(InvisibleCaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Captcha error.");
                return View();
            }

            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                MembershipUser newUser = Membership.CreateUser(model.Email, model.Password, model.Email, "-", "-", true, out createStatus);

                // show error if there is problems
                if (createStatus != MembershipCreateStatus.Success)
                {
                    ModelState.AddModelError(string.Empty, SF.ErrorCodeToString(createStatus));
                    return View(model);
                }

                // show error that the user didn't create
                User u = _db.Users.FirstOrDefault(r => r.UserName == model.Email);
                if (u == null)
                {
                    ModelState.AddModelError(string.Empty, "Can't find user");
                    return View(model);
                }

                u.Roles = SF.RolesStringAdd(u.Roles, "User");
                u.FirstName = model.FirstName;
                u.LastName = model.LastName;
                u.City = model.City;
                u.RoleDefault = "User";
                u.Phone = model.Phone;
                u.CreationDate = DateTime.Now;
                u.LastModified = DateTime.Now;
                u.SendNewsletters = model.SendNewsletters;

                _db.Entry(u).State = EntityState.Modified;
                _db.SaveChanges();
               
                if (u.SendNewsletters == true)
                {
                    SF.RegisterToNewsletter(model.FirstName + " " + model.LastName, model.Email, string.Empty, string.Empty);
                }

                if (Membership.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    LS.CurrentHttpContext.Items["_CurrentUser"] = _db.Users.FirstOrDefault(r => r.UserName == u.UserName);
                    SF.SendContentRegisterEmail();

                    TempData["register"] = RP.T("Register.Success.Message");
                    return RedirectToAction("ProfileEdit");                
                }
                else
                {
                    ModelState.AddModelError("", LocalizationHelpers.GetLocalResource("~/Views/Account/LogOn.cshtml", "UsernameIncorrect"));
                }
               

            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProfileEdit()
        {

            User u = _db.Users.FirstOrDefault(r => r.UserName == User.Identity.Name);
            ProfileEditModel m = new ProfileEditModel();
            m.Email = u.Email;
            m.FirstName = u.FirstName;
            m.LastName = u.LastName;
            m.City = u.City;
            m.Phone = u.Phone;
            m.SendNewsletters = u.SendNewsletters;
 
            return View(m);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProfileEdit(ProfileEditModel model)
        {
            if (ModelState.IsValid)
            {
       
                    User u = _db.Users.FirstOrDefault(r => r.UserName == User.Identity.Name);
                    u.Email = model.Email;
                    u.FirstName = model.FirstName;
                    u.LastName = model.LastName;
                    u.Phone = model.Phone;
                    u.SendNewsletters = model.SendNewsletters;

                    _db.Entry(u).State = EntityState.Modified;
                    _db.SaveChanges();

                    ViewBag.Message = "Updated";
                    View(model);                    
            }
            return View(model);
        }
               

        #region ShopStatus Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
