using System.Collections.Generic;

namespace TCU.English.Models
{
    public class ListeningBaseCombined
    {
        public TestCategory TestCategory { get; set; }
        public ListeningMedia ListeningMedia { get; set; }
        public List<ListeningBaseQuestion> ListeningBaseQuestions { get; set; }
    }
}
