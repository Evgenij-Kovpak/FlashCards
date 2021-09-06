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
    using DAL.Enities;
    using System.Windows.Input;

    class FlipCardTrainingVM : BaseViewModel
    {
        private Model model = null;
        private FlipCard flipCard;
        private List<FlipCard> flipCards = null;
        private List<FlipCardWithKnowledge> _flipWithKnowledges = null;
        private List<FlipCardKnowledge> performance = null;
        private sbyte? user = null;
        private Deck currentDeck;

        private int iter = 0;

        private bool isUserGuessing = true;
        private bool isUserRating = false;

        public FlipCard FlipCard
        {
            get { return flipCard; }
            set { flipCard = value; onPropertyChanged(nameof(FlipCard)); }
        }

        public List<FlipCard> FlipCards
        {
            get { return flipCards; }
            set { flipCards = value; onPropertyChanged(nameof(FlipCards)); }
        }

        public List<FlipCardKnowledge> Performance
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

        public FlipCardTrainingVM()
        {
            
        }

        public FlipCardTrainingVM(Model model, List<FlipCard> setOfFlipcards, List<FlipCardWithKnowledge> fcwk, sbyte id_user, Deck deck)
        {
            this.model = model;
            FlipCards = setOfFlipcards;
            _flipWithKnowledges = fcwk;

            Performance = new List<FlipCardKnowledge>();
            user = id_user;
            currentDeck = deck;
            Title = deck.DeckName;

            GetNewWord();
        }

        private FlipCardKnowledge SaveProgres(sbyte factor)
        {
            foreach (var fb in _flipWithKnowledges)
            {
                if (fb.FlipCard == FlipCard)
                {
                    factor += fb.Knowledge;
                    if (factor > 127 || factor < 0)
                    {
                        return new FlipCardKnowledge((sbyte)user, (uint)FlipCard.Id, fb.Knowledge);
                    }
                    else
                    {
                        _flipWithKnowledges[_flipWithKnowledges.IndexOf(fb)].Knowledge = factor;
                        return new FlipCardKnowledge((sbyte)user, (uint)FlipCard.Id, factor);
                    }
                }
            }
            return null;
        }

        private static Random random = new Random();
        private List<FlipCard> Shuffle(List<FlipCard> list)
        {
            for (int n = list.Count - 1; n > 1; n--)
            {
                int rng = random.Next(n + 1);
                FlipCard value = list[rng];
                list[rng] = list[n];
                list[n] = value;
            }
            return list;
        }

        private void GetNewWord()
        {
            FlipCard = FlipCards[iter++];


            if (iter > FlipCards.Count - 1)
            {
                iter = 0;
                FlipCards = Shuffle(FlipCards);
            }
        }

        private ICommand showAnswer = null;

        public ICommand ShowAnswer
        {
            get
            {
                if (showAnswer == null)
                {
                    showAnswer = new RelayCommand(
                        arg =>
                        {
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
                            model.UpdateFlipCardKnowledge(SaveProgres(-1));
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
                            model.UpdateFlipCardKnowledge(SaveProgres(1));
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
                            model.UpdateFlipCardKnowledge(SaveProgres(3));
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
                            Mediator.Notify("BackFromTrainFC", true);
                        },
                        arg => true
                        );
                }

                return goBack;
            }
        }
    }
}
