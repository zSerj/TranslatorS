using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using log4net;
using TranslatorService.DatabaseContext;

namespace TranslatorService
{
    public class TranslateService : ITranslateService
    {
        static TranslateService()
        {
            logger = LogManager.GetLogger(typeof(TranslateService));
            log4net.Config.XmlConfigurator.Configure();
        }

        public static readonly ILog logger;  
        
        // function adds word to dictionary
        public string AddWord(Word wrd)
        {
            string result = "";
            using (var context = new DictionaryContext())
                try
                {
                        if (wrd is EnglishWord)
                        {
                            var ExistEnglishWordList = context.EnglishWords.ToList<EnglishWord>();
                            EnglishWord target = ExistEnglishWordList.Where(s => s.Content == wrd.Content).FirstOrDefault<EnglishWord>();
                            if (target == null) // Translated word doesn't exist in voc.
                            {
                                List<RussianWord> AddedTranslations = new List<RussianWord>();
                                foreach (RussianWord AddedTranslation in ((EnglishWord)wrd).Translations)
                                {
                                    var ExistRussianWordList = context.RussianWords.ToList<RussianWord>();
                                    RussianWord CurTranslation = ExistRussianWordList.Where(s => s.Content == AddedTranslation.Content).FirstOrDefault<RussianWord>();
                                    if (CurTranslation == null) // this translation doesn't exist in voc.
                                        AddedTranslations.Add(new RussianWord() { Content = AddedTranslation.Content });
                                    else
                                        AddedTranslations.Add(CurTranslation);
                                }
                                context.EnglishWords.Add(new EnglishWord() { Content = wrd.Content, Translations = AddedTranslations });
                            }
                            else  // translated word exists in voc. 
                            {
                                List<RussianWord> AddedTranslations = new List<RussianWord>();
                                foreach (RussianWord AddedTranslation in ((EnglishWord)wrd).Translations)
                                {
                                    var ExistRussianWordList = context.RussianWords.ToList<RussianWord>();
                                    RussianWord CurTranslation = ExistRussianWordList.Where(s => s.Content == AddedTranslation.Content).FirstOrDefault<RussianWord>();
                                    if (CurTranslation == null) // this translation doesn't exist in voc.
                                        AddedTranslations.Add(new RussianWord() { Content = AddedTranslation.Content });
                                    else
                                        AddedTranslations.Add(CurTranslation);
                                }

                                foreach (RussianWord AddedTr in AddedTranslations)
                                    target.Translations.Add(AddedTr);
                            }
                        }
                        else
                        {
                            var ExistRussianWordList = context.RussianWords.ToList<RussianWord>();
                            RussianWord target = ExistRussianWordList.Where(s => s.Content == wrd.Content).FirstOrDefault<RussianWord>();
                            if (target == null) // the translated word doesn't yet exist in voc.
                            {
                                List<EnglishWord> AddedTranslations = new List<EnglishWord>();
                                foreach (EnglishWord AddedTranslation in ((RussianWord)wrd).Translations)
                                {
                                    var ExistEnglishWordList = context.EnglishWords.ToList<EnglishWord>();
                                    EnglishWord CurTranslation = ExistEnglishWordList.Where(s => s.Content == AddedTranslation.Content).FirstOrDefault<EnglishWord>();
                                    if (CurTranslation == null) // this translation doesn't exist in voc.
                                        AddedTranslations.Add(new EnglishWord() { Content = AddedTranslation.Content });
                                    else
                                        AddedTranslations.Add(CurTranslation);
                                }
                                context.RussianWords.Add(new RussianWord() { Content = wrd.Content, Translations = AddedTranslations });
                            }
                            else  // the translated word exists in voc.
                            {
                                List<EnglishWord> AddedTranslations = new List<EnglishWord>();
                                foreach (EnglishWord AddedTranslation in ((RussianWord)wrd).Translations)
                                {
                                    var ExistEnglishWordList = context.EnglishWords.ToList<EnglishWord>();
                                    EnglishWord CurTranslation = ExistEnglishWordList.Where(s => s.Content == AddedTranslation.Content).FirstOrDefault<EnglishWord>();
                                    if (CurTranslation == null) // this translation doesn't exist in voc.
                                        AddedTranslations.Add(new EnglishWord() { Content = AddedTranslation.Content });
                                    else
                                        AddedTranslations.Add(CurTranslation);
                                }

                                foreach (EnglishWord AddedTr in AddedTranslations)
                                    target.Translations.Add(AddedTr);
                            }
                        }
                        
                            context.SaveChanges();
                      
                    result = string.Format("word {0} successfully added", wrd.Content);
                }
           catch(OutOfMemoryException exception)
           {
               logger.Error(exception);
               result = string.Format("word {0} isn't added.", wrd.Content);
           }
           catch(InvalidOperationException exception)
                {
                    logger.Error(exception);
                    result = string.Format("word {0} isn't added.", wrd.Content);
                }
           catch(InvalidCastException exception)
                {
                    logger.Error(exception);
                    result = string.Format("word {0} isn't added", wrd.Content);
                }
           catch (DbUpdateException exception)
                {
                    logger.Error(exception);
                    result = string.Format("word {0} has already been added", wrd.Content);
                }
            catch (SqlException exception)
            {
                logger.Error(exception);
            }
            return result;
        }

