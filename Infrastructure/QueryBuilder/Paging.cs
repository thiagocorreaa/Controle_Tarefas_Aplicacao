namespace Infrastructure.QueryBuilder
{
    public class Paging
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int RecordCount { get; set; }

        public bool IsPagingEnabled => (PageSize.HasValue && PageSize.Value != 0);
    }
}