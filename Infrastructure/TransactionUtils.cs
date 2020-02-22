using System.Transactions;

namespace Infrastructure
{
    public class TransactionUtils
    {
        public static TransactionScope New(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TransactionManager.MaximumTimeout
            };

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}