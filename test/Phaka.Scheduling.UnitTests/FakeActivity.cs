namespace Phaka.Scheduling.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeActivity : IEquatable<FakeActivity>
    {
        public FakeActivity(string name) : this(name, TimeSpan.FromMilliseconds(10))
        {
        }

        public FakeActivity(string name, TimeSpan delay)
        {
            Name = name;
            Delay = delay;
        }

        public string Name { get; }

        public TimeSpan Delay { get; }

        public Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Delay((TimeSpan) Delay, cancellationToken);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(FakeActivity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FakeActivity) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(FakeActivity left, FakeActivity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FakeActivity left, FakeActivity right)
        {
            return !Equals(left, right);
        }
    }
}