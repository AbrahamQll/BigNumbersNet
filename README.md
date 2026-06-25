# BigNumbersNet

[![NuGet Version](https://img.shields.io/nuget/v/BigNumbersNet.svg)](https://www.nuget.org/packages/BigNumbersNet)

A simple arbitrary-precision integer library for .NET, facilitating calculations for exceptionally large numbers.

## Features

- **Immutable Data Type**: Prevents unintended side-effects with thread-safe math operations.
- **Full Operator Overloading**: Seamlessly write expressions using standard operators (`+`, `-`, `*`, `/`, `%`, `< `, `>`, `==`).
- **Standard Math Functions**: Out-of-the-box support for `Abs`, `Pow`, `Max`, `Min`, and `GreatestCommonDivisor (GCD)`.
- **Targeting Modern Frameworks**: Optimized for both **.NET 8** and **.NET 10**.

## Installation

Install via the NuGet Package Manager:

```bash
dotnet add package BigNumbersNet
```

## Quick Start

```csharp
using BigNumbersNet;

// Parsing large values
BigNumber val1 = BigNumber.Parse("987654321012345678901234567890");
BigNumber val2 = BigNumber.Parse("123456789012345678901234567890");

// Standard operations
BigNumber sum = val1 + val2;
BigNumber difference = val1 - val2;
BigNumber product = val1 * val2;
BigNumber quotient = val1 / val2;
BigNumber remainder = val1 % val2;

// Advanced Math
BigNumber gcd = BigNumber.GreatestCommonDivisor(val1, val2);
BigNumber raised = BigNumber.Pow(BigNumber.Parse("5"), 100);

Console.WriteLine($"Sum: {sum}");
Console.WriteLine($"GCD: {gcd}");
```

## Performance & Design Note

The library stores decimal digits internally in base-10 byte arrays, reducing allocation overhead and memory mutations. While it is stable for basic mathematical assertions, it relies on schoolbook implementation algorithms. For cryptographically secure operations or heavy high-performance computations, we recommend reviewing performance against platform-standard `System.Numerics.BigInteger`.

## License

This project is licensed under the GPL-3.0 license.
