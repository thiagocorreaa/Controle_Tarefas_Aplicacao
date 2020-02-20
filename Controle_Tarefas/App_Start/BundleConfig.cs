using System.Web;
using System.Web.Optimization;

namespace Controle_Tarefas
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         "~/Scripts/jquery-1.10.2.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
               "~/Scripts/Inputmask/inputmask.js",
               "~/Scripts/Inputmask/jquery.inputmask.js",
               "~/Scripts/Inputmask/inputmask.extensions.js",
               "~/Scripts/Inputmask/inputmask.date.extensions.js",
               //and other extensions you want to include
               "~/Scripts/Inputmask/inputmask.numeric.extensions.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/site.css",
                      "~/Content/cardStyles.css",
                      "~/Content/timeline.css",
                      "~/Content/jquery.qtip.min.css",
                      "~/Content/Datatables/css/jquery.dataTables.css",
                      "~/Content/Datatables/css/buttons.dataTables.css",
                      "~/Content/Datatables/css/select.dataTables.css"));

            bundles.Add(new ScriptBundle("~/bundles/daterangepicker").Include(
               "~/Scripts/daterangepicker/daterangepicker.js",
               "~/Scripts/daterangepicker/moment.min.js"));

            bundles.IgnoreList.Clear();
        }
    }
}
