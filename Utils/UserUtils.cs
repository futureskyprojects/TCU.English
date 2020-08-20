using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;

namespace TCU.English.Utils
{
    public static class UserUtils
    {
        public static string DisplayFirstUserType(this User user)
        {
            if (user.UserTypeUser != null && user.UserTypeUser.Count > 0)
            {
                return user.UserTypeUser.First().UserType.Display();
            }
            else
            {
                return "Unknow Error";
            }
        }
    }
}
