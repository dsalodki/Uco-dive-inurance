using Uco.Models;
using System.Linq;
using System;
namespace Uco.Infrastructure.Livecycle
{
    public static partial class LS
    {
        public static User CurrentUser
        {
            get
            {
                if (LS.CurrentHttpContext == null) return new User() { UserName = "Anonymous", Roles = "|Anonymous|" };
                else if (LS.CurrentHttpContext.Items["_CurrentUser"] == null) return new User() { UserName = "Anonymous", Roles = "|Anonymous|" };
                else return LS.CurrentHttpContext.Items["_CurrentUser"] as User;
            }
        }
        public static bool isLogined()
        {

            return Guid.Empty != CurrentUser.ID;
        }
    }
}