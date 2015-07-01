using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.TranslatorServiceReference;
using Client.TypeConverters;
using log4net;

namespace Client.VM
{
    class editVM:INotifyPropertyChanged
    {
        static editVM()
        {
            logger = LogManager.GetLogger(typeof(editVM));
            log4net.Config.XmlConfigurator.Configure();
        }

        public static readonly ILog logger;

        DispatcherTimer StatusReset = new DispatcherTimer();

        BackgroundWorker bgWorker;
        BackgroundWorker bgWorkerForAdd;
        BackgroundWorker bgWorkerForUpdate;
        BackgroundWorker bgWorkerForRemove;

        public editVM()
        {
            StatusReset.Interval = new TimeSpan(0, 0, 5);
            StatusReset.Tick += StatusReset_Tick;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(ExecuteGetTranslatedWrdFromVocabulary);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetTranslatedWrdFromVocabulary_Completed);

            bgWorkerForAdd = new BackgroundWorker();
            bgWorkerForAdd.DoWork += new DoWorkEventHandler(ExecuteAddWordToVocabulary);
            bgWorkerForAdd.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AddWordToVocabulary_Completed);

            bgWorkerForUpdate = new BackgroundWorker();
            bgWorkerForUpdate.DoWork += new DoWorkEventHandler(ExecuteUpdateWrdToVocabulary);
            bgWorkerForUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateWrdToVocabulary_Completed);

            bgWorkerForRemove = new BackgroundWorker();
            bgWorkerForRemove.DoWork += new DoWorkEventHandler(ExecuteRemoveWrdFromVocabulary);
            bgWorkerForRemove.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RemoveWordFromVocabulary_Completed);

            this.client = new TranslateServiceClient();
            this.TranslationMode = Mode.EngToRus;
            this.Translations = new ObservableCollection<string>();
            this.AddToTranslations = new DelegateCommand(AddWordToTbTranslations, (y)=>TranslationCurrent.Length>0);
            this.AddWordToVocabulary = new DelegateCommand(AddWrdToVocabulary, (x)=>TranslatedWord.Length>0 && Translations.Count>0);
            this.RemoveWordFromVocabulary = new DelegateCommand(RemoveWrdFromVocabulary, (x) => TranslatedWord.Length > 0);
            this.UpdateWordToVocabulary = new DelegateCommand(UpdateWrdToVocabulary, (x)=>TranslatedWord.Length>0 && Translations.Count>0);
            this.GetTranslatedWordFromVocabulary = new DelegateCommand(GetTranslatedWrdFromVocabulary, (x) => TranslatedWord != "");
            this.RemoveLastAddedTranslation = new DelegateCommand(RemoveLastAddedItemFromTranslations,
                (x) => this.SelectedTranslation != "" && this.SelectedTranslation!=null);
            this.RemoveAllTranslations = new DelegateCommand(deleteAllTranslations,
                (x) => this.Translations.Count > 0);
            this.Exit = new DelegateCommand(closeApp);
            this.MinimizeWindow = new DelegateCommand(minimize);
        }

        private void StatusReset_Tick(object sender, EventArgs e)
        {
            this.Status = "";
            this.StatusReset.Stop();
        }

        #region VM fields

        TranslateServiceClient client;
        string translatedWord = "";
        string translationCurrent = "";
        string selectedTranslation = "";
        string status;
        Mode translationMode;
        ICommand addToTranslations;
        ICommand addWordToVocabulary;
        ICommand updateWordToVocabulary;
        ICommand removeWordFromVocabulary;
        ICommand removeLastAddedTranslation;
        ICommand getTranslatedWordFromVocabulary;
        ICommand removeAllTranslations;
        ICommand exit;
        ICommand minimizeWindow;

        #endregion

        #region VM properties

        public string SelectedTranslation
        {
            get { return selectedTranslation; }
            set { selectedTranslation = value; OnPropertyChanged("SelectedTranslation");
            ((DelegateCommand)RemoveLastAddedTranslation).RaiseCanExecuteChanged();
            }
        }

        public string TranslatedWord
        {
            get { return translatedWord; }
            set
            {
                translatedWord = value;
                OnPropertyChanged("TranslatedWord");
                ((DelegateCommand)this.AddWordToVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.RemoveWordFromVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.UpdateWordToVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.GetTranslatedWordFromVocabulary).RaiseCanExecuteChanged();
            }
        }

        public string TranslationCurrent
        {
            get { return translationCurrent; }
            set
            {
                translationCurrent = value;
                OnPropertyChanged("TranslationCurrent");
                ((DelegateCommand)this.AddToTranslations).RaiseCanExecuteChanged();
            }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }

        public ObservableCollection<string> Translations { get; set; }

        public Mode TranslationMode
        {
            get { return translationMode; }
            set { translationMode = value; OnPropertyChanged("TranslationMode"); }
        }

        #region Commands

        public ICommand RemoveLastAddedTranslation
        {
            get { return removeLastAddedTranslation; }
            set { removeLastAddedTranslation = value; }
        }


        public ICommand RemoveWordFromVocabulary
        {
            get { return removeWordFromVocabulary; }
            set { removeWordFromVocabulary = value; }
        }

        public ICommand GetTranslatedWordFromVocabulary
        {
            get { return getTranslatedWordFromVocabulary; }
            set { getTranslatedWordFromVocabulary = value; }
        }

        public ICommand AddToTranslations { get { return addToTranslations; } set { addToTranslations = value; } }
        public ICommand AddWordToVocabulary { get { return addWordToVocabulary; } set { addWordToVocabulary = value; } }
        public ICommand UpdateWordToVocabulary { get { return updateWordToVocabulary; } set { updateWordToVocabulary = value; } }

        public ICommand RemoveAllTranslations
        {
            get { return removeAllTranslations; }
            set { removeAllTranslations = value; }
        }

        public ICommand Exit
        {
            get { return exit; }
            set { exit = value; }
        }


        public ICommand MinimizeWindow
        {
            get { return minimizeWindow; }
            set { minimizeWindow = value; }
        }

        #endregion
        #endregion

        #region types
        public enum Mode { RusToEng, EngToRus }
        #endregion

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion

        #region VM Functions


        // This function adds the word to Translations list
        private void AddWordToTbTranslations(object item)
        {
            try
            {
                this.Translations.Add((string)item);
                ((DelegateCommand)this.RemoveLastAddedTranslation).RaiseCanExecuteChanged();
                ((DelegateCommand)this.RemoveAllTranslations).RaiseCanExecuteChanged();
                ((DelegateCommand)this.AddWordToVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.UpdateWordToVocabulary).RaiseCanExecuteChanged();
                this.TranslationCurrent = "";
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
        }

        // This function clear last added to Translations list item
        private void RemoveLastAddedItemFromTranslations(object parameter)
        {
            try
            {
                this.Translations.RemoveAt(this.Translations.IndexOf(SelectedTranslation));
                ((DelegateCommand)this.RemoveLastAddedTranslation).RaiseCanExecuteChanged();
                ((DelegateCommand)this.RemoveAllTranslations).RaiseCanExecuteChanged();
                ((DelegateCommand)this.AddWordToVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.UpdateWordToVocabulary).RaiseCanExecuteChanged();
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
        }

        // This function adds the word to vocabulary, sending request to wcf servive
        private void AddWrdToVocabulary(object translatedItem)
        {
            OnStartAddWordToVocabulary();
        }

        #region [background worker for AddWordToVocabulary operation]' items
        private void OnStartAddWordToVocabulary()
        {
            this.Status = "the operation add is being executed";    //Initial status
            bgWorkerForAdd.RunWorkerAsync();
        }

        private void ExecuteAddWordToVocabulary(object sender, DoWorkEventArgs e)
        {
            try
            {
                    Word[] translations;
                    if (TranslationMode == Mode.EngToRus) // chosen english to russian mode
                    {
                        translations = Converters.ConvertToRus(this.Translations);
                        // convert to service recognizable format

                        Status = client.AddWord(new TranslatorServiceReference.EnglishWord()
                        {
                            Content = (string)TranslatedWord,
                            Translations = (RussianWord[])translations
                        }); // request to a service AddWord method
                    }
                    else if (TranslationMode == Mode.RusToEng) // chosen russian to english mode
                    {
                        translations = Converters.ConvertToEng(this.Translations);
                        // convert to service recognizable format

                        Status = client.AddWord(new TranslatorServiceReference.RussianWord() { Content = (string)TranslatedWord, Translations = (EnglishWord[])translations });
                        // request to a service AddWord method
                    }
            }
            catch (ThreadStateException exception)
            {
                logger.Error(exception);
            }
            catch (NullReferenceException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error(exception);
            }
        }

        void AddWordToVocabulary_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Translations.Clear();
                });
                TranslatedWord = "";
                StatusReset.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion


        // This function update the word translations, sending request to wcf servive
        private void UpdateWrdToVocabulary(object obj)
        {
            OnStartUpdateWrdToVocabulary();
        }

        #region [background worker for UpdateWrdToVocabulary operation]' items
        private void OnStartUpdateWrdToVocabulary()
        {
            this.Status = "the operation update is being executed";    //Initial status
            bgWorkerForUpdate.RunWorkerAsync();
        }
        private void ExecuteUpdateWrdToVocabulary(object sender, DoWorkEventArgs e)
            {
                try
                {
                    Status = "Operation 'update' is being executed";

                        if (TranslationMode == Mode.EngToRus) // chosen english to russian mode
                        {
                            RussianWord[] translations = Converters.ConvertToRus(Translations);
                            Status = client.UpdateWord(new EnglishWord() { Content = (string)TranslatedWord, Translations = translations });
                        }
                        else // chosen russian to english mode
                        {
                            EnglishWord[] translations = Converters.ConvertToEng(Translations);
                            Status = client.UpdateWord(new RussianWord() { Content = (string)TranslatedWord, Translations = translations });
                        }
                }
                catch (ThreadStateException exception)
                {
                    logger.Error(exception);
                }
                catch (NullReferenceException exception)
                {
                    logger.Error(exception);
                }
                catch (InvalidCastException exception)
                {
                    logger.Error(exception);
                }
                catch (InvalidOperationException exception)
                {
                    logger.Error(exception);
                }
            }


        void UpdateWrdToVocabulary_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Translations.Clear();
                });
                TranslatedWord = "";
                StatusReset.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        // This function removes word from vocabulary, sending request to wcf servive
        private void RemoveWrdFromVocabulary(object sender)
        {
            OnStartRemoveWrdFromVocabulary();
        }

        #region [background worker for RemoveWrdFromVocabulary operation]' items
        private void OnStartRemoveWrdFromVocabulary()
        {
            this.Status = "the operation remove is being executed";    //Initial status
            bgWorkerForRemove.RunWorkerAsync();
        }     
        private void ExecuteRemoveWrdFromVocabulary (object obj, DoWorkEventArgs e)
        {
            try
            {
                    if (TranslationMode == Mode.EngToRus)
                        Status = client.RemoveWord(new EnglishWord() { Content = TranslatedWord });
                    else
                        Status = client.RemoveWord(new RussianWord() { Content = TranslatedWord });
            }
            catch (ThreadStateException exception)
            {
                logger.Error(exception);
            }
            catch (NullReferenceException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error(exception);
            }
        }
        void RemoveWordFromVocabulary_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Translations.Clear();
                });
                TranslatedWord = "";
                StatusReset.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion

        // This function gets translation of the entered word, sending request to wcf servive 
        private void GetTranslatedWrdFromVocabulary(object obj)
        {
            OnStartGetTranslatedWrdFromVocabulary(); // doing operation through background worker
        }

        #region [background worker for GetTranslatedWrdFromVocabulary operation]' items
        private void OnStartGetTranslatedWrdFromVocabulary()
        {
            this.Status = "the operation translation is being executed";    //Initial status
            bgWorker.RunWorkerAsync();
        }

        private void ExecuteGetTranslatedWrdFromVocabulary(object sender, DoWorkEventArgs e)
        {
            try
            {
                Word[] words;
                if (TranslationMode == Mode.EngToRus) // chosen english to russian mode
                {
                    words = client.GetTranslation(new TranslatorServiceReference.EnglishWord() { Content = TranslatedWord });
                    if (words.Length == 0)
                        Status = "This word doesn't exist in dictionary";
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (Word w in words)
                            Translations.Add(w.Content);
                    });
                }
                else if (TranslationMode == Mode.RusToEng) // chosen russian to english mode
                {
                    words = client.GetTranslation(new TranslatorServiceReference.RussianWord() { Content = TranslatedWord });
                    if (words.Length == 0)
                        Status = "This word doesn't exist in dictionary";
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (Word w in words)
                            Translations.Add(w.Content);
                    });

                }
                if (Status != "This word doesn't exist in dictionary")
                    Status = "Operation has successfully done!";
            }
            catch (ThreadStateException exception)
            {
                logger.Error(exception);
            }
            catch (NullReferenceException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error(exception);
            }
        }

        void GetTranslatedWrdFromVocabulary_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ((DelegateCommand)this.RemoveAllTranslations).RaiseCanExecuteChanged();
                ((DelegateCommand)this.RemoveLastAddedTranslation).RaiseCanExecuteChanged();
                StatusReset.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        // This function clear current translations list
        private void deleteAllTranslations(object o)
        {
            try
            {
                this.Translations.Clear();
                ((DelegateCommand)this.RemoveAllTranslations).RaiseCanExecuteChanged();
                ((DelegateCommand)this.RemoveLastAddedTranslation).RaiseCanExecuteChanged();
                ((DelegateCommand)this.AddWordToVocabulary).RaiseCanExecuteChanged();
                ((DelegateCommand)this.UpdateWordToVocabulary).RaiseCanExecuteChanged();
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
            }
        }

        // This function closes application
        public void closeApp(object o)
        {
            Application.Current.Windows[1].Close();
        }

        // This function minimizes application
        public void minimize(object o)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
