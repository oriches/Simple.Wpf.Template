namespace WpfTemplate.Models
{
    using System.Collections.Generic;

    public sealed class Memory
    {
        private sealed class WorkingSetPrivateManagedEqualityComparer : IEqualityComparer<Memory>
        {
            public bool Equals(Memory x, Memory y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.WorkingSetPrivate == y.WorkingSetPrivate && x.Managed == y.Managed;
            }

            public int GetHashCode(Memory obj)
            {
                unchecked
                {
                    return (obj.WorkingSetPrivate.GetHashCode() * 397) ^ obj.Managed.GetHashCode();
                }
            }
        }

        private static readonly IEqualityComparer<Memory> ComparerInstance = new WorkingSetPrivateManagedEqualityComparer();

        public static IEqualityComparer<Memory> Comparer
        {
            get { return ComparerInstance; }
        }

        public Memory(decimal workingSetPrivate, decimal managed)
        {
            WorkingSetPrivate = workingSetPrivate;
            Managed = managed;
        }

        public decimal WorkingSetPrivate { get; private set; }

        public decimal Managed { get; private set; }
    }
}
