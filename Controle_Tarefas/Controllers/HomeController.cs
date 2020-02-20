using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult GridBasePreAprovada(int draw, int start, int length)
        {
            
            string search = Request.QueryString["search[value]"];
            int count = 0; // taxas_PreaprovadasBusiness.BuscaListaGridPaginadaCount(search);
            var retorno = new
            {
                recordsTotal = count,
                // data = taxas_PreaprovadasBusiness.BuscaListaGridPaginada(search, start, length).Select(x => new { CNPJ = x.Cnpj, Nivel = x.Niveis.Nivel, Data_Upload = DateTime.Parse(x.Data_Insert.ToString()).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture), Data_Validade = DateTime.Parse(x.Data_Fin.ToString()).ToString("dd/MM/yyyy ", CultureInfo.InvariantCulture) }),
                data = new { titulo = "teste", status = "Ativo", descricao = "TEste", data_criacao = "", data_edicao = "", data_remocao = "", data_conclusao = "" },
                recordsFiltered = count,
                draw = draw
            };

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}