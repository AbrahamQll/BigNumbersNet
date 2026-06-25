# BigNumbersNet

[![NuGet Version](https://img.shields.io/nuget/v/BigNumbersNet.svg)](https://www.nuget.org/packages/BigNumbersNet)

An arbitrary-precision decimal and integer arithmetic library for .NET, facilitating calculations for exceptionally large numbers and high-precision fractions.

## Features

- **Decimals & Integers**: Full support for arbitrarily large integers and high-precision fractional decimals.
- **Immutable Data Type**: Prevents unintended side-effects with thread-safe mathematical operations.
- **Full Operator Overloading**: Seamlessly write expressions using standard operators (`+`, `-`, `*`, `/`, `%`, `<`, `>`, `==`).
- **Standard Math Functions**: Out-of-the-box support for `Abs`, `Pow`, `Max`, `Min`, and `GreatestCommonDivisor (GCD)`.
- **Targeting Modern Frameworks**: Optimized for both **.NET 8** and **.NET 10**.

## Installation

Install via the NuGet Package Manager:

```bash
dotnet add package BigNumbersNet
```

## Quick Start

### Basic Arithmetic & Decimals

```csharp
using BigNumbersNet;

// Parsing large decimal and integer values
BigNumber val1 = BigNumber.Parse("98765432101234567890.123456");
BigNumber val2 = BigNumber.Parse("12345678901234567890.500000");

// Standard operations
BigNumber sum = val1 + val2;        // Automatically aligns scales
BigNumber difference = val1 - val2;
BigNumber product = val1 * val2;

Console.WriteLine($"Sum: {sum}");                 // Prints normalized decimal
Console.WriteLine($"Product: {product}");
```

### Division Operators (Important Note)

Unlike standard C# integer types, the `/` operator in `BigNumbersNet` performs high-precision fractional division. If you require truncating integer division (similar to standard `BigInteger`), use the `IntegerDivide` method:

```csharp
BigNumber numA = BigNumber.Parse("5");
BigNumber numB = BigNumber.Parse("2");

// 1. High-precision decimal division
BigNumber decimalQuotient = numA / numB; 
Console.WriteLine(decimalQuotient); // Outputs "2.5"

// 2. Truncating integer division
BigNumber integerQuotient = BigNumber.IntegerDivide(numA, numB);
Console.WriteLine(integerQuotient); // Outputs "2"
```

### Advanced Math Operations

```csharp
// Greatest Common Divisor (supported on integers)
BigNumber gcd = BigNumber.GreatestCommonDivisor(BigNumber.Parse("45"), BigNumber.Parse("30")); // 15

// Exponents
BigNumber raised = BigNumber.Pow(BigNumber.Parse("5.5"), 10);
```

## Performance & Design Note

The library stores decimal digits internally in base-10 byte arrays to control allocations and memory mutations. While stable for mathematical assertions, it relies on schoolbook implementation algorithms. For cryptographically secure operations or heavy high-performance computations, we recommend reviewing performance against platform-standard `System.Numerics.BigInteger`.

## License

This project is licensed under the GPL-3.0-or-later license.
