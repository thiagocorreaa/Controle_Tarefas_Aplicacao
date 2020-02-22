using System;
using System.Collections.Generic;
using System.Linq;
using Business.Contracts;
using DataService.Controle_Tarefas;
using Models.Controle_Tarefas;

namespace Business.Controle_Tarefas
{
    public class TarefasBusiness : ITarefasBusiness
    {
        public List<Tarefas> BuscaListaGridPaginada(string search, int start, int length)
        {
            var retorno = new List<Tarefas>();
            using (var service = new TarefasDataService())
            {
                return service.ListAll().ToList()
                    .Where(v => v.Descricao.Contains(search) || v.Titulo.Contains(search))
                    .OrderByDescending(x => x.Data_Criacao)
                    .Skip(start)
                    .Take(length)
                    .ToList();
            }
        }
        
        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
      
        #endregion
    }
}
