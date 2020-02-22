using System;
using System.Collections.Generic;
using Models.Controle_Tarefas;

namespace Business.Contracts
{
    public interface ITarefasBusiness : IDisposable
    {
        List<Tarefas> BuscaListaGridPaginada(string search, int start, int length);
    }
}
