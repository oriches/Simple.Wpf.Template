using System;

namespace Simple.Wpf.Template.Modules;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleConfigurationAttribute : Attribute, IComparable<ModuleConfigurationAttribute>
{
    public ModuleContext Context { get; set; }

    public int Position { get; set; }

    public int CompareTo(ModuleConfigurationAttribute other)
    {
        if (ReferenceEquals(this, other)) return 0;

        if (ReferenceEquals(null, other)) return 1;

        var contextComparison = Context.CompareTo(other.Context);
        if (contextComparison != 0) return contextComparison;

        return Position.CompareTo(other.Position);
    }
}