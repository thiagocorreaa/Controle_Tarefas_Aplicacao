using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models.Controle_Tarefas;

namespace Controle_Tarefas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GridTarefas(int draw, int start, int length)
        {
            List<Tarefas> lstTarefas = new List<Tarefas>();

            lstTarefas.Add(new Tarefas() { Data_Criacao = DateTime.Now, Descricao = "Teste carrega Grid", Status = true, Titulo = "Teste" });
            lstTarefas.Add(new Tarefas() { Data_Criacao = DateTime.Now, Descricao = "História Curupira", Status = true, Titulo = "História Curupira" });
            lstTarefas.Add(new Tarefas() { Data_Criacao = DateTime.Now, Descricao = "Boitata", Status = true, Titulo = "Boitata" });

            string search = Request.QueryString["search[value]"];
            int count = 1; // taxas_PreaprovadasBusiness.BuscaListaGridPaginadaCount(search);
            var retorno = new
            {
                recordsTotal = count,
                // data = taxas_PreaprovadasBusiness.BuscaListaGridPaginada(search, start, length).Select(x => new { CNPJ = x.Cnpj, Nivel = x.Niveis.Nivel, Data_Upload = DateTime.Parse(x.Data_Insert.ToString()).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture), Data_Validade = DateTime.Parse(x.Data_Fin.ToString()).ToString("dd/MM/yyyy ", CultureInfo.InvariantCulture) }),
                data = lstTarefas,
                recordsFiltered = count,
                draw = draw
            };

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}