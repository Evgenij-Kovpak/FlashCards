using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace FlashCards.ViewModel
{
    using Model;
    using DAL.Enities;
    class MainViewModel : BaseViewModel
    {
        Model model = new Model();

        private BaseViewModel _actualViewModel = null;
        public BaseViewModel ActualViewModel
        {
            get { return _actualViewModel; }
            set { _actualViewModel = value; onPropertyChanged(nameof(ActualViewModel)); }
        }

        List<BaseViewModel> vms = new List<BaseViewModel>();
        public List<BaseViewModel> Vms
        {
            get
            {
                if (vms == null)
                    vms = new List<BaseViewModel>();
                return vms;
            }
        }

        private LoggingPageViewModel loginPage = null;
        public LoggingPageViewModel LoginPage
        {
            get { return loginPage; }
            set { loginPage = value; onPropertyChanged(nameof(LoginPage)); }
        }

        private TabVM tabPage = null;
        public TabVM TabPage
        {
            get { return tabPage; }
            set { tabPage = value; onPropertyChanged(nameof(TabPage)); }
        }

        private LanguageTrainingVM langTrain = null;
        public LanguageTrainingVM LangTrain
        {
            get { return langTrain; }
            set { langTrain = value; onPropertyChanged(nameof(LangTrain)); }
        }

        private EditFlaszkardViewModel efcardVM = null;
        public EditFlaszkardViewModel EfcardVM
        {
            get { return efcardVM; }
            set { efcardVM = value; onPropertyChanged(nameof(EfcardVM)); }
        }

        private FlipCardTrainingVM flipTrain = null;
        public FlipCardTrainingVM FlipTrain
        {
            get { return flipTrain; }
            set { flipTrain = value; onPropertyChanged(nameof(FlipTrain)); }
        }

        public MainViewModel()
        {
            LoginPage = new LoggingPageViewModel(model);
            TabPage = new TabVM();
            LangTrain = new LanguageTrainingVM();
            EfcardVM = new EditFlaszkardViewModel();
            FlipTrain = new FlipCardTrainingVM();

            Vms.Add(LoginPage);
            Vms.Add(TabPage);
            Vms.Add(LangTrain);
            Vms.Add(EfcardVM);
            Vms.Add(FlipTrain);

            this._actualViewModel = LoginPage; 

            Mediator.Subscribe("GoToTabsPage", GoToTabsScreen);
            Mediator.Subscribe("Logout", BackToLoginPage);
            Mediator.Subscribe("TrainLangs", TrainPredefinedLangs);
            Mediator.Subscribe("BackFromTrain1", GoBackFromTrainLang);
            Mediator.Subscribe("EditFlashCard", GoToEditionScreen);
            Mediator.Subscribe("BackFromEditionFC", GoBackFromEditionScreen);
            Mediator.Subscribe("TrainFC", TrainFlipCards);
            Mediator.Subscribe("BackFromTrainFC", GoBackFromTrainFlipcards);
        }

        public void ChangeViewModel(BaseViewModel viewModel)
        {
            if (!Vms.Contains(viewModel))
                Vms.Add(viewModel);
            ActualViewModel = Vms.FirstOrDefault(vm => vm == viewModel);
        }
        private void GoToEditionScreen(object obj)
        {
            Deck selectedDeck = obj as Deck;
            EfcardVM = new EditFlaszkardViewModel(model, selectedDeck, TabPage.LoggedUser);
            ChangeViewModel(Vms[3]);
        }

        private void GoBackFromEditionScreen(object obj)
        {
            bool selection = (bool)obj;
            TabPage.IsSelectedFlipCardTab = selection;
            ChangeViewModel(Vms[1]);
        }

        private void GoToTabsScreen(object obj)
        {
            TabPage = new TabVM(model, (sbyte?)obj);
            ChangeViewModel(Vms[1]);
        }

        private void BackToLoginPage(object obj)
        {
            LoginPage = new LoggingPageViewModel(model);
            ChangeViewModel(Vms[0]);
        }

        private void TrainPredefinedLangs(object obj)
        {
            List<List<ITrainData>> daneTreningowe = obj as List<List<ITrainData>>;

                LangTrain = new LanguageTrainingVM(
                model,
                daneTreningowe[0].Cast<Word>().ToList(),
                daneTreningowe[1].Cast<Word>().ToList(),
                daneTreningowe[2].Cast<FrontBack>().ToList(),
                TabPage.LoggedUser,
                TabPage.LangTabVM.SelectedLangZ.LangName,
                TabPage.LangTabVM.SelectedLangNa
                );
            ChangeViewModel(Vms[2]);
        }

        private void GoBackFromTrainLang(object obj)
        {
            bool selection = (bool)obj;
            TabPage.IsSelectedFlipCardTab = selection;
            ChangeViewModel(Vms[1]);
        }
        private void TrainFlipCards(object obj)
        {
            List<List<ITrainData>> daneTreningowe = obj as List<List<ITrainData>>;

            FlipTrain = new FlipCardTrainingVM(
                model,
                daneTreningowe[0].Cast<FlipCard>().ToList(),
                daneTreningowe[1].Cast<FlipCardWithKnowledge>().ToList(),
                (sbyte)TabPage.LoggedUser,
                TabPage.FcardTabVM.SelectedDeck
                );
            ChangeViewModel(Vms[4]);
        }

        private void GoBackFromTrainFlipcards(object obj)
        {
            bool selection = (bool)obj;
            TabPage.IsSelectedFlipCardTab = selection;
            ChangeViewModel(Vms[1]);
        }
    }
}
