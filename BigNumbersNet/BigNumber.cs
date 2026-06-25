using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BigNumbersNet
{
    /// <summary>
    /// Represents an arbitrarily large signed decimal or integer of arbitrary precision.
    /// </summary>
    public sealed class BigNumber :
        IComparable<BigNumber>,
        IEquatable<BigNumber>,
        IComparable,
        IFormattable,
        IParsable<BigNumber>,
        ISpanParsable<BigNumber>
    {
        // Internal digit storage: index 0 is the least significant digit (LSD).
        private readonly byte[] _digits;
        private readonly int _scale;
        private readonly bool _isNegative;

        /// <summary>
        /// Gets or sets the default precision (decimal places) used during fractional division.
        /// </summary>
        public static int DefaultDivisionScale { get; set; } = 50;

        /// <summary>
        /// Gets the value 0.
        /// </summary>
        public static BigNumber Zero { get; } = new BigNumber(new byte[] { 0 }, 0, false);

        /// <summary>
        /// Gets the value 1.
        /// </summary>
        public static BigNumber One { get; } = new BigNumber(new byte[] { 1 }, 0, false);

        /// <summary>
        /// Gets the value -1.
        /// </summary>
        public static BigNumber MinusOne { get; } = new BigNumber(new byte[] { 1 }, 0, true);

        /// <summary>
        /// Gets a value indicating whether this <see cref="BigNumber"/> is zero.
        /// </summary>
        public bool IsZero => _digits.Length == 1 && _digits[0] == 0;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BigNumber"/> represents a negative number.
        /// </summary>
        public bool IsNegative => _isNegative && !IsZero;

        /// <summary>
        /// Gets a number that indicates the sign (negative, zero, or positive) of this <see cref="BigNumber"/>.
        /// </summary>
        public int Sign => IsZero ? 0 : (IsNegative ? -1 : 1);

        /// <summary>
        /// Gets a value indicating whether the current <see cref="BigNumber"/> is an even integer.
        /// </summary>
        public bool IsEven => _scale == 0 && _digits[0] % 2 == 0;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="BigNumber"/> is an odd integer.
        /// </summary>
        public bool IsOdd => !IsEven;

        private BigNumber(byte[] digits, int scale, bool isNegative)
        {
            _digits = digits;
            _scale = scale;
            _isNegative = isNegative;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigNumber"/> class from a string.
        /// </summary>
        /// <param name="value">The string representation of the number.</param>
        public BigNumber(string value)
        {
            var parsed = Parse(value);
            _digits = parsed._digits;
            _scale = parsed._scale;
            _isNegative = parsed._isNegative;
        }

        #region Parsing and Conversions

        /// <summary>
        /// Parses a string representation of a decimal or integer into a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The string containing the number to parse.</param>
        /// <returns>A new <see cref="BigNumber"/> representing the parsed value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not in a valid format.</exception>
        public static BigNumber Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Parse(value.AsSpan());
        }

        /// <summary>
        /// Parses a span of characters representing a decimal or integer into a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The character span containing the number to parse.</param>
        /// <returns>A new <see cref="BigNumber"/> representing the parsed value.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not in a valid format.</exception>
        public static BigNumber Parse(ReadOnlySpan<char> value)
        {
            if (TryParse(value, out var result))
            {
                return result;
            }
            throw new FormatException("The input string was not in a correct format.");
        }

        /// <summary>
        /// Tries to parse a string representation of a decimal or integer into a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="result">The resulting <see cref="BigNumber"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string? value, [NotNullWhen(true)] out BigNumber? result)
        {
            if (value == null)
            {
                result = null;
                return false;
            }
            return TryParse(value.AsSpan(), out result);
        }

        /// <summary>
        /// Tries to parse a character span representing a decimal or integer into a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The character span to parse.</param>
        /// <param name="result">The resulting <see cref="BigNumber"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, [NotNullWhen(true)] out BigNumber? result)
        {
            result = null;
            if (value.IsEmpty) return false;

            bool isNegative = false;
            int startIndex = 0;

            if (value[0] == '-')
            {
                isNegative = true;
                startIndex = 1;
            }
            else if (value[0] == '+')
            {
                startIndex = 1;
            }

            ReadOnlySpan<char> content = value[startIndex..];
            if (content.IsEmpty) return false;

            int dotIndex = content.IndexOf('.');
            ReadOnlySpan<char> integerPart;
            ReadOnlySpan<char> fractionalPart;
            int scale = 0;

            if (dotIndex == -1)
            {
                integerPart = content;
                fractionalPart = ReadOnlySpan<char>.Empty;
            }
            else
            {
                integerPart = content[..dotIndex];
                fractionalPart = content[(dotIndex + 1)..];
                scale = fractionalPart.Length;
            }

            foreach (char c in integerPart)
            {
                if (!char.IsAsciiDigit(c)) return false;
            }
            foreach (char c in fractionalPart)
            {
                if (!char.IsAsciiDigit(c)) return false;
            }

            int totalLength = integerPart.Length + fractionalPart.Length;
            if (totalLength == 0) return false;

            Span<char> combined = stackalloc char[totalLength];
            integerPart.CopyTo(combined);
            fractionalPart.CopyTo(combined[integerPart.Length..]);

            int firstNonZero = 0;
            while (firstNonZero < combined.Length - 1 && combined[firstNonZero] == '0')
            {
                firstNonZero++;
            }
            ReadOnlySpan<char> finalSpan = combined[firstNonZero..];

            byte[] digits = new byte[finalSpan.Length];
            for (int i = 0; i < finalSpan.Length; i++)
            {
                digits[i] = (byte)(finalSpan[finalSpan.Length - 1 - i] - '0');
            }

            if (digits.Length == 1 && digits[0] == 0)
            {
                isNegative = false;
            }

            result = Normalize(digits, scale, isNegative);
            return true;
        }

        /// <summary>
        /// Implicitly converts a 64-bit signed integer to a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The 64-bit signed integer to convert.</param>
        public static implicit operator BigNumber(long value)
        {
            if (value == 0) return Zero;
            bool isNegative = value < 0;
            ulong absValue = value == long.MinValue ? (ulong)long.MaxValue + 1 : (ulong)Math.Abs(value);

            var tempDigits = new System.Collections.Generic.List<byte>();
            while (absValue > 0)
            {
                tempDigits.Add((byte)(absValue % 10));
                absValue /= 10;
            }
            return new BigNumber(tempDigits.ToArray(), 0, isNegative);
        }

        /// <summary>
        /// Implicitly converts a 32-bit signed integer to a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The 32-bit signed integer to convert.</param>
        public static implicit operator BigNumber(int value) => (long)value;

        /// <summary>
        /// Explicitly converts a <see cref="BigNumber"/> to a 64-bit signed integer, truncating any fractional part.
        /// </summary>
        /// <param name="value">The <see cref="BigNumber"/> to convert.</param>
        /// <returns>A 64-bit signed integer representation of the value.</returns>
        /// <exception cref="OverflowException">Thrown when the value is too large to fit in a 64-bit signed integer.</exception>
        public static explicit operator long(BigNumber value)
        {
            BigNumber truncated = value.Truncate();
            if (truncated.IsZero) return 0;

            long result = 0;
            long multiplier = 1;

            for (int i = 0; i < truncated._digits.Length; i++)
            {
                try
                {
                    checked
                    {
                        result += truncated._digits[i] * multiplier;
                        if (i < truncated._digits.Length - 1)
                        {
                            multiplier *= 10;
                        }
                    }
                }
                catch (OverflowException)
                {
                    throw new OverflowException("The BigNumber value is too large to fit in a 64-bit signed integer.");
                }
            }

            return truncated.IsNegative ? -result : result;
        }

        /// <summary>
        /// Explicitly converts a <see cref="BigNumber"/> to a 32-bit signed integer, truncating any fractional part.
        /// </summary>
        /// <param name="value">The <see cref="BigNumber"/> to convert.</param>
        /// <returns>A 32-bit signed integer representation of the value.</returns>
        /// <exception cref="OverflowException">Thrown when the value is too large to fit in a 32-bit signed integer.</exception>
        public static explicit operator int(BigNumber value)
        {
            long longValue = (long)value;
            if (longValue < int.MinValue || longValue > int.MaxValue)
            {
                throw new OverflowException("The BigNumber value is too large to fit in a 32-bit signed integer.");
            }
            return (int)longValue;
        }

        /// <summary>
        /// Explicitly converts a <see cref="BigNumber"/> to a double-precision floating-point number.
        /// </summary>
        /// <param name="value">The <see cref="BigNumber"/> to convert.</param>
        /// <returns>A double-precision floating-point representation of the value.</returns>
        public static explicit operator double(BigNumber value)
        {
            return double.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Explicitly converts a <see cref="BigNumber"/> to a decimal floating-point number.
        /// </summary>
        /// <param name="value">The <see cref="BigNumber"/> to convert.</param>
        /// <returns>A decimal floating-point representation of the value.</returns>
        public static explicit operator decimal(BigNumber value)
        {
            return decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion

        #region Standard Arithmetic Operations

        /// <summary>
        /// Adds two <see cref="BigNumber"/> values.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            int targetScale = Math.Max(a._scale, b._scale);
            byte[] alignedA = AlignScale(a._digits, a._scale, targetScale);
            byte[] alignedB = AlignScale(b._digits, b._scale, targetScale);

            if (a.IsNegative == b.IsNegative)
            {
                var sumDigits = AddAbsolute(alignedA, alignedB);
                return Normalize(sumDigits, targetScale, a.IsNegative);
            }

            int cmp = CompareAbsolute(alignedA, alignedB);
            if (cmp == 0) return Zero;

            if (cmp > 0)
            {
                var diffDigits = SubtractAbsolute(alignedA, alignedB);
                return Normalize(diffDigits, targetScale, a.IsNegative);
            }
            else
            {
                var diffDigits = SubtractAbsolute(alignedB, alignedA);
                return Normalize(diffDigits, targetScale, b.IsNegative);
            }
        }

        /// <summary>
        /// Subtracts one <see cref="BigNumber"/> value from another.
        /// </summary>
        /// <param name="a">The first operand (minuend).</param>
        /// <param name="b">The second operand (subtrahend).</param>
        /// <returns>The difference of <paramref name="a"/> minus <paramref name="b"/>.</returns>
        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            return a + (-b);
        }

        /// <summary>
        /// Negates a <see cref="BigNumber"/> value.
        /// </summary>
        /// <param name="a">The value to negate.</param>
        /// <returns>The negated value of <paramref name="a"/>.</returns>
        public static BigNumber operator -(BigNumber a)
        {
            if (a.IsZero) return a;
            return new BigNumber(a._digits, a._scale, !a._isNegative);
        }

        /// <summary>
        /// Returns the unary plus of a <see cref="BigNumber"/> value.
        /// </summary>
        /// <param name="a">The operand.</param>
        /// <returns>The same value as <paramref name="a"/>.</returns>
        public static BigNumber operator +(BigNumber a) => a;

        /// <summary>
        /// Multiplies two <see cref="BigNumber"/> values.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The product of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            if (a.IsZero || b.IsZero) return Zero;
            bool resultNegative = a.IsNegative != b.IsNegative;
            byte[] resultDigits = MultiplyAbsolute(a._digits, b._digits);
            int targetScale = a._scale + b._scale;
            return Normalize(resultDigits, targetScale, resultNegative);
        }

        /// <summary>
        /// Divides one <see cref="BigNumber"/> value by another, producing a high-precision decimal result.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>The quotient of <paramref name="a"/> divided by <paramref name="b"/>.</returns>
        public static BigNumber operator /(BigNumber a, BigNumber b)
        {
            if (b.IsZero) throw new DivideByZeroException();
            if (a.IsZero) return Zero;

            bool resultNegative = a.IsNegative != b.IsNegative;

            // Always perform high-precision decimal division
            int targetScale = Math.Max(DefaultDivisionScale, Math.Max(a._scale, b._scale));
            int shift = targetScale + b._scale - a._scale;
            byte[] scaledA;
            int finalScale = targetScale;

            if (shift >= 0)
            {
                scaledA = AlignScale(a._digits, 0, shift);
            }
            else
            {
                scaledA = a._digits;
                finalScale = targetScale - shift;
            }

            var (quotientDigits, _) = DivideAbsolute(scaledA, b._digits);
            return Normalize(quotientDigits, finalScale, resultNegative);
        }

        /// <summary>
        /// Calculates the remainder of dividing one <see cref="BigNumber"/> value by another.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>The remainder of dividing <paramref name="a"/> by <paramref name="b"/>.</returns>
        public static BigNumber operator %(BigNumber a, BigNumber b)
        {
            if (b.IsZero) throw new DivideByZeroException();
            if (a.IsZero) return Zero;

            // Fractional modulo: a - (b * Truncate(a / b))
            var quotient = a / b;
            var integerPart = quotient.Truncate();
            return a - (b * integerPart);
        }

        #endregion

        #region Advanced Operations

        /// <summary>
        /// Performs truncating integer division on two values, discarding any fractional remainder.
        /// </summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor.</param>
        /// <returns>The quotient as an integer.</returns>
        public static BigNumber IntegerDivide(BigNumber left, BigNumber right)
        {
            if (right.IsZero) throw new DivideByZeroException();
            if (left.IsZero) return Zero;

            bool resultNegative = left.IsNegative != right.IsNegative;

            BigNumber intLeft = left.Truncate();
            BigNumber intRight = right.Truncate();

            var (quotientDigits, _) = DivideAbsolute(intLeft._digits, intRight._digits);
            return Normalize(quotientDigits, 0, resultNegative);
        }

        /// <summary>
        /// Discards any fractional digits, returning the integer part of the number.
        /// </summary>
        /// <returns>The truncated integer <see cref="BigNumber"/> value.</returns>
        public BigNumber Truncate()
        {
            if (_scale == 0) return this;
            if (_digits.Length <= _scale)
            {
                return Zero;
            }
            byte[] truncatedDigits = new byte[_digits.Length - _scale];
            Array.Copy(_digits, _scale, truncatedDigits, 0, truncatedDigits.Length);
            return Normalize(truncatedDigits, 0, _isNegative);
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="BigNumber"/>.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>The absolute value of <paramref name="value"/>.</returns>
        public static BigNumber Abs(BigNumber value)
        {
            return value.IsNegative ? -value : value;
        }

        /// <summary>
        /// Returns the larger of two <see cref="BigNumber"/> values.
        /// </summary>
        /// <param name="val1">The first value to compare.</param>
        /// <param name="val2">The second value to compare.</param>
        /// <returns>The larger of <paramref name="val1"/> and <paramref name="val2"/>.</returns>
        public static BigNumber Max(BigNumber val1, BigNumber val2)
        {
            return val1 > val2 ? val1 : val2;
        }

        /// <summary>
        /// Returns the smaller of two <see cref="BigNumber"/> values.
        /// </summary>
        /// <param name="val1">The first value to compare.</param>
        /// <param name="val2">The second value to compare.</param>
        /// <returns>The smaller of <paramref name="val1"/> and <paramref name="val2"/>.</returns>
        public static BigNumber Min(BigNumber val1, BigNumber val2)
        {
            return val1 < val2 ? val1 : val2;
        }

        /// <summary>
        /// Raises a <see cref="BigNumber"/> value to a specified power.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <param name="exponent">The exponent power, which must be non-negative.</param>
        /// <returns>The result of raising <paramref name="baseValue"/> to the <paramref name="exponent"/> power.</returns>
        public static BigNumber Pow(BigNumber baseValue, int exponent)
        {
            if (exponent < 0)
                throw new ArgumentOutOfRangeException(nameof(exponent), "Exponent must be non-negative.");
            if (exponent == 0)
                return One;
            if (baseValue.IsZero)
                return Zero;

            BigNumber result = One;
            BigNumber current = baseValue;
            int exp = exponent;

            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    result *= current;
                }
                current *= current;
                exp >>= 1;
            }

            return result;
        }

        /// <summary>
        /// Calculates the greatest common divisor of two integer values.
        /// </summary>
        /// <param name="left">The first integer operand.</param>
        /// <param name="right">The second integer operand.</param>
        /// <returns>The greatest common divisor of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static BigNumber GreatestCommonDivisor(BigNumber left, BigNumber right)
        {
            if (left._scale > 0 || right._scale > 0)
            {
                throw new InvalidOperationException("GreatestCommonDivisor is only supported on integers.");
            }

            BigNumber a = Abs(left);
            BigNumber b = Abs(right);

            while (!b.IsZero)
            {
                BigNumber temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        #endregion

        #region Absolute Calculations Implementation (Internal Helpers)

        private static BigNumber Normalize(byte[] digits, int scale, bool isNegative)
        {
            // 1. Trim trailing fractional zeroes (at index 0 since stored LSD-first)
            int start = 0;
            while (scale > 0 && start < digits.Length && digits[start] == 0)
            {
                start++;
                scale--;
            }

            byte[] finalDigits;
            if (start > 0)
            {
                if (start >= digits.Length)
                {
                    finalDigits = new byte[] { 0 };
                    scale = 0;
                }
                else
                {
                    finalDigits = new byte[digits.Length - start];
                    Array.Copy(digits, start, finalDigits, 0, finalDigits.Length);
                }
            }
            else
            {
                finalDigits = digits;
            }

            // 2. Trim leading integer zeroes (at the end of the array)
            int end = finalDigits.Length;
            while (end > 1 && finalDigits[end - 1] == 0)
            {
                end--;
            }
            if (end < finalDigits.Length)
            {
                Array.Resize(ref finalDigits, end);
            }

            if (finalDigits.Length == 1 && finalDigits[0] == 0)
            {
                isNegative = false;
                scale = 0;
            }

            return new BigNumber(finalDigits, scale, isNegative);
        }

        private static byte[] AlignScale(byte[] a, int currentScale, int targetScale)
        {
            if (currentScale == targetScale) return a;
            int diff = targetScale - currentScale;
            byte[] result = new byte[a.Length + diff];
            Array.Copy(a, 0, result, diff, a.Length);
            return result;
        }

        private static int CompareAbsolute(byte[] a, byte[] b)
        {
            int maxLength = Math.Max(a.Length, b.Length);
            for (int i = maxLength - 1; i >= 0; i--)
            {
                int valA = i < a.Length ? a[i] : 0;
                int valB = i < b.Length ? b[i] : 0;
                if (valA != valB)
                    return valA.CompareTo(valB);
            }
            return 0;
        }

        private static int CompareAbsoluteScaled(byte[] a, int scaleA, byte[] b, int scaleB)
        {
            int targetScale = Math.Max(scaleA, scaleB);
            byte[] alignedA = AlignScale(a, scaleA, targetScale);
            byte[] alignedB = AlignScale(b, scaleB, targetScale);
            return CompareAbsolute(alignedA, alignedB);
        }

        private static byte[] AddAbsolute(byte[] a, byte[] b)
        {
            int length = Math.Max(a.Length, b.Length);
            byte[] result = new byte[length + 1];
            int carry = 0;

            for (int i = 0; i < length; i++)
            {
                int valA = i < a.Length ? a[i] : 0;
                int valB = i < b.Length ? b[i] : 0;
                int sum = valA + valB + carry;
                result[i] = (byte)(sum % 10);
                carry = sum / 10;
            }

            if (carry > 0)
            {
                result[length] = (byte)carry;
                return result;
            }

            Array.Resize(ref result, length);
            return result;
        }

        private static byte[] SubtractAbsolute(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length];
            int borrow = 0;

            for (int i = 0; i < a.Length; i++)
            {
                int valA = a[i];
                int valB = i < b.Length ? b[i] : 0;
                int diff = valA - valB - borrow;

                if (diff < 0)
                {
                    diff += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                result[i] = (byte)diff;
            }

            int nonZeroLength = result.Length;
            while (nonZeroLength > 1 && result[nonZeroLength - 1] == 0)
            {
                nonZeroLength--;
            }

            if (nonZeroLength < result.Length)
            {
                Array.Resize(ref result, nonZeroLength);
            }
            return result;
        }

        private static byte[] MultiplyAbsolute(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                int carry = 0;
                for (int j = 0; j < b.Length; j++)
                {
                    int current = result[i + j] + (a[i] * b[j]) + carry;
                    result[i + j] = (byte)(current % 10);
                    carry = current / 10;
                }
                if (carry > 0)
                {
                    result[i + b.Length] += (byte)carry;
                }
            }

            int len = result.Length;
            while (len > 1 && result[len - 1] == 0)
            {
                len--;
            }

            if (len < result.Length)
            {
                Array.Resize(ref result, len);
            }
            return result;
        }

        private static (byte[] quotient, byte[] remainder) DivideAbsolute(byte[] a, byte[] b)
        {
            int cmp = CompareAbsolute(a, b);
            if (cmp < 0)
            {
                return (new byte[] { 0 }, a);
            }
            if (cmp == 0)
            {
                return (new byte[] { 1 }, new byte[] { 0 });
            }

            byte[] quotient = new byte[a.Length];
            byte[] currentRemainder = new byte[] { 0 };

            for (int i = a.Length - 1; i >= 0; i--)
            {
                if (currentRemainder.Length == 1 && currentRemainder[0] == 0)
                {
                    currentRemainder = new byte[] { a[i] };
                }
                else
                {
                    byte[] temp = new byte[currentRemainder.Length + 1];
                    temp[0] = a[i];
                    Array.Copy(currentRemainder, 0, temp, 1, currentRemainder.Length);
                    currentRemainder = temp;
                }

                byte d = 0;
                while (CompareAbsolute(currentRemainder, b) >= 0)
                {
                    currentRemainder = SubtractAbsolute(currentRemainder, b);
                    d++;
                }
                quotient[i] = d;
            }

            int qLen = quotient.Length;
            while (qLen > 1 && quotient[qLen - 1] == 0)
            {
                qLen--;
            }

            if (qLen < quotient.Length)
            {
                Array.Resize(ref quotient, qLen);
            }

            return (quotient, currentRemainder);
        }

        #endregion

        #region Standard Interface Implementations

        /// <summary>
        /// Compares the current instance with another <see cref="BigNumber"/> and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(BigNumber? other)
        {
            if (other is null) return 1;
            if (ReferenceEquals(this, other)) return 0;
            if (this.IsZero && other.IsZero) return 0;

            if (this.IsNegative != other.IsNegative)
            {
                return this.IsNegative ? -1 : 1;
            }

            int cmpAbs = CompareAbsoluteScaled(this._digits, this._scale, other._digits, other._scale);
            return this.IsNegative ? -cmpAbs : cmpAbs;
        }

        /// <summary>
        /// Compares the current instance with another object and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is BigNumber other) return CompareTo(other);
            throw new ArgumentException("Object must be of type BigNumber", nameof(obj));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(BigNumber? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.IsZero && other.IsZero) return true;

            return this.IsNegative == other.IsNegative &&
                   this._scale == other._scale &&
                   CompareAbsolute(this._digits, other._digits) == 0;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj) => obj is BigNumber other && Equals(other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(IsNegative);
            hash.Add(_scale);
            foreach (var digit in _digits)
            {
                hash.Add(digit);
            }
            return hash.ToHashCode();
        }

        /// <summary>
        /// Compares two <see cref="BigNumber"/> instances for equality.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BigNumber? a, BigNumber? b) => a is null ? b is null : a.Equals(b);

        /// <summary>
        /// Compares two <see cref="BigNumber"/> instances for inequality.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is not equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BigNumber? a, BigNumber? b) => !(a == b);

        /// <summary>
        /// Evaluates if one <see cref="BigNumber"/> is strictly less than another.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is strictly less than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator <(BigNumber? a, BigNumber? b) => a is null ? b is not null : a.CompareTo(b) < 0;

        /// <summary>
        /// Evaluates if one <see cref="BigNumber"/> is strictly greater than another.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is strictly greater than <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator >(BigNumber? a, BigNumber? b) => a is not null && a.CompareTo(b) > 0;

        /// <summary>
        /// Evaluates if one <see cref="BigNumber"/> is less than or equal to another.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator <=(BigNumber? a, BigNumber? b) => a is null || a.CompareTo(b) <= 0;

        /// <summary>
        /// Evaluates if one <see cref="BigNumber"/> is greater than or equal to another.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns><c>true</c> if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        public static bool operator >=(BigNumber? a, BigNumber? b) => a is null ? b is null : a.CompareTo(b) >= 0;

        /// <summary>
        /// Converts the current <see cref="BigNumber"/> value to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of this <see cref="BigNumber"/>.</returns>
        public override string ToString() => ToString(null, null);

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">The format to use, or <c>null</c> to use the default format.</param>
        /// <param name="formatProvider">The provider to use to format the value, or <c>null</c> to obtain the format information from the current locale setting of the operating system.</param>
        /// <returns>The string representation of this <see cref="BigNumber"/> formatted as specified.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (IsZero) return "0";

            var sb = new StringBuilder(_digits.Length + _scale + 2 + (IsNegative ? 1 : 0));
            if (IsNegative)
            {
                sb.Append('-');
            }

            if (_scale == 0)
            {
                for (int i = _digits.Length - 1; i >= 0; i--)
                {
                    sb.Append((char)('0' + _digits[i]));
                }
            }
            else if (_digits.Length <= _scale)
            {
                sb.Append("0.");
                int padding = _scale - _digits.Length;
                for (int i = 0; i < padding; i++)
                {
                    sb.Append('0');
                }
                for (int i = _digits.Length - 1; i >= 0; i--)
                {
                    sb.Append((char)('0' + _digits[i]));
                }
            }
            else
            {
                int dotPosition = _scale;
                for (int i = _digits.Length - 1; i >= 0; i--)
                {
                    sb.Append((char)('0' + _digits[i]));
                    if (i == dotPosition)
                    {
                        sb.Append('.');
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string representation of a number using a specified format provider.
        /// </summary>
        /// <param name="s">The string containing the number to parse.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>The parsed <see cref="BigNumber"/> value.</returns>
        public static BigNumber Parse(string s, IFormatProvider? provider) => Parse(s);

        /// <summary>
        /// Tries to parse a string representation of a number using a specified format provider.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="result">The resulting <see cref="BigNumber"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out BigNumber? result) => TryParse(s, out result);

        /// <summary>
        /// Parses a character span representing a number using a specified format provider.
        /// </summary>
        /// <param name="s">The character span containing the number to parse.</param>
        /// <param name="provider">The format provider.</param>
        /// <returns>The parsed <see cref="BigNumber"/> value.</returns>
        public static BigNumber Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);

        /// <summary>
        /// Tries to parse a character span representing a number using a specified format provider.
        /// </summary>
        /// <param name="s">The character span to parse.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="result">The resulting <see cref="BigNumber"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [NotNullWhen(true)] out BigNumber? result) => TryParse(s, out result);

        #endregion
    }
}