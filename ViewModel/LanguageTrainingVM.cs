using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.ViewModel
{
    using BaseClasses;
    using System.Diagnostics;
    using FlashCards.Model;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using FlashCards.DAL.Enities;

    class LanguageTrainingVM : BaseViewModel
    {
        private Model model = null;
        private Word question;
        private Word answer;
        private List<Word> questions = null;
        private List<Word> answers = null;
        private List<FrontBack> _frontBack = null;
        private List<WordKnowledge> performance = null;
        private sbyte? user = null;
        private Language _translation;
        private string otherTranslations;
        private int iter = 0;
        private bool isUserGuessing = true;
        private bool isUserRating = false;

        public string OtherTranslations
        {
            get { return otherTranslations; }
            set { otherTranslations = value; onPropertyChanged(nameof(OtherTranslations)); }
        }

        public Word Question
        {
            get { return question; }
            set { question = value; onPropertyChanged(nameof(Question)); }
        }

        public Word Answer
        {
            get { return answer; }
            set { answer = value; onPropertyChanged(nameof(Answer)); }
        }

        public List<Word> Questions
        {
            get { return questions; }
            set { questions = value; onPropertyChanged(nameof(Questions)); }
        }

        public List<Word> Answers
        {
            get { return answers; }
            set { answers = value; onPropertyChanged(nameof(Answers)); }
        }

        public List<FrontBack> FrontBacks
        {
            get { return _frontBack; }
            set { _frontBack = value; onPropertyChanged(nameof(FrontBacks)); }
        }

        public List<WordKnowledge> Performance
        {
            get { return performance; }
            set { performance = value; onPropertyChanged(nameof(Performance)); }
        }

        public bool IsUserGuessing
        {
            get { return isUserGuessing; }
            set { isUserGuessing = value; onPropertyChanged(nameof(isUserGuessing)); }
        }

        public bool IsUserRating
        {
            get { return isUserRating; }
            set { isUserRating = value; onPropertyChanged(nameof(IsUserRating)); }
        }
        public string Title { get; set; }

        public LanguageTrainingVM() 
        {
            
        }

        public LanguageTrainingVM(Model model, List<Word> questions, List<Word> answers, List<FrontBack> frontBack, sbyte? id_user, string langA, Language langB)
        {
            this.model = model;
            Questions = questions;
            Answers = answers;
            _frontBack = frontBack;
            Performance = new List<WordKnowledge>();
            user = id_user;
            Title = langA+" -> "+langB.LangName;
            _translation = langB;
            GetNewWord();

        }

        private WordKnowledge SaveProgres(sbyte factor)
        {
            foreach (var fb in FrontBacks)
            {
                if( (fb.Front == Question && fb.Back == Answer) || (fb.Back == Question && fb.Front == Answer))
                {
                    factor += fb.Knowledge;
                    if (factor > 127 || factor < 0)
                    {
                        return new WordKnowledge(fb.Front.Id, fb.Back.Id, (sbyte)user, fb.Knowledge);
                    }
                    else
                    {
                        FrontBacks[FrontBacks.IndexOf(fb)].Knowledge = factor;
                        return new WordKnowledge(fb.Front.Id, fb.Back.Id, (sbyte)user, factor);
                    }     
                }
            }
            return null;
        }

        private static Random random = new Random();
        private List<Word> Shuffle(List<Word> list)
        {
            for (int n = list.Count - 1; n > 1; n--)
            {
                int rng = random.Next(n + 1);
                Word value = list[rng];
                list[rng] = list[n];
                list[n] = value;
            }
            return list;
        }

        private void GetNewWord()
        {
            Question = Questions[iter++];
            Answer = FindAnswerByGUID(Question);

            if (iter > Questions.Count-1)
            {
                iter = 0;
                Questions = Shuffle(Questions);
            }
        }

        private List<Word> FindOtherMeanings(Word origin)
        {
            return model.PassOtherTranslations(origin, _translation);
        }

        private Word FindAnswerByGUID(Word q)
        {
            Word ans = null;

            foreach (var w in Answers)
                if (w.GUID == q.GUID)
                {
                    ans = w;
                    break;
                } 
            return ans;
        }

        private ICommand showAnswer = null;

        public ICommand ShowAnswer
        {
            get
            {
                if(showAnswer == null)
                {
                    showAnswer = new RelayCommand(
                        arg =>
                        {
                            List<Word> others = FindOtherMeanings(Question);
                            if (others.Any())
                            {
                                OtherTranslations = "Other : ";
                                foreach (var other in others)
                                {
                                    OtherTranslations += other + ", ";
                                }
                            }
                            else
                                OtherTranslations = "";

                            IsUserGuessing = false;
                            IsUserRating = true;
                        },
                        arg => true
                        );
                }

                return showAnswer;
            }
        }


        private ICommand grantMinusOne = null;

        public ICommand GrantMinusOne
        {
            get
            {
                if (grantMinusOne == null)
                {
                    grantMinusOne = new RelayCommand(
                        arg =>
                        {
                            model.UpdateWordKnowledge(SaveProgres(-1));
                            GetNewWord();
                            IsUserGuessing = true;
                            IsUserRating = false;
                        },
                        arg => true
                        );
                }

                return grantMinusOne;
            }
        }

        private ICommand grantPlusOne = null;

        public ICommand GrantPlusOne
        {
            get
            {
                if (grantPlusOne == null)
                {
                    grantPlusOne = new RelayCommand(
                        arg =>
                        {
                            model.UpdateWordKnowledge(SaveProgres(1));
                            GetNewWord();
                            IsUserGuessing = true;
                            IsUserRating = false;
                        },
                        arg => true
                        );
                }

                return grantPlusOne;
            }
        }

        private ICommand grantPlusThree = null;

        public ICommand GrantPlusThree
        {
            get
            {
                if (grantPlusThree == null)
                {
                    grantPlusThree = new RelayCommand(
                        arg =>
                        {
                            model.UpdateWordKnowledge(SaveProgres(3));
                            GetNewWord();
                            IsUserGuessing = true;
                            IsUserRating = false;
                        },
                        arg => true
                        );
                }

                return grantPlusThree;
            }
        }

        private ICommand goBack = null;

        public ICommand GoBack
        {
            get
            {
                if (goBack == null)
                {
                    goBack = new RelayCommand(
                        arg =>
                        {
                            Mediator.Notify("BackFromTrain1", false);
                        },
                        arg => true
                        );
                }

                return goBack;
            }
        }
    }
}
