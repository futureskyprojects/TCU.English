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


        /// <summary>
        /// Load nội dung đánh giá của GV cho bài thi này
        /// </summary>
        /// <param name="id">Mã bài thi của HV</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LoadInstructorComments(int id)
        {
            if (id <= 0)
                return Content(string.Empty);

            // Lấy bài thi theo mã
            PieceOfTest piece = _PieceOfTestManager.Get(id);

            // Nếu không có, trả về
            if (piece == null)
                return Content(string.Empty);

            // Nếu không có nội dung đánh giá
            if (string.IsNullOrEmpty(piece.InstructorComments))
                return Content(string.Empty);

            // Trả về nội dung đánh giá
            return Content(piece.InstructorComments);

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
