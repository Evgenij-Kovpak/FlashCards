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

    class LanguagesTabViewModel : BaseViewModel
    {
        private Model model = null;
        private sbyte? loggedUser = null;
        private ObservableCollection<Language> langCollection = new ObservableCollection<Language>();
        private List<string> difficulties = new List<string>();

        private Language selectedLangZ = null;
        private Language selectedLangNa = null;
        private string selectedDifficulty = null;

        public string SelectedDifficulty
        {
            get { return selectedDifficulty; }
            set { selectedDifficulty = value; onPropertyChanged(nameof(SelectedDifficulty)); }
        }

        public Language SelectedLangNa
        {
            get { return selectedLangNa; }
            set { selectedLangNa = value; onPropertyChanged(nameof(SelectedLangNa)); }
        }

        public Language SelectedLangZ
        {
            get { return selectedLangZ; }
            set { selectedLangZ = value; onPropertyChanged(nameof(SelectedLangZ)); }
        }

        public ObservableCollection<Language> LangCollection
        {
            get { return langCollection; }
            set { langCollection = value; onPropertyChanged(nameof(LangCollection)); }
        }

        public List<string> Difficulties
        {
            get { return difficulties; }
            set { difficulties = value; onPropertyChanged(nameof(Difficulties)); }
        }

        public sbyte? LoggedUser
        {
            get { return loggedUser; }
            set { loggedUser = value; onPropertyChanged(nameof(LoggedUser)); }
        }

        public LanguagesTabViewModel(Model model, sbyte? user)
        {
            LoggedUser = user;
            this.model = model;
            Difficulties = this.model.PassDifficulties();
            LangCollection = this.model.Langs;
        }

        public LanguagesTabViewModel()
        {
            
        }

        private bool Match(Word wordA, Word wordB, WordKnowledge wordKnowledge)
        {
            if (wordA.GUID == wordB.GUID && wordKnowledge.Id_word_back == wordB.Id && wordKnowledge.Id_word_front == wordA.Id) 
                return true;
            return false;
        }
        public List<FrontBack> CreateFrontBack(List<Word> langA, List<Word> langB, List<WordKnowledge> wordKnowledges)
        {
            List<FrontBack> frontBackList = new List<FrontBack>();

            foreach (Word wordA in langA)
            {
                foreach (Word wordB in langB)
                {
                    if (wordA.GUID == wordB.GUID)
                    {
                        sbyte knowledge = 0;
                        foreach (WordKnowledge wordKnowledge in wordKnowledges)
                        {
                            if (Match(wordA, wordB, wordKnowledge)) knowledge = wordKnowledge.Knowledge;
                        }
                        frontBackList.Add(new FrontBack(wordA, wordB, knowledge));
                        break;
                    }
                }
            }

            return frontBackList;
        }

        private void FindMinAndMaxKnowledge(List<FrontBack> frontBackList, out sbyte min, out sbyte max)
        {
            max = 0;
            min = 127;

            foreach (FrontBack frontBack in frontBackList)
            {
                if (frontBack.Knowledge > max) max = frontBack.Knowledge;
                if (frontBack.Knowledge < min) min = frontBack.Knowledge;
            }
        }

        private static Random random = new Random();

        public List<Word> Shuffle(List<Word> list)
        {
            for (int n = list.Count-1; n > 1; n--)
            {
                int rng = random.Next(n + 1);
                Word value = list[rng];
                list[rng] = list[n];
                list[n] = value;
            }

            return list;
        }

        public void SplitWords(List<Word> allWords, sbyte idFront, sbyte idBack, out List<Word> langA, out List<Word> langB)
        {
            langA = new List<Word>();
            langB = new List<Word>();

            foreach(Word word in allWords)
            {
                if (word.Id_lang == idFront)
                    langA.Add(word);
                else if (word.Id_lang == idBack)
                    langB.Add(word);
                else
                    Debug.WriteLine("Data passed to 'CreateQueue' method appeared to be incorrect");
            }
        }

        public List<Word> CreateQueue(
            List<Word> allWords, List<WordKnowledge> wordKnowledges, sbyte idFront, sbyte idBack,
            out List<FrontBack> fBL, out List<Word> translations
            )
        {
            sbyte origin = idFront;
            sbyte translation = idBack;

            idFront = Math.Min(origin, translation);
            idBack = Math.Max(origin, translation);

            SplitWords(allWords, idFront, idBack, out List<Word> langA, out List<Word> langB);
            if (origin < translation)
                translations = langB;
            else
                translations = langA;

            List<FrontBack> frontBackList = CreateFrontBack(langA, langB, wordKnowledges);
            fBL = frontBackList;

            FindMinAndMaxKnowledge(frontBackList, out sbyte maxKnowledge, out sbyte minKnowledge);

            sbyte difference = minKnowledge;
            difference -= maxKnowledge;

            sbyte tempDifference = difference;
            sbyte differenceDecreaser = 1;
            while (tempDifference > 5)
            {
                tempDifference /= 2;
                differenceDecreaser += 1;
            }

            List<Word> queue = new List<Word>();

            foreach (FrontBack frontBack in frontBackList)
            {
                sbyte ownDifference = difference;
                ownDifference /= differenceDecreaser;
                sbyte repetitions = maxKnowledge;
                repetitions += ownDifference;
                repetitions -= frontBack.Knowledge;
                repetitions += 1;

                if (repetitions > 5)
                    repetitions = 5;

                if (repetitions <= 0)
                    repetitions = 1;

                for (int i = 0; i < repetitions; i++)
                {
                    if (origin < translation)
                        queue.Add(frontBack.Front);
                    else
                        queue.Add(frontBack.Back);

                }

            }

            queue = Shuffle(queue);

            return queue;
        }
       
        private ICommand train = null;

        public ICommand Train
        {
            get
            {
                if (train == null)
                {
                    train = new RelayCommand(
                        arg =>
                        {
                            if (SelectedLangZ != null && SelectedLangNa != null && SelectedDifficulty != null)
                            {
                                if (SelectedLangZ.Id != SelectedLangNa.Id)  
                                {
                                    List<List<ITrainData>> daneTreningowe = new List<List<ITrainData>>();

                                    List<Word> questions = CreateQueue(
                                            model.PassWordCollection(SelectedLangZ.Id, SelectedLangNa.Id, SelectedDifficulty),
                                            model.PassUserPerformance(LoggedUser, SelectedLangZ.Id, SelectedLangNa.Id),
                                            SelectedLangZ.Id,
                                            SelectedLangNa.Id,
                                            out List<FrontBack> fBL,
                                            out List<Word> translations
                                            );

                                    daneTreningowe.Add(questions.Cast<ITrainData>().ToList());
                                    daneTreningowe.Add(translations.Cast<ITrainData>().ToList());
                                    daneTreningowe.Add(fBL.Cast<ITrainData>().ToList());

                                    Mediator.Notify("TrainLangs", daneTreningowe);
                                }
                                else
                                    System.Windows.MessageBox.Show("Choose different languages");
                            }
                            else
                                System.Windows.MessageBox.Show("Select each of the options");
                        },
                        arg => true
                        );
                }

                return train;
            }
        }

        private ICommand logout = null;

        public ICommand Logout
        {
            get
            {
                if (logout == null)
                {
                    logout = new RelayCommand(
                        arg =>
                        {
                            Mediator.Notify("Logout", "");
                        },
                        arg => true
                        );
                }

                return logout;
            }
        }
    }
}
