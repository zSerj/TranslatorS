using System.Data.Entity;

namespace TranslatorService.DatabaseContext
{
    public class DictionaryContext : DbContext
    {
        public DbSet<EnglishWord> EnglishWords { get; set; }
        public DbSet<RussianWord> RussianWords { get; set; }

        public DictionaryContext()
            : base("name=DictionaryDbConnectionString")
        {
        }
    }
}
