using System;
using System.Collections.Generic;
using System.Linq;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Infrastructure.Repositories
{
    public static partial class RP
    {
        #region Get/Clean Repository

        public static void CleanMagazineBanners1Repository()
        {
            string lang = SF.GetLangCodeThreading();

            string key = string.Format("MagazineBanners1Reprository_{0}_{1}", lang, RP.GetCurrentSettings().ID.ToString());

            LS.Cache[key] = null;
        }

        public static List<MagazineBanner1> GetMagazineBanners1Reprository()
        {
            string lang = SF.GetLangCodeThreading();
            string key = string.Format("MagazineBanners1Reprository_{0}_{1}", lang, RP.GetCurrentSettings().ID.ToString());

            if (LS.Cache[key] == null)
            {
                using (Db _db = new Db())
                {
                    int did = RP.GetAdminCurrentSettingsRepository().ID;
                    List<MagazineBanner1> l = _db.MagazineBanners1.Where(r => r.DomainID == did
                        && r.LangCode == lang).ToList();

                    LS.Cache[key] = l;
                    return l;
                }

            }
            else return LS.Cache[key] as List<MagazineBanner1>;
        }

        #endregion

    }
}