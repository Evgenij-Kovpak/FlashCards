using FlashCards.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.ViewModel
{
    using FlashCards.ViewModel.BaseClasses;
    using Model;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using DAL.Enities;

    class EditFlaszkardViewModel : BaseViewModel
    {
        private Model model = null;
        private Deck editedDeck = null;
        private string _deckTitle = "";
        private ObservableCollection<FlipCard> setOfFlipCards = new ObservableCollection<FlipCard>();
        private FlipCard _selectedFlipCard = null;
        private string _front = "";
        private sbyte? _loggedUser = null;
        private string _back = "";

        public string Back
        {
            get { return _back; }
            set { _back = value; onPropertyChanged(nameof(Back)); }
        }
        public string Front
        {
            get { return _front; }
            set { _front = value; onPropertyChanged(nameof(Front)); }
        }
        public FlipCard SelectedFlipCard
        {
            get { return _selectedFlipCard; }
            set { _selectedFlipCard = value; onPropertyChanged(nameof(SelectedFlipCard)); }
        }
        public string DeckTitle
        {
            get { return _deckTitle; }
            set { _deckTitle = value; onPropertyChanged(nameof(SelectedFlipCard)); }
        }
        public ObservableCollection<FlipCard> SetOfFlipCards
        {
            get
            {
                return setOfFlipCards;
            }
            set
            {
                setOfFlipCards = value; onPropertyChanged(nameof(SetOfFlipCards));
            }
        }

        public EditFlaszkardViewModel(Model model, Deck deck, sbyte? user)
        {
            this.model = model;
            this._loggedUser = user;
            this.editedDeck = deck;
            DeckTitle = editedDeck.DeckName;
            SetOfFlipCards = model.PassDeckContent(editedDeck);
        }
        public EditFlaszkardViewModel()
        {
            
        }

        private void RefreshFlipCards() => SetOfFlipCards = model.PassDeckContent(editedDeck);
        private void LoadFC(FlipCard fc)
        {
            Front = fc.FrontContent;
            Back = fc.BackContent;
        }
        private void list_SelectionChanged(object sender)
        {
            if (SelectedFlipCard != null)
            {
                LoadFC(SelectedFlipCard);
            }
        }
        private void ClearForm()
        {
            Front = "";
            Back = "";
            SelectedFlipCard = null;
        }
        private void Edit(object obj)
        {
            if (SelectedFlipCard != null)
            {
                if (!string.IsNullOrEmpty(Front.Trim()) && !string.IsNullOrEmpty(Back.Trim()))
                {
                    if (model.EditFlipCardContent(SelectedFlipCard, new FlipCard(Front, Back, (sbyte)editedDeck.Id)))
                    {
                        RefreshFlipCards();
                        ClearForm();
                    }
                    else
                        System.Windows.MessageBox.Show("This card is exist");
                }
                else
                    System.Windows.MessageBox.Show("Add fields");

            }
            else
                System.Windows.MessageBox.Show("Select a card to edit");
        }
        private void Add(object obj)
        {
            if (!string.IsNullOrEmpty(Front.Trim()) && !string.IsNullOrEmpty(Back.Trim()))
            {
                if (model.AddFlipCardToFCs(Front, Back, (sbyte)editedDeck.Id))
                {
                    RefreshFlipCards();
                    ClearForm();
                }
                else
                    System.Windows.MessageBox.Show("The card already exists in this deck");
            }
            else
                System.Windows.MessageBox.Show("Fill in the fields");
        }
        private void Delete(object obj)
        {
            if (SelectedFlipCard != null)
            {
                if (model.DeleteFlipcard(SelectedFlipCard))
                {
                    SelectedFlipCard = null;
                    RefreshFlipCards();
                }
            }
            else
                System.Windows.MessageBox.Show("Check the box to remove");
        }

        private void LeaveEdition(object obj)
        {
            Deck test = new Deck(DeckTitle);
            bool canChangeTitle = true;

            foreach (var deck in model.Decks)
            {
                if ((deck.Equals(test) && deck.Id != editedDeck.Id) || string.IsNullOrEmpty(DeckTitle.Trim()))
                {
                    canChangeTitle = false;
                    break;
                }
            }

            if (canChangeTitle)
            {
                model.EditDeckTitle(editedDeck, test);
            }
            else
                System.Windows.MessageBox.Show("Deck rename failed");

            Mediator.Notify("BackFromEditionFC", true);
        }

        private ICommand _loadFC = null;

        public ICommand LoadFlipCard
        {
            get
            {
                if (_loadFC == null)
                {
                    _loadFC = new RelayCommand(
                        list_SelectionChanged,
                        arg => true
                        );
                }

                return _loadFC;
            }
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

        private ICommand _exit;

        public ICommand Exit
        {
            get
            {
                if (_exit == null)
                {
                    _exit = new RelayCommand(
                       LeaveEdition,
                        arg => true
                    );
                }
                return _exit;
            }
        }
    }
}
