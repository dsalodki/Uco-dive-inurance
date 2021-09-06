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

        public static void CleanInsuranceRepository()
        {
            LS.Cache.Remove("InsuranceReprository");
        }

   
        #endregion

    }
}