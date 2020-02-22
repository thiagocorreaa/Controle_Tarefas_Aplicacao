using Infrastructure.Mapping.Controle_Tarefas;
using System.Data.Entity;

namespace Infrastructure
{
    public partial class TarefasContext : DbContext
    {
        static TarefasContext()
        {
            Database.SetInitializer<TarefasContext>(null);
        }

        public TarefasContext() : this("Name=TarefasContext") { }

        protected TarefasContext(string connectionStringName)
            : base(connectionStringName)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {         
            modelBuilder.Configurations.Add(new TarefasMap());
        }
    }
}