using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Web.Configuration;
using Uco.Infrastructure.Repositories;
using Uco.Infrastructure.Livecycle;

namespace Uco.Infrastructure.Repositories
{
    public static partial class RP
    {
        #region Get/Clean Repository

        public static void CleanBannersRepository()
        {
            string lang = SF.GetLangCodeThreading();


            foreach (string BannerGroup in System.Configuration.ConfigurationManager.AppSettings["BannerGroup"].Split(',').ToList())
            {
                string key = string.Format("BannersReprository_{0}_{1}_{2}", lang, BannerGroup, RP.GetCurrentSettings().ID.ToString());

                LS.Cache[key] = null;
            }

        }

        public static List<Banner> GetBannersReprository(string BannerGroup)
        {
            string lang = SF.GetLangCodeThreading();
            string key = string.Format("BannersReprository_{0}_{1}_{2}",lang, BannerGroup,RP.GetCurrentSettings().ID.ToString());
            
            if (LS.Cache[key] == null)
            {
                using (Db _db = new Db())
                {
                    int did = RP.GetAdminCurrentSettingsRepository().ID;
                    List<Banner> l = _db.Banners.Where(r => r.BannerGroup == BannerGroup && r.DomainID == did
                        && r.LangCode == lang
                        && r.ShowDateMin <= DateTime.Now
                        && r.ShowDateMax > DateTime.Now).ToList();
                        

                
                    //Output
                    foreach (Banner item in l)
                    {
                        if (item.BannerTypeName == Banner.BannerType.Text) item.Output = "<div class='banner'>" + item.Text + "</div>";
                        else if (item.BannerTypeName == Banner.BannerType.Html) item.Output ="<div class='banner'>" + item.Html + "</div>";
                        else if (item.BannerTypeName == Banner.BannerType.Image)
                        {
                            if (!string.IsNullOrEmpty(item.PreLink)) item.Output = "<div class='banner'>" + "<a " + (item.NewWindow ? "target='_blank' " : "") + "href='" + item.PreLink + "'><img alt='" + item.Title + "' src='" + item.MainFile + "'></a>" + "</div>";
                            else if (string.IsNullOrEmpty(item.PreLink)) item.Output = "<div class='banner'>" + "<img alt='" + item.Title + "' src='" + item.MainFile + "'>" + "</div>";
                        }
                        else if (item.BannerTypeName == Banner.BannerType.Flash) item.Output =
                               "<div class='banner banner_holder' style='width:" + item.Width + "px;height:" + item.Height + "px;position:relative;'>"
                                    + "<div class='banner_bottom' style='width:" + item.Width + "px;height:" + item.Height + "px;left:0;position:absolute;top:0;z-index:100;'>"
                                        + "<object height=\"" + item.Height + "\" width=\"" + item.Width + "\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\">"
                                            + "<param name=\"movie\" value=\"" + item.MainFile + "\" />"
                                            + "<param name=\"play\" value=\"true\" />"
                                            + "<param name=\"loop\" value=\"true\" />"
                                            + "<param name=\"wmode\" value=\"transparent\" />"
                                            + "<param name=\"quality\" value=\"high\" />"
                                            + "<embed height=\"" + item.Height + "\" width=\"" + item.Width + "\" pluginspage=\"http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash\" quality=\"high\" wmode=\"transparent\" loop=\"true\" play=\"true\" src=\"" + item.MainFile + "\">"
                                        + "</object>"
                                    + "</div>"
                                    + (string.IsNullOrEmpty(item.PreLink) ? "" : ("<a " + (item.NewWindow ? "target='_blank' " : "") + "href='" + item.PreLink + "' class='banner_top' style='width:" + item.Width + "px;height:" + item.Height + "px;display:block;left:0;position:absolute;top:0;z-index: 1000;'></a>"))
                                + "</div>";
                        else if (item.BannerTypeName == Banner.BannerType.FlashAndBackground) item.Output =
                                "<div class='banner banner_holder' style='width:" + item.Width + "px;height:" + item.Height + "px;position:relative;'>"
                                    + "<div syle='width:" + item.Width + "px;height:" + item.Height + "px;display:block;left:0;position:absolute;top:0;z-index: 0;'><img alt='" + item.Title + "' src='" + item.OtherFile + "' style='border-width: 0px; width:" + item.Width + "px; height:" + item.Height + "px;'></div>"
                                    + "<div class='banner_bottom' style='width:" + item.Width + "px;height:" + item.Height + "px;left:0;position:absolute;top:0;z-index:100;'>"
                                        + "<object height=\"" + item.Height + "\" width=\"" + item.Width + "\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\">"
                                            + "<param name=\"movie\" value=\"" + item.MainFile + "\" />"
                                            + "<param name=\"play\" value=\"true\" />"
                                            + "<param name=\"loop\" value=\"true\" />"
                                            + "<param name=\"wmode\" value=\"transparent\" />"
                                            + "<param name=\"quality\" value=\"high\" />"
                                            + "<embed height=\"" + item.Height + "\" width=\"" + item.Width + "\" pluginspage=\"http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash\" quality=\"high\" wmode=\"transparent\" loop=\"true\" play=\"true\" src=\"" + item.MainFile + "\">"
                                        + "</object>"
                                    + "</div>"
                                    + (string.IsNullOrEmpty(item.PreLink) ? "" : ("<a " + (item.NewWindow ? "target='_blank' " : "") + "href='" + item.PreLink + "' class='banner_top' style='width:" + item.Width + "px;height:" + item.Height + "px;display:block;left:0;position:absolute;top:0;z-index: 1000;'></a>"))
                                + "</div>";
                    }
                    LS.Cache[key] = l;
                    return l;
                }
                
            }
            else return LS.Cache[key] as List<Banner>;
        }

        #endregion

    }
}