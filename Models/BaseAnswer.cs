using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class BaseAnswer
    {
        public string AnswerContent { get; set; }
        public bool IsCorrect { get; set; } = false;

        public static List<BaseAnswer> Generate(int size)
        {
            List<BaseAnswer> baseAnswers = new List<BaseAnswer>();
            for (int i = 0; i < size; i++)
            {
                baseAnswers.Add(new BaseAnswer());
            }
            return baseAnswers;
        }

        public static List<BaseAnswer> GetAnswers(string answer)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<BaseAnswer>>(answer);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
