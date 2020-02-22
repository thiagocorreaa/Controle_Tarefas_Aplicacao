using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitTest.Initialization
{
    public class UnitTestRegistry : Registry
    {
        static IEnumerable<string> ALLOWED_ASSEMBLIES = new[] { "Tarefas" };

        public UnitTestRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssembliesFromPath(AppDomain.CurrentDomain.BaseDirectory, ShouldLoadAssembly);
                scan.WithDefaultConventions();
            });
        }

        public static bool ShouldLoadAssembly(Assembly candidate)
        {
            var exists = ALLOWED_ASSEMBLIES.Any(
                allowed => candidate.FullName.StartsWith(allowed, StringComparison.InvariantCultureIgnoreCase)
            );

            return exists;
        }
    }
}