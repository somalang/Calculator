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
        public static implicit operator decimal(CalcValue? calcValue)
        {
            return calcValue?.value ?? 0;
        }

        public static implicit operator CalcValue(decimal value)
        {
            return new CalcValue(value);
        }

        // 연산자 재정의
        public static CalcValue operator +(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) throw new ArgumentNullException();
            return new CalcValue(left.value + right.value);
        }

        public static CalcValue operator -(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) throw new ArgumentNullException();
            return new CalcValue(left.value - right.value);
        }

        public static CalcValue operator *(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) throw new ArgumentNullException();
            return new CalcValue(left.value * right.value);
        }

        public static CalcValue operator /(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) throw new ArgumentNullException();
            if (right.value == 0) throw new DivideByZeroException("0으로 나눌 수 없습니다");
            return new CalcValue(left.value / right.value);
        }

        // 단항 연산자
        public static CalcValue operator -(CalcValue? value)
        {
            if (value is null) throw new ArgumentNullException();
            return new CalcValue(-value.value);
        }

        public static CalcValue operator +(CalcValue? value)
        {
            if (value is null) throw new ArgumentNullException();
            return new CalcValue(+value.value);
        }

        // 기타 연산 메서드
        public CalcValue Sqrt()
        {
            if (value < 0) throw new InvalidOperationException("음수의 제곱근을 구할 수 없습니다");
            return new CalcValue((decimal)Math.Sqrt((double)value));
        }

        public CalcValue Square() => new CalcValue(value * value);

        public CalcValue Reciprocal()
        {
            if (value == 0) throw new DivideByZeroException("0의 역수를 구할 수 없습니다");
            return new CalcValue(1 / value);
        }

        public CalcValue Percent() => new CalcValue(value / 100);

        // 비교 연산자
        public static bool operator ==(CalcValue? left, CalcValue? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(CalcValue? left, CalcValue? right) => !(left == right);

        public static bool operator >(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) return false;
            return left.value > right.value;
        }

        public static bool operator <(CalcValue? left, CalcValue? right)
        {
            if (left is null || right is null) return false;
            return left.value < right.value;
        }

        public static bool operator >=(CalcValue? left, CalcValue? right) => left > right || left == right;

        public static bool operator <=(CalcValue? left, CalcValue? right) => left < right || left == right;

        // 인터페이스 구현 (nullable 반영)
        public int CompareTo(CalcValue? other)
        {
            if (other is null) return 1;
            return value.CompareTo(other.value);
        }

        public bool Equals(CalcValue? other)
        {
            if (other is null) return false;
            return value.Equals(other.value);
        }

        public override bool Equals(object? obj) => Equals(obj as CalcValue);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public string ToString(string format) => value.ToString(format);
    }
}