        public string UpdateWord(Word wrd)
        {
            string result = string.Format("The word {0} has successfully updated",wrd.Content);
            try
            {
                RemoveWord(wrd);
                AddWord(wrd);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error(exception);
                result = string.Format( "word {0} isn't updated", wrd.Content);
            }
            catch (InvalidCastException exception)
            {
                logger.Error(exception);
                result = string.Format("word {0} isn't updated", wrd.Content);
            }          
            return result;
        }

        public string RemoveWord(Word wrd)
        {
            using (var context = new DictionaryContext())
                try
                {
                    {
                        Word removedWord;
                        if (wrd is EnglishWord)
                        {
                            removedWord = (Word)context.EnglishWords.Where(wr => wr.Content == wrd.Content).First();
                            context.EnglishWords.Remove((EnglishWord)removedWord);
                        }
                        else
                        {
                            removedWord = (Word)context.RussianWords.Where(wr => wr.Content == wrd.Content).First();
                            context.RussianWords.Remove((RussianWord)removedWord);
                        }
                        context.SaveChanges();
                    }
                }
                catch(InvalidOperationException exception)
                {
                    logger.Error(exception);
                    return string.Format("failed to remove word {0}", wrd.Content);
                }
                catch (InvalidCastException exception)
                {
                    logger.Error(exception);
                    return string.Format("failed to remove word {0}", wrd.Content);
                }
                catch(NullReferenceException exception)
                {
                    logger.Error(exception);
                    return string.Format("failed to remove word {0}", wrd.Content);
                }
            return string.Format("The word {0} has been successfully deleted from vocabulary",wrd.Content);
        }

        public List<Word> GetTranslation(Word wrd)
        {
            using (var context = new DictionaryContext())
                try
                {
                    {
                        List<Word> result = new List<Word>();

                        if (wrd is EnglishWord)
                        {
                            IQueryable<System.Collections.Generic.ICollection<RussianWord>> TranslationsToRussian = from EnglishWords in context.EnglishWords
                                                                                                                    where EnglishWords.Content == wrd.Content
                                                                                                                    select EnglishWords.Translations;
                            foreach (System.Collections.Generic.ICollection<RussianWord> groupOfTranslations in TranslationsToRussian)
                                foreach (RussianWord wr in groupOfTranslations)
                                    result.Add(wr);
                            return result;
                        }
                        else
                        {
                            IQueryable<System.Collections.Generic.ICollection<EnglishWord>> TranslationsToEnglish = from RussianWords in context.RussianWords
                                                                                                                    where RussianWords.Content == wrd.Content
                                                                                                                    select RussianWords.Translations;
                            foreach (System.Collections.Generic.ICollection<EnglishWord> groupOfTranslations in TranslationsToEnglish)
                                foreach (EnglishWord wr in groupOfTranslations)
                                    result.Add(wr);
                            return result;
                        }
                    }
                }
            catch(InvalidOperationException exception)
                {
                    logger.Error(exception);
                    return null;
                }
        }

        public void WakeUp()
        {
        }
    }
}
