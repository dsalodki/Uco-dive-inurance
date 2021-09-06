using System;
using System.IO;
using System.Web;

namespace Uco.Infrastructure.Logger
{
    public static class Logger
    {
        private static string _path;

        private const string _fileName = "idive.co.il_log.txt";

        private static string _path_error;

        private const string _fileName_error = "idive.co.il_error_log.txt";

        static Logger()
        {
            Logger._path = string.Concat(HttpRuntime.AppDomainAppPath, "log\\");
            Logger._path_error = string.Concat(HttpRuntime.AppDomainAppPath, "log\\");
        }

        public static void Error(string textToLog, string path = "", string fileName = "idive.co.il_error_log.txt")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Logger._path_error;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.Exists(path))
            {
                using (StreamWriter streamWriter = new StreamWriter(string.Concat(path, fileName), true))
                {
                    streamWriter.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss tt} --------- {1}", DateTime.Now, textToLog));
                }
            }
        }

        public static void Information(string textToLog, string path = "", string fileName = "idive.co.il_log.txt")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Logger._path;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.Exists(path))
            {
                using (StreamWriter streamWriter = new StreamWriter(string.Concat(path, fileName), true))
                {
                    streamWriter.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss tt} --------- {1}", DateTime.Now, textToLog));
                }
            }
        }
    }
}