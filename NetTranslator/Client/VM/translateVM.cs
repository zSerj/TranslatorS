using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.TranslatorServiceReference;
using log4net;

namespace Client.VM
{
    class translateVM:INotifyPropertyChanged
    {
        static translateVM()
        {
            logger = LogManager.GetLogger(typeof(translateVM));
            log4net.Config.XmlConfigurator.Configure();
        }

        public static readonly ILog logger;

        DispatcherTimer StatusReset = new DispatcherTimer();

        BackgroundWorker bgWorker;

        public translateVM()
        {
            StatusReset.Interval = new TimeSpan(0, 0, 5);
            StatusReset.Tick += StatusReset_Tick;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(ExecuteGetTranslatedWrdFromVocabulary);
            bgWorker.RunWorkerCompleted += GetTranslation_RunWorkerCompleted;

            this.client = new TranslateServiceClient();
            this.TranslationMode = Mode.EngToRus;
            this.Translations = new ObservableCollection<string>();
            this.GetTranslatedWordFromVocabulary = new DelegateCommand(GetTranslatedWrdFromVocabulary, 
                (x)=>this.TranslatedWord!="" );
            this.Exit = new DelegateCommand(closeApp);
            this.MinimizeWindow = new DelegateCommand(minimize);
            this.ShowEditForm = new DelegateCommand(ShowClientViewEdit);
            this.Status = "";
        }

        private void StatusReset_Tick(object sender, EventArgs e)
        {
            this.Status = "";
            this.StatusReset.Stop();
        }

        private void GetTranslation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(this.Translations.Count == 0 )
                this.TranslatedWord = "";
            StatusReset.Start();
        }

        editVM vmEdit;
        TranslateServiceClient client;
        string translatedWord = "";
        string status;
        Mode translationMode;
        ICommand getTranslatedWordFromVocabulary;
        ICommand exit;
        ICommand minimizeWindow;
        ICommand showEditForm;
        ICommand setLanguage;

        #region VM properties

        public string TranslatedWord
        {
            get { return this.translatedWord; }
            set
            {
                this.translatedWord = value;
                OnPropertyChanged("TranslatedWord");
                ((DelegateCommand)this.GetTranslatedWordFromVocabulary).RaiseCanExecuteChanged();
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

        public ICommand SetLanguage
        {
            get { return setLanguage; }
            set { setLanguage = value; }
        }

        public ICommand ShowEditForm
        {
            get { return showEditForm; }
            set { showEditForm = value; }
        }

        public ICommand GetTranslatedWordFromVocabulary
        {
            get { return getTranslatedWordFromVocabulary; }
            set { getTranslatedWordFromVocabulary = value; }
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
        public enum Mode { RusToEng, EngToRus}
        public enum Language { English, Russian }
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
        
        // This function gets translation of the entered word, sending request to wcf servive 
        private void GetTranslatedWrdFromVocabulary(object obj)
        {
            OnStartGetTranslatedWrdFromVocabulary(); // doing operation through background worker
        }

        #region [background worker for GetTranslatedWrdFromVocabulary operation]' items
        private void OnStartGetTranslatedWrdFromVocabulary()
        {
            this.Status = "the operation translation is being executed";    //Initial status
            this.Translations.Clear();
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
        #endregion

        // This function closes application
        public void closeApp(object o)
        {
            Application.Current.Shutdown();
        }

        // This function minimizes application
        public void minimize(object o)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        // show View EditView 
        public void ShowClientViewEdit(object o)
        {
            try
            {
                ClientViewEdit formClientViewEdit = new ClientViewEdit();
                vmEdit = new editVM();
                formClientViewEdit.DataContext = vmEdit;
                formClientViewEdit.ShowDialog();
            }
            catch(Exception exception)
            {
                logger.Error(exception);
            }
        }

        // was intended to use to call service just the window loaded to minimize the delay while first service call
        public void WakeUpService()
        { 
            
        }

        #endregion
    }
}
