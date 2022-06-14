namespace Simple.Wpf.Template.Extensions
{
    using System.Collections.Generic;

    public static class AddRangeExtensions
    {
        public static void AddRange<T>(this ICollection<T> oc, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                oc.Add(item);
            }
        }
    }
}
