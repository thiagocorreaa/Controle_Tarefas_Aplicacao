using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure;
using models = Models.Controle_Tarefas;

namespace DataService.Controle_Tarefas
{
    public class TarefasDataService : IDisposable
    {
        public IList<models.Tarefas> ListAll()
        {
            using (var uow = DbHelper.UnitOfWork(nameof(TarefasContext)))
            using (var repository = uow.Repository<models.Tarefas>())
                return repository.ListAll();
        }

        public bool Edit(models.Tarefas tarefa)
        {
            bool updated = false;

            using (var uow = DbHelper.UnitOfWork(nameof(TarefasContext)))
            using (var repository = uow.Repository<models.Tarefas>())
            {
                var tarefaEdit = repository.ListWhere(new List<Expression<Func<models.Tarefas, object>>> { }, s => s.Id_Tarefa == tarefa.Id_Tarefa).FirstOrDefault();

                tarefaEdit.Data_Edicao = DateTime.Now;
                tarefaEdit.Descricao = tarefa.Descricao;
                tarefaEdit.Titulo = tarefa.Titulo;
                tarefaEdit.Data_Conclusao = tarefa.Data_Conclusao;
                tarefaEdit.Status = tarefa.Status;

                repository.Update(tarefaEdit);     
                
                uow.SaveChanges();

                updated = true;
            }

            return updated;
        }

        public bool Remove(models.Tarefas tarefa)
        {
            bool rowRemoved = false;

            using (var uow = DbHelper.UnitOfWork(nameof(TarefasContext)))
            {
                using (var repository = uow.Repository<models.Tarefas>())
                {
                    var tarefaRemove = repository.ListWhere(new List<Expression<Func<models.Tarefas, object>>> { }, s => s.Id_Tarefa == tarefa.Id_Tarefa).FirstOrDefault();
                    tarefaRemove.Data_Remocao = DateTime.Now;

                    repository.Delete(tarefaRemove);
                    uow.SaveChanges();

                    rowRemoved = true;
                }
            }

            return rowRemoved;
        }

        public bool Add(models.Tarefas tarefa)
        {
            bool rowInserted = false;

            using (var uow = DbHelper.UnitOfWork(nameof(TarefasContext)))
            {
                using (var repository = uow.Repository<models.Tarefas>())
                {
                    tarefa.Data_Criacao = DateTime.Now;                   
                   
                    repository.Add(tarefa);
                    uow.SaveChanges();

                    rowInserted = true;
                }
            }

            return rowInserted;
        }


        #region IDisposable Members

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
            }

            disposed = true;
        }
               
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
