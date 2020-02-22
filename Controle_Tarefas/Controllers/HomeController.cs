using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Business.Controle_Tarefas;
using Controle_Tarefas.Models;
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

        public ActionResult GestaoTarefas(TarefaModelView tarefaModel)
        {
            string acao = Request["acao"];

            Tarefas tarefa = BindViewModelToModel(tarefaModel);

            switch (acao)
            {
                case "remove":
                    tarefasDataService.Remove(tarefa);
                    break;
                case "edit":
                    tarefasDataService.Edit(tarefa);
                    break;
                case "create":
                    tarefasDataService.Add(tarefa);
                    break;
            }

            return View("Index");
        }

        private Tarefas BindViewModelToModel(TarefaModelView tarefaModel)
        {
            Tarefas tarefa = new Tarefas()
            {
                Titulo = tarefaModel.Titulo,
                Status = tarefaModel.Status_Descricao.Equals("Ativo"),
                Id_Tarefa = tarefaModel.Id_Tarefa,
                Descricao = tarefaModel.Descricao,
                Data_Conclusao = tarefaModel.Tarefa_Concluida.Equals("Sim") ? DateTime.Now : default(DateTime?)
            };

            return tarefa;
        }

        public ActionResult GridTarefas(int draw, int start, int length)
        {
            string search = Request.QueryString["search[value]"];

            List<Tarefas> tarefas = tarefasBusiness.BuscaListaGridPaginada(search, start, length);
            List<TarefaModelView> tarefasViewModel = new List<TarefaModelView>();

            tarefas.ForEach(x =>
            {
                tarefasViewModel.Add(BindModelViewModel(x));
            });

            int count = tarefasDataService.ListAll().Count;

            var retorno = new
            {
                recordsTotal = count,
                data = tarefasViewModel,
                recordsFiltered = count,
                draw = draw
            };

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        private TarefaModelView BindModelViewModel(Tarefas x)
        {
            TarefaModelView retorno = new TarefaModelView()
            {
                Data_Conclusao = x.Data_Conclusao,
                Data_Criacao = x.Data_Criacao,
                Data_Edicao = x.Data_Criacao,
                Data_Remocao = x.Data_Remocao,
                Descricao = x.Descricao,
                Id_Tarefa = x.Id_Tarefa,
                Status_Descricao = x.Status ? "Ativo" : "Inativo",
                Tarefa_Concluida = x.Data_Conclusao.HasValue ? "Sim" : "Não",
                Titulo = x.Titulo
            };

            return retorno;
        }
    }
}