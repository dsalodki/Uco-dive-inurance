using Uco.Infrastructure.Livecycle;

namespace Uco.Infrastructure
{
    public static partial class SF
    {
        public static bool WebconfigAutoAddTranslations()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["AutoAddTranslations"] == null) return false;
            if (System.Configuration.ConfigurationManager.AppSettings["AutoAddTranslations"].ToString() == "true") return true;
            else return false;
        }

        public static string WebconfigLanguages()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Languages"] == null) return "";
            else return System.Configuration.ConfigurationManager.AppSettings["Languages"].ToString();
        }
    }
}