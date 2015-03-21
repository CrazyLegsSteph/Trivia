using System;
using System.Collections.Generic;
using System.Linq;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;
using TShockAPI;
using Newtonsoft.Json;


namespace TRIVIA
{
    public class TriviaManager
    {
        public QAndA CurrentQAndA { get; set; }
        public List<QAndA> QuestionsAndAnswers { get; set; }
        public List<string> WrongAnswers = new List<string>();
        private Random rnd;

        public bool Enabled { get; set; }

        public void GetRandomQAndA()
        {
            CurrentQAndA = QuestionsAndAnswers[rnd.Next(0, QuestionsAndAnswers.Count)];
        }

        public void StartTrivia()
        {
            GetRandomQAndA();
            AnnounceQuestion();
            Enabled = true;
        }

        public void EndTrivia(TSPlayer ts, bool correctanswer)
        {
            Enabled = false;
            if (!correctanswer)
                TSPlayer.All.SendErrorMessage("[Trivia] Time is up!");
            else
            {
                TSPlayer.All.SendInfoMessage(string.Format("{0} answered the trivia correctly! the answer{1} {2}", ts.Name, CurrentQAndA.Answers.Count >1 ? "s were": " was", string.Join(", ",CurrentQAndA.Answers)));
                
                if (Trivia.config.DisplayWrongAnswers && WrongAnswers.Count > 0)
                    TSPlayer.All.SendErrorMessage(string.Format("Wrong answers were: {0}", string.Join(", ", WrongAnswers)));

                if (SEconomyPlugin.Instance != null)
                {
                    IBankAccount Server = SEconomyPlugin.Instance.GetBankAccount(TSServerPlayer.Server.UserID);
                    IBankAccount Player = SEconomyPlugin.Instance.GetBankAccount(ts.Index);
                    Server.TransferToAsync(Player, Trivia.config.CurrencyAmount, BankAccountTransferOptions.AnnounceToReceiver, "answering the trivia question correctly", "Answered trivia question");
                }
                else
                    ts.SendErrorMessage("Transaction failed because SEconomy is disabled!");
            }
            WrongAnswers.Clear();
        }

        public void AnnounceQuestion()
        {
            TSPlayer.All.SendInfoMessage("[Trivia] Type /answer or /a <answer here>");
            TSPlayer.All.SendInfoMessage("[Trivia] " + CurrentQAndA.Question);
            TSPlayer.All.SendInfoMessage("[Trivia] " + CurrentQAndA.Answer);
        }
        public TriviaManager()
        {
            QuestionsAndAnswers = new List<QAndA>();
            rnd = new Random();
        }
        public bool IsCorrectAnswer(string answer)
        {
            return CurrentQAndA.Answers.Any(a => a.Equals(answer, StringComparison.CurrentCultureIgnoreCase));            
        }
    }
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
