using System.Windows;
using System.Windows.Media;

namespace Simple.Wpf.Template.Views.Extensions;

public static class DependencyObjectExtensions
{
    public static T FindAncestor<T>(this DependencyObject current) where T : DependencyObject
    {
        current = VisualTreeHelper.GetParent(current);

        while (current != null)
        {
            if (current is T ancestor) return ancestor;
            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    public static T FindAncestor<T>(this DependencyObject current, T lookupItem) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T ancestor && Equals(ancestor, lookupItem)) return ancestor;
            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    public static T FindAncestor<T>(this DependencyObject current, string parentName) where T : DependencyObject
    {
        while (current != null)
        {
            if (!string.IsNullOrEmpty(parentName))
            {
                if (current is T and FrameworkElement frameworkElement && frameworkElement.Name == parentName)
                    return (T)current;
            }
            else if (current is T dependencyObject)
            {
                return dependencyObject;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        ;

        return null;
    }

    public static T FindDescendant<T>(DependencyObject parent, string childName) where T : DependencyObject
    {
        if (parent == null) return null;

        T foundChild = null;

        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is not T childType)
            {
                foundChild = FindDescendant<T>(child, childName);

                if (foundChild != null) break;
            }
            else if (!string.IsNullOrEmpty(childName))
            {
                if (childType is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    foundChild = childType;
                    break;
                }

                foundChild = FindDescendant<T>(childType, childName);

                if (foundChild != null) break;
            }
            else
            {
                foundChild = childType;
                break;
            }
        }

        return foundChild;
    }

    public static T FindDescendant<T>(this DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;

        T foundChild = null;

        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is not T childType)
            {
                foundChild = FindDescendant<T>(child);
                if (foundChild != null) break;
            }
            else
            {
                foundChild = childType;
                break;
            }
        }

        return foundChild;
    }
}