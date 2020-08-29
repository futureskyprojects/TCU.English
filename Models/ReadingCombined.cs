using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class ReadingCombined
    {
        public TestCategory TestCategory { get; set; }
        public List<ReadingPartTwo> ReadingPartTwos { get; set; }
    }
}
