using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Models;
using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Uco.Infrastructure.Repositories;

namespace Uco.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,DomainAdmin")]
    public class ShopDashboardController : BaseAdminController
    {
        public ActionResult Index(string m)
        {
            ViewBag.M = m;
            return View();
        }

        // Statistics
        [ChildActionOnly]
        //[OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _Statistics()
        {
            DateTime FromDate = DateTime.Today.Date.AddMonths(-3);
            DateTime ToDate = DateTime.Today.Date;
            List<ShopAnaliticsData> l = _db.ShopAnaliticsDatas
                .Where(
                    r => r.ShopDomainID == AdminCurrentSettingsRepository.ID
                    && r.ShopDate >= FromDate
                    && r.ShopDate <= ToDate
                )
                .OrderBy(r => r.ShopDate)
                .ToList();

            string Chart = "<div id='chart_div'></div><script type='text/javascript'>" +
                                               "google.load('visualization', '1', {packages:['corechart']});" +
                                               "google.setOnLoadCallback(drawChart);" +
                                               "function drawChart() {" +
                                                 "var data = new google.visualization.DataTable();" +
                                                 "data.addColumn('string', 'תאריכים');" +
                                                 "data.addColumn('number', 'טלפונים');" +
                                                 "data.addColumn('number', 'פניות');" +
                                                 "data.addColumn('number', 'מוצרים נרכשו');" +
                                                 "data.addColumn('number', 'חשיפות');" +
                                                 "data.addColumn('number', 'כניסות');";

            for (DateTime i = DateTime.Now.Date.AddMonths(-3); i <= DateTime.Now.Date; i = i.AddDays(1))
            {
                int Unic = l.Where(r => r.ShopDate == i).Sum(r => r.ShopUnic);
                int Show = l.Where(r => r.ShopDate == i).Sum(r => r.ShopShow);
                int Buy = l.Where(r => r.ShopDate == i).Sum(r => r.ShopBuy);
                int Phone = l.Where(r => r.ShopDate == i).Sum(r => r.ShopPhone);
                int Contact = l.Where(r => r.ShopDate == i).Sum(r => r.ShopContact);

                Chart = Chart + "data.addRow(['" + String.Format("{0:dd-MM-yyyy}", i) + "', " + Phone + "," + Contact + ", " + Buy + ", " + Show + ", " + Unic + "]);";
            }

            Chart = Chart + "var chart = new google.visualization.LineChart(document.getElementById('chart_div'));" +
                "chart.draw(data, {height: 250, min: 0, curveType: \"none\", hAxis: { slantedText: false,minValue: 0},vAxis: {minValue: 0}, chartArea:{left:50,top:10,width:\"100%\",height:\"220\"}});" +
                "}" +
            "</script>";
            ViewBag.Chart = Chart;
            return View();
        }

        //ShopContact Not Done
        public ActionResult _AjaxContactNotDone([DataSourceRequest]DataSourceRequest request)
        {
            //return View(new GridModel<ShopContact>
            //{
            //    Data = _db.Contacts.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ContactDone == false).ToList()
            //});
            IQueryable<Contact> items = _db.Contacts.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        //ShopContact Done
        //public ActionResult _AjaxContactNotDoneMark([DataSourceRequest]DataSourceRequest request, int ID)
        //{
        //    ShopContact o = _db.Contacts.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID && r.ContactDone == false);
        //    if (o == null) return Json(new { result = false });
        //    o.ContactDone = true;
        //    _db.Entry(o).State = EntityState.Modified;
        //    _db.SaveChanges();
        //    return Json(new { result = true });
        //}

        //Products Statistic
        //[GridAction]
        //[OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _AjaxProductsStatistic([DataSourceRequest]DataSourceRequest request)
        {
            List<ShopAnaliticsData> l = _db.ShopAnaliticsDatas.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopPageRouteUrl == "p").ToList();
            List<ShopAnaliticsData> lr = new List<ShopAnaliticsData>();
            int i = 1;
            foreach (int item in l.Select(r => r.ShopPageID).Distinct())
            {
                ShopAnaliticsData NewAnaliticsData = new ShopAnaliticsData();
                ShopAnaliticsData OldAnaliticsData = l.FirstOrDefault(r => r.ShopPageID == item);
                if (OldAnaliticsData == null) continue;

                NewAnaliticsData.ID = OldAnaliticsData.ShopPageID;
                NewAnaliticsData.ShopDomainID = OldAnaliticsData.ID;
                NewAnaliticsData.ShopPageID = OldAnaliticsData.ShopPageID;
                NewAnaliticsData.ShopPageTitle = OldAnaliticsData.ShopPageTitle;
                NewAnaliticsData.ShopPageRouteUrl = OldAnaliticsData.ShopPageRouteUrl;
                NewAnaliticsData.ShopReferalUrl = OldAnaliticsData.ShopReferalUrl;
                NewAnaliticsData.ShopReferalSearch = OldAnaliticsData.ShopReferalSearch;

                NewAnaliticsData.ShopUnic = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopUnic);
                NewAnaliticsData.ShopShow = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopShow);
                NewAnaliticsData.ShopPhone = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopPhone);
                NewAnaliticsData.ShopContact = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopContact);
                NewAnaliticsData.ShopBuy = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopBuy);

                lr.Add(NewAnaliticsData);
                i = i + 1;
            }

            //return View(new GridModel<ShopAnaliticsData>
            //{
            //    Data = lr
            //});

            DataSourceResult result = lr.ToDataSourceResult(request);
            return Json(result);
        }

        //[GridAction]
        public ActionResult _AjaxProductsStatisticDelete([DataSourceRequest]DataSourceRequest request, int ID)
        {
            _db.Database.ExecuteSqlCommand("DELETE FROM ShopAnaliticsDatas WHERE ShopDomainID = '" + AdminCurrentSettingsRepository.ID + "' AND ShopPageID = '" + ID + "'");

            List<ShopAnaliticsData> l = _db.ShopAnaliticsDatas.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopPageRouteUrl == "p").ToList();
            List<ShopAnaliticsData> lr = new List<ShopAnaliticsData>();

            foreach (int item in l.Select(r => r.ShopPageID).Distinct())
            {
                ShopAnaliticsData NewAnaliticsData = new ShopAnaliticsData();
                ShopAnaliticsData OldAnaliticsData = l.FirstOrDefault(r => r.ShopPageID == item);
                if (OldAnaliticsData == null) continue;

                NewAnaliticsData.ID = OldAnaliticsData.ShopPageID;
                NewAnaliticsData.ShopDomainID = OldAnaliticsData.ID;
                NewAnaliticsData.ShopPageID = OldAnaliticsData.ShopPageID;
                NewAnaliticsData.ShopPageTitle = OldAnaliticsData.ShopPageTitle;
                NewAnaliticsData.ShopPageRouteUrl = OldAnaliticsData.ShopPageRouteUrl;
                NewAnaliticsData.ShopReferalUrl = OldAnaliticsData.ShopReferalUrl;
                NewAnaliticsData.ShopReferalSearch = OldAnaliticsData.ShopReferalSearch;

                NewAnaliticsData.ShopUnic = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopUnic);
                NewAnaliticsData.ShopShow = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopShow);
                NewAnaliticsData.ShopPhone = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopPhone);
                NewAnaliticsData.ShopContact = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopContact);
                NewAnaliticsData.ShopBuy = l.Where(r => r.ShopPageID == item).Sum(r => r.ShopBuy);

                lr.Add(NewAnaliticsData);
            }

            //return View(new GridModel<ShopAnaliticsData>
            //{
            //    Data = lr
            //});
            DataSourceResult result = lr.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult _ProductsStatisticDelete()
        {
            _db.Database.ExecuteSqlCommand("DELETE FROM ShopAnaliticsDatas WHERE ShopDomainID = '" + AdminCurrentSettingsRepository.ID + "'");
            CleanCache.CleanOutputCache();
            return RedirectToAction("Index");
        } 

        //Products NOT Payed
        //[GridAction]
        public ActionResult _AjaxOrderNotPayed([DataSourceRequest]DataSourceRequest request)
        {            
            IQueryable<ShopOrder> items = _db.ShopOrders.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopStatus == ShopOrderStatusEnum.Placed);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }
        //mark as payed
        public JsonResult _AjaxOrderNotPayedMark(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID && r.ShopStatus == ShopOrderStatusEnum.Placed);
            if (o == null) return Json(new { result = false });
            o.ShopStatus = ShopOrderStatusEnum.Payed;
            o.ShopLog = o.ShopLog + DateTime.Now + " - סומן כשולם\r\n";
            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { result = true });
        }

        //Products NOT Shipped
        public ActionResult _AjaxOrderNotShipped([DataSourceRequest]DataSourceRequest request)
        {            
            IQueryable<ShopOrder> items = _db.ShopOrders.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopStatus == ShopOrderStatusEnum.Payed);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }
        //mark as shiped
        public JsonResult _AjaxOrderNotShippedMark(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID && r.ShopStatus == ShopOrderStatusEnum.Payed);
            if (o == null) return Json(new { result = false });
            o.ShopStatus = ShopOrderStatusEnum.Shipped;
            o.ShopLog = o.ShopLog + DateTime.Now + " - סומן כנשלח\r\n";
            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { result = true });
        }

        //Products NOT Received
        public ActionResult _AjaxOrderNotReceived([DataSourceRequest]DataSourceRequest request)
        {           
            IQueryable<ShopOrder> items = _db.ShopOrders.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopStatus == ShopOrderStatusEnum.Shipped);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }
        //mark as shiped
        public JsonResult _AjaxOrderNotReceivedMark(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID && r.ShopStatus == ShopOrderStatusEnum.Shipped);
            if (o == null) return Json(new { result = false });
            o.ShopStatus = ShopOrderStatusEnum.Received;
            o.ShopLog = o.ShopLog + DateTime.Now + " - סומן כהתקבל\r\n";
            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { result = true });
        }

        //mark as Canceled
        public JsonResult _AjaxOrderCancelMark(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID);
            if (o == null) return Json(new { result = false });
            o.ShopStatus = ShopOrderStatusEnum.Canceled;
            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { result = true });
        }

        //Upgrade options
        public ActionResult _UpgradeOptions()
        {
            return View();
        }
    }
}
