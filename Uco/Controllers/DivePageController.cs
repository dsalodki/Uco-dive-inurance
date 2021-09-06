using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using Uco.Utils;

namespace Uco.Controllers
{
    public class DivePageController : BaseController
    {
        [AuthorizeDivePage]
        public ActionResult Index()
        {
            if (IsNewUserRegistration())
            {
                return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "ProfileEdit" }));
            }

            ViewBag.Certificates = _db.Certificates.AsNoTracking().Where(x => x.UserID == LS.CurrentUser.ID).ToArray();

            //if session have Idive Insurance 
            //ViewBag.Insurance = Idive Insurance
            //else
            if (Session["insurance"] != null)
            {
                ViewBag.Insurance = Session["insurance"];
            }
            else
            {
                ViewBag.Insurance = _db.Insurances.AsNoTracking().FirstOrDefault(x => x.UserID == LS.CurrentUser.ID);
            }

            ViewBag.LastDive = _db.Dives.AsNoTracking().OrderByDescending(x => x.DateOfDive).FirstOrDefault(x => x.UserID == LS.CurrentUser.ID);

            return View();
        }

        [AuthorizeDivePage]
        public ActionResult CertificateList()
        {
            var model = _db.Certificates.AsNoTracking().Where(x => x.UserID == LS.CurrentUser.ID).ToArray();

            return View(model);
        }

        [AuthorizeDivePage]
        public ActionResult CertificateEdit(int? id)
        {
            Certificate certificate = null;
            if (id.HasValue)
            {
                certificate = _db.Certificates.FirstOrDefault(x => x.UserID == LS.CurrentUser.ID && x.ID == id.Value);
            }
            if (certificate == null)
            {
                certificate = new Certificate();
            }

            InitDDLForCertificate(certificate);

            return View(certificate);
        }

        [HttpPost]
        [AuthorizeDivePage]
        public ActionResult CertificateEdit(Certificate model, HttpPostedFileBase certificateImageFront, HttpPostedFileBase certificateImageBack)
        {
            if(!IsValidFile(certificateImageFront) || !IsValidFile(certificateImageBack))
            {
                InitDDLForCertificate(model);
                return View(model);
            }

            // upload picture
            if (certificateImageFront != null)
            {
                var newFileName = SaveAndGetName(certificateImageFront);
                model.CertificateImageFront = newFileName;
            }
            if (certificateImageBack != null)
            {
                var newFileName = SaveAndGetName(certificateImageBack);
                model.CertificateImageBack = newFileName;
            }

            if (model.ID > 0)
            {
                var filesToRemove = new List<string>();
                var certificate = _db.Certificates.FirstOrDefault(x => x.ID == model.ID && x.UserID == LS.CurrentUser.ID);
                if (certificate != null)
                {
                    if (model.CertificateImageFront != null)
                    {
                        filesToRemove.Add(certificate.CertificateImageFront);
                        certificate.CertificateImageFront = model.CertificateImageFront;
                    }
                    if (model.CertificateImageBack != null)
                    {
                        filesToRemove.Add(certificate.CertificateImageBack);
                        certificate.CertificateImageBack = model.CertificateImageBack;
                    }
                    certificate.TypeOfCertificate = model.TypeOfCertificate;

                    _db.SaveChanges();
                    RemoveFiles(filesToRemove);
                }
            }
            else
            {
                model.UserID = LS.CurrentUser.ID;
                model.CreateDate = DateTime.UtcNow;
                _db.Certificates.Add(model);
                _db.SaveChanges();
            }

            return RedirectToAction("CertificateList");
        }

        [AuthorizeDivePage]
        public ActionResult DeleteCertificate(int id)
        {
            var certificate = _db.Certificates.FirstOrDefault(x => x.ID == id && x.UserID == LS.CurrentUser.ID);
            if (certificate != null)
            {
                if (certificate.CertificateImageFront != null)
                {
                    RemoveFile(certificate.CertificateImageFront);
                }
                if (certificate.CertificateImageBack != null)
                {
                    RemoveFile(certificate.CertificateImageBack);
                }
                _db.Certificates.Remove(certificate);
                _db.SaveChanges();
            }

            return RedirectToAction("CertificateList");

        }

        [AuthorizeDivePage]
        public ActionResult InsuranceEdit()
        {
            var model = _db.Insurances.FirstOrDefault(x => x.UserID == LS.CurrentUser.ID);
            if (model != null && !model.ExternalInsurance)
            {
                ViewBag.Error = RP.T("DivePage.InsuranceEdit.ExternalInsurance", "לא ניתן לערוך ביטוח חיצוני");
            }

            if (model == null)
            {
                model = new Insurance();
                var today = DateTime.UtcNow.Date;
                model.InsuranceStartDate = today;
                model.InsuranceEndDate = today;
                model.ExternalInsurance = true;
            }

            InitDDLForInsurance(model);

            return View(model);
        }

        [HttpPost]
        [AuthorizeDivePage]
        public ActionResult InsuranceEdit(Insurance model, HttpPostedFileBase filePath)
        {
            if (!IsValidInsuranceFile(filePath))
            {
                model.ExternalInsurance = true;
                InitDDLForInsurance(model);
                return View(model);
            }

            var insurance = _db.Insurances.FirstOrDefault(x => x.UserID == LS.CurrentUser.ID);
            // denied editing
            if (insurance != null && !insurance.ExternalInsurance)
            {
                return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "Index" }) + "#page_2");
            }

            string fileToRemove = null;
            if (filePath != null)
            {
                fileToRemove = insurance?.FilePath;
                var newFileName = SaveAndGetName(filePath);
                model.FilePath = newFileName;
            }

            if (insurance != null)
            {
                if (model.FilePath != null)
                {
                    insurance.FilePath = model.FilePath;
                }
                insurance.InsuranceStartDate = model.InsuranceStartDate;
                insurance.InsuranceEndDate = model.InsuranceEndDate;
                insurance.Organization = model.Organization;
                insurance.TypeOfInsurance = model.TypeOfInsurance;
                _db.SaveChanges();
                RemoveFile(fileToRemove);
            }
            else
            {
                model.UserID = LS.CurrentUser.ID;
                model.CreateDate = DateTime.UtcNow;
                model.ExternalInsurance = true;
                _db.Insurances.Add(model);
                _db.SaveChanges();
            }

            Session["insurance"] = null;

            return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "Index" }) + "#page_2");
        }

        [AuthorizeDivePage]
        public ActionResult DiveAdd()
        {
            var model = new Dive();
            model.DiveDate = DateTime.UtcNow.Date;

            InitDDLForDive();

            return View(model);
        }

        [HttpPost]
        [AuthorizeDivePage]
        public ActionResult DiveAdd(Dive model, HttpPostedFileBase validateImage)
        {
            if (!IsValidFile(validateImage))
            {
                InitDDLForDive();
                return View(model);
            }

            if (validateImage != null)
            {
                var newFileName = SaveAndGetName(validateImage);
                model.ValidateImage = newFileName;
            }

            model.Signature = SaveSignature(model.SignatureData);

            model.DateOfDive = model.DiveDate + model.DiveTime;
            model.UserID = LS.CurrentUser.ID;
            model.CreateDate = DateTime.UtcNow;

            _db.Dives.Add(model);
            _db.SaveChanges();

            return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "Index" }) + "#page_3");
        }

        [AuthorizeDivePage]
        public ActionResult ProfileEdit()
        {
            InitDDLForProfile();

            var user = LS.CurrentUser;
            // default email should be overwrited
            if (user.Email == user.IdNumber + "@id.id")
            {
                user.Email = null;
            }

            return View(user);
        }

        [HttpPost]
        [AuthorizeDivePage]
        public ActionResult ProfileEdit(User model, HttpPostedFileBase userImage, HttpPostedFileBase userBanner)
        {
            if(!IsValidFile(userImage) || !IsValidFile(userBanner))
            {
                InitDDLForProfile();
                return View(model);
            }

            var filesToRemove = new List<string>();
            if (userImage != null)
            {
                filesToRemove.Add(LS.CurrentUser.UserImage);
                var newFileName = SaveAndGetName(userImage);
                model.UserImage = newFileName;
            }
            if (userBanner != null)
            {
                filesToRemove.Add(LS.CurrentUser.UserBanner);
                var newFileName = SaveAndGetName(userBanner);
                model.UserBanner = newFileName;
            }

            var curUser = _db.Users.FirstOrDefault(x => x.ID == LS.CurrentUser.ID);
            var isSuccessfullNewUserRegistration = IsNewUserRegistration();
            if (model.UserImage != null)
            {
                curUser.UserImage = model.UserImage;
            }
            if (model.UserBanner != null)
            {
                curUser.UserBanner = model.UserBanner;
            }
            curUser.FirstName = model.FirstName;
            curUser.LastName = model.LastName;
            curUser.Email = model.Email;
            curUser.Phone = model.Phone;
            curUser.City = model.City;
            curUser.FullNameEnglish = model.FullNameEnglish;
            _db.SaveChanges();
            RemoveFiles(filesToRemove);

            if (isSuccessfullNewUserRegistration)
            {
                return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "Index", isSuccessfullNewUserRegistration = true }));
            }
            return Redirect(Url.RouteUrl(new { controller = "DivePage", action = "Index" }) + "#page_4");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            var model = new User();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User model)
        {
            var isIdNumberValid = SF.IDNumberValidator(model.IdNumber);
            if (!isIdNumberValid)
            {
                ViewBag.Error = RP.T("DivePage.Login.WrongIdNumber", "תעודת זהות לא תקינה");
                return View(model);
            }
            //Validation from 1800 to today year
            var isYearValid = SF.YearValidator(model.Password);
            if (!isYearValid)
            {
                ViewBag.Error = RP.T("DivePage.Login.WrongYear", "שנת לידה לא תקינה");
                return View(model);
            }

            var user = _db.Users.FirstOrDefault(x => x.IdNumber == model.IdNumber);

            var wrongCredentials = RP.T("DivePage.Login.WrongCredentials", "פרטי גישה לא תואמים. בבקשה לפנות לתמיכה במייל.");
            if (user != null && user.Password != model.Password)
            {
                ViewBag.Error = wrongCredentials;
                return View(model);
            }

            if (user == null)
            {
                //register new user - check by idNumber from insurance.idive
                // Use GetInternalUser method pass idNumber
                try
                {
                    user = GetInternalUser(model.IdNumber, model.Password);
                }
                catch (Exception ex)
                {
                    SF.LogError(ex);
                    ViewBag.Error = RP.T("DivePage.Login.CanNotConnect", "אין חיבור לשרת ביטוח.");
                    return View(model);
                }
                if (user.Password != model.Password)
                {
                    ViewBag.Error = wrongCredentials;
                    return View(model);
                }
                _db.Users.Add(user);
                _db.SaveChanges();
                if (user.Email != model.IdNumber + "@id.id")
                {
                    SendEmail(user);
                }
            }

            Insurance insurance = null;
            try
            {
                insurance = GetInternalInsurance(model.IdNumber, user.ID);
            }
            catch (Exception ex)
            {
                SF.LogError(ex);
                ViewBag.Error = RP.T("DivePage.Login.CanNotConnect", "אין חיבור לשרת ביטוח.");
                return View(model);
            }
            if (insurance == null)
            {
                var today = DateTime.UtcNow.Date;
                insurance = _db.Insurances.FirstOrDefault(x => x.UserID == user.ID && x.InsuranceEndDate >= today);
            }
            //Save insurance to session
            Session["insurance"] = insurance;


            FormsAuthentication.SetAuthCookie(model.IdNumber, false);

            var returnUrl = Convert.ToString(Request.QueryString["returnUrl"]);
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        [AuthorizeDivePage]
        public ActionResult Share()
        {
            var url = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/DivePage/View/{LS.CurrentUser.ID}";
            return View("~/Views/DivePage/Share.cshtml", "~/Views/Shared/_LayoutDivePage.cshtml", url);
        }

        [AllowAnonymous]
        public ActionResult View(Guid id)
        {
            ViewBag.User = _db.Users.FirstOrDefault(x => x.ID == id);
            var certificated = _db.Certificates.Where(x => x.UserID == id).ToArray();
            ViewBag.Certificates = certificated;

            //if session have Idive Insurance 
            //ViewBag.Insurance = Idive Insurance
            //else
            if (Session["insurance"] != null)
            {
                ViewBag.Insurance = Session["insurance"];
            }
            else
            {
                ViewBag.Insurance = _db.Insurances.FirstOrDefault(x => x.UserID == id);
            }
            var lastDive = _db.Dives.Where(x => x.UserID == id).OrderByDescending(x => x.DateOfDive).FirstOrDefault();
            ViewBag.LastDive = lastDive;
            var allSettings = _db.SettingsAll.FirstOrDefault();
            var typesOfCertificate0 = allSettings.TypesOfCertificate0.Split(',');
            var typesOfCertificate1 = allSettings.TypesOfCertificate1.Split(',');

            if (lastDive == null)
            {
                ViewBag.IsValidLastDive = false;
            }
            else
            {
                if ((lastDive.DateOfDive >= DateTime.UtcNow.AddMonths(-6) && certificated.Any(x => typesOfCertificate0.Contains(x.TypeOfCertificate)))
                    || certificated.Any(x => typesOfCertificate1.Contains(x.TypeOfCertificate)))
                {
                    ViewBag.IsValidLastDive = true;
                }
                else
                {
                    ViewBag.IsValidLastDive = false;
                }
            }


            return View();
        }

        private string SaveSignature(string data)
        {
            var index = data.IndexOf(",");
            var base64String = data.Substring(index + 1);
            var imageBytes = Convert.FromBase64String(base64String);
            var userId = LS.CurrentUser.ID;
            var folder = $"\\Content\\UserFiles\\{userId}";
            var absolutePathToFolder = Server.MapPath(folder);
            if (!Directory.Exists(absolutePathToFolder))
            {
                Directory.CreateDirectory(absolutePathToFolder);
            }
            var newName = $"{folder}\\{Guid.NewGuid()}.png";
            var filePath = Server.MapPath(newName);
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            newName = newName.Replace("\\", "/");
            return newName;
        }

        private void InitDDLForProfile()
        {
            string AllCitiesInIsrael = RP.T("DivePage.ProfileEdit.AllCitiesInIsrael", "אבטליון,אביאל,אביבים,אביגדור,אביחיל,אביטל,אביעזר,אבירים,אבן יהודה,אבן יצחק,אבן מנחם,אבן ספיר,אבן שמואל,אבני איתן,אבני חפץ,אדירים,אדמית,אדרת,אודים,אודם,אוהד,אומץ,אופקים,אור הגנוז,אור הנר,אור יהודה,אור עקיבא,אורה,אורות,אורטל,אורים,אורנית,אושרת,אזור,אחווה,אחוזם,אחיהוד,אחיטוב,אחיסמך,אחיעזר,אייל,איילת השחר,אילון,אילות,אילניה,אילת,איתמר,איתן,איתנים,אלומה,אלומות,אלון,אלון הגליל,אלון מורה,אלון שבות,אלוני אבא,אלוני הבשן,אלוני יצחק,אלונים,אילי סיני,אליעד,אליכין,אליפז,אליפלט,אליקים,אלישיב,אלישמע,אלמגור,אלמוג,אלעזר,אלפי מנשה,אלקוש,אלקנה,אלרום,אמונים,אמירים,אמנון,אמציה,אניעם,אפיק,אפק,אפרת,ארבל,ארגמן,ארז,אריאל,אשבול,אשדוד,אשדות יעקב,אשחר,אשכולות,אשלים,אשקלון,אשתאול,באר טוביה,באר יעקב,באר שבע,בארות יצחק,בארותיים,בארי,בדולח,בוסתן הגליל,בורגתה,בחן,ביצרון,בית אורן,בית אל,בית אלעזרי,בית אלפא,בית אריה,בית ברל,בית גוברין,בית גמליאל,בית ג'ן,בית דגן,בית הגדי,בית הילל,בית הלוי,בית העמק,בית הערבה,בית השיטה,בית זית,בית זרע,בית חגי,בית חורון,בית חנן,בית חנניה,בית חרות,בית חשמונאי,בית יהושוע,בית יוסף,בית ינאי,בית יצחק,בית לחם הגלילית,בית ליד,בית מאיר,בית מירסים,בית נחמיה,בית ניר,בית נקופה,בית עובד,בית עוזיאל,בית עזרא,בית קמה,בית קשת,בית רבן,בית רימון,בית שאן,בית שמש,בית שערים,בית שקמה,ביתן אהרון,ביתר,בן זכאי,בן עמי,בן שמן,בני ברק,בני דרום,בני דרור,בני יהודה,בני נעים,בני עטרות,בני עי ש,בני עצמון,בני ציון,בני רא ם,בניה,בנימינה,בסמת טבעון,בצרה,בצת,בקוע,בקעות,בר גיורא,בר יוחאי,ברור חיל,ברוש,ברכה,ברעם,ברק,ברקאי,ברקן,ברקת,בת חפר,בת ים,בת עין,בת שלמה,גאולי תימן,גאולים,גבולות,גבים,גבע,גבע בנימין,גבע כרמל,גבעולים,גבעון החדשה,גבעת אבני,גבעת אלה,גבעת ברנר,גבעת השלושה,גבעת זאב,גבעת חיים,גבעת ח ן,גבעת יואב,גבעת יערים,גבעת ישעיהו,גבעת נילי,גבעת עדה,גבעת עוז,גבעת שמואל,גבעת שפירא,גבעתי,גבעתיים,גברעם,גבת,גדות,גדיד,גדיש,גדעונה,גדרה,גונן,גורן,גורנות הגליל,גזית,גזר,גיאה,גיבתון,גיזו,גילגל,גילון,גילת,גינוסר,גינתון,גיתה,גיתית,גלאון,גליל ים,גלעד,גמזו,גן אור,גן הדרום,גן השומרון,גן חיים,גן יאשיה,גן יבנה,גן נר,גן שורק,גן שלמה,גן שמואל,גנות,גנות הדר,גני הדר,גני טל,גני יהודה,גני יוחנן,גני תקווה,גניגר,גנים,געש,געתון,גפן,גרופית,גשור,גשר,גשר הזיו,גת,דליאת א-כרמל,דבורה,דביר,דגניה,דובב,דוברת,דוגית,דולב,דור,דורות,דימונה,דישון,דליה,דלתון,דן,דפנה,דקל,האון,הבונים,הגושרים,הדר עם,הוד השרון,הודיה,הודיות,הושעיה,הזורע,הזורעים,החותרים,היוגב,הילה,המעפיל,הנשיא,הסוללים,העוגן,הר אדר,הראל,הרדוף,הרצליה,הררית,ורד ירחו,זוהר,זימרת,זיקים,זיתן,זיכרון יעקב,זכריה,זנוח,זרועה,זרזיר,זרחיה,זרעית,חבצלת השרון,חברון,חגור,חד נס,חדרה,חולדה,חולון,חולית,חולתה,חומש,חוסן,חופית,חוקוק,חורה,חורפיש,חורשים,חזון,חיבת ציון,חיננית,חיפה,חלוץ,חלמיש,חלץ,חמד,חמדיה,חמרה,חניאל,חניתה,חנתון,חספין,חפץ חיים,חפצי-בה,חצב,חצבה,חצור אשדוד,חצור הגלילית,חצרות יסף,חצרים,חרב לאת,חרוצים,חרות,חריש,חרמש,חרשים,חשמונאים,טבעון,טבריה,טירת יהודה,טירת כרמל,טירת צבי,טל שחר,טללים,טלמון,טנא,טפחות,יאנוח,יבול,יבנאל,יבנה,יגור,יגל,יד בנימין,יד השמונה,יד חנה,יד מרדכי,יד נתן,יד רמבם,ידידיה,יהוד,יהל,יובל,יובלים,יודפת,יוטבתה,יונתן,יוקנעם,יושיביה,יזרעאל,יחיעם,ייטב,יינון,יכיני,ינוב,יסוד המעלה,יסודות,יסעור,יעד,יעלון,יעף,יערה,יערית,יפו,יפית,יפעת,יפתח,יצהר,יציץ,יקום,יקיר,יראון,ירדנה,ירוחם,ירושלים,ירחיב,ירכא,ירקונה,ישע,ישעי,ישרש,יתד,כברי,כדים,כוכב השחר,כוכב יאיר,כוכב יעקב,כוכב מיכאל,כורזים,כחל,כינרת,כיסופים,כלנית,כנות,כנף,כפר אביב,כפר אדומים,כפר אוריה,כפר אזר,כפר אחים,כפר ביאליק,כפר בילו,כפר בלום,כפר בן נון,כפר ברוך,כפר גדעון,כפר גלים,כפר גליקסון,כפר גילעדי,כפר דניאל,כפר דרום,כפר החורש,כפר המכבי,כפר הנגיד,כפר הנשיא,כפר הס,כפר הרואה,כפר הריף,כפר ויתקין,כפר ורבורג,כפר ורדים,כפר זיתים,כפר חבד,כפר חיטים,כפר חיים,כפר חנניה,כפר חסידים,כפר חרוב,כפר טרומן,כפר יאסיף,כפר יהושוע,כפר יונה,כפר יחזקאל,כפר יעבץ,כפר כנא,כפר מונש,כפר מימון,כפר מלל,כפר מנחם,כפר מסריק,כפר מרדכי,כפר נטר,כפר סבא,כפר סולד,כפר סילבר,כפר סירקין,כפר עזה,כפר עציון,כפר פינס,כפר קדום,כפר קיש,כפר קרע,כפר רופין,כפר רות,כפר שמאי,כפר שמואל,כפר שמריהו,כפר תבור,כפר תפוח,כרכום,כרכור,כרם בן זמרה,כרם מהרל,כרם שלום,כרמי יוסף,כרמי צור,כרמיאל,כרמיה,כרמים,כרמל,לבון,לביא,להב,להבות הבשן,להבות חביבה,להבים,לוד,לוחמי הגטאות,לוטם,לוטן,ליבנים,לימן,לכיש,לפידות,מאור,מאיר שפיה,מבוא ביתר,מבוא דותן,מבוא חורון,מבוא חמה,מבוא מודיעים,מבועים,מבטחים,מבקיעים,מבשרת ציון,מגדים,מגדל,מגדל העמק,מגדל עוז,מגדלים,מגידו,מגל,מגן,מגן שאול,מגשימים,מדרך עוז,מודיעין,מולדת,מוצא,מורג,מורן,מורשת,מזור,מזכרת בתיה,מזרע,מזרעה,מחולה,מחניים,מטולה,מטע,מי עמי,מיטב,מיצר,מירב,מירון,מישור אדומים,מישר,מיתר,מכבים,מכורה,מכמורת,מכמנים,מלאה,מלילות,מלכיה,מלכישוע,מנוחה,מנוף,מנורה,מנות,מנחמיה,מנרה,מסד,מסדה,מסילות,מסילת ציון,מעאר,מעברות,מעגלים,מעגן,מעגן מיכאל,מעוז חיים,מעון,מעונה,מעיין ברוך,מעיין צבי,מעיליה,מעלה אדומים,מעלה אפריים,מעלה גלבוע,מעלה גמלא,מעלה החמישה,מעלה לבונה,מעלה מיכמש,מעלה עמוס,מעלה שומרון,מעלות תרשיחא,מענית,מפלסים,מצדות יהודה,מצובה,מצליח,מצפה,מצפה אביב,מצפה יריחו,מצפה נטופה,מצפה רמון,מצפה שלם,מצר,מרגליות,מרום גולן,מרחביה,משאבי שדה,משגב דב,משגב עם,משואה,משואות יצחק,משמר איילון,משמר דוד,משמר הירדן,משמר הנגב,משמר העמק,משמר השבעה,משמר השרון,משמרות,משמרת,משען,מתן,מתת,מתתיהו,נאות גולן,נאות הכיכר,נאות מרדכי,נבטים,נגבה,נהורה,נהלל,נהריה,נוב,נוגה,נווה אור,נווה אטיב,נווה אילן,נווה איתן,נווה אפרים,נווה דניאל,נווה דקלים,נווה זוהר,נווה חריף,נווה ים,נווה ימין,נווה ירק,נווה מבטח,נווה מיכאל,נווה עובד,נווה שלום,נועם,נופים,נופך,נוקדים,נורדיה,נחושה,נחל אבנת,נחל אלישע,נחל אשבל,נחל גבעות,נחל חמדת,נחל משכיות,נחל נמרוד,נחל עוז,נחל רותם,נחל רחלים,נחלה,נחליאל,נחלים,נחם,נחף,נחשולים,נחשון,נחשונים,נטועה,נטור,נטעים,נטף,נילי,ניסנית,ניצני סיני,ניצני עוז,ניצנים,ניר אליהו,ניר בנים,ניר גלים,ניר דוד,ניר חן,ניר יפה,ניר יצחק,ניר ישראל,ניר משה,ניר עוז,ניר עם,ניר עציון,ניר עקיבא,ניר צבי,נירים,נירית,נס הרים,נס עמים,נס ציונה,נערן,נעורים,נעלה,נעמה,נען,נצר חזני,נצר סרני,נצרים,נצרת עלית,נשר,נתיב הגדוד,נתיב הלה,נתיב העשרה,נתיב השיירה,נתיבות,נתניה,סאסא,סביון,סבסטיה,סגולה,סג ור,סוסיה,סופה,סלעית,סעד,ספיר,עברון,עגור,עדי,עולש,עומר,עופר,עופרה,עופרים,עוצם,עותניאל,עזוז,עזר,עזריאל,עזריה,עזריקם,עטרת,עידן,עיינות,עין איילה,עין גב,עין גדי,עין דור,עין הבשור,עין הוד,עין החורש,עין המפרץ,עין הנציב,עין העמק,עין השופט,עין השלושה,עין ורד,עין זיוון,עין חוד,עין חצבה,עין חרוד,עין יהב,עין יעקב,עין כרמל,עין מאהל,עין עירון,עין צורים,עין שמר,עין שריד,עין תמר,עינת,עוספיא,עכו,עלומים,עלי,עלי זהב,עלמה,עלמון,עמוקה,עמינדב,עמיעד,עמיעוז,עמיקם,עמיר,עמישב,עמנואל,עמקה,ענב,עפולה,עץ אפרים,ערבונה,ערד,ערוגות,ערוער,עשרת,עתלית,פארן,פאת שדה,פדואל,פדויים,פדייה,פוריה,פורת,פטיש,פי נר,פלך,פלמחים,פני חבר,פסגות,פעמי תשז,פצאל,פקיעין,פרדס חנה,פרדסיה,פרי גן,פתח תקווה,פתחיה,צאלים,צביה,צבעון,צובה,צופיה,צופים,צופית,צופר,צור הדסה,צור יגאל,צור משה,צור נתן,צוריאל,צורית,צורן,ציפורי,צלפון,צפריה,צפרירים,צפת,צרופה,צרעה,קבוצת יבנה,קדומים,קדימה,קדמה,קדמת צבי,קדרון,קדרים,קדש ברנע,קוממיות,קורנית,קטורה,קטיף,קטנה,קידר,קיסריה,קלחים,קליה,קלע,קציר,קצרין,קריות,קריית אונו,קריית ארבע,קריית אתא,קריית ביאליק,קריית גת,קריית חיים,קריית טבעון,קריית ים,קריית יערים,קריית מוצקין,קריית מלאכי,קריית נטפים,קריית ספר,קריית ענבים,קריית עקרון,קריית שמונה,קרני שומרון,קשת,ראש הניקרה,ראש העין,ראש פינה,ראש צורים,ראשון לציון,רבבה,רבדים,רביבים,רביד,רגבה,רגבים,רהט,רוגלית,רווחה,רוויה,רוחמה,רועי,רחוב,רחובות,ריחן,רימונים,רכסים,רם און,רמה,רמון,רמות,רמות השבים,רמות מאיר,רמות מנשה,רמות נפתלי,רמלה,רמת אפעל,רמת גן,רמת דוד,רמת הכובש,רמת השופט,רמת השרון,רמת יוחנן,רמת ישי,רמת מגשימים,רמת פנקס,רמת צבי,רמת רזיאל,רמת רחל,רנן,רעות,רעים,רעננה,רפיח ים,רקפת,רשפון,רשפים,רתמים,שאר ישוב,שבי ציון,שבי שומרון,שגב,שגב שלום,שדה אילן,שדה אליהו,שדה אליעזר,שדה בוקר,שדה דוד,שדה ורבורג,שדה יואב,שדה יעקב,שדה יצחק,שדה משה,שדה נחום,שדה נחמיה,שדה ניצן,שדה עוזיהו,שדה צבי,שדות ים,שדות מיכה,שדי אברהם,שדי חמד,שדי תרומות,שדמה,שדמות דבורה,שדמות מחולה,שדרות,שהם,שואבה,שובה,שובל,שומרה,שומריה,שומרת,שורש,שורשים,שושנת העמקים,שזור,שחר,שחרות,שיבולים,שיזפון,שילה,שילת,שלווה,שלוחות,שלומי,שליו,שמיר,שני,שניר,שעל,שעלבים,שער אפרים,שער הגולן,שער העמקים,שער מנשה,שערי תקווה,שפיים,שפיר,שפר,שפרעם,שקד,שקף,שרונה,שריד,שרשרת,שתולה,שתולים,תדהר,תובל,תומר,תושייה,תימורים,תירוש,תל אביב,תל יוסף,תל יצחק,תל מונד,תל עדשים,תל קציר,תל שבע,תל תאומים,תלם,תלמי אליהו,תלמי אלעזר,תלמי בילו,תלמי יוסף,תלמי יחיאל,תלמי יפה,תלמי מנשה,תלמים,תנובות,תעוז,תפוח,תפרח,תקומה,תקוע,תרום");
            var cities = AllCitiesInIsrael.Split(',');
            var list = cities.Select(x => new SelectListItem
            {
                Selected = false,
                Text = x,
                Value = x
            });

            ViewBag.Cities = new SelectList(list, "Value", "Text");
        }

        private void InitDDLForDive()
        {
            var list = new Dictionary<int, string>
    {
        { (int)AirTank.None, RP.T("DivePage.DiveAdd.AirTank.None", "בחר מיכל אוויר") },
        { (int)AirTank.Ten, "10" },
        { (int)AirTank.Fifteen, "15" },
        { (int)AirTank.Twenty, "20" },
    };

            ViewBag.AirTanks = new SelectList(list, "Key", "Value");

            list = new Dictionary<int, string>
            {
                { (int)AirDiveOrNitrox.None, RP.T("DivePage.DiveAdd.AirDiveOrNitrox.None", "בחר סוג אוויר")},
                { (int)AirDiveOrNitrox.Air, RP.T("DivePage.DiveAdd.AirDiveOrNitrox.Air", "צלילת אוויר")},
                { (int)AirDiveOrNitrox.Nitrox, RP.T("DivePage.DiveAdd.AirDiveOrNitrox.Nitrox", "צלילת נייטרוקס")},
            };

            ViewBag.AirDiveOrNitrox = new SelectList(list, "Key", "Value");

            list = new Dictionary<int, string>
            {
                { (int)ValidateType.None, RP.T("DivePage.DiveAdd.ValidateType.None", "בחר סוג אימות")},
                { (int)ValidateType.Receipt, RP.T("DivePage.DiveAdd.ValidateType.Receipt", "צילום   קבלה לשרותי המועדון")},
                { (int)ValidateType.Selfie, RP.T("DivePage.DiveAdd.ValidateType.Selfie", "צילום סלפי על רקע שלט מועדון﻿")},
                { (int)ValidateType.ExternalPage, RP.T("DivePage.DiveAdd.ValidateType.ExternalPage", "יומן צלילות מודפס")},
            };

            ViewBag.ValidateTypes = new SelectList(list, "Key", "Value");
        }

        private void InitDDLForInsurance(Insurance insurance)
        {
            var allSettings = _db.SettingsAll.AsNoTracking().First();
            var insuranceOrganizations = allSettings.InsuranceOrganizations.Split(',');
            var list = insuranceOrganizations.Select(x => new SelectListItem
            {
                Selected = insurance.Organization == x,
                Text = x,
                Value = x
            });

            ViewBag.Organizations = new SelectList(list, "Value", "Text");

            var typesOfInsurance = allSettings.TypesOfInsurance.Split(',');
            list = typesOfInsurance.Select(x => new SelectListItem
            {
                Selected = insurance.TypeOfInsurance == x,
                Text = x,
                Value = x
            });

            ViewBag.TypesOfInsurance = new SelectList(list, "Value", "Text");
        }

        private void InitDDLForCertificate(Certificate certificate)
        {
            var allSettings = _db.SettingsAll.AsNoTracking().First();
            var typesOfCertificate = string.Concat(allSettings.TypesOfCertificate0, ",", allSettings.TypesOfCertificate1).Split(',');
            var list = typesOfCertificate.Select(x => new SelectListItem
            {
                Selected = certificate == null ? false : certificate.TypeOfCertificate == x,
                Text = x,
                Value = x
            });

            ViewBag.TypesOfCertificate = new SelectList(list, "Value", "Text");
        }

        private string SaveAndGetName(HttpPostedFileBase file)
        {
            var fileName = file.FileName;
            var extension = System.IO.Path.GetExtension(fileName);
            var userId = LS.CurrentUser.ID;
            var folder = $"\\Content\\UserFiles\\{userId}";
            var absolutePathToFolder = Server.MapPath(folder);
            if (!Directory.Exists(absolutePathToFolder))
            {
                Directory.CreateDirectory(absolutePathToFolder);
            }

            var newName = $"{folder}\\{Guid.NewGuid().ToString() + extension}";
            var filePath = Server.MapPath(newName);
            file.SaveAs(filePath);
            newName = newName.Replace("\\", "/");

            return newName;
        }

        private void RemoveFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                RemoveFile(file);
            }
        }

        private void RemoveFile(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return;
            }
            var relativePath = relativeUrl.Replace('/', '\\');
            var absolutePath = Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }
        }

        private void SendEmail(User model)
        {
            var subject = RP.T("DivePage.Login.Subject", "You successfully registered on idive");
            var body = "<div style='text-align: right; direction: rtl;'>" +
                    RP.GetTextComponent("SuccessRegistrationEmail").Replace("{IdNumber}", model.IdNumber).Replace("{Password}", model.Password) + "</div>";

            _db.OutEmails.Add(new OutEmail
            {
                MailTo = model.Email,
                Subject = subject,

                Body = body,
                TimesSent = 0,
                LastTry = DateTime.Now
            });
            _db.SaveChanges();
        }

        private bool IsFileSizeExceed(HttpPostedFileBase file)
        {
            var maxFileSize = int.Parse(WebConfigurationManager.AppSettings["maxFileSize"]);

            if (file != null && file.ContentLength > maxFileSize)
            {
                ViewBag.Error = RP.T("DivePage.FileSizeLimit", "בבקשה לעלות תמונה בדוגל מקסימאלע של 5MB");
                return true;
            }
            return false;
        }

        private User GetInternalUser(string idNumber, string password)
        {
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["InsuranceDb"].ConnectionString;
            var user = new User()
            {
                ID = Guid.NewGuid(),
                IdNumber = idNumber,
                Roles = "User",
                Email = idNumber + "@id.id",
                CreationDate = DateTime.UtcNow,
                Password = password
            };

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var sql = "SELECT TOP 1 [BirthDate], [UserFname], [UserLname], [UserFnameEng], [UserLnameEng], [City], [Mobile], [Mail] FROM [TravelInsurances] WHERE [IdNumber] = @IdNumber AND [ToDelete] = 0 AND [IsCleared] = 1 ORDER BY [PayDate] DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    var idNumberParameter = new SqlParameter("@IdNumber", System.Data.SqlDbType.NVarChar, 200);
                    idNumberParameter.Value = idNumber;
                    cmd.Parameters.Add(idNumberParameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var birthday = reader.GetDateTime(0);
                            user.Password = birthday.Year.ToString();
                            user.FirstName = reader.GetString(1);
                            user.LastName = reader.GetString(2);
                            var firstNameEn = reader.GetString(3);
                            var lastNameEn = reader.GetString(4);
                            user.FullNameEnglish = string.Concat(firstNameEn, " ", lastNameEn);
                            user.City = reader.GetString(5);
                            if (!reader.IsDBNull(6))
                            {
                                user.Phone = reader.GetString(6);
                            }
                            user.Email = reader.GetString(7);
                        }
                    }
                }
            }
            return user;
        }

        private Insurance GetInternalInsurance(string idNumber, Guid userId)
        {
            Insurance insurance = null;
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["InsuranceDb"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var sql = "SELECT TOP 1 [InsuranceDate], [Days], (SELECT TOP 1 Title FROM AbstractPages where ID = [TravelInsurances].TravelInsurancePeriods) AS InsuranceType, [ID] FROM [TravelInsurances] WHERE [IdNumber] = @IdNumber AND [ToDelete] = 0 AND [IsCleared] = 1 AND DATEADD(minute, -1, CONVERT(datetime, CONVERT(date, DATEADD(day, [Days], [InsuranceDate])))) >=GETDATE() ORDER BY [PayDate]";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    var idNumberParameter = new SqlParameter("@IdNumber", System.Data.SqlDbType.NVarChar, 200);
                    idNumberParameter.Value = idNumber;
                    cmd.Parameters.Add(idNumberParameter);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var insuranceDate = reader.GetDateTime(0);
                            var insuranceDays = reader.GetInt32(1);
                            var insuranceType = reader.GetString(2);
                            var orderId = reader.GetInt32(3);
                            insurance = new Insurance
                            {
                                Organization = RP.T("Insurance.InternalOrganization", "Idive"),
                                CreateDate = DateTime.UtcNow,
                                InsuranceStartDate = insuranceDate,
                                InsuranceEndDate = insuranceDate.AddDays(insuranceDays).Date.AddMinutes(-1),
                                ExternalInsurance = false,
                                FileType = FileType.None,
                                UserID = userId,
                                OrderId = orderId
                            };
                        }
                    }
                }
            }

            return insurance;
        }

        private bool IsNewUserRegistration()
        {
            var user = LS.CurrentUser;
            if (user.Email == user.IdNumber + "@id.id")
            {
                return true;
            }
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.Phone))
            {
                return true;
            }

            return false;
        }

        private bool IsValidFile(HttpPostedFileBase file)
        {
            if(file == null)
            {
                return true;
            }

            if (IsFileSizeExceed(file))
            {
                return false;
            }

            var filename = file.FileName;
            var extention = Path.GetExtension(filename);
            if (extention == ".jpg" || extention == ".png")
            {
                return true;
            }
            ViewBag.Error = RP.T("DivePage.InvalidExtention", "קובץ מסוג {0} לא מותר").Replace("{0}", extention);
            return false;
        }

        private bool IsValidInsuranceFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return true;
            }

            if (IsFileSizeExceed(file))
            {
                return false;
            }

            var filename = file.FileName;
            var extention = Path.GetExtension(filename);
            if (extention == ".jpg" || extention == ".png" || extention == ".pdf")
            {
                return true;
            }
            ViewBag.Error = RP.T("DivePage.InvalidExtention", "קובץ מסוג {0} לא מותר").Replace("{0}", extention);
            return false;
        }
    }
}
