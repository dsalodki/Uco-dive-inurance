using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Uco.Infrastructure.Livecycle;
using Uco.Models;
using System.Data.Entity;

namespace Uco.Infrastructure.Repositories
{
    public static partial class RP
    {

        #region Get/Clean Repository

        public static void CleanTranslationsRepository()
        {
            foreach (string item in SF.WebconfigLanguages().Split(','))
            {
                LS.Cache.Remove(string.Format("Translation_{0}", item));
            }
        }

        public static List<Translation> GetTranslationsReprository()
        {
            string LanguageCode = SF.GetLangCodeThreading(); 
            string Token = "Translation_" + LanguageCode;

            if (LS.Cache[Token] == null)
            {
                List<Translation> l = _db.Translations.Where(r => r.LangCode == LanguageCode).ToList();
                LS.Cache[Token] = l;
            }
            return LS.Cache[Token] as List<Translation>;
        }

        #endregion

        #region Get/Set single

        //Get translation as string
        public static string T(string SystemName, string DefaultValue = "")
        {
            if (string.IsNullOrEmpty(SystemName)) return SystemName;
            SystemName = SystemName.Trim();

            Translation TranslationsDbItem = GetTranslationsReprository().FirstOrDefault(r => r.SystemName == SystemName);
            if (TranslationsDbItem != null) return TranslationsDbItem.Text;
            else if (SF.WebconfigAutoAddTranslations())
            {
                string DefaultValueResource = Uco.Models.Resources.SystemModels.ResourceManager.GetString(DefaultValue);
                DefaultValue = (string.IsNullOrEmpty(DefaultValueResource) ? DefaultValue : DefaultValueResource);

                DefaultValueResource = Uco.Models.Resources.DataModels.ResourceManager.GetString(DefaultValue);
                DefaultValue = (string.IsNullOrEmpty(DefaultValueResource) ? DefaultValue : DefaultValueResource);

                TranslationsDbItem = new Translation();
                TranslationsDbItem.SystemName = SystemName;
                TranslationsDbItem.Text = (string.IsNullOrEmpty(DefaultValue) ? SystemName : DefaultValue);
                _db.Translations.Add(TranslationsDbItem);

                _db.SaveChanges();

                CleanTranslationsRepository();

                return TranslationsDbItem.Text;
            }
            else return SystemName;
        }

        //Get translation as IHtmlString
        public static IHtmlString TH(string SystemName, string DefaultValue = "")
        {
            return new HtmlString(T(SystemName, DefaultValue));
        }

        //Get translation as string from model name
        public static string TM(string ModelName, string ModelField, string ModelGroup = "", string DefaultValue = "")
        {
            var SystemName = string.Format("Models{0}.{1}.{2}", ModelGroup, ModelField, ModelName);
            return T(SystemName, DefaultValue);
        }

        //Translation View General
        public static string TVG(string Name)
        {
            string SystemName = string.Empty;
            string RouteArea = string.Empty;
            string RouteController = string.Empty;
            string RouteAction = string.Empty;
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"] != null) RouteArea = LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"].ToString();

            if (string.IsNullOrEmpty(RouteArea)) SystemName = string.Format("Views.Shared.Global.{0}", Name);
            else SystemName = string.Format("Areas.{0}.Views.Shared.Global.{1}", RouteArea, Name);

            return T(SystemName, Name);
        }

        //Translation View Local
        public static string TVL(string Name)
        {
            string SystemName = string.Empty;
            string RouteArea = string.Empty;
            string RouteController = string.Empty;
            string RouteAction = string.Empty;
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"] != null) RouteArea = LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"].ToString();
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["controller"] != null) RouteController = LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["action"] != null) RouteAction = LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["action"].ToString();

            if (string.IsNullOrEmpty(RouteArea)) SystemName = string.Format("Views.{0}.{1}.{2}", RouteController, RouteAction, Name);
            else SystemName = string.Format("Areas.{0}.Views.{1}.{2}.{3}", RouteArea, RouteController, RouteAction, Name);

            return T(SystemName, Name);
        }

        //Translation Controler
        public static string TC(string Name)
        {
            string SystemName = string.Empty;
            string RouteArea = string.Empty;
            string RouteController = string.Empty;
            string RouteAction = string.Empty;
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"] != null) RouteArea = LS.CurrentHttpContext.Request.RequestContext.RouteData.DataTokens["area"].ToString();
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["controller"] != null) RouteController = LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            if (LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["action"] != null) RouteAction = LS.CurrentHttpContext.Request.RequestContext.RouteData.Values["action"].ToString();

            if (string.IsNullOrEmpty(RouteArea)) SystemName = string.Format("Controllers.{0}.{1}.{2}", RouteController, RouteAction, Name);
            else SystemName = string.Format("Areas.{0}.Controllers.{1}.{2}.{3}", RouteArea, RouteController, RouteAction, Name);

            return T(SystemName, Name);
        }

        #endregion
    }
}