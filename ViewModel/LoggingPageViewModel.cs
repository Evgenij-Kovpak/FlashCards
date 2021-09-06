using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FlashCards.ViewModel
{
    using BaseClasses;
    using FlashCards.Model;
    using FlashCards.DAL.Enities;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    class LoggingPageViewModel : BaseViewModel
    {
        private Model model = null;
        private User _selectedUserFromList;
        private string _name;
        private string _surname;
        private ObservableCollection<User> _listOfUsers = null;

        public LoggingPageViewModel(Model model)
        {
            this.model = model;
            ListOfUsers = this.model.Users;
            this._name = "";
            this._surname = "";
        }
        public LoggingPageViewModel()
        {
            
        }


        public User SelectedUserFromList
        {
            get { return _selectedUserFromList; }
            set { _selectedUserFromList = value; onPropertyChanged(nameof(SelectedUserFromList)); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; onPropertyChanged(nameof(Name)); }
        }

        public string Surname
        {
            get { return _surname; }
            set { _surname = value; onPropertyChanged(nameof(Surname)); }
        }

        public ObservableCollection<User> ListOfUsers
        {
            get { return _listOfUsers; }
            set { _listOfUsers = value; onPropertyChanged(nameof(ListOfUsers)); }
        }

        private void ClearForm()
        {
            Name = "";
            Surname = "";
            SelectedUserFromList = null;
        }

        private void GetUserData(User u)
        {
            Name = u.Name;
            Surname = u.Surname;
        }

        private void ListSelectionChanged(object sender)
        {
            if (SelectedUserFromList != null)
            {
                GetUserData(SelectedUserFromList);
            }
        }

        private ICommand _loadUser = null;

        public ICommand LoadUser
        {
            get
            {
                if (_loadUser == null)
                {
                    _loadUser = new RelayCommand(
                        ListSelectionChanged,
                        arg => true
                        );
                }
                return _loadUser;
            }
        }

        private ICommand _addUser = null;

        public ICommand AddUser
        {
            get
            {
                if (_addUser == null)
                {
                    _addUser = new RelayCommand(
                        arg =>
                        {
                            bool IsNameValid = !(string.IsNullOrEmpty(Name.Trim()));
                            bool IsSurnameValid = !(string.IsNullOrEmpty(Surname.Trim()));

                            if (IsNameValid && IsSurnameValid)
                            {
                                if (model.AddUserToUsers(Name, Surname))
                                {
                                    ClearForm();
                                    System.Windows.MessageBox.Show("User added");
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show("User is exist");
                                }
                            }
                            else
                                System.Windows.MessageBox.Show("Incorrect data");

                        },
                        arg => true
                        );
                }
                return _addUser;
            }
        }


        private ICommand _loginAndAdd = null;

        public ICommand LoginAndAdd
        {
            get
            {
                if (_loginAndAdd == null)
                {
                    _loginAndAdd = new RelayCommand(
                        arg =>
                        {
                            bool IsNameValid = !(string.IsNullOrEmpty(Name.Trim()));
                            bool IsSurenameValid = !(string.IsNullOrEmpty(Surname.Trim()));

                            if(IsSurenameValid && IsNameValid)
                            {
                                if (model.AddUserToUsers(Name, Surname))
                                {

                                    sbyte? userTrial = model.PassUserIdIfExists(Name, Surname);
                                    if (userTrial != null)
                                    {
                                        ClearForm();
                                        Mediator.Notify("GoToTabsPage", userTrial);
                                    }
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show("User is exist");
                                }
                            }
                            else
                                System.Windows.MessageBox.Show("Incorrect data");

                        },
                        arg => true
                        );
                }
                return _loginAndAdd;
            }
        }


        private ICommand _login = null;

        public ICommand LogIn
        {
            get
            {
                if (_login == null)
                {
                    _login = new RelayCommand(
                    arg =>
                        {
                            bool IsNameValid = !(string.IsNullOrEmpty(Name.Trim()));
                            bool IsSurnameValid = !(string.IsNullOrEmpty(Surname.Trim()));

                            if (IsSurnameValid && IsNameValid)
                            {
                                sbyte? userTrial = model.PassUserIdIfExists(Name, Surname);
                                if (userTrial != null)
                                {
                                    ClearForm();
                                    Mediator.Notify("GoToTabsPage", userTrial);
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show("User does not exist");
                                }
                            }
                            else
                                System.Windows.MessageBox.Show("Incorrect data");
                        },
                        arg => true
                    );
                }
                return _login;
            }
        }
    }
}
