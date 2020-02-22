using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Business.Controle_Tarefas;
using DataService.Controle_Tarefas;
using Models.Controle_Tarefas;

namespace Controle_Tarefas.Controllers
{
    public class HomeController : Controller
    {
        private TarefasDataService _tarefaDataService;
        private TarefasBusiness _tarefasBusiness;

        public TarefasDataService tarefasDataService
        {
            get
            {
                return this._tarefaDataService = this._tarefaDataService ?? new TarefasDataService();
            }
        }

        public TarefasBusiness tarefasBusiness
        {
            get
            {
                return this._tarefasBusiness = this._tarefasBusiness ?? new TarefasBusiness();
            }
        }

        public HomeController()
        {
           
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GestaoTarefas(Tarefas tarefa)
        {
            switch(Request["acao"])
            {
                case "remove":
                    tarefa.Data_Remocao = DateTime.Now;
                    tarefasDataService.Remove(tarefa);
                    break;
                case "edit":
                    tarefa.Data_Criacao = DateTime.Now;
                    tarefasDataService.Edit(tarefa);
                    break;
                case "create":
                    tarefa.Data_Criacao = DateTime.Now;
                    tarefa.Status = tarefa.Status_Descricao.Equals("Ativo");
                    tarefasDataService.Add(tarefa);
                    break;
            }            

            return View("Index");
        }

        public ActionResult GridTarefas(int draw, int start, int length)
        {
            string search = Request.QueryString["search[value]"];

            List<Tarefas> tarefas = tarefasBusiness.BuscaListaGridPaginada(search, start, length);

            int count = tarefasDataService.ListAll().Count;

            var retorno = new
            {
                recordsTotal = count,
                data = tarefas,                
                recordsFiltered = count,
                draw = draw
            };

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}