using StructureMap;

namespace Infrastructure
{
    public static class DbHelper
    {
        public static IUnitOfWork UnitOfWork(string instanceKey)
        {
            var container = Container.For<InfrastructureRegistry>();
            return container.GetInstance<IUnitOfWork>(instanceKey);
        }
    }
}