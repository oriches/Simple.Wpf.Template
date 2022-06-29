using System.Reflection;

namespace Simple.Wpf.Template.Tests;

public static class TestHelper
{
    public static IEnumerable<PropertyInfo> PropertiesImplementingInterface<T>(object instance)
    {
        return instance.GetType()
            .GetProperties()
            .Where(x => x.PropertyType == typeof(T) || x.PropertyType.GetInterfaces()
                .Any(y => y == typeof(T)));
    }
}