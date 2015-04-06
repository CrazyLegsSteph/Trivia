using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Trivia
{
    public class QAndA
    {
        public string Question;

        public string Answer;

        [JsonIgnore]
        public List<string> Answers
        { get { return Answer.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList(); } }

        public QAndA(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }
    }
}
