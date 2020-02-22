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
        public DateTime? Data_Edicao { get; set; }
        public Nullable<DateTime> Data_Remocao { get; set; }
        public Nullable<DateTime> Data_Conclusao { get; set; }

        public string Status_Descricao
        {
            get
            {
                return this.Status ? "Ativo" : "Inativo";
            }
        }

        public string Data_Criacao_Format
        {
            get
            {
                return this.Data_Criacao.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string Data_Edicao_Format
        {
            get
            {
                return this.Data_Edicao.HasValue ? 
                    this.Data_Edicao.Value.ToString("dd/MM/yyyy HH:mm") :
                    string.Empty;
            }
        }

        public string Data_Remocao_Format
        {
            get
            {
                return this.Data_Remocao.HasValue ?
                    this.Data_Remocao.Value.ToString("dd/MM/yyyy HH:mm") :
                    string.Empty;
            }
        }

        public string Data_Conclusao_Format
        {
            get
            {
                return this.Data_Conclusao.HasValue ?
                    this.Data_Conclusao.Value.ToString("dd/MM/yyyy HH:mm") :
                    string.Empty;
            }
        }
    }
}
