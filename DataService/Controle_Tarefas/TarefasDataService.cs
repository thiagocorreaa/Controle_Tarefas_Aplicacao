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

        public void Edit(models.Tarefas tarefa)
        {
            throw new NotImplementedException();
        }

        public bool Remove(models.Tarefas tarefa)
        {
            bool rowRemoved = false;

            using (var uow = DbHelper.UnitOfWork(nameof(TarefasContext)))
            {
                using (var repository = uow.Repository<models.Tarefas>())
                {
                    var tarefaRemove = repository.ListWhere(new List<Expression<Func<models.Tarefas, object>>> { }, s => s.Id_Tarefa == tarefa.Id_Tarefa);
                    repository.Delete(tarefaRemove.FirstOrDefault());
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
