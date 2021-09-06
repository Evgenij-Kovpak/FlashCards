using FlashCards.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.ViewModel
{
    using FlashCards.ViewModel.BaseClasses;
    using Model;
    using System.Windows.Input;
    using DAL.Enities;

    class FlaszkardsViewModel : BaseViewModel
    {
        private string _deckTitle = "";
        private sbyte? _loggedUser = null;
        private Model model = null;
        private Deck _selectedDeck;
        ObservableCollection<Deck> setOfDecks = new ObservableCollection<Deck>();

        public ObservableCollection<Deck> SetOfDecks
        {
            get
            {
                return setOfDecks;
            }
            set
            {
                setOfDecks = value; onPropertyChanged(nameof(SetOfDecks));
            }
        }

        public string DeckTitle
        {
            get { return _deckTitle; }
            set { _deckTitle = value; onPropertyChanged(nameof(DeckTitle)); }
        }


        public Deck SelectedDeck
        {
            get { return _selectedDeck; }
            set { _selectedDeck = value; onPropertyChanged(nameof(SelectedDeck)); }
        }

        public FlaszkardsViewModel(Model model, sbyte? user)
        {
            this._loggedUser = user;
            this.model = model;
            this.SetOfDecks = this.model.Decks;
        }
        public FlaszkardsViewModel()
        {
            
        }

        private static Random random = new Random();

        public List<FlipCard> Shuffle(List<FlipCard> list)
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

        private void FindMinAndMaxKnowledge(List<FlipCardWithKnowledge> flipCardFullList, out sbyte min, out sbyte max)
        {
            max = 0;
            min = 127;

            foreach (FlipCardWithKnowledge flipCardFull in flipCardFullList)
            {
                if (flipCardFull.Knowledge > max) max = flipCardFull.Knowledge;
                if (flipCardFull.Knowledge < min) min = flipCardFull.Knowledge;
            }
        }

        public List<FlipCardWithKnowledge> createFlipCardsWithKnowledge(List<FlipCard> flipCardList, List<FlipCardKnowledge> flipCardKnowledgeList)
        {
            List<FlipCardWithKnowledge> fullFlipCards = new List<FlipCardWithKnowledge>();

            foreach (FlipCard flipCard in flipCardList)
            {
                sbyte knowledge = 0;
                foreach (FlipCardKnowledge flipCardKnowledge in flipCardKnowledgeList)
                {
                    if (flipCard.Id == flipCardKnowledge.Id_FlipCard)
                        knowledge = flipCardKnowledge.Knowledge;
                }
                fullFlipCards.Add(new FlipCardWithKnowledge(flipCard, knowledge));
            }

            return fullFlipCards;
        }

        public List<FlipCard> CreateQueue(List<FlipCard> flipCardList, List<FlipCardKnowledge> flipCardKnowledgeList, out List<FlipCardWithKnowledge> fcwk)
        {
            List<FlipCardWithKnowledge> fullFlipCards = createFlipCardsWithKnowledge(flipCardList, flipCardKnowledgeList);

            FindMinAndMaxKnowledge(fullFlipCards, out sbyte maxKnowledge, out sbyte minKnowledge);

            fcwk = fullFlipCards;

            sbyte difference = minKnowledge;
            difference -= maxKnowledge;

            sbyte tempDifference = difference;
            sbyte differenceDecreaser = 1;
            while (tempDifference > 5)
            {
                tempDifference /= 2;
                differenceDecreaser += 1;
            }

            List<FlipCard> queue = new List<FlipCard>();

            foreach (FlipCardWithKnowledge flipCardFull in fullFlipCards)
            {
                sbyte ownDifference = difference;
                ownDifference /= differenceDecreaser;
                sbyte repetitions = maxKnowledge;
                repetitions += ownDifference;
                repetitions -= flipCardFull.Knowledge;
                repetitions += 1;

                if (repetitions > 5)
                    repetitions = 5;

                if (repetitions <= 0)
                    repetitions = 1;

                for (int i = 0; i < repetitions; i++)
                {
                    queue.Add(flipCardFull.FlipCard);
                }

            }

            queue = Shuffle(queue);

            foreach (FlipCard item in queue)
            {
                System.Diagnostics.Debug.WriteLine(item);
            }

            return queue;
        }
        private void ClearForm()
        {
            DeckTitle = "";
            SelectedDeck = null;
        }
        private void Edit(object obj)
        {
            if (SelectedDeck != null)
                Mediator.Notify("EditFlashCard", SelectedDeck);
            else
                System.Windows.MessageBox.Show("Choose a deck");
        }
        private void Add(object obj)
        {
            if (!string.IsNullOrEmpty(DeckTitle.Trim()))
            {
                if (model.AddDeckToDecks(DeckTitle))
                {
                    ClearForm();
                }
                else
                    System.Windows.MessageBox.Show("The deck already exists");
            }
            else
                System.Windows.MessageBox.Show("Invalid title");
        }
        private void Delete(object obj)
        {
            if (SelectedDeck != null)
            {
                if (model.DeleteDeck(SelectedDeck))
                {
                    SelectedDeck = null;
                }
            }
            else
                System.Windows.MessageBox.Show("Choose a deck");
        }
        private void Train(object obj)
        {
            if (SelectedDeck != null)
            {
                List<List<ITrainData>> daneTreningowe = new List<List<ITrainData>>();

                List<FlipCard> queue = CreateQueue(
                    model.PassFlipCardCollection((sbyte)SelectedDeck.Id),
                    model.PassUserPerformanceFC(_loggedUser, (sbyte)SelectedDeck.Id),
                    out List<FlipCardWithKnowledge> fcwk
                    );

                if (queue.Any())
                {
                    daneTreningowe.Add(queue.Cast<ITrainData>().ToList());
                    daneTreningowe.Add(fcwk.Cast<ITrainData>().ToList());
                    Mediator.Notify("TrainFC", daneTreningowe);
                }
                else
                    System.Windows.MessageBox.Show("The deck is empty");
            }
            else
                System.Windows.MessageBox.Show("Choose a deck for training");
        }
        private void LogOut(object obj)
        {
            Mediator.Notify("Logout", "");
        }

        private ICommand _change;

        public ICommand Change
        {
            get
            {
                if (_change == null)
                {
                    _change = new RelayCommand(
                       Edit,
                        arg => true
                    );
                }
                return _change;
            }
        }

        private ICommand _adding;

        public ICommand Adding
        {
            get
            {
                if (_adding == null)
                {
                    _adding = new RelayCommand(
                       Add,
                        arg => true
                    );
                }
                return _adding;
            }
        }

        private ICommand _remove;

        public ICommand Remove
        {
            get
            {
                if (_remove == null)
                {
                    _remove = new RelayCommand(
                       Delete,
                        arg => true
                    );
                }
                return _remove;
            }
        }

        private ICommand _trainee;

        public ICommand Trainee
        {
            get
            {
                if (_trainee == null)
                {
                    _trainee = new RelayCommand(
                       Train,
                        arg => true
                    );
                }
                return _trainee;
            }
        }

        private ICommand _logout;

        public ICommand Logout
        {
            get
            {
                if (_logout == null)
                {
                    _logout = new RelayCommand(
                       LogOut,
                        arg => true
                    );
                }
                return _logout;
            }
        }
    }
}
