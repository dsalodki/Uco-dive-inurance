using Uco.Models;

namespace Uco.Infrastructure.Livecycle
{
    public static partial class LS
    {
        public static Db CurrentEntityContext
        {
            get
            {
                if (LS.CurrentHttpContext == null) return null;
                else if (LS.CurrentHttpContext.Items["_EntityContext"] == null) return null;
                else return LS.CurrentHttpContext.Items["_EntityContext"] as Db;
            }
        }
    }
}