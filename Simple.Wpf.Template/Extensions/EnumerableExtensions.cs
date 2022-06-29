using System;
using System.Collections.Generic;

namespace Simple.Wpf.Template.Extensions;

public static class EnumerableExtensions
{
    public static void AddRange<T>(this ICollection<T> oc, IEnumerable<T> collection)
    {
        foreach (var item in collection) oc.Add(item);
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var i in collection) action(i);

        return collection;
    }
}