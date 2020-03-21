using System;

namespace ToDo.Frontend.Services
{
    public struct RequestParam : IEquatable<RequestParam>
    {        
        public string Name { get; }
        
        public object Value { get; }
        
        public RequestParam(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            return obj is RequestParam other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode(StringComparison.Ordinal) : 0) * 397)
                       ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(RequestParam left, RequestParam right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RequestParam left, RequestParam right)
        {
            return !(left == right);
        }

        public bool Equals(RequestParam other)
        {
            return Name == other.Name && Equals(Value, other.Value);
        }
    }
}