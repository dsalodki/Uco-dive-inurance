using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Uco.Infrastructure
{
    public class UcoEmailAttribute : RegularExpressionAttribute
    {
        public UcoEmailAttribute()
            : base(GetRegex())
        { }

        private static string GetRegex()
        {
            return @"^[\w-]+(\.[\w-]+)*@([A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*?\.[A-Za-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$";
        }
    }
}