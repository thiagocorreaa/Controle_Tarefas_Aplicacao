namespace Infrastructure.Caching
{
    public enum ExpirationType
    {
        /// <summary>
        /// 0 - None
        /// </summary>
        None = 0,

        /// <summary>
        /// 1 - Absolute: Will expire the entry after a set amount of time.
        /// </summary>
        Absolute = 1,

        /// <summary>
        /// 2 - Sliding: Will expire the entry if it hasn't been accessed in a set amount of time.
        /// </summary>
        Sliding = 2
    }
}
