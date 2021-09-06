using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uco.Infrastructure
{
    public static partial class SF
    {

        public static List<int> GetListIntFromString(string Text)
        {
            if (string.IsNullOrEmpty(Text)) return new List<int>();
            List<int> l = new List<int>();
            foreach (string item in Text.Split(','))
            {
                int tempInt = 0;
                int.TryParse(item.Trim(), out tempInt);
                l.Add(tempInt);
            }
            return l.Distinct().ToList();
        }
    }
}