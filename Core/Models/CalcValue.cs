using System;

namespace Calculator.Core.Models
{
    public class CalcValue : IComparable<CalcValue>, IEquatable<CalcValue>
    {
        public static readonly CalcValue ZERO = new CalcValue(0);
        public static readonly CalcValue ONE = new CalcValue(1);

        private readonly decimal value;
        public decimal Value => value;

        public CalcValue(decimal value)
        {
            this.value = value;
        }

        // 암시적 변환 연산자
        public static implicit operator decimal(CalcValue calcValue)
        {
            return calcValue?.value ?? 0;
        }

        public static implicit operator CalcValue(decimal value)
        {
            return new CalcValue(value);
        }

        // 비교 연산자
        public static bool operator ==(CalcValue left, CalcValue right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(CalcValue left, CalcValue right)
        {
            return !(left == right);
        }

        // CLR 구조 활용 - 인터페이스 구현
        public int CompareTo(CalcValue other)
        {
            if (other == null) return 1;
            return value.CompareTo(other.value);
        }

        public bool Equals(CalcValue other)
        {
            if (other == null) return false;
            return value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CalcValue);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}