namespace Simple.Wpf.Template.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ForEachExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var i in collection)
            {
                action(i);
            }

            return collection;
        }
    }
}
