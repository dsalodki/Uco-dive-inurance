using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Infrastructure.Repositories
{

    public static partial class RP
    {        
        public static void CleanCategoryPageRepository()
        {
            foreach (Settings item in RP.GetSettingsRepositoryList())
            {
                foreach (string item2 in System.Configuration.ConfigurationManager.AppSettings["Languages"].Split(','))
                {
                    LS.Cache.Remove(string.Format("ShopCategoryPage_{0}_{1}", item.ID, item2));
                }
            }
        }

        public static List<ShopCategoryPage> GetCategoryPageReprository()
        {
            string LanguageCode = RP.GetCurrentSettings().LanguageCode;
            int DomainID = RP.GetCurrentSettings().ID;
            string Token = "ShopCategoryPage_" + DomainID + "_" + LanguageCode;

            if (LS.Cache[Token] == null)
            {
                List<ShopCategoryPage> l = new List<ShopCategoryPage>();
                l = _db.ShopCategoryPages.Where(r => r.DomainID == DomainID && r.LanguageCode == LanguageCode).ToList();

                LS.Cache[Token] = l;
                return l;
            }
            else return LS.Cache[Token] as List<ShopCategoryPage>;
        }

        public static List<AbstractPage> GetAllPageReprository()
        {
            string LanguageCode = RP.GetCurrentSettings().LanguageCode;
            int DomainID = RP.GetCurrentSettings().ID;
            string Token = "AllPages_" + DomainID + "_" + LanguageCode;

            if (LS.Cache[Token] == null)
            {
                var l = new List<AbstractPage>();
                l = _db.AbstractPages.Where(r => r.DomainID == DomainID && r.ShowInAdminMenu == true).ToList();

                LS.Cache[Token] = l;
                return l;
            }
            else return LS.Cache[Token] as List<AbstractPage>;
        }
    }
}