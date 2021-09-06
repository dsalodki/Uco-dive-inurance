using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Infrastructure.Repositories
{
    public static partial class RP
    {

        #region Get/Clean Repository

        public static void CleanTextComponentRepository()
        {
            foreach (Settings item in RP.GetSettingsRepositoryList())
            {
                foreach (string item2 in System.Configuration.ConfigurationManager.AppSettings["Languages"].Split(','))
                {
                    LS.Cache.Remove(string.Format("TextComponent_{0}_{1}", item.ID, item2));
                }
            }
        }

        private static List<TextComponent> GetTextComponentReprository()
        {
            string LanguageCode = RP.GetCurrentSettings().LanguageCode;
            int DomainID = RP.GetCurrentSettings().ID;
            string Token = "TextComponent_" + DomainID + "_" + LanguageCode;

            if (LS.Cache[Token] == null)
            {
                List<TextComponent> l = new List<TextComponent>();
                l = _db.TextComponents.Where(r => r.DomainID == DomainID && r.LangCode == LanguageCode).ToList();

                LS.Cache[Token] = l;
                return l;
            }
            else return LS.Cache[Token] as List<TextComponent>;
        }

        #endregion

        #region Get/Set single

        public static string GetTextComponent(string SystemName)
        {
            TextComponent tc = GetTextComponentReprository().FirstOrDefault(r => r.SystemName == SystemName && r.Visible == true);
            if (tc != null) return tc.Text;
            else return string.Empty;
        }
		
		public static IHtmlString Text(string SystemName)
        {
            TextComponent tc = GetTextComponentReprository().FirstOrDefault(r => r.SystemName == SystemName && r.Visible);
            if (tc == null) return new HtmlString(string.Empty);
            else return new HtmlString(tc.Text);
        }

        #endregion
    }
}