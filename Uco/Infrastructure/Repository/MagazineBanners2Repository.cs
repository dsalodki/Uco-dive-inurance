using System;
using System.Collections.Generic;
using System.Linq;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Infrastructure.Repositories
{
    public static partial class RP
    {
        public static void CleanMagazineBanners2Repository()
        {
            string lang = SF.GetLangCodeThreading();

            string key = string.Format("MagazineBanners2Reprository_{0}_{1}", lang, RP.GetCurrentSettings().ID.ToString());

            LS.Cache[key] = null;
        }

        public static List<MagazineBanner2> GetMagazineBanners2Reprository()
        {
            string lang = SF.GetLangCodeThreading();
            string key = string.Format("MagazineBanners2Reprository_{0}_{1}", lang, RP.GetCurrentSettings().ID.ToString());

            if (LS.Cache[key] == null)
            {
                using (Db _db = new Db())
                {
                    int did = RP.GetAdminCurrentSettingsRepository().ID;
                    List<MagazineBanner2> l = _db.MagazineBanners2.Where(r => r.DomainID == did
                        && r.LangCode == lang).ToList();

                    LS.Cache[key] = l;
                    return l;
                }

            }
            else return LS.Cache[key] as List<MagazineBanner2>;
        }
    }
}