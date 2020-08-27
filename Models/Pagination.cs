using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class Pagination
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int NumberPage { get; set; }
        public int PageCurrent { get; set; }
        public int Offset { get; set; }
        public string Type { get; set; }
        public string PageKey { get; set; } = "page";
        public string TypeKey { get; set; } = "type";

        public Pagination(string actionName, string controllerName)
        {
            this.ActionName = actionName;
            this.ControllerName = controllerName;
        }

        public Dictionary<string, string> BuildParams(string page)
        {
            if (Type == null)
            {
                return new Dictionary<string, string> { { PageKey, page } };
            }
            else
            {
                return new Dictionary<string, string> { { PageKey, page }, { TypeKey, Type } };
            }
        }
    }
}
