using System;
using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using System.Reflection;

namespace Trivia
{
    [ApiVersion(1,19)]
    public class Trivia : TerrariaPlugin
    {
        public static TriviaManager TriviaManager = new TriviaManager();

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ancientgods"; }
        }
        public override string Name
        {
            get { return "Trivia"; }
        }
        public override string Description
        {
            get { return ""; }
        }
        public Trivia(Main game)
            : base(game)
        {
            Order = 1;
        }
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(Answer, "answer", "a"));
            Commands.ChatCommands.Add(new Command("trivia.reload", Reload_Config, "triviareload"));
            TriviaManager.Initialize();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        private void Answer(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! proper syntax: /answer (or /a) <answer here>");
                return;
            }
            if (!TriviaManager.PendingAnswer)
            {
                args.Player.SendErrorMessage("Trivia isn't currently running!");
                return;
            }
            string answer = string.Join(" ", args.Parameters);
            if (TriviaManager.IsAnswerCorrect(answer))
                TriviaManager.EndTrivia(args.Player);
            else
            {
                TriviaManager.WrongAnswers.Add(answer);
                args.Player.SendErrorMessage(string.Format("{0} is not the correct answer! better luck next time!", answer));
            }
        }

        private void Reload_Config(CommandArgs args)
        {
            TriviaManager.ReloadConfig(args);
        }
    }
}
