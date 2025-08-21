using System;

namespace Calculator.Core.Models
{
    public class CalcValue : IComparable<CalcValue>, IEquatable<CalcValue>
    {
        // 상수
        public static readonly CalcValue ZERO = new CalcValue(0);
        public static readonly CalcValue ONE = new CalcValue(1);

        // 필드
        private readonly decimal value;

        // 속성
        public decimal Value => value;

        // 생성자
        public CalcValue(decimal value)
        {
            this.value = value;
        }

        // Public 메서드 - 연산자 오버로딩 (CLR 구조 활용)
        public static CalcValue operator +(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.value + right.value);
        }

        public static CalcValue operator -(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.value - right.value);
        }

        public static CalcValue operator *(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.value * right.value);
        }

        public static CalcValue operator /(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            if (right.value == 0) throw new DivideByZeroException("0으로 나눌 수 없음");
            return new CalcValue(left.value / right.value);
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

        // 계산 메서드
        public CalcValue Sqrt()
        {
            if (value < 0) throw new InvalidOperationException("음수의 제곱근을 구할 수 없음");
            return new CalcValue((decimal)Math.Sqrt((double)value));
        }

        public CalcValue Square()
        {
            return new CalcValue(value * value);
        }

        public CalcValue Percent()
        {
            return new CalcValue(value / 100);
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