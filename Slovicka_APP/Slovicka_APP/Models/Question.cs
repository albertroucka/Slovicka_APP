using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }

        List<Translate> Usages = new List<Translate>();
        int used = 0;

        public Question GetNewQuestion(List<Translate> gameList, bool translate)
        {
            Random r = new Random();
            var posts = gameList;

            if (Usages.Count == gameList.Count)
            {
                used = 0;
                Usages.Clear();
            }

            if (used > 0)
            {
                while (true)
                {
                    bool b = false;
                    int i = r.Next(0, posts.Count);
                    var quest = posts[i];

                    foreach (var item in Usages)
                    {
                        if (item.FirstWord == quest.FirstWord)
                        {
                            b = true; break;
                        }
                    }

                    if (b == false)
                    {
                        Question question = new Question();

                        if (translate == false)
                        {
                            question.QuestionText = quest.FirstWord;
                            question.CorrectAnswer = quest.SecondWord;
                        }
                        else
                        {
                            question.QuestionText = quest.SecondWord;
                            question.CorrectAnswer = quest.FirstWord;
                        }

                        Usages.Add(quest);
                        used++;
                        return question;
                    }
                }
            }
            else
            {
                Question question = new Question();
                int i = r.Next(0, posts.Count);
                var quest = posts[i];

                if (translate == false)
                {
                    question.QuestionText = quest.FirstWord;
                    question.CorrectAnswer = quest.SecondWord;
                }
                else
                {
                    question.QuestionText = quest.SecondWord;
                    question.CorrectAnswer = quest.FirstWord;
                }
                Usages.Add(quest);
                used++;
                return question;
            }
        }
    }

    public class QuestionABC
    {
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string CorrectAnswer { get; set; }

        List<Translate> Usages = new List<Translate>();
        int used = 0; int i;

        public QuestionABC GetNewQuestion(List<Translate> gameList, bool translate)
        {
            QuestionABC question = new QuestionABC();
            Random r = new Random(); var posts = gameList;

            if (Usages.Count == gameList.Count)
            {
                used = 0;
                Usages.Clear();
            }

            if (used > 0)
            {
                while (true)
                {
                    bool b = false;
                    i = r.Next(0, posts.Count);
                    var quest = posts[i];

                    foreach (var item in Usages)
                    {
                        if (item.FirstWord == quest.FirstWord)
                        {
                            b = true; 
                            break;
                        }
                    }

                    if (b == false)
                    {
                        if (translate == false)
                        {
                            question.QuestionText = quest.FirstWord;
                            question.CorrectAnswer = quest.SecondWord;
                        }
                        else
                        {
                            question.QuestionText = quest.SecondWord;
                            question.CorrectAnswer = quest.FirstWord;
                        }

                        Usages.Add(quest);
                        used++;
                        break;
                    }
                }
            }
            else
            {
                int i = r.Next(0, posts.Count);
                var quest = posts[i];

                if (translate == false)
                {
                    question.QuestionText = quest.FirstWord;
                    question.CorrectAnswer = quest.SecondWord;
                }
                else
                {
                    question.QuestionText = quest.SecondWord;
                    question.CorrectAnswer = quest.FirstWord;
                }

                Usages.Add(quest);
                used++;
            }

            var quests = Usages[Usages.Count - 1]; int i1 = 0;
            var optionA = quests; var optionB = new Translate(); var optionC = new Translate();

            while (i1 < 2)
            {
                i = r.Next(0, posts.Count);
                quests = posts[i];

                if (quests.FirstWord != optionA.FirstWord && quests.FirstWord != optionB.FirstWord)
                {
                    if (optionB.FirstWord == null)
                    {
                        optionB = quests;
                        i1++;
                    }
                    else
                    {
                        optionC = quests;
                        i1++;
                    }
                }
            }

            if (translate == false)
            {
                i = r.Next(0, 3);
                if (i == 0)
                {
                    question.OptionA = optionA.SecondWord;
                    question.OptionB = optionB.SecondWord;
                    question.OptionC = optionC.SecondWord;
                }
                else if (i == 1)
                {
                    question.OptionA = optionC.SecondWord;
                    question.OptionB = optionA.SecondWord;
                    question.OptionC = optionB.SecondWord;
                }
                else
                {
                    question.OptionA = optionB.SecondWord;
                    question.OptionB = optionC.SecondWord;
                    question.OptionC = optionA.SecondWord;
                }
            }
            else
            {
                i = r.Next(0, 3);
                if (i == 0)
                {
                    question.OptionA = optionA.FirstWord;
                    question.OptionB = optionB.FirstWord;
                    question.OptionC = optionC.FirstWord;
                }
                else if (i == 1)
                {
                    question.OptionA = optionC.FirstWord;
                    question.OptionB = optionA.FirstWord;
                    question.OptionC = optionB.FirstWord;
                }
                else
                {
                    question.OptionA = optionB.FirstWord;
                    question.OptionB = optionC.FirstWord;
                    question.OptionC = optionA.FirstWord;
                }
            }

            return question;
        }
    }

    public class QuestionCards
    {
        public List<Translate> GetRandomSequence(List<Translate> gameList, bool translate)
        {
            List<Translate> CardsList = new List<Translate>();
            Random r = new Random(); int i; int x = 0; bool b = false;

            while (x < gameList.Count)
            {
                i = r.Next(0, gameList.Count);
                var quest = gameList[i]; b = false;
                if (x > 0)
                {
                    foreach (var item in CardsList)
                    {
                        if (item.FirstWord == quest.FirstWord || item.FirstWord == quest.SecondWord)
                        {
                            b = true;
                        }
                    }

                    if (b == false)
                    {
                        if (translate)
                        {
                            Translate t = new Translate() { GroupName = quest.GroupName, FirstWord = quest.SecondWord, SecondWord = quest.FirstWord };
                            CardsList.Add(t);
                        }
                        else
                        {
                            CardsList.Add(quest);
                        }
                        x++;
                    }
                }
                else
                {
                    if (translate)
                    {
                        Translate t = new Translate() { GroupName= quest.GroupName, FirstWord = quest.SecondWord, SecondWord = quest.FirstWord};
                        CardsList.Add(t);
                    }
                    else
                    {
                        CardsList.Add(quest);
                    }
                    x++;
                }
            }

            return CardsList;
        }
    }
}
