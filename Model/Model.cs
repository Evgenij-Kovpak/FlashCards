using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.Model
{
    using DAL.Enities;
    using DAL.Commands;
    using System.Collections.ObjectModel;

    class Model
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Language> Langs { get; set; } = new ObservableCollection<Language>();
        public ObservableCollection<Word> Words { get; set; } = new ObservableCollection<Word>();
        public ObservableCollection<WordKnowledge> WordKnowledges { get; set; } = new ObservableCollection<WordKnowledge>();

        public ObservableCollection<FlipCard> FlipCards { get; set; } = new ObservableCollection<FlipCard>();
        public ObservableCollection<FlipCardKnowledge> FlipCardKnowledges { get; set; } = new ObservableCollection<FlipCardKnowledge>();
        public ObservableCollection<Deck> Decks { get; set; } = new ObservableCollection<Deck>();

        public Model()
        {
            var users = SetOfUsers.GetAllUsers();
            foreach (var user in users) Users.Add(user);

            var langs = SetOfLanguages.GetAllLanguages();
            foreach (var lang in langs) Langs.Add(lang);

            var words = SetOfWords.GetAllWords();
            foreach (var word in words) Words.Add(word);

            var wordKnowledges = SetOfWordKnwoledges.GetAllWordKnowledges();
            foreach (var wks in wordKnowledges) WordKnowledges.Add(wks);

            var flipcards = SetOfFlipCards.GetAllFlipCards();
            foreach (var flipcard in flipcards) FlipCards.Add(flipcard);

            var flipCardKnowledges = SetOfFlipCardKnowledges.GetAllFlipCardKnowledges();
            foreach (var flipCardKwl in flipCardKnowledges) FlipCardKnowledges.Add(flipCardKwl);

            var decks = SetOfDecks.GetAllDecks();
            foreach (var deck in decks) Decks.Add(deck);
        }

        public ObservableCollection<FlipCard> PassDeckContent(Deck deck)
        {
            ObservableCollection<FlipCard> tmp = new ObservableCollection<FlipCard>();
            foreach (var fcard in FlipCards)
            {
                if (fcard.Id_Deck == deck.Id)
                {
                    tmp.Add(fcard);
                }
            }

            return tmp;
        }

        public bool FlipCardExist(FlipCard fc) => FlipCards.Contains(fc);
        public bool AddFlipCardToFCs(string front, string back, sbyte id_deck)
        {
            FlipCard fc = new FlipCard(front, back, id_deck);

            if (!FlipCardExist(fc))
            {
                if (SetOfFlipCards.AddNewFlipCard(fc))
                {
                    FlipCards.Add(fc);
                    return true;
                }
            }
            return false;
        }

        public bool DeckExist(Deck d) => Decks.Contains(d);
        public bool AddDeckToDecks(string deckName)
        {
            Deck d = new Deck(deckName);

            if (!DeckExist(d))
            {
                if (SetOfDecks.AddNewDeck(d))
                {
                    Decks.Add(d);
                    return true;
                }
            }
            return false;
        }
        public bool DeleteFlipcard(FlipCard flipCard)
        {
            foreach (var knowledge in FlipCardKnowledges.ToList())
            {
                if (flipCard.Id == knowledge.Id_FlipCard)
                {
                    if (SetOfFlipCardKnowledges.DeleteFlipCardKnowledge(knowledge))
                    {
                        FlipCardKnowledges.Remove(knowledge);
                    }
                }
            }

            if (SetOfFlipCards.DeleteFlipCard(flipCard))
            {
                FlipCards.Remove(flipCard);
                return true;
            }

            return false;
        }

        public bool DeleteDeck(Deck deck)
        {
            if (DeckExist(deck))
            {
                foreach (var flipcard in FlipCards.ToList())
                {
                    if (flipcard.Id_Deck == deck.Id)
                    {
                        foreach (var flipcardKnowledge in FlipCardKnowledges.ToList())
                        {
                            if (flipcardKnowledge.Id_FlipCard == flipcard.Id)
                            {
                                if (SetOfFlipCardKnowledges.DeleteFlipCardKnowledge(flipcardKnowledge))
                                {
                                    FlipCardKnowledges.Remove(flipcardKnowledge);
                                }
                            }
                        }

                        if (SetOfFlipCards.DeleteFlipCard(flipcard))
                        {
                            FlipCards.Remove(flipcard);
                        }
                    }
                }

                if (SetOfDecks.DeleteDeck(deck))
                {
                    Decks.Remove(deck);
                    return true;
                }              
            }
            return false;
        }

        public bool EditFlipCardContent(FlipCard oldFlipCard, FlipCard newFlipCard)
        {
            if (!FlipCardExist(newFlipCard))
            {
                if (SetOfFlipCards.EditFlipCard(newFlipCard, (uint)oldFlipCard.Id, newFlipCard.Id_Deck))
                {
                    newFlipCard.Id = oldFlipCard.Id;
                    FlipCards[FlipCards.IndexOf(oldFlipCard)] = newFlipCard;
                    return true;
                }
            }
            return false;
        }

        public void EditDeckTitle(Deck oldDeck, Deck newDeck)
        {
            if(SetOfDecks.EditDeck(newDeck, (sbyte)oldDeck.Id))
            {
                newDeck.Id = oldDeck.Id;
                Decks[Decks.IndexOf(oldDeck)] = newDeck;
            }
        }

        public bool FlipCardKnowledgeExist(FlipCardKnowledge f) => FlipCardKnowledges.Contains(f);
        public void UpdateFlipCardKnowledge(FlipCardKnowledge flipKnowledge)
        {
            if (FlipCardKnowledgeExist(flipKnowledge))
            {
                var oldLevel = FlipCardKnowledges[FlipCardKnowledges.IndexOf(flipKnowledge)];

                if (oldLevel.Knowledge != flipKnowledge.Knowledge)
                {
                    if (SetOfFlipCardKnowledges.EditFlipCardKnowledge(flipKnowledge, oldLevel.Id))
                    {
                        flipKnowledge.Id = oldLevel.Id;
                        FlipCardKnowledges[FlipCardKnowledges.IndexOf(oldLevel)] = flipKnowledge;
                    }

                }
            }
            else 
            {
                if (SetOfFlipCardKnowledges.AddNewFlipCardKnowledge(flipKnowledge))
                {
                    FlipCardKnowledges.Add(flipKnowledge);
                }
            }
        }

        public List<FlipCard> PassFlipCardCollection(sbyte deck_id)
        {
            List<FlipCard> flipCards = new List<FlipCard>();
            foreach (var fc in FlipCards)
            {
                if (fc.Id_Deck == deck_id)
                    flipCards.Add(fc);
            }
            return flipCards;
        }

        private FlipCard FindFlipCardById(uint id)
        {
            foreach (var f in FlipCards)
            {
                if (f.Id == id)
                    return f;
            }
            return null;
        }

        public List<FlipCardKnowledge> PassUserPerformanceFC(sbyte? user_id, sbyte deck)
        {
            List<FlipCardKnowledge> currentUserPerformance = new List<FlipCardKnowledge>();

            foreach (var fcKnowledge in FlipCardKnowledges)
            {
                if (fcKnowledge.Id_User == user_id && deck == FindFlipCardById(fcKnowledge.Id_FlipCard).Id_Deck)
                {
                    currentUserPerformance.Add(fcKnowledge);
                }
            }
            return currentUserPerformance;
        }

        public List<string> PassDifficulties()
        {
            HashSet<string> distinctDiffs = new HashSet<string>();

            foreach (var word in Words)
            {
                distinctDiffs.Add(word.Difficulty);
            }

            return distinctDiffs.ToList<string>();
        }

        public bool PersonExists(User user) => Users.Contains(user);

        public sbyte? PassUserIdIfExists(string name, string surname)
        {
            User u = new User(name, surname);
            if (PersonExists(u))
            {
                return Users[Users.IndexOf(u)].Id;
            }
            return null;
        }

        public bool AddUserToUsers(string name, string surname)
        {
            User u = new User(name, surname);

            if (!PersonExists(u))
            {
                if (SetOfUsers.AddNewUser(u))
                {
                    Users.Add(u);
                    return true;
                }
            }
            return false;
        }
        
        public List<Word> PassOtherTranslations(Word origin, Language translation)
        {
            List<Word> others = new List<Word>();
            foreach (var w in Words)
            {
                if (w.Id_lang == origin.Id_lang && w.WordName.ToLower() == origin.WordName.ToLower() && w.GUID != origin.GUID)
                {
                    foreach (var t in Words)
                    {
                        if( w.GUID == t.GUID && t.Id_lang == translation.Id)
                        {
                            others.Add(t);
                        }
                    }
                }
            }
            return others;
        }

        public List<Word> PassWordCollection(sbyte word_id, sbyte translation_id, string difficulty)
        {
            List<Word> randomWords = new List<Word>();
            foreach (var word in Words)
            {
                if ((word.Id_lang == word_id || word.Id_lang == translation_id) && word.Difficulty.Equals(difficulty))
                    randomWords.Add(word);
            }
            return randomWords;
        }

        private Word FindWordById(uint id)
        {
            foreach (var w in Words)
            {
                if (w.Id == id)
                    return w;
            }
            return null;
        }

        public List<WordKnowledge> PassUserPerformance(sbyte? user_id, sbyte langA, sbyte langB)
        {
            List<WordKnowledge> currentUserPerformance = new List<WordKnowledge>();

            sbyte minLang = Math.Min(langA, langB);
            sbyte maxLang = Math.Max(langA, langB);

            foreach (var wk in WordKnowledges)
            {
                if (wk.Id_user == user_id && minLang == FindWordById(wk.Id_word_front).Id_lang && maxLang == FindWordById(wk.Id_word_back).Id_lang)
                {
                    currentUserPerformance.Add(wk);
                }
            }

            return currentUserPerformance;
        }

        public bool WordKnowledgeExists(WordKnowledge wk) => WordKnowledges.Contains(wk); 

        public void UpdateWordKnowledge(WordKnowledge knowledge)
        {
            if (WordKnowledgeExists(knowledge))
            {
                var oldLevel = WordKnowledges[WordKnowledges.IndexOf(knowledge)];

                if (oldLevel.Knowledge != knowledge.Knowledge)
                {
                    if (SetOfWordKnwoledges.EditWordKnowledge(knowledge, oldLevel.Id))
                    {
                        knowledge.Id = oldLevel.Id;
                        WordKnowledges[WordKnowledges.IndexOf(oldLevel)] = knowledge;
                    }
                }
            }
            else 
            {
                if (SetOfWordKnwoledges.AddWordKnowledge(knowledge))
                {
                    WordKnowledges.Add(knowledge);
                }
            }
        }
    }
}
