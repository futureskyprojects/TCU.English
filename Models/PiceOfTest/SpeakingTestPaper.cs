using System.Linq;
using TCU.English.Models.DataManager;
using TCU.English.Utils;

namespace TCU.English.Models.PiceOfTest
{
    public class SpeakingTestPaper
    {
        public class SpeakingDTO
        {
            public SpeakingEmbedCombined Speaking { get; set; }
            public float Scores { get; set; } = -1;
        }

        public int PiceOfTestId { get; set; }
        public SpeakingDTO SpeakingPart { get; set; }

        #region GENRATE QUESTION
        public static SpeakingDTO Generate(TestCategoryManager _TestCategoryManager, SpeakingEmbedManager _SpeakingEmbedManager)
        {
            var category = _TestCategoryManager
                .GetForGenerateTest(TestCategory.SPEAKING)
                .ToList()
                .Shuffle() // Trộn
                .First();
            var questions = _SpeakingEmbedManager.GetByCategoryId(category.Id);

            return new SpeakingDTO
            {
                Scores = -1,
                Speaking = new SpeakingEmbedCombined
                {
                    TestCategory = category,
                    SpeakingEmbed = questions
                }
            };
        }

        #endregion
    }
}
