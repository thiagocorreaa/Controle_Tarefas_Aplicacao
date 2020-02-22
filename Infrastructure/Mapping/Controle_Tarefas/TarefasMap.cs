using System.Data.Entity.ModelConfiguration;
using Models.Controle_Tarefas;

namespace Infrastructure.Mapping.Controle_Tarefas
{
    public class TarefasMap : EntityTypeConfiguration<Tarefas>
    {
        public TarefasMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id_Tarefa });

            ToTable("Tarefas");

            this.Property(t => t.Id_Tarefa).HasColumnName("Id_Tarefa");
            this.Property(t => t.Titulo).HasColumnName("Titulo");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Descricao).HasColumnName("Descricao");
            this.Property(t => t.Data_Remocao).HasColumnName("Data_Remocao").HasColumnType("datetime"); ;
            this.Property(t => t.Data_Edicao).HasColumnName("Data_Edicao").HasColumnType("datetime"); ;
            this.Property(t => t.Data_Criacao).HasColumnName("Data_Criacao").HasColumnType("datetime"); ;
            this.Property(t => t.Data_Conclusao).HasColumnName("Data_Conclusao").HasColumnType("datetime"); ;
        }
    }
}
