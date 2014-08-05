using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using Wolfje.Plugins.SEconomy.Journal;
using Wolfje.Plugins.SEconomy;


namespace TRIVIA
{
    [ApiVersion(1, 16)]
    public class Trivia : TerrariaPlugin
    {
        List<string> WrongAnswers = new List<string>();
        Timer Timer = new Timer(1000);
        public trivia T = new trivia("","");
        public int seconds = 0;
        private static Config config;
        Random rnd = new Random();
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
            get { return "Trivia, yay!"; }
        }

        public override string Description
        {
            get { return "Trivia, yay!"; }
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
            Timer.Elapsed += OnTimer;
            Timer.Start();
            ReadConfig();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Timer.Elapsed -= OnTimer;
            }
            base.Dispose(disposing);
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            seconds++;
            if (seconds == config.QuestionInterval)
            {
                SetNewQuestion();
            }
            if (seconds >= config.AnswerTime + config.QuestionInterval)
            {
                seconds = 0;
                EndTrivia(null, false);
            }
        }

        private void Answer(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! proper syntax: /answer,/a <answer here>");
                return;
            }
            if (T.Answer == "")
            {
                args.Player.SendErrorMessage("Trivia isn't currently running!");
                return;
            }
            string answer = string.Join(" ", args.Parameters).ToLower();
            if (!T.Answer.Contains(","))
            {
                if (T.Answer.Equals(answer.ToLower()))
                {
                    EndTrivia(args, true);
                    return;
                }
            }
            List<string> validanswers = new List<string>(T.Answer.Split(','));
            if (validanswers.Contains(answer.ToLower()))
            {
                EndTrivia(args, true);
                return;
            }
            else
            {
                WrongAnswers.Add(string.Format("{0}: {1} ", args.Player.Name, answer));
                args.Player.SendErrorMessage(string.Format("{0} is not the correct answer! better luck next time!", answer));
            }
        }

        public void EndTrivia(CommandArgs args, bool CorrectAnswer)
        {
            if (!CorrectAnswer)
                TSPlayer.All.SendErrorMessage("[Trivia] Time is up!");

            else
            {
                TSPlayer.All.SendInfoMessage(string.Format("{0} answered the trivia correctly! the answer was: {1}", args.Player.Name, T.Answer.Replace(',', '/')));
                if (config.DisplayWrongAnswers && WrongAnswers.Count > 0)
                {
                    TSPlayer.All.SendErrorMessage(string.Format("Wrong answers were: {0}", string.Join(", ", WrongAnswers)));
                }
                if (config.GiveSEconomyCurrency)
                {
                    IBankAccount Server = SEconomyPlugin.Instance.GetBankAccount(TSServerPlayer.Server.UserID);
                    IBankAccount Player = SEconomyPlugin.Instance.GetBankAccount(args.Player.Index);

                    Server.TransferToAsync(Player, config.CurrencyAmount, BankAccountTransferOptions.AnnounceToReceiver, "answering the trivia question correctly", "Answered trivia question");                    
                }
            }
            seconds = 0;
            T.Answer = ""; T.Question = "";
            WrongAnswers.Clear();
        }

        public void SetNewQuestion()
        {
            List<trivia> AllQuestionsAndAnswers = new List<trivia>(config.QuestionsandAnswers);
            trivia newtrivia = AllQuestionsAndAnswers[rnd.Next(0, AllQuestionsAndAnswers.Count)];
            T = new trivia(newtrivia.Question, newtrivia.Answer.ToLower());
            TSPlayer.All.SendInfoMessage("[Trivia] Type /answer or /a <answer here>");
            TSPlayer.All.SendInfoMessage("[Trivia] " +  T.Question);
        }

        public class trivia
        {
            public string Question;
            public string Answer;
            public trivia(string question, string answer)
            {
                Question = question;
                Answer = answer;
            }
        }

        public static trivia[] DefaultTrivia = new trivia[]
        {
            new trivia("Who was terraria made by?","Redigit"),
            new trivia("When was terraria first released?","2011"),
            new trivia("Which mod is this server running on?","TShock"),
            new trivia("What is the capital of Belgium?","Brussels"),
            new trivia("How many main islands make up the state of Hawaii?","8,eight"),
            new trivia("What is the official language of Egypt?","Arabic"),
            new trivia("What gas filled the Hindenburg airship?","Hydrogen"),
            new trivia("How many bytes are there in a kilobyte?","1024"),
            new trivia("How many days were there in the ancient Egyptian year?","365"),
            new trivia("How many horns does the average African black rhino have?","2,two"),
            new trivia("Tennis player Margaret Smith Court who holds a record 62 Grand Slam titles is from which country?","Australia"),
            new trivia("What is the international radio code word for the letter F?","Foxtrot"),
            new trivia("What disease is carcinomaphobia the fear of?","Cancer"),
            new trivia("What nationality was Pope John Paul II?","Polish"),
            new trivia("What sensory organ do starfish have at the end of each arm?","eye"),
            new trivia("What year was the first iPhone released?","2007"),
            new trivia("When was Twitter first launched?","2006"),
            new trivia("According to the Dewey Decimal System, library books numbered in the 500s are part of what category?","Sience"),
            new trivia("What is the capital of Germany?","Berlin"),
            new trivia("Mount Rushmore is located near which town in South Dakota?","Keystone"),
            new trivia("In what year did Ford first offer bucket seats on its automobiles?","1903"),
            new trivia("What gives beer its distinctive bitter flavour?","Hops"),
            new trivia("Who created the operating system \"Linux\"?","Linus Torvalds"),
            new trivia("When duct tape was first created in 1942, what was it known as?","Duck Tape"),
            new trivia("Which desert is found in South East Mongolia and Northern China?","Gobi"),
            new trivia("What is the only nation that borders both Pakistan and Bangladesh?","India"),
            new trivia("What fluid is stored in the gallbladder?","Bile"),
            new trivia("Where was King Arthur's court?","Camelot"),
            new trivia("What describes the amount of light allowed to pass through a camera lens?","Aperture"),
            new trivia("What did Nestle freeze dry in 1938 that led to the development of powdered food products?","Coffee"),
            new trivia("What comic strip was set in the Okeefenokee Swamp?","Pogo"),
            new trivia("Who aimed his \"Emporio\" clothing line at younger buyers?","Giorgio Armani"),
            new trivia("The gopher is a member of what order of mammals?","Rodents"),
            new trivia("Which university did Bill Gates drop out of in the 1970s to start Microsoft?","Harvard"),
            new trivia("How many openings were there in the Berlin Wall?","2"),
            new trivia("What is the molten rock magma called once it flows from a volcano?","Lava"),
            new trivia("How many bones are there in the human body?","206"),
            new trivia("What are categorized as the biological class \"aves\"?","Birds"),
            new trivia("What is a calorie a unit of?","Energy"),
            new trivia("What color is a polar bear's skin?","Black"),
            new trivia("How many carats is pure gold?","24"),
            new trivia("What kind of animal is raised in a warren?","Rabbit"),
            new trivia("What is the common name for butterfly larvae?","Caterpillars"),
            new trivia("What is March's birthstone?","Aquamarine"),
            new trivia("A 12-sided polygon is known as what?","Dodecagon"),
            new trivia("Poison Ivy is a relative to what plant?","Cashew"),
            new trivia("What is the second most common element in the Sun after hydrogen?","Helium"),
            new trivia("What is the fin on the back of a whale called?","Dorsal"),
            new trivia("Do ants sleep?","No"),
            new trivia("How many chuck would a Woodchuck chuck if a Woodchuck could chuck wood?","Chuck if I know"),
            new trivia("What element's chemical symbol is Pb?","Lead"),
            new trivia("What is a wallaby?","Kangaroo"),
            new trivia("If you suffer from phobophobia, what do you fear?","Phobias"),
            new trivia("What color is produced by the complete absorption of light rays?","Black"),
            new trivia("Which Greek goddess gave her name to the city of Athens?","Athena"),
            new trivia("What is the oldest university in the US?","Harvard"),
            new trivia("What continent borders the Weddell Sea?","Antarctica"),
            new trivia("Which popular Greek philosopher is said to have tutored Alexander the Great?","Aristotle"),
            new trivia("What do fried spiders taste like?","Nuts"),
            new trivia("What is the capital of Monaco?","Monaco"),
            new trivia("What is the capital of Egypt?","Cairo"),
            new trivia("In which ocean is the area known as Polynesia located?","Pacific"),
            new trivia("Which is US's largest state by land area?","Alaska"),
            new trivia("This US state was the first to end Marijuana prohibition in 2013?","Colorado"),
            new trivia("Which Australian state was formerly known as Van Diemens land?","Tasmania"),
            new trivia("Which continent would you find Morocco in?","Africa"),
            new trivia("What country is sometimes called the Land of Fire and Ice?","Iceland"),
            new trivia("What is the capital of Tunisia?","Tunis"),
            new trivia("In England every year since 1841, Oxford and Cambridge Universities have competed in which sport?","Rowing"),
            new trivia("These two words differ in spelling by one letter: one means to influence, the other means to cause. What are the two words?","Affect/Effect"),
            new trivia("Which product, well-known for its humorous advertising, was advertised in the 1960's with the two slogans: \"Small Wonder\" and \"Relieves Gas Pains\"?","Volkswagon"),
            new trivia("How many African countries lie on the Mediterranean Sea?","5"),
            new trivia("What suspenseful 2006 Tom Hanks movie was titled after one of the greatest thinkers of all time?","The Da Vinci Code"),
            new trivia("What type of painting is applied directly to a wall?","Mural"),
            new trivia("Between 1969 and 1977 which male entertainer performed more than 700 times, exclusively at the Las Vegas Hilton?","Elvis Presley"),
            new trivia("British explorer Richard Burton set out to Africa in 1858 to discover what?","The source of the Nile River"),
            new trivia("In the 17-18th centuries he was employed by the British government to help stop piracy on the high seas, but he turned to piracy himself, was caught and executed in England. Who was he?","Captain William Kidd"),
            new trivia("What colour eggs do owls lay?","White"),
            new trivia("This word is a five-letter synonym for jollity, gladness and gaiety","mirth"),
            new trivia("What is the official currency of the Vatican state?","Euro"),
            new trivia("Does light travel faster through water or air?","Air"),
            new trivia("What is the name of the seaside resort town in South East England where a major battle raged in the year 1066?","Hastings"),
            new trivia("In order of area: What are the three largest countries in the world?","Russia Canada China"),
            new trivia("In 1927, what adventurous 25 year old man was the first Time Magazine Person of the Year?","Charles Lindbergh"),
            new trivia("The only English word with five consecutive vowels is something the British do while waiting for a bus. What is it?","Queuing"),
            new trivia("What is the most common gift for fathers day?","tie"),
            new trivia("Because of the population problem, in China, married couples lose various governmental benefits after the birth of which child?","2"),
            new trivia("In 1507 German cartographer Martin Waldseemüller was the first mapmaker to use what name on a map of the new world?","America"),
            new trivia("Name all the planets in our solar system without a moon","Mercury Venus"),
            new trivia("Ancient Romans thought this chemical element was water ice, permanently frozen after great lengths of time. They also knew of the ability of this substance to split light into a spectrum. What was it?","Quartz"),
            new trivia("What is the largest island of French Polynesia?","Tahiti"),
            new trivia("The fastest growing plant can increase up to 35 inches (or 90 cm) in one day. What is it?","Bamboo"),
            new trivia("On what island is the country of Haiti located?","Hispaniola"),
            new trivia("What game uses a deck of cards, a board with holes in it, and small wooden pegs?","Cribbage"),             new trivia("The oldest street in San Francisco was originally called Dupont Street, but was later re-named after a President. Which street is this?","Grant Street"),
            new trivia("How many minutes long is each period in a game of professional ice hockey?","20 minutes"),
            new trivia("The city of Bratislava is the capital city of what new country?","Slovakia"),
            new trivia("When this musical artist released her first album, in 1985, it became the best-selling debut album of all time. Who was the artist?","Whitney Houston"),
            new trivia("Which military conquest in 1588 firmly established Britain as the world's leading naval power?","Defeat of the Spanish Armada"),
            new trivia("These plants, native to hot, dry regions of the U.S. and Mexico, are grown for ornament, fiber, and food, but more famously, as the main ingredient of tequila. What are they?","Agave"),
            new trivia("Which 1990's animated Disney film earned an Academy Award nomination in the category of Best Picture?","Beauty and the Beast"),
            new trivia("This Olympic event is a 3,000 meter footrace around a track. Runners must jump four hurdles and a water obstacle. What is this event called?","Steeplechase"),
            new trivia("What Korean type of martial arts was first contested for medals at the 2000 Sydney Olympics?","Tae Kwon Do"),
            new trivia("What are the names of the greek and roman wine gods in that order?","Dionysus and Bacchus"),
            new trivia("This book is considered a milestone in the history of feminism. It is called \"A Vindication of the Rights of Women,\" and was written by Mary Wollstonecraft in what year?","1792"),
            new trivia("What plant kingdom includes yeast, mold, and mushrooms?","Fungi"),
            new trivia("Which letter begins more words in the English language than any other letter?","S"),
            new trivia("Before the development of railroads, what was the principal means of land transportation for people in Europe, in the 18th and 19th Centuries?","Stagecoach"),
            new trivia("The alcoholic beverage sherry originated in which country?","Spain"),
            new trivia("What 19th century French impressionist painted The Boating Party?","Renoir"),
            new trivia("The largest money-losing film of all time was a 1980 film about land wars in Wyoming, which cost over $55 Million and grossed less than $2 Million. Critics panned this 3 1/2 hour film, and audiences rejected it. What was the title?","Heaven's Gate"),
            new trivia("The coldest place on the earth is","Verkoyansk"),
            new trivia("In what city can you find the Petronas Tower?","Kuala Lampur"),
            new trivia("what is a blue heeler?","Dog"),
            new trivia("The symbol of the Olympic Games is composed of how many interlocking rings?","5"),
            new trivia("Lapis lazuli is a deep shade of what colour?","Blue"),
            new trivia("Dong (lol) is the main unit of currency in what country?","Vietnam"),
            new trivia("What gift is associated with the 25th wedding anniversary?","Silver"),
            new trivia("In what year did the Berlin Wall fall?","1989"),
            new trivia("How many letters are there in the Greek Alphabet?","24"),
            new trivia("The study of butterflies and moths is called what?","Lepidopterology"),
            new trivia("What is the study or collection of postage stamps called?","Philately"),
            new trivia("What is the currency of Thailand?","Baht"),
            new trivia("How do you say Merry Christmas in Spanish?","Feliz Navidad"),
            new trivia("What do you call a tailless cat?","Manx"),
            new trivia("Whose nose grows when he tells a lie?","Pinocchio"),
            new trivia("What type of creature is a painted lady?","Butterfly"),
            new trivia("What is a baby kangaroo called?","Joey"),
            new trivia("What is the first letter of the Greek alphabet?","Alpha"),
            new trivia("What is the capital city of Fiji?","Suva"),
            new trivia("In which country will you find a city called Adelaide?","Australia"),
            new trivia("What do you call a group of frogs?","Army"),
            new trivia("Full stop to English people, what is it to Americans?","Period"),
            new trivia("What do you call the traditional Japanese art of folding paper?","Origami"),
            new trivia("What “C “ is the dried flesh of coconut?","Copra"),
            new trivia("Gingivitis is the infection of the ________","Gums"),
            new trivia("What is the left hand page of the book called?","Verso"),
            new trivia("What is a word that is formed by joining the first letters or the first few letters of a series of words called?","Acronym"),
            new trivia("Who painted the \"Last Supper\"?","Leonardo Da Vinci"),
            new trivia("What is the second highest mountain in the world?","k2"),
            new trivia("What is mozzarella?","Cheese"),
            new trivia("What spice is used in a Whisky Sling?","Nutmeg"),
            new trivia("How many squares are there in a chess board?","64"),
            new trivia("Which country are all Nippon Airlines from?","Japan"),
            new trivia("Whose house can be found at 221b Baker Street, London?","Sherlock Holmes"),
            new trivia("Halleys Comet last appeared in 1986 and will appear again in what year?","2062"),
            new trivia("A young swan is called a what?","Cygnet"),
            new trivia("Cosmology is the study of what?","Cosmetics"),
            new trivia("What is the name given to binge-eating followed by induced vomiting?","Bulimia"),
            new trivia("In the game of scrabble how many points is the letter Z worth?","10"),
            new trivia("an otologist is a doctor who specializes in which part of the body?","ear"),
            new trivia("Mozarts Symphony No. 31 in D is also known as what?","The Paris"),
            new trivia("Which painter painted the Sunflowers?","Vincent Van Gogh"),
            new trivia("What is the study of soil is called?","Pedology"),
            new trivia("In Astrology, which star sign occurs between Aries and Gemini","Taurus"),
            new trivia("Richard Bachman is the pen name of which author?","Stephen King"),
            new trivia("Pertussis is also known as what kind of cough?","Whooping"),
            new trivia("What is the largest organ of the human body?","skin"),
            new trivia("A sphygmomanometer is a device used to measure what?","Blood Pressure"),
            new trivia("What is the capital city of Japan?","Tokyo"),
            new trivia("What gift is associated with 40th wedding anniversary?","Ruby"),
            new trivia("What is the capital city of western australia?","Perth"),
            new trivia("Where is the Sea of Tranquility?","Moon"),
            new trivia("_______ is the male part of the flower","Stamen"),
            new trivia("What is the branch of Physics that deals with light and its properties?","Optics"),
            new trivia("The dot on top of an \"i\" is actually called what?","Tittle"),
            new trivia("The hashtag is actually called what?","Octothorpe"),
            new trivia("what is the vertical groove on the median line of the upper lip called?","Philtrum"),
            new trivia("What is the protective point or knob on the far end of an umbrella called?","Ferrule"),
            new trivia("The plastic bit at the end of a shoelace is called what?","Aglet"),
            new trivia("an indentation at the bottom of a molded glass bottle is called what?","Punt"),
            new trivia("A question mark followed my an exclamation point is called what?","Interrobang"),
            new trivia("What is the actual name of the infinity symbol?","Lemniscate"),
            new trivia("Who is the largest of the Sesame Street muppets?","Snuffleupagus"),
            new trivia("In 1937, Walt Disney studio released the world's first full-length animated film. What was the title?","Snow White and the Seven Dwarves"),
            new trivia("What colour is vermilion a shade of?","Red"),
            new trivia("King Zog ruled which country?","Albania"),
            new trivia("What colour is Spock's blood?","Green"),
            new trivia("Where in your body is your patella?","Knee"),
            new trivia("Where can you find London bridge today?","Arizona"),
            new trivia("What spirit is mixed with ginger beer in a Moscow mule?","Vodka"),
            new trivia("Who was the first man in space?","Yuri Gagarin"),
            new trivia("Who starred as the Six Million Dollar Man?","Lee Majors"),
            new trivia("In the song Waltzing Matilda, What is a Jumbuck?","Sheep"),
            new trivia("Who is Dick Grayson better known as?","Robin"),
            new trivia("What is a funambulist?","Tightrope Walker"),
            new trivia("In which war was the charge of the Light Brigade?","Crimean"),
            new trivia("Who invented the television?","John Logie Baird"),
            new trivia("Who would use a mashie niblick?","A golfer"),
            new trivia("What did Jack Horner pull from his pie?","A plum"),
            new trivia("How many feet in a fathom?","6"),
            new trivia("which film had the song Springtime for Hitler?","The Producers"),
            new trivia("What was Erich Weiss better known as?","Harry Houdini"),
            new trivia("Who wrote Gone with the Wind?","Margaret Mitchell"),
            new trivia("What does ring o' ring a roses refer to?","The Black Death"),
            new trivia("What would a Scotsman do with a spurtle?","Eat Porridge"),
            new trivia("What was the name of the inn in Treasure Island?","Admiral Benbow"),
            new trivia("If you had pogonophobia what would you be afraid of?","Beards"),
            new trivia("Which country grows the most fruit?","China"),
            new trivia("What would you do with a maris piper?","Eat it"),
            new trivia("In Casablanca what is the name of the nightclub?","Rick's"),
            new trivia("What is the currency of Austria?","Shillings"),
            new trivia("What is the Islamic equal to the red cross?","Red Crescent"),
            new trivia("Triskadeccaphobia is the fear of what?","The number 13"),
            new trivia("What is classified by the A B O system?","Blood Types"),
            new trivia("Ray Bolger played who in The Wizard of Oz?","Scarecrow"),
            new trivia("On the Moh scale, the hardest substance is diamond. what is the softest?","Talc"),
            new trivia("Where did the Pied Piper play?","Hamlin"),
            new trivia("La Giaconda is better known as what?","Mona Lisa"),
            new trivia("Who wrote the Opera Madam Butterfly?","Puccini"),
            new trivia("Which non alcoholic cordial is made from pomegranates?","Grenadine"),
            new trivia("Which 1993 Disney film starred Bet Middler as a witch?","Hocus Pocus"),
            new trivia("Eric Arthur Blaire was the real name of which author?","George Orwell"),
            new trivia("Who wrote Catch 22?","Joseph Heller"),
            new trivia("What is the answer to life, the universe, and everything?","42"),
            new trivia("In Japan what is Seppuku?","Hari Kari"),
            new trivia("What martial arts name means gentle way?","Judo"),
            new trivia("Kimberlite contains what precious item?","Diamonds"),
            new trivia("In Greek mythology a Hamadryads spirit guarded what?","Trees"),
            new trivia("Who wrote The Rights of Man and The Age of Reason?","Thomas Paine"),
            new trivia("What is mainly extracted from pitchblende?","Uranium"),
        };

        class Config
        {
            public int QuestionInterval = 120;
            public int AnswerTime = 45;
            public bool DisplayWrongAnswers = true;
            public bool GiveSEconomyCurrency = false;
            public int CurrencyAmount = 100;
            public trivia[] QuestionsandAnswers = DefaultTrivia;
        }

        private static void CreateConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "Trivia.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }

        private static bool ReadConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "Trivia.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("Trivia config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }

        private void Reload_Config(CommandArgs args)
        {
            if (ReadConfig())
            {
                WrongAnswers.Clear();
                T.Answer = ""; T.Question = "";
                args.Player.SendMessage("Trivia config reloaded sucessfully.", Color.Green);
            }
        }
    }
}