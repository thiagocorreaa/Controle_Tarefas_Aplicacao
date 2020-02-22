using System;
using System.Collections.Generic;
using System.Linq;
using DataService.Controle_Tarefas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Controle_Tarefas;

namespace UnitTest.Controllers
{
    [TestClass]
    public class TarefasTest
    {
        private TarefasDataService _service;

        public TarefasDataService service
        {
            get
            {
                return _service = _service ?? new TarefasDataService();
            }
        }

        [TestMethod]
        public void GetListaTarefas()
        {
            List<Tarefas> tarefas = service.ListAll().ToList();

            Assert.IsTrue(tarefas.Count() > 0);
           
        }

        [TestMethod]
        public void InserirTarefa()
        {
            Tarefas tarefa = new Tarefas() { Data_Criacao = DateTime.Now, Descricao = "Teste carrega Grid", Status = true, Titulo = "Teste" };

            bool inseriu = service.Add(tarefa);

            Assert.IsTrue(inseriu);
        }
    }
}
