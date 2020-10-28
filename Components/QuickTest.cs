using Microsoft.AspNetCore.Mvc;
using TCU.English.Models;
using TCU.English.Models.DataManager;
using TCU.English.Models.Repository;

namespace TCU.English.Components
{
    public class QuickTest : ViewComponent
    {
        private readonly VocabularyManager _VocabularyManager;

        public QuickTest(IDataRepository<Vocabulary> _VocabularyManager)
        {
            this._VocabularyManager = _VocabularyManager as VocabularyManager;
        }

        public IViewComponentResult Invoke()
        {
            return View(_VocabularyManager.GenerateQuickTestQuenstion(5));
        }


    }
}
