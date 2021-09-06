using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using System.Runtime;
using System.Runtime.Caching;
using System.Data.SqlClient;

namespace Uco.Infrastructure
{
    public static partial class SF
    {
        public static DateTime shortDateNow
        {
            get
            {
                if (MemoryCache.Default["shotDateNow"] == null)
                {
                    MemoryCache.Default["shotDateNow"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                }
                else
                {
                    DateTime _shortDateNow = (DateTime)MemoryCache.Default["shotDateNow"];
                    if (_shortDateNow.Hour != DateTime.Now.Hour)
                    {
                        _shortDateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        MemoryCache.Default["shotDateNow"] = _shortDateNow;
                    }
                }
                return (DateTime)MemoryCache.Default["shotDateNow"];

            }
        }

        #region Banners

        public static void BannersAddClick(int BannersId)
        {
            try
            {
                List<BannersStatistic> list = MemoryCache.Default["BannerList"] as List<BannersStatistic>;

                if (list == null)
                {
                    list = new List<BannersStatistic>();
                }

                if (list.Any(b => b.BannerID == BannersId && b.Date == shortDateNow))
                {
                    BannersStatistic item = list.FirstOrDefault(b => b.BannerID == BannersId && b.Date == shortDateNow);
                    item.CountClicks++;
                }
                else
                {
                    var item = new BannersStatistic()
                    {
                        Date = shortDateNow,
                        BannerID = BannersId,
                        CountViews = 0,
                        CountClicks = 1
                    };
                    list.Add(item);
                }

                MemoryCache.Default["BannerList"] = list;

            }
            catch
            {

            }
        }

        public static void BannersAddView(int BannersId)
        {
            try
            {
                List<BannersStatistic> list = MemoryCache.Default["BannerList"] as List<BannersStatistic>;

                if (list == null)
                {
                    list = new List<BannersStatistic>();
                }


                if (list.Any(b => b.BannerID == BannersId && b.Date == shortDateNow))
                {
                    BannersStatistic item = list.FirstOrDefault(b => b.BannerID == BannersId && b.Date == shortDateNow);
                    item.CountViews++;
                }
                else
                {
                    var item = new BannersStatistic()
                    {
                        Date = shortDateNow,
                        BannerID = BannersId,
                        CountViews = 1,
                        CountClicks = 0
                    };
                    list.Add(item);
                }

                MemoryCache.Default["BannerList"] = list;

            }
            catch
            {

            }
        }

        public static int CountBannersClicks(int BannersId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var _startDate = String.Format("{0:yyyy/MM/dd 00:00:00.000}", startDate);
                var _endDate = String.Format("{0:yyyy/MM/dd 23:59:59.999}", endDate);

                //var _startDate = String.Format("{0:yyyy/MM/dd/}", startDate).Replace("/", "-");
                //_startDate += " 00:00:00.000";
                //var _endDate = String.Format("{0:yyyy/MM/dd/}", endDate).Replace("/", "-");
                //_endDate += " 23:59:59.999";

                string query = "SELECT Sum(CountClicks)" +
                            " FROM [dbo].[BannersStatistics] " +
                            " where [dbo].[BannersStatistics].[BannerID] = " + BannersId +
                            " And [dbo].[BannersStatistics].[Date] >= '" + _startDate + "'" +
                            " And [dbo].[BannersStatistics].[Date] < '" + _endDate + "'" +
                             " Group by [dbo].[BannersStatistics].[BannerID]";

                int ClickCount = _db.Database.SqlQuery<int>(query).SingleOrDefault();

                return ClickCount;
            }
            catch
            { return 0; }
        }

        public static int CountBannersViews(int BannersId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var _startDate = String.Format("{0:yyyy/MM/dd 00:00:00.000}", startDate);
                var _endDate = String.Format("{0:yyyy/MM/dd 23:59:59.999}", endDate);

                //var _startDate = String.Format("{0:yyyy/MM/dd/}", startDate).Replace("/", "-");
                //_startDate += " 00:00:00.000";
                //var _endDate = String.Format("{0:yyyy/MM/dd/}", endDate).Replace("/", "-");
                //_endDate += " 23:59:59.999";

                string query = "SELECT Sum(CountViews)" +
                            " FROM [dbo].[BannersStatistics] " +
                            " where [dbo].[BannersStatistics].[BannerID] = " + BannersId +
                            " And [dbo].[BannersStatistics].[Date] >= '" + _startDate + "'" +
                            " And [dbo].[BannersStatistics].[Date] < '" + _endDate + "'" +
                             " Group by [dbo].[BannersStatistics].[BannerID]";

                int ClickCount = _db.Database.SqlQuery<int>(query).SingleOrDefault();

                return ClickCount;
            }
            catch
            {
                return 0;
            }
        }
        #endregion



