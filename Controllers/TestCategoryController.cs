using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [AuthorizeRoles(UserType.ROLE_ALL, UserType.ROLE_MANAGER_LIBRARY)]
    public class TestCategoryController : Controller
    {
        private readonly UserManager _UserManager;
        private readonly UserTypeManager _UserTypeManager;
        private readonly TestCategoryManager _TestCategoryManager;
        public TestCategoryController(IDataRepository<User> _UserManager, IDataRepository<UserType> _UserTypeManager, IDataRepository<TestCategory> _TestCategoryManager)
        {
            this._UserManager = (UserManager)_UserManager;
            this._UserTypeManager = (UserTypeManager)_UserTypeManager;
            this._TestCategoryManager = (TestCategoryManager)_TestCategoryManager;
        }
        public IActionResult Index()
        {
            return NotFound();
        }

        [HttpPost]
        public IActionResult Create(TestCategory testCategory)
        {
            if (testCategory.TypeCode == null || testCategory.PartId <= 0 || !TestCategory.Types.Contains(testCategory.TypeCode))
            {
                return BadRequest();
            }
            else
            {
                return PartialView(testCategory);
            }
        }

        [HttpPost]
        public IActionResult CreateAjax(TestCategory testCategory)
        {
            if (ModelState.IsValid)
            {
                if (_TestCategoryManager.IsExists(testCategory))
                {
                    return Json(new { status = false, message = $"{nameof(TestCategory.Name)} Already exists" });
                }
                else
                {
                    int uId = User.Id();
                    testCategory.CreatorId = uId;
                    _TestCategoryManager.Add(testCategory);
                    return Json(new { status = true, message = "Successfully created, the list will refresh again in 1 second." });
                }
            }
            else
            {
                string errors = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (errors.Length > 0)
                            errors += "\r\n";
                        errors += error.ErrorMessage;
                    }
                }

                return Json(new { status = false, message = errors });
            }
        }

        [HttpGet]
        public IActionResult Update(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            else
            {
                var testCategory = _TestCategoryManager.Get(id);
                if (testCategory == null)
                    return NotFound();
                ViewBag.IsShowImmediately = true;
                return PartialView(testCategory);
            }
        }

        [HttpPost]
        public IActionResult UpdateAjax(TestCategory testCategory)
        {
            if (ModelState.IsValid)
            {
                _TestCategoryManager.Update(testCategory);
                return Json(new { status = true, message = "Successfully updated, the list will refresh again in 1 second." });
            }
            else
            {
                string errors = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (errors.Length > 0)
                            errors += "\r\n";
                        errors += error.ErrorMessage;
                    }
                }

                return Json(new { status = false, message = errors });
            }
        }



        [HttpDelete]
        public IActionResult Delete(long id)
        {
            var testCategory = _TestCategoryManager.Get(id);
            if (testCategory == null)
            {
                return Json(new { success = false, responseText = "This category was not found." });
            }
            else
            {
                _TestCategoryManager.Delete(testCategory);
                return Json(new { success = true, user = JsonConvert.SerializeObject(testCategory), responseText = "Deleted" });
            }
        }
    }
}
