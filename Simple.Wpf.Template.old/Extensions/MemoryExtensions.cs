namespace Simple.Wpf.Template.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Models;

    public static class MemoryExtensions
    {
        private static readonly IDictionary<MemoryUnits, string> UnitsAsString = new Dictionary<MemoryUnits, string>();
        private static readonly IDictionary<MemoryUnits, decimal> UnitsMultiplier = new Dictionary<MemoryUnits, decimal>();

        private static readonly Type MemoryUnitsType = typeof(MemoryUnits);

        public static string WorkingSetPrivateAsString(this Memory memory)
        {
            return ValueAsString(() => memory.WorkingSetPrivate, MemoryUnits.Mega, 2);
        }

        public static string ManagedAsString(this Memory memory)
        {
            return ValueAsString(() => memory.Managed, MemoryUnits.Mega, 2);
        }

        private static string ValueAsString(Func<decimal> valueFunc, MemoryUnits units, int decimalPlaces)
        {
            return $"{decimal.Round(valueFunc()*GetMultiplier(units), decimalPlaces):0.00} {GetUnitString(units)}";
        }

        private static decimal GetMultiplier(MemoryUnits units)
        {
            if (UnitsMultiplier.TryGetValue(units, out var unitsMultiplier))
            {
                return unitsMultiplier;
            }

            unitsMultiplier = 1 / Convert.ToDecimal(units);

            UnitsMultiplier.Add(units, unitsMultiplier);
            return unitsMultiplier;
        }

        private static string GetUnitString(MemoryUnits units)
        {
            if (UnitsAsString.TryGetValue(units, out var unitsString))
            {
                return unitsString;
            }

            var memInfo = MemoryUnitsType.GetMember(units.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            unitsString = ((DescriptionAttribute)attributes[0]).Description;

            UnitsAsString.Add(units, unitsString);
            return unitsString;
        }
    }
}
