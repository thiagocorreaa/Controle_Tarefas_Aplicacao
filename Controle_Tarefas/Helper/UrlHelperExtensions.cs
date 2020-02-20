using System;
using System.Reflection;
using System.Web.Mvc;

namespace Controle_Tarefas.Helper
{
    public static class UrlHelperExtensions
    {
        public static string ContentVersioned(this UrlHelper urlHelper, string contentPath)
        {
            string url = urlHelper.Content(contentPath);
            string version = GetBuildVersion();

            return $"{url}?v={version}";
        }

        public static string GetBuildVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }
    }
}