using System;

namespace Models.Controle_Tarefas
{
    public class Tarefas
    {
        public int Id_Tarefa { get; set; }
        public string Titulo { get; set; }
        public bool Status { get; set; }
        public string Descricao { get; set; }
        public DateTime Data_Criacao { get; set; }
        public Nullable<DateTime> Data_Edicao { get; set; }
        public Nullable<DateTime> Data_Remocao { get; set; }
        public Nullable<DateTime> Data_Conclusao { get; set; }      
    }    
}
