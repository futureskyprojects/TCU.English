using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    public static class DiscussMessageUtils
    {
        public static string FriendMessage(string msg, DateTime createTime)
        {
            return $"<div class=\"d-flex justify-content-between w-75 mb-4\">" +
             $"<div class=\"card bg-gray-500 p-3 text-dark\">" +
             $"<p class=\"mb-0\" style=\"font-family: Tahoma\">{msg}</p>" +
             $"<div class=\"pt-2\">" +
             $"<small class=\"float-right bg-white p-1 rounded\">{createTime:hh:mm:ss dd/MM/yyyy}</small>" +
             $"</div>" +
             $"</div>" +
             $"</div>";
        }

        public static string YourMessage(string msg, DateTime createTime)
        {
            return
            $"<div class=\"d-flex justify-content-between w-75 float-right\">" +
            $"<div class=\"card bg-blue p-3 text-white\">" +
            $"<p class=\"mb-0\" style=\"font-family: Tahoma\">{msg}</p>" +
            $"<div class=\"pt-2\">" +
            $"<small class=\"bg-white p-1 rounded text-dark\">{createTime:hh:mm:ss dd/MM/yyyy}</small>" +
            $"</div>" +
            $"</div>" +
            $"</div>";
        }
    }
}
