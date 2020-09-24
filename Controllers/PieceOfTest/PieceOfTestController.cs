using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Controllers
{
    [Authorize]
    public class PieceOfTestController : Controller
    {
        private readonly PieceOfTestManager _PieceOfTestManager;
        public PieceOfTestController(IDataRepository<PieceOfTest> _PieceOfTestManager)
        {
            this._PieceOfTestManager = (PieceOfTestManager)_PieceOfTestManager;
        }

        [HttpDelete]
        public IActionResult Delete(long id)
        {
            var pieceOfTest = _PieceOfTestManager.Get(id);
            if (pieceOfTest == null)
            {
                return Json(new { success = false, responseText = "Did not find the test you requested." });
            }
            else if (pieceOfTest.UserId == User.Id())
            {
                if (pieceOfTest.ResultOfUserJson != null && pieceOfTest.ResultOfUserJson.Length > 0)
                {
                    return Json(new { success = false, responseText = "You cannot delete the test once it is completed." });
                }
                else
                {
                    _PieceOfTestManager.Delete(pieceOfTest);
                    return Json(new { success = true, user = JsonConvert.SerializeObject(pieceOfTest), responseText = "Deleted" });
                }
            }
            else
            {
                return Json(new { success = false, responseText = "You cannot delete other people's tests." });
            }
        }
    }
}
