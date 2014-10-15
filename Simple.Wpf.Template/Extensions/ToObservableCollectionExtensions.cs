namespace Simple.Wpf.Template.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class ToObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }
    }
}
