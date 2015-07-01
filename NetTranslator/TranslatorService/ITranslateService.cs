using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace TranslatorService
{
    
    [ServiceContract]
    public interface ITranslateService
    {
        [OperationContract]
        string AddWord(Word wrd);

        [OperationContract]
        string UpdateWord(Word wrd);

        [OperationContract]
        string RemoveWord(Word wrd);

        [OperationContract]
        List<Word> GetTranslation(Word wrd);

        [OperationContract]
        void WakeUp();

        // TODO: Add service operations here
    }

    
    [DataContract]
    [KnownType(typeof(EnglishWord))]
    [KnownType(typeof(RussianWord))]
    public class Word
    {
        int id;
        string content;

        [DataMember]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    [DataContract]
    public partial class EnglishWord:Word
    {
        public EnglishWord()
        {
            this.Translations = new List<RussianWord>();
        }
        [DataMember]
        public ICollection<RussianWord> Translations { get; set; }

    }

    [DataContract]
    public partial class RussianWord : Word
    {
        public RussianWord()
        {
            this.Translations = new List<EnglishWord>();
        }
        [DataMember]
        public ICollection<EnglishWord> Translations { get; set; }

    }
}
