using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Timers;

namespace TRIVIA
{
    [ApiVersion(1, 17)]
    public class Trivia : TerrariaPlugin
    {
        public static TriviaManager TM = new TriviaManager();

        Timer Timer = new Timer(1000) { Enabled = true };
        public int seconds = 0;
        public static Config config;

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
            Timer.Elapsed += OnTimer;
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
            if (seconds > config.QuestionInterval && !TM.Enabled)
            {
                seconds = 0;
                TM.StartTrivia();
            }
            else if (seconds > config.AnswerTime)
            {
                seconds = 0;
                TM.EndTrivia(null, false);
            }
        }
        private void Answer(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! proper syntax: /answer,/a <answer here>");
                return;
            }
            if (!TM.Enabled)
            {
                args.Player.SendErrorMessage("Trivia isn't currently running!");
                return;
            }
            string answer = string.Join(" ", args.Parameters);
            if (TM.IsCorrectAnswer(answer))
                TM.EndTrivia(args.Player, true);
            else
            {
                TM.WrongAnswers.Add(answer);
                args.Player.SendErrorMessage(string.Format("{0} is not the correct answer! better luck next time!", answer));
            }
        }

        public static QAndA[] DefaultTrivia = new QAndA[]
        {
            new QAndA("Who was terraria made by?","Redigit"),
            new QAndA("When was terraria first released?","2011"),
            new QAndA("Which mod is this server running on?","TShock"),
            new QAndA("What is the capital of Belgium?","Brussels"),
            new QAndA("How many main islands make up the state of Hawaii?","8,eight"),
            new QAndA("What is the official language of Egypt?","Arabic"),
            new QAndA("What gas filled the Hindenburg airship?","Hydrogen"),
            new QAndA("How many bytes are there in a kilobyte?","1024"),
            new QAndA("How many days were there in the ancient Egyptian year?","365"),
            new QAndA("How many horns does the average African black rhino have?","2,two"),
            new QAndA("Tennis player Margaret Smith Court who holds a record 62 Grand Slam titles is from which country?","Australia"),
            new QAndA("What is the international radio code word for the letter F?","Foxtrot"),
            new QAndA("What disease is carcinomaphobia the fear of?","Cancer"),
            new QAndA("What nationality was Pope John Paul II?","Polish"),
            new QAndA("What sensory organ do starfish have at the end of each arm?","eye"),
            new QAndA("What year was the first iPhone released?","2007"),
            new QAndA("When was Twitter first launched?","2006"),
            new QAndA("According to the Dewey Decimal System, library books numbered in the 500s are part of what category?","Sience"),
            new QAndA("What is the capital of Germany?","Berlin"),
            new QAndA("Mount Rushmore is located near which town in South Dakota?","Keystone"),
            new QAndA("In what year did Ford first offer bucket seats on its automobiles?","1903"),
            new QAndA("What gives beer its distinctive bitter flavour?","Hops"),
            new QAndA("Who created the operating system \"Linux\"?","Linus Torvalds"),
            new QAndA("When duct tape was first created in 1942, what was it known as?","Duck Tape"),
            new QAndA("Which desert is found in South East Mongolia and Northern China?","Gobi"),
            new QAndA("What is the only nation that borders both Pakistan and Bangladesh?","India"),
            new QAndA("What fluid is stored in the gallbladder?","Bile"),
            new QAndA("Where was King Arthur's court?","Camelot"),
            new QAndA("What describes the amount of light allowed to pass through a camera lens?","Aperture"),
            new QAndA("What did Nestle freeze dry in 1938 that led to the development of powdered food products?","Coffee"),
            new QAndA("What comic strip was set in the Okeefenokee Swamp?","Pogo"),
            new QAndA("Who aimed his \"Emporio\" clothing line at younger buyers?","Giorgio Armani"),
            new QAndA("The gopher is a member of what order of mammals?","Rodents"),
            new QAndA("Which university did Bill Gates drop out of in the 1970s to start Microsoft?","Harvard"),
            new QAndA("How many openings were there in the Berlin Wall?","2"),
            new QAndA("What is the molten rock magma called once it flows from a volcano?","Lava"),
            new QAndA("How many bones are there in the human body?","206"),
            new QAndA("What are categorized as the biological class \"aves\"?","Birds"),
            new QAndA("What is a calorie a unit of?","Energy"),
            new QAndA("What color is a polar bear's skin?","Black"),
            new QAndA("How many carats is pure gold?","24"),
            new QAndA("What kind of animal is raised in a warren?","Rabbit"),
            new QAndA("What is the common name for butterfly larvae?","Caterpillars"),
            new QAndA("What is March's birthstone?","Aquamarine"),
            new QAndA("A 12-sided polygon is known as what?","Dodecagon"),
            new QAndA("Poison Ivy is a relative to what plant?","Cashew"),
            new QAndA("What is the second most common element in the Sun after hydrogen?","Helium"),
            new QAndA("What is the fin on the back of a whale called?","Dorsal"),
            new QAndA("Do ants sleep?","No"),
            new QAndA("How many chuck would a Woodchuck chuck if a Woodchuck could chuck wood?","Chuck if I know"),
            new QAndA("What element's chemical symbol is Pb?","Lead"),
            new QAndA("What is a wallaby?","Kangaroo"),
            new QAndA("If you suffer from phobophobia, what do you fear?","Phobias"),
            new QAndA("What color is produced by the complete absorption of light rays?","Black"),
            new QAndA("Which Greek goddess gave her name to the city of Athens?","Athena"),
            new QAndA("What is the oldest university in the US?","Harvard"),
            new QAndA("What continent borders the Weddell Sea?","Antarctica"),
            new QAndA("Which popular Greek philosopher is said to have tutored Alexander the Great?","Aristotle"),
            new QAndA("What do fried spiders taste like?","Nuts"),
            new QAndA("What is the capital of Monaco?","Monaco"),
            new QAndA("What is the capital of Egypt?","Cairo"),
            new QAndA("In which ocean is the area known as Polynesia located?","Pacific"),
            new QAndA("Which is US's largest state by land area?","Alaska"),
            new QAndA("This US state was the first to end Marijuana prohibition in 2013?","Colorado"),
            new QAndA("Which Australian state was formerly known as Van Diemens land?","Tasmania"),
            new QAndA("Which continent would you find Morocco in?","Africa"),
            new QAndA("What country is sometimes called the Land of Fire and Ice?","Iceland"),
            new QAndA("What is the capital of Tunisia?","Tunis"),
            new QAndA("In England every year since 1841, Oxford and Cambridge Universities have competed in which sport?","Rowing"),
            new QAndA("These two words differ in spelling by one letter: one means to influence, the other means to cause. What are the two words?","Affect/Effect"),
            new QAndA("Which product, well-known for its humorous advertising, was advertised in the 1960's with the two slogans: \"Small Wonder\" and \"Relieves Gas Pains\"?","Volkswagon"),
            new QAndA("How many African countries lie on the Mediterranean Sea?","5"),
            new QAndA("What suspenseful 2006 Tom Hanks movie was titled after one of the greatest thinkers of all time?","The Da Vinci Code"),
            new QAndA("What type of painting is applied directly to a wall?","Mural"),
            new QAndA("Between 1969 and 1977 which male entertainer performed more than 700 times, exclusively at the Las Vegas Hilton?","Elvis Presley"),
            new QAndA("British explorer Richard Burton set out to Africa in 1858 to discover what?","The source of the Nile River"),
            new QAndA("In the 17-18th centuries he was employed by the British government to help stop piracy on the high seas, but he turned to piracy himself, was caught and executed in England. Who was he?","Captain William Kidd"),
            new QAndA("What colour eggs do owls lay?","White"),
            new QAndA("This word is a five-letter synonym for jollity, gladness and gaiety","mirth"),
            new QAndA("What is the official currency of the Vatican state?","Euro"),
            new QAndA("Does light travel faster through water or air?","Air"),
            new QAndA("What is the name of the seaside resort town in South East England where a major battle raged in the year 1066?","Hastings"),
            new QAndA("In order of area: What are the three largest countries in the world?","Russia Canada China"),
            new QAndA("In 1927, what adventurous 25 year old man was the first Time Magazine Person of the Year?","Charles Lindbergh"),
            new QAndA("The only English word with five consecutive vowels is something the British do while waiting for a bus. What is it?","Queuing"),
            new QAndA("What is the most common gift for fathers day?","tie"),
            new QAndA("Because of the population problem, in China, married couples lose various governmental benefits after the birth of which child?","2"),
            new QAndA("In 1507 German cartographer Martin Waldseemüller was the first mapmaker to use what name on a map of the new world?","America"),
            new QAndA("Name all the planets in our solar system without a moon","Mercury Venus"),
            new QAndA("Ancient Romans thought this chemical element was water ice, permanently frozen after great lengths of time. They also knew of the ability of this substance to split light into a spectrum. What was it?","Quartz"),
            new QAndA("What is the largest island of French Polynesia?","Tahiti"),
            new QAndA("The fastest growing plant can increase up to 35 inches (or 90 cm) in one day. What is it?","Bamboo"),
            new QAndA("On what island is the country of Haiti located?","Hispaniola"),
            new QAndA("What game uses a deck of cards, a board with holes in it, and small wooden pegs?","Cribbage"),
            new QAndA("The oldest street in San Francisco was originally called Dupont Street, but was later re-named after a President. Which street is this?","Grant Street"),
            new QAndA("How many minutes long is each period in a game of professional ice hockey?","20 minutes"),
            new QAndA("The city of Bratislava is the capital city of what new country?","Slovakia"),
            new QAndA("When this musical artist released her first album, in 1985, it became the best-selling debut album of all time. Who was the artist?","Whitney Houston"),
            new QAndA("Which military conquest in 1588 firmly established Britain as the world's leading naval power?","Defeat of the Spanish Armada"),
            new QAndA("These plants, native to hot, dry regions of the U.S. and Mexico, are grown for ornament, fiber, and food, but more famously, as the main ingredient of tequila. What are they?","Agave"),
            new QAndA("Which 1990's animated Disney film earned an Academy Award nomination in the category of Best Picture?","Beauty and the Beast"),
            new QAndA("This Olympic event is a 3,000 meter footrace around a track. Runners must jump four hurdles and a water obstacle. What is this event called?","Steeplechase"),
            new QAndA("What Korean type of martial arts was first contested for medals at the 2000 Sydney Olympics?","Tae Kwon Do"),
            new QAndA("What are the names of the greek and roman wine gods in that order?","Dionysus and Bacchus"),
            new QAndA("This book is considered a milestone in the history of feminism. It is called \"A Vindication of the Rights of Women,\" and was written by Mary Wollstonecraft in what year?","1792"),
            new QAndA("What plant kingdom includes yeast, mold, and mushrooms?","Fungi"),
            new QAndA("Which letter begins more words in the English language than any other letter?","S"),
            new QAndA("Before the development of railroads, what was the principal means of land transportation for people in Europe, in the 18th and 19th Centuries?","Stagecoach"),
            new QAndA("The alcoholic beverage sherry originated in which country?","Spain"),
            new QAndA("What 19th century French impressionist painted The Boating Party?","Renoir"),
            new QAndA("The largest money-losing film of all time was a 1980 film about land wars in Wyoming, which cost over $55 Million and grossed less than $2 Million. Critics panned this 3 1/2 hour film, and audiences rejected it. What was the title?","Heaven's Gate"),
            new QAndA("The coldest place on the earth is","Verkoyansk"),
            new QAndA("In what city can you find the Petronas Tower?","Kuala Lampur"),
            new QAndA("what is a blue heeler?","Dog"),
            new QAndA("The symbol of the Olympic Games is composed of how many interlocking rings?","5"),
            new QAndA("Lapis lazuli is a deep shade of what colour?","Blue"),
            new QAndA("Dong (lol) is the main unit of currency in what country?","Vietnam"),
            new QAndA("What gift is associated with the 25th wedding anniversary?","Silver"),
            new QAndA("In what year did the Berlin Wall fall?","1989"),
            new QAndA("How many letters are there in the Greek Alphabet?","24"),
            new QAndA("The study of butterflies and moths is called what?","Lepidopterology"),
            new QAndA("What is the study or collection of postage stamps called?","Philately"),
            new QAndA("What is the currency of Thailand?","Baht"),
            new QAndA("How do you say Merry Christmas in Spanish?","Feliz Navidad"),
            new QAndA("What do you call a tailless cat?","Manx"),
            new QAndA("Whose nose grows when he tells a lie?","Pinocchio"),
            new QAndA("What type of creature is a painted lady?","Butterfly"),
            new QAndA("What is a baby kangaroo called?","Joey"),
            new QAndA("What is the first letter of the Greek alphabet?","Alpha"),
            new QAndA("What is the capital city of Fiji?","Suva"),
            new QAndA("In which country will you find a city called Adelaide?","Australia"),
            new QAndA("What do you call a group of frogs?","Army"),
            new QAndA("Full stop to English people, what is it to Americans?","Period"),
            new QAndA("What do you call the traditional Japanese art of folding paper?","Origami"),
            new QAndA("What “C “ is the dried flesh of coconut?","Copra"),
            new QAndA("Gingivitis is the infection of the ________","Gums"),
            new QAndA("What is the left hand page of the book called?","Verso"),
            new QAndA("What is a word that is formed by joining the first letters or the first few letters of a series of words called?","Acronym"),
            new QAndA("Who painted the \"Last Supper\"?","Leonardo Da Vinci"),
            new QAndA("What is the second highest mountain in the world?","k2"),
            new QAndA("What is mozzarella?","Cheese"),
            new QAndA("What spice is used in a Whisky Sling?","Nutmeg"),
            new QAndA("How many squares are there in a chess board?","64"),
            new QAndA("Which country are all Nippon Airlines from?","Japan"),
            new QAndA("Whose house can be found at 221b Baker Street, London?","Sherlock Holmes"),
            new QAndA("Halleys Comet last appeared in 1986 and will appear again in what year?","2062"),
            new QAndA("A young swan is called a what?","Cygnet"),
            new QAndA("Cosmology is the study of what?","Cosmetics"),
            new QAndA("What is the name given to binge-eating followed by induced vomiting?","Bulimia"),
            new QAndA("In the game of scrabble how many points is the letter Z worth?","10"),
            new QAndA("an otologist is a doctor who specializes in which part of the body?","ear"),
            new QAndA("Mozarts Symphony No. 31 in D is also known as what?","The Paris"),
            new QAndA("Which painter painted the Sunflowers?","Vincent Van Gogh"),
            new QAndA("What is the study of soil is called?","Pedology"),
            new QAndA("In Astrology, which star sign occurs between Aries and Gemini","Taurus"),
            new QAndA("Richard Bachman is the pen name of which author?","Stephen King"),
            new QAndA("Pertussis is also known as what kind of cough?","Whooping"),
            new QAndA("What is the largest organ of the human body?","skin"),
            new QAndA("A sphygmomanometer is a device used to measure what?","Blood Pressure"),
            new QAndA("What is the capital city of Japan?","Tokyo"),
            new QAndA("What gift is associated with 40th wedding anniversary?","Ruby"),
            new QAndA("What is the capital city of western australia?","Perth"),
            new QAndA("Where is the Sea of Tranquility?","Moon"),
            new QAndA("_______ is the male part of the flower","Stamen"),
            new QAndA("What is the branch of Physics that deals with light and its properties?","Optics"),
            new QAndA("The dot on top of an \"i\" is actually called what?","Tittle"),
            new QAndA("The hashtag is actually called what?","Octothorpe"),
            new QAndA("what is the vertical groove on the median line of the upper lip called?","Philtrum"),
            new QAndA("What is the protective point or knob on the far end of an umbrella called?","Ferrule"),
            new QAndA("The plastic bit at the end of a shoelace is called what?","Aglet"),
            new QAndA("an indentation at the bottom of a molded glass bottle is called what?","Punt"),
            new QAndA("A question mark followed my an exclamation point is called what?","Interrobang"),
            new QAndA("What is the actual name of the infinity symbol?","Lemniscate"),
            new QAndA("Who is the largest of the Sesame Street muppets?","Snuffleupagus"),
            new QAndA("In 1937, Walt Disney studio released the world's first full-length animated film. What was the title?","Snow White and the Seven Dwarves"),
            new QAndA("What colour is vermilion a shade of?","Red"),
            new QAndA("King Zog ruled which country?","Albania"),
            new QAndA("What colour is Spock's blood?","Green"),
            new QAndA("Where in your body is your patella?","Knee"),
            new QAndA("Where can you find London bridge today?","Arizona"),
            new QAndA("What spirit is mixed with ginger beer in a Moscow mule?","Vodka"),
            new QAndA("Who was the first man in space?","Yuri Gagarin"),
            new QAndA("Who starred as the Six Million Dollar Man?","Lee Majors"),
            new QAndA("In the song Waltzing Matilda, What is a Jumbuck?","Sheep"),
            new QAndA("Who is Dick Grayson better known as?","Robin"),
            new QAndA("What is a funambulist?","Tightrope Walker"),
            new QAndA("In which war was the charge of the Light Brigade?","Crimean"),
            new QAndA("Who invented the television?","John Logie Baird"),
            new QAndA("Who would use a mashie niblick?","A golfer"),
            new QAndA("What did Jack Horner pull from his pie?","A plum"),
            new QAndA("How many feet in a fathom?","6"),
            new QAndA("which film had the song Springtime for Hitler?","The Producers"),
            new QAndA("What was Erich Weiss better known as?","Harry Houdini"),
            new QAndA("Who wrote Gone with the Wind?","Margaret Mitchell"),
            new QAndA("What does ring o' ring a roses refer to?","The Black Death"),
            new QAndA("What would a Scotsman do with a spurtle?","Eat Porridge"),
            new QAndA("What was the name of the inn in Treasure Island?","Admiral Benbow"),
            new QAndA("If you had pogonophobia what would you be afraid of?","Beards"),
            new QAndA("Which country grows the most fruit?","China"),
            new QAndA("What would you do with a maris piper?","Eat it"),
            new QAndA("In Casablanca what is the name of the nightclub?","Rick's"),
            new QAndA("What is the currency of Austria?","Shillings"),
            new QAndA("What is the Islamic equal to the red cross?","Red Crescent"),
            new QAndA("Triskadeccaphobia is the fear of what?","The number 13"),
            new QAndA("What is classified by the A B O system?","Blood Types"),
            new QAndA("Ray Bolger played who in The Wizard of Oz?","Scarecrow"),
            new QAndA("On the Moh scale, the hardest substance is diamond. what is the softest?","Talc"),
            new QAndA("Where did the Pied Piper play?","Hamlin"),
            new QAndA("La Giaconda is better known as what?","Mona Lisa"),
            new QAndA("Who wrote the Opera Madam Butterfly?","Puccini"),
            new QAndA("Which non alcoholic cordial is made from pomegranates?","Grenadine"),
            new QAndA("Which 1993 Disney film starred Bet Middler as a witch?","Hocus Pocus"),
            new QAndA("Eric Arthur Blaire was the real name of which author?","George Orwell"),
            new QAndA("Who wrote Catch 22?","Joseph Heller"),
            new QAndA("What is the answer to life, the universe, and everything?","42"),
            new QAndA("In Japan what is Seppuku?","Hari Kari"),
            new QAndA("What martial arts name means gentle way?","Judo"),
            new QAndA("Kimberlite contains what precious item?","Diamonds"),
            new QAndA("In Greek mythology a Hamadryads spirit guarded what?","Trees"),
            new QAndA("Who wrote The Rights of Man and The Age of Reason?","Thomas Paine"),
            new QAndA("What is mainly extracted from pitchblende?","Uranium"),
        };
        public class Config
        {
            public int QuestionInterval = 120;
            public int AnswerTime = 45;
            public bool DisplayWrongAnswers = true;
            public int CurrencyAmount = 100;
            public QAndA[] QuestionsAndAnswers = DefaultTrivia;
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
                TShock.Log.ConsoleError(ex.Message);
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
                            TM.QuestionsAndAnswers = new List<QAndA>(config.QuestionsAndAnswers);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    TShock.Log.ConsoleError("Trivia config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
            }
            return false;
        }
        private void Reload_Config(CommandArgs args)
        {
            if (ReadConfig())
            {
                args.Player.SendMessage("Trivia config reloaded sucessfully.", Color.Green);
            }
        }
    }
}