namespace Infrastructure.QueryBuilder
{
    public class Topping
    {
        public const int TOP_NONE = -1;

        public int PageSize { get; set; }

        public bool IsDefined => (PageSize != TOP_NONE && PageSize > 0);

        public Topping()
        {
            PageSize = TOP_NONE;
        }
    }
}