        public static void RegisterToNewsletter(string name, string email, string idNumber, string data)
        {

            bool isExist = _db.Newsletters.Any(r => r.NewsletterEmail == email);

            if (!isExist)
            {
                Newsletter newsletter = new Newsletter
                {
                    DomainID = RP.GetCurrentSettings().ID,
                    NewsletterAccept = true,
                    NewsletterDate = DateTime.Now,
                    NewsletterEmail = email,
                    NewsletterIdNumber = idNumber,
                    NewsletterName = name,
                    NewsletterData = data
                };

                _db.Newsletters.Add(newsletter);
                _db.SaveChanges();
            }
        }



        public static List<string> GetCities()
        {
            string AllCitiesInIsrael = "אבטליון,אביאל,אביבים,אביגדור,אביחיל,אביטל,אביעזר,אבירים,אבן יהודה,אבן יצחק,אבן מנחם,אבן ספיר,אבן שמואל,אבני איתן,אבני חפץ,אדירים,אדמית,אדרת,אודים,אודם,אוהד,אומץ,אופקים,אור הגנוז,אור הנר,אור יהודה,אור עקיבא,אורה,אורות,אורטל,אורים,אורנית,אושרת,אזור,אחווה,אחוזם,אחיהוד,אחיטוב,אחיסמך,אחיעזר,אייל,איילת השחר,אילון,אילות,אילניה,אילת,איתמר,איתן,איתנים,אלומה,אלומות,אלון,אלון הגליל,אלון מורה,אלון שבות,אלוני אבא,אלוני הבשן,אלוני יצחק,אלונים,אילי סיני,אליעד,אליכין,אליפז,אליפלט,אליקים,אלישיב,אלישמע,אלמגור,אלמוג,אלעזר,אלפי מנשה,אלקוש,אלקנה,אלרום,אמונים,אמירים,אמנון,אמציה,אניעם,אפיק,אפק,אפרת,ארבל,ארגמן,ארז,אריאל,אשבול,אשדוד,אשדות יעקב,אשחר,אשכולות,אשלים,אשקלון,אשתאול,באר טוביה,באר יעקב,באר שבע,בארות יצחק,בארותיים,בארי,בדולח,בוסתן הגליל,בורגתה,בחן,ביצרון,בית אורן,בית אל,בית אלעזרי,בית אלפא,בית אריה,בית ברל,בית גוברין,בית גמליאל,בית ג'ן,בית דגן,בית הגדי,בית הילל,בית הלוי,בית העמק,בית הערבה,בית השיטה,בית זית,בית זרע,בית חגי,בית חורון,בית חנן,בית חנניה,בית חרות,בית חשמונאי,בית יהושוע,בית יוסף,בית ינאי,בית יצחק,בית לחם הגלילית,בית ליד,בית מאיר,בית מירסים,בית נחמיה,בית ניר,בית נקופה,בית עובד,בית עוזיאל,בית עזרא,בית קמה,בית קשת,בית רבן,בית רימון,בית שאן,בית שמש,בית שערים,בית שקמה,ביתן אהרון,ביתר,בן זכאי,בן עמי,בן שמן,בני ברק,בני דרום,בני דרור,בני יהודה,בני נעים,בני עטרות,בני עי ש,בני עצמון,בני ציון,בני רא ם,בניה,בנימינה,בסמת טבעון,בצרה,בצת,בקוע,בקעות,בר גיורא,בר יוחאי,ברור חיל,ברוש,ברכה,ברעם,ברק,ברקאי,ברקן,ברקת,בת חפר,בת ים,בת עין,בת שלמה,גאולי תימן,גאולים,גבולות,גבים,גבע,גבע בנימין,גבע כרמל,גבעולים,גבעון החדשה,גבעת אבני,גבעת אלה,גבעת ברנר,גבעת השלושה,גבעת זאב,גבעת חיים,גבעת ח ן,גבעת יואב,גבעת יערים,גבעת ישעיהו,גבעת נילי,גבעת עדה,גבעת עוז,גבעת שמואל,גבעת שפירא,גבעתי,גבעתיים,גברעם,גבת,גדות,גדיד,גדיש,גדעונה,גדרה,גונן,גורן,גורנות הגליל,גזית,גזר,גיאה,גיבתון,גיזו,גילגל,גילון,גילת,גינוסר,גינתון,גיתה,גיתית,גלאון,גליל ים,גלעד,גמזו,גן אור,גן הדרום,גן השומרון,גן חיים,גן יאשיה,גן יבנה,גן נר,גן שורק,גן שלמה,גן שמואל,גנות,גנות הדר,גני הדר,גני טל,גני יהודה,גני יוחנן,גני תקווה,גניגר,גנים,געש,געתון,גפן,גרופית,גשור,גשר,גשר הזיו,גת,דליאת א-כרמל,דבורה,דביר,דגניה,דובב,דוברת,דוגית,דולב,דור,דורות,דימונה,דישון,דליה,דלתון,דן,דפנה,דקל,האון,הבונים,הגושרים,הדר עם,הוד השרון,הודיה,הודיות,הושעיה,הזורע,הזורעים,החותרים,היוגב,הילה,המעפיל,הנשיא,הסוללים,העוגן,הר אדר,הראל,הרדוף,הרצליה,הררית,ורד ירחו,זוהר,זימרת,זיקים,זיתן,זיכרון יעקב,זכריה,זנוח,זרועה,זרזיר,זרחיה,זרעית,חבצלת השרון,חברון,חגור,חד נס,חדרה,חולדה,חולון,חולית,חולתה,חומש,חוסן,חופית,חוקוק,חורה,חורפיש,חורשים,חזון,חיבת ציון,חיננית,חיפה,חלוץ,חלמיש,חלץ,חמד,חמדיה,חמרה,חניאל,חניתה,חנתון,חספין,חפץ חיים,חפצי-בה,חצב,חצבה,חצור אשדוד,חצור הגלילית,חצרות יסף,חצרים,חרב לאת,חרוצים,חרות,חריש,חרמש,חרשים,חשמונאים,טבעון,טבריה,טירת יהודה,טירת כרמל,טירת צבי,טל שחר,טללים,טלמון,טנא,טפחות,יאנוח,יבול,יבנאל,יבנה,יגור,יגל,יד בנימין,יד השמונה,יד חנה,יד מרדכי,יד נתן,יד רמבם,ידידיה,יהוד,יהל,יובל,יובלים,יודפת,יוטבתה,יונתן,יוקנעם,יושיביה,יזרעאל,יחיעם,ייטב,יינון,יכיני,ינוב,יסוד המעלה,יסודות,יסעור,יעד,יעלון,יעף,יערה,יערית,יפו,יפית,יפעת,יפתח,יצהר,יציץ,יקום,יקיר,יראון,ירדנה,ירוחם,ירושלים,ירחיב,ירכא,ירקונה,ישע,ישעי,ישרש,יתד,כברי,כדים,כוכב השחר,כוכב יאיר,כוכב יעקב,כוכב מיכאל,כורזים,כחל,כינרת,כיסופים,כלנית,כנות,כנף,כפר אביב,כפר אדומים,כפר אוריה,כפר אזר,כפר אחים,כפר ביאליק,כפר בילו,כפר בלום,כפר בן נון,כפר ברוך,כפר גדעון,כפר גלים,כפר גליקסון,כפר גילעדי,כפר דניאל,כפר דרום,כפר החורש,כפר המכבי,כפר הנגיד,כפר הנשיא,כפר הס,כפר הרואה,כפר הריף,כפר ויתקין,כפר ורבורג,כפר ורדים,כפר זיתים,כפר חבד,כפר חיטים,כפר חיים,כפר חנניה,כפר חסידים,כפר חרוב,כפר טרומן,כפר יאסיף,כפר יהושוע,כפר יונה,כפר יחזקאל,כפר יעבץ,כפר כנא,כפר מונש,כפר מימון,כפר מלל,כפר מנחם,כפר מסריק,כפר מרדכי,כפר נטר,כפר סבא,כפר סולד,כפר סילבר,כפר סירקין,כפר עזה,כפר עציון,כפר פינס,כפר קדום,כפר קיש,כפר קרע,כפר רופין,כפר רות,כפר שמאי,כפר שמואל,כפר שמריהו,כפר תבור,כפר תפוח,כרכום,כרכור,כרם בן זמרה,כרם מהרל,כרם שלום,כרמי יוסף,כרמי צור,כרמיאל,כרמיה,כרמים,כרמל,לבון,לביא,להב,להבות הבשן,להבות חביבה,להבים,לוד,לוחמי הגטאות,לוטם,לוטן,ליבנים,לימן,לכיש,לפידות,מאור,מאיר שפיה,מבוא ביתר,מבוא דותן,מבוא חורון,מבוא חמה,מבוא מודיעים,מבועים,מבטחים,מבקיעים,מבשרת ציון,מגדים,מגדל,מגדל העמק,מגדל עוז,מגדלים,מגידו,מגל,מגן,מגן שאול,מגשימים,מדרך עוז,מודיעין,מולדת,מוצא,מורג,מורן,מורשת,מזור,מזכרת בתיה,מזרע,מזרעה,מחולה,מחניים,מטולה,מטע,מי עמי,מיטב,מיצר,מירב,מירון,מישור אדומים,מישר,מיתר,מכבים,מכורה,מכמורת,מכמנים,מלאה,מלילות,מלכיה,מלכישוע,מנוחה,מנוף,מנורה,מנות,מנחמיה,מנרה,מסד,מסדה,מסילות,מסילת ציון,מעאר,מעברות,מעגלים,מעגן,מעגן מיכאל,מעוז חיים,מעון,מעונה,מעיין ברוך,מעיין צבי,מעיליה,מעלה אדומים,מעלה אפריים,מעלה גלבוע,מעלה גמלא,מעלה החמישה,מעלה לבונה,מעלה מיכמש,מעלה עמוס,מעלה שומרון,מעלות תרשיחא,מענית,מפלסים,מצדות יהודה,מצובה,מצליח,מצפה,מצפה אביב,מצפה יריחו,מצפה נטופה,מצפה רמון,מצפה שלם,מצר,מרגליות,מרום גולן,מרחביה,משאבי שדה,משגב דב,משגב עם,משואה,משואות יצחק,משמר איילון,משמר דוד,משמר הירדן,משמר הנגב,משמר העמק,משמר השבעה,משמר השרון,משמרות,משמרת,משען,מתן,מתת,מתתיהו,נאות גולן,נאות הכיכר,נאות מרדכי,נבטים,נגבה,נהורה,נהלל,נהריה,נוב,נוגה,נווה אור,נווה אטיב,נווה אילן,נווה איתן,נווה אפרים,נווה דניאל,נווה דקלים,נווה זוהר,נווה חריף,נווה ים,נווה ימין,נווה ירק,נווה מבטח,נווה מיכאל,נווה עובד,נווה שלום,נועם,נופים,נופך,נוקדים,נורדיה,נחושה,נחל אבנת,נחל אלישע,נחל אשבל,נחל גבעות,נחל חמדת,נחל משכיות,נחל נמרוד,נחל עוז,נחל רותם,נחל רחלים,נחלה,נחליאל,נחלים,נחם,נחף,נחשולים,נחשון,נחשונים,נטועה,נטור,נטעים,נטף,נילי,ניסנית,ניצני סיני,ניצני עוז,ניצנים,ניר אליהו,ניר בנים,ניר גלים,ניר דוד,ניר חן,ניר יפה,ניר יצחק,ניר ישראל,ניר משה,ניר עוז,ניר עם,ניר עציון,ניר עקיבא,ניר צבי,נירים,נירית,נס הרים,נס עמים,נס ציונה,נערן,נעורים,נעלה,נעמה,נען,נצר חזני,נצר סרני,נצרים,נצרת עלית,נשר,נתיב הגדוד,נתיב הלה,נתיב העשרה,נתיב השיירה,נתיבות,נתניה,סאסא,סביון,סבסטיה,סגולה,סג ור,סוסיה,סופה,סלעית,סעד,ספיר,עברון,עגור,עדי,עולש,עומר,עופר,עופרה,עופרים,עוצם,עותניאל,עזוז,עזר,עזריאל,עזריה,עזריקם,עטרת,עידן,עיינות,עין איילה,עין גב,עין גדי,עין דור,עין הבשור,עין הוד,עין החורש,עין המפרץ,עין הנציב,עין העמק,עין השופט,עין השלושה,עין ורד,עין זיוון,עין חוד,עין חצבה,עין חרוד,עין יהב,עין יעקב,עין כרמל,עין מאהל,עין עירון,עין צורים,עין שמר,עין שריד,עין תמר,עינת,עוספיא,עכו,עלומים,עלי,עלי זהב,עלמה,עלמון,עמוקה,עמינדב,עמיעד,עמיעוז,עמיקם,עמיר,עמישב,עמנואל,עמקה,ענב,עפולה,עץ אפרים,ערבונה,ערד,ערוגות,ערוער,עשרת,עתלית,פארן,פאת שדה,פדואל,פדויים,פדייה,פוריה,פורת,פטיש,פי נר,פלך,פלמחים,פני חבר,פסגות,פעמי תשז,פצאל,פקיעין,פרדס חנה,פרדסיה,פרי גן,פתח תקווה,פתחיה,צאלים,צביה,צבעון,צובה,צופיה,צופים,צופית,צופר,צור הדסה,צור יגאל,צור משה,צור נתן,צוריאל,צורית,צורן,ציפורי,צלפון,צפריה,צפרירים,צפת,צרופה,צרעה,קבוצת יבנה,קדומים,קדימה,קדמה,קדמת צבי,קדרון,קדרים,קדש ברנע,קוממיות,קורנית,קטורה,קטיף,קטנה,קידר,קיסריה,קלחים,קליה,קלע,קציר,קצרין,קריות,קריית אונו,קריית ארבע,קריית אתא,קריית ביאליק,קריית גת,קריית חיים,קריית טבעון,קריית ים,קריית יערים,קריית מוצקין,קריית מלאכי,קריית נטפים,קריית ספר,קריית ענבים,קריית עקרון,קריית שמונה,קרני שומרון,קשת,ראש הניקרה,ראש העין,ראש פינה,ראש צורים,ראשון לציון,רבבה,רבדים,רביבים,רביד,רגבה,רגבים,רהט,רוגלית,רווחה,רוויה,רוחמה,רועי,רחוב,רחובות,ריחן,רימונים,רכסים,רם און,רמה,רמון,רמות,רמות השבים,רמות מאיר,רמות מנשה,רמות נפתלי,רמלה,רמת אפעל,רמת גן,רמת דוד,רמת הכובש,רמת השופט,רמת השרון,רמת יוחנן,רמת ישי,רמת מגשימים,רמת פנקס,רמת צבי,רמת רזיאל,רמת רחל,רנן,רעות,רעים,רעננה,רפיח ים,רקפת,רשפון,רשפים,רתמים,שאר ישוב,שבי ציון,שבי שומרון,שגב,שגב שלום,שדה אילן,שדה אליהו,שדה אליעזר,שדה בוקר,שדה דוד,שדה ורבורג,שדה יואב,שדה יעקב,שדה יצחק,שדה משה,שדה נחום,שדה נחמיה,שדה ניצן,שדה עוזיהו,שדה צבי,שדות ים,שדות מיכה,שדי אברהם,שדי חמד,שדי תרומות,שדמה,שדמות דבורה,שדמות מחולה,שדרות,שהם,שואבה,שובה,שובל,שומרה,שומריה,שומרת,שורש,שורשים,שושנת העמקים,שזור,שחר,שחרות,שיבולים,שיזפון,שילה,שילת,שלווה,שלוחות,שלומי,שליו,שמיר,שני,שניר,שעל,שעלבים,שער אפרים,שער הגולן,שער העמקים,שער מנשה,שערי תקווה,שפיים,שפיר,שפר,שפרעם,שקד,שקף,שרונה,שריד,שרשרת,שתולה,שתולים,תדהר,תובל,תומר,תושייה,תימורים,תירוש,תל אביב,תל יוסף,תל יצחק,תל מונד,תל עדשים,תל קציר,תל שבע,תל תאומים,תלם,תלמי אליהו,תלמי אלעזר,תלמי בילו,תלמי יוסף,תלמי יחיאל,תלמי יפה,תלמי מנשה,תלמים,תנובות,תעוז,תפוח,תפרח,תקומה,תקוע,תרום";
            var cities = AllCitiesInIsrael.Split(',');
            return cities.ToList();
        }


