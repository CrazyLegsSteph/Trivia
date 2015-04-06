using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Trivia
{
    public class TriviaManager
    {
        public Config Config;
        public bool PendingAnswer { get { return QuestionAsked; } }
        private Timer _timer = new Timer(1000);
        private QAndA CurrentQandA;
        public List<string> WrongAnswers = new List<string>();

        public bool Enabled
        {
            get
            {
                return _timer.Enabled;
            }
            set
            {
                _timer.Enabled = value;
            }
        }

        public TriviaManager()
        {
            _timer.Elapsed += _timer_Elapsed;
        }

        ~TriviaManager()
        {
            _timer.Elapsed -= _timer_Elapsed;
        }

        public void Start()
        {
            this.Enabled = true;
        }

        public void Stop()
        {
            this.Enabled = false;
        }

        public void Initialize()
        {
            Load_Config();
        }

        public void Load_Config()
        {
            Config = new Config(Config.SavePath);
            this.Enabled = Config.Enabled;
        }

        public void ReloadConfig(CommandArgs args)
        {
            Config.Reload(args);
        }

        private void Reset()
        {
            count = 0;
            QuestionAsked = false;
        }

        private int count = 0;
        private bool QuestionAsked;
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            count++;
            if (count > Config.QuestionInterval && !QuestionAsked)
            {
                count = 0;
                QuestionAsked = true;
                SetNewQandA();
                AnnouceQuestion();
            }
            if (count > Config.AnswerTime && QuestionAsked)
            {
                count = 0;
                QuestionAsked = false;
                WrongAnswers.Clear();
                TSPlayer.All.SendErrorMessage("[Trivia] Time is up!");
            }
        }

        public bool IsAnswerCorrect(string answer)
        {
            return CurrentQandA.Answers.Any(a => a.Equals(answer, StringComparison.CurrentCultureIgnoreCase));
        }

        private void AnnouceQuestion()
        {
            TSPlayer.All.SendInfoMessage("[Trivia] Type /answer or /a <answer here>");
            TSPlayer.All.SendInfoMessage("[Trivia] " + CurrentQandA.Question);
        }

        private void SetNewQandA()
        {
            Random rnd = new Random();
            CurrentQandA = Config.QuestionsAndAnswers[rnd.Next(0, Config.QuestionsAndAnswers.Length)];
        }

        public void EndTrivia(TSPlayer ts)
        {
            Reset();
            TSPlayer.All.SendInfoMessage(string.Format("{0} answered the trivia correctly! the answer{1} {2}", ts.Name, CurrentQandA.Answers.Count > 1 ? "s were" : " was", string.Join(", ", CurrentQandA.Answers)));
            if (Config.DisplayWrongAnswers && WrongAnswers.Count > 0)
                TSPlayer.All.SendErrorMessage(string.Format("Wrong answers were: {0}", string.Join(", ", WrongAnswers)));
            WrongAnswers.Clear();

            if (SEconomyPlugin.Instance != null)
            {
                IBankAccount Server = SEconomyPlugin.Instance.GetBankAccount(TSServerPlayer.Server.UserID);
                IBankAccount Player = SEconomyPlugin.Instance.GetBankAccount(ts.Index);
                Server.TransferToAsync(Player, Config.CurrencyAmount, BankAccountTransferOptions.AnnounceToReceiver, "answering the trivia question correctly", "Answered trivia question");
            }
            else
                ts.SendErrorMessage("[Trivia] Transaction failed because SEconomy is disabled!");
        }
    }
}
