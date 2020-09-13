using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models;

namespace TCU.English.Utils
{
    public static class AnswerValidationUtils
    {
        public static string BaseAnswerValidation(this List<BaseAnswer> answers, bool isCheckAnswerContent = true)
        {
            if (answers == null)
                return "Answer list can not be null.";
            if (answers.Count <= 0)
                return "Full answer of question is required.";
            if (answers.Any(it => it.AnswerContent == null || it.AnswerContent.Length <= 0) && isCheckAnswerContent)
                return "All answer of question must have content.";
            if (!answers.Any(it => it.IsCorrect))
                return "Answer of question must have one correct option.";
            if (answers.FindAll(it => it.IsCorrect).Count > 1)
                return "There cannot be more than one correct answer";
            return "";
        }
    }
}