        public static bool IDNumberValidator(string strID)
        {
            int stringInt;
            bool _tryParse = int.TryParse(strID, out stringInt);
            if (_tryParse == false)
                return _tryParse;

            int[] id_12_digits = { 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            int count = 0;

            if (strID == null)
                return false;

            strID = strID.PadLeft(9, '0');

            for (int i = 0; i < 9; i++)
            {
                int num = Int32.Parse(strID.Substring(i, 1)) * id_12_digits[i];

                if (num > 9)
                    num = (num / 10) + (num % 10);

                count += num;
            }

            return (count % 10 == 0);
        }


        // email for buyer of the shop
        public static string SendOrderPayed(ShopOrder o)
        {
            //string _Url = string.Format("http://" + LS.CurrentHttpContext.Request.ServerVariables["HTTP_HOST"].ToString() + "/Insurance/PaymentSuccess?OrderID={0}&IdNumber={1}&birthyear={2}", o.ID, o.IdNumber, o.BirthDate.Year);
            //string Link = "<a style='color: #378515;' href='" + _Url + "'>" + "מס' פוליסה: " + o.InsuranceDate.Year + "/" + o.ID + "</a>";
            //TravelInsurancePeriodPage _page = _db.TravelInsurancePeriodPages.Find(o.TravelInsurancePeriods);

            string HtmlBefore = "";
            string HtmlAfter = "";
            //string Newsletter = "<input type='checkbox' checked='checked' />" + RP.GetTextComponent("עמוד רכישה - הצהרות בריאות");

            OutEmail om = new OutEmail();
            om.MailTo = o.ShopEmail;
            if (o.ShopPayType == ShopPayTypeEnum.Phone)
            {
                om.Subject = RP.TH("חנות - מייל הזמנה חדשה  תשלום דרך הטלפון - כותרת").ToString();
            }
            else
            {
                om.Subject = RP.TH("חנות - מייל הזמנה חדשה - כותרת").ToString();
            }
            om.TimesSent = 0;
            om.LastTry = DateTime.Now;
            om.Body = HtmlBefore + RP.GetTextComponent("חנות - מייל הזמנה חדשה") + HtmlAfter;
            //om.Body = om.Body.Replace("ID", o.InsuranceDate.Year + "/" + o.ID);

            //int days = o.Days;

            //om.Body = om.Body.Replace("{Days}", days.ToString());
            //om.Body = om.Body.Replace("{InsuranceDate}", String.Format("{0:d/M/yyyy HH:mm}", o.InsuranceDate));
            //om.Body = om.Body.Replace("{InsuranceDateEnd}", String.Format("{0:d/M/yyyy HH:mm}", o.InsuranceDateEnd));


            //if (o.Gender == InsuranceGender.Female)
            //    om.Body = om.Body.Replace("{Pregnant}", Pregnant);
            //else
            //    om.Body = om.Body.Replace("{Pregnant}", "");

            om.Body = om.Body.Replace("src=\"/", "src=\"" + "http://" + LS.CurrentHttpContext.Request.ServerVariables["HTTP_HOST"].ToString() + "/");

            // fix link
            om.Body = om.Body.Replace("href=\"/", "href=\"" + "http://" + LS.CurrentHttpContext.Request.ServerVariables["HTTP_HOST"].ToString() + "/");


            foreach (PropertyInfo propertyInfo in typeof(ShopOrder).GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    object Value = propertyInfo.GetValue(o, null);
                    if (Value != null) om.Body = om.Body.Replace("{" + propertyInfo.Name + "}", Value.ToString());
                    else om.Body = om.Body.Replace("{" + propertyInfo.Name + "}", string.Empty);
                }

                string ShopProducts = "";
                foreach (var item in o.ShopGetDataFromXML<ShopCartItem>())
                {
                    ShopProducts = ShopProducts + item.ShopQuantity + " " + item.ShopProductTitle + ": " + (item.ShopPrice * item.ShopQuantity) + " ₪" + "<br>";
                }
                om.Body = om.Body.Replace("{ShopProducts}", ShopProducts);
            }


            string[] emails;



            string EmailsToSend = o.ShopEmail + "," + RP.GetCurrentSettings().AdminEmail;
            emails = EmailsToSend.Split(',');

            foreach (string item in emails)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    OutEmail email = new OutEmail()
                    {
                        Body = om.Body,
                        DomainID = om.DomainID,
                        LastTry = om.LastTry,
                        MailTo = item,
                        Subject = om.Subject,
                        TimesSent = om.TimesSent,
                    };
                    _db.OutEmails.Add(email);
                }
            }

            _db.SaveChanges();

            return string.Empty;
        }

        ///// <summary>
        ///// it Is only for intialize the old database to new one - don't use it when the new website already published and works
        ///// </summary>
        ///// <returns></returns>

        //public static int AddAllInsurancesToWebNewsletters()
        //{
        //    var allNewsLetterEmails = _db.Newsletters.Select(n => n.NewsletterEmail);

        //    var notNewsletterInsurances = _db.TravelInsurances.Where(t => t.IsCleared == true && allNewsLetterEmails.Contains(t.Mail) == false).ToList();
        //    var notNewsletterInsurancesTop = notNewsletterInsurances.Take(1000).ToList();

        //    var distinctItems = notNewsletterInsurancesTop.GroupBy(x => x.Mail).Select(y => y.First());

        //    int counter = 0;
        //    foreach (var item in distinctItems)
        //    {

        //        var _newsletter = new Newsletter()
        //        {
        //            ShopDomainID = RP.GetCurrentSettings().ID,
        //            NewsletterAccept = true,
        //            NewsletterDate = item.SubmitDate,
        //            NewsletterEmail = item.Mail,
        //            NewsletterIdNumber = item.IdNumber,
        //            NewsletterName = item.UserFname + " " + item.UserLname,
        //        };
        //        _db.Newsletters.Add(_newsletter);
        //        counter++;
        //    }
        //    _db.SaveChanges();
        //    return counter;
        //}


        /// <summary>
        /// it Is only for intialize the old database to new one - don't use it when the new website already published and works
        /// </summary>
        /// <returns></returns>



        public static User GetUser(Guid id)
        {
            User user = _db.Users.FirstOrDefault(u => u.ID == id);
            return user;
        }



        public static void SendContentRegisterEmail()
        {
            User user = LS.CurrentUser;

            OutEmail om = new OutEmail();
            om.MailTo = user.Email;

            om.Subject = RP.TH("אתר התוכן - אימייל הרשמה לאתר - כותרת").ToString();
            om.TimesSent = 0;
            om.LastTry = DateTime.Now;
            om.Body = RP.GetTextComponent("EmailRegisterContactPage");
            //om.Body = om.Body.Replace("ID", o.InsuranceDate.Year + "/" + o.ID);



            // fix image to currect domain
            om.Body = om.Body.Replace("src=\"/", "src=\"" + "http://" + LS.CurrentHttpContext.Request.ServerVariables["HTTP_HOST"].ToString() + "/");

            // fix link
            om.Body = om.Body.Replace("href=\"/", "href=\"" + "http://" + LS.CurrentHttpContext.Request.ServerVariables["HTTP_HOST"].ToString() + "/");


            foreach (PropertyInfo propertyInfo in typeof(User).GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    object Value = propertyInfo.GetValue(user, null);
                    if (Value != null) om.Body = om.Body.Replace("{" + propertyInfo.Name + "}", Value.ToString());
                    else om.Body = om.Body.Replace("{" + propertyInfo.Name + "}", string.Empty);
                }
            }


            if (!string.IsNullOrEmpty(user.Email))
            {
                OutEmail email = new OutEmail()
                {
                    Body = om.Body,
                    DomainID = om.DomainID,
                    LastTry = om.LastTry,
                    MailTo = user.Email,
                    Subject = om.Subject,
                    TimesSent = om.TimesSent,
                };
                _db.OutEmails.Add(email);
            }

            _db.SaveChanges();
        }


        public static List<AbstractPage> GetBradcrumbs(AbstractPage ap)
        {
            List<AbstractPage> list = new List<AbstractPage>();
            int DomainPageID = RP.GetAdminCurrentSettingsRepository().DomainPageID;
            var a = ap;
            list.Add(ap);
            while (a.ID != DomainPageID)
            {
                a = GetParent(a.ParentID);
                if (a.ID == DomainPageID)
                    break;
                if (a != null)
                {
                    list.Add(a);
                }
                else
                {
                    break;
                }
            }
            list.Reverse();
            return list;
        }

        private static AbstractPage GetParent(int parentId)
        {
            return LS.CurrentEntityContext.AbstractPages.Where(x => x.ID == parentId).FirstOrDefault();
        }
    }
}