# Roman

[![NuGet Downloads](https://img.shields.io/nuget/dt/Roman.svg)](https://www.nuget.org/packages/Roman/)
[![NuGet Version](https://img.shields.io/nuget/v/Roman.svg)](https://www.nuget.org/packages/Roman/)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/deliciousNesquik/roman/blob/main/LICENSE.md)
[![CodeOfConduct](https://img.shields.io/badge/code_of_conduct-enforced_)](https://github.com/deliciousNesquik/roman/blob/main/CODE_OF_CONDUCT.md)

A small, dependency-free C# library for Roman numerals: conversion to and from Arabic
integers, arithmetic, and comparison. Immutable, allocation-light, and fully unit-tested.

**🌐 Language:** **English** · [Русский](https://github.com/deliciousNesquik/roman/blob/main/README.ru.md)

## Table of contents

- [Features](#features)
- [Installation](#installation)
- [Quick start](#quick-start)
- [Usage](#usage)
  - [Creating values](#creating-values)
  - [Conversions](#conversions)
  - [Arithmetic](#arithmetic)
  - [Comparison](#comparison)
  - [Parsing modes (strict / lenient)](#parsing-modes-strict--lenient)
- [Limitations](#limitations)
- [API reference](#api-reference)
- [Performance](#performance)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## Features

- Convert Arabic numbers (1–3999) to Roman and back
- Arithmetic: addition, subtraction, multiplication, division
- Comparison operators: `>`, `<`, `>=`, `<=`, `==`, `!=`
- Implements `IComparable<Roman>` and `IEquatable<Roman>`
- Immutable type — every operation returns a new value
- Well-defined `null` semantics in comparison operators
- Case-insensitive parsing with surrounding whitespace trimmed
- Strict (default) and lenient (`RomanStyle.Lenient`) parsing modes
- `TryParse` for exception-free parsing
- Allocation-light conversion via `stackalloc Span<char>`
- No external dependencies

## Installation

```bash
# .NET CLI
dotnet add package Roman
```

```powershell
# Package Manager
Install-Package Roman
```

```xml
<!-- PackageReference -->
<PackageReference Include="Roman" Version="2.0.1" />
```

## Quick start

```csharp
using RomanNumerals;

// From an integer
var a = new Roman(42);
Console.WriteLine(a);            // XLII

// From a string
var b = new Roman("XIX");
Console.WriteLine(b.ToInt());    // 19

// Arithmetic
Console.WriteLine(a + b);        // LXI  (61)

// Comparison
if (a > b)
    Console.WriteLine("42 > 19");

// Exception-free parsing
if (Roman.TryParse("MCMXCIV", out var year))
    Console.WriteLine(year.ToInt());   // 1994
```

> **Note:** the type is `Roman`, the namespace is `RomanNumerals`. Add `using RomanNumerals;`
> and use `new Roman(...)`.

## Usage

### Creating values

```csharp
var fromInt    = new Roman(1984);        // MCMLXXXIV
var fromString = new Roman("MMXXIII");   // 2023
var lower      = new Roman("  xlii  ");  // 42 (trimmed, case-insensitive)
var copy       = new Roman(fromInt);     // copy constructor
```

### Conversions

```csharp
// Parse — throws on invalid input
var r1 = Roman.Parse(500);
var r2 = Roman.Parse("D");

// TryParse — never throws on bad input
if (Roman.TryParse(42, out var r3)) { /* ... */ }
if (Roman.TryParse("invalid", out var r4)) { /* not reached */ }

// Explicit Roman -> int (implicit would let mixed expressions bypass the range guard)
var value = (int)new Roman(42);   // 42
var same  = new Roman(42).ToInt(); // 42

// Explicit int -> Roman (can throw)
var r5 = (Roman)99;            // XCIX
```

### Arithmetic

Operations compute on the underlying integer and re-validate the result against the 1–3999
range, throwing `OverflowException` when the result overflows (> 3999) or underflows (< 1).
Division is integer division.

```csharp
new Roman(10) + new Roman(5);   // XV  (15)
new Roman(50) - new Roman(20);  // XXX (30)
new Roman(7)  * new Roman(8);   // LVI (56)
new Roman(20) / new Roman(4);   // V   (5)
new Roman(10) / new Roman(3);   // III (3, integer division)

new Roman(3999) + new Roman(1); // throws: result > 3999
new Roman(5)    - new Roman(5); // throws: result < 1
```

An `int` operand works on either side and is range-checked identically — the operand is read as an
Arabic quantity, so only the *result* has to be representable:

```csharp
new Roman(10) + 5;              // XV  (15)
50 - new Roman(20);             // XXX (30)
new Roman(5)  + -3;             // II  (2, out-of-range operand, in-range result)
new Roman(3999) + 1;            // throws OverflowException
new Roman(10) / 0;              // throws DivideByZeroException
```

Comparison and equality against an `int` never construct a `Roman`, so an unrepresentable bound is
a fair question:

```csharp
new Roman(3999) < 5000;         // true
new Roman(42) == 42;            // true  (agrees with .Equals(42))
```

### Comparison

```csharp
var a = new Roman(50);
var b = new Roman(30);

a > b;            // true
a == b;           // false
a.CompareTo(b);   // > 0
a.Equals(b);      // false
```

`null` semantics mirror `Comparer<T>` (null sorts lowest):

```csharp
Roman x = new Roman(5);
Roman y = null;

x > y;    // true   (a value is greater than null)
y < x;    // true   (null is less than any value)
x == y;   // false
x != y;   // true

Roman p = null, q = null;
p == q;   // true
p >= q;   // true
```

### Parsing modes (strict / lenient)

By default parsing is **strict** — it accepts only the canonical form and rejects non-canonical
input such as `"IIII"`. This applies to the constructors, `Parse(string)` and `TryParse(string, …)`:

```csharp
new Roman("IV");    // OK -> 4
new Roman("IIII");  // FormatException: not canonical
```

For **lenient** parsing (accept non-canonical forms), pass `RomanStyle.Lenient` to the `Parse` /
`TryParse` overloads — modeled after `int.Parse(string, NumberStyles)`:

```csharp
var roman = Roman.Parse("IIII", RomanStyle.Lenient);  // parses as 4
Console.WriteLine(roman);                              // IV (output is always canonical)

if (Roman.TryParse("IIII", RomanStyle.Lenient, out var r))
    Console.WriteLine(r);                              // IV
```

`RomanStyle.Strict` (the default) rejects garbage characters (`ArgumentException`), out-of-range
values (`ArgumentOutOfRangeException`), and otherwise valid but non-canonical forms
(`FormatException`). A null string is an `ArgumentNullException`; empty and whitespace-only strings
are an `ArgumentException`. `TryParse` still returns `false` for all of those rather than throwing.

An undefined `RomanStyle` — anything produced by a cast, such as `(RomanStyle)7` — is rejected
with `ArgumentOutOfRangeException` rather than silently selecting a mode. `TryParse` throws in that
case too, the way `int.TryParse` does for an invalid `NumberStyles`: a bad argument is a caller
error, not a parse failure to recover from. `default(RomanStyle)` is `Strict`, so an uninitialized
field parses strictly.

Lenient parsing subtracts every symbol that is smaller than the largest symbol to its right, not
just one that precedes a larger neighbour. That gives the readings attested in Roman inscriptions
for multi-symbol subtractive runs:

```csharp
Roman.Parse("IIX",  RomanStyle.Lenient).ToInt();   // 8
Roman.Parse("XIIX", RomanStyle.Lenient).ToInt();   // 18
Roman.Parse("IIC",  RomanStyle.Lenient).ToInt();   // 98
```

If the subtractions take the result below 1, parsing throws `ArgumentOutOfRangeException` rather
than returning a value: `Roman.Parse("IIIIIIIIIIX", RomanStyle.Lenient)` would be 0. Lenient mode
is not a validator — it still accepts forms no convention allows, such as `"IC"` for 99.

## Limitations

- **Range is 1–3999** (`MMMCMXCIX`). Values outside this range throw
  `ArgumentOutOfRangeException`. There is no representation for `0` or negatives.
- **Round-trip is value-preserving, not string-preserving.**
  `Roman.Parse("IIII", RomanStyle.Lenient).ToString()` returns `"IV"`. Strict parsing (the
  default) rejects non-canonical input outright.
- Arithmetic results must stay within 1–3999, otherwise they throw.

## API reference

### Constructors

| Constructor | Description |
|-------------|-------------|
| `Roman(int value)` | Creates a value from an integer (1–3999) |
| `Roman(string roman)` | Creates a value from a string (strict) |
| `Roman(Roman other)` | Copy constructor |

### Static methods

| Method | Description |
|--------|-------------|
| `Parse(int value)` | Parses an `int`; throws on error |
| `Parse(string roman)` | Parses a string (strict); throws on error |
| `Parse(string roman, RomanStyle style)` | Parses with a mode; `Strict` throws `FormatException` on non-canonical input |
| `TryParse(int value, out Roman? result)` | Exception-free `int` parsing |
| `TryParse(string roman, out Roman? result)` | Exception-free string parsing (strict) |
| `TryParse(string roman, RomanStyle style, out Roman? result)` | Exception-free string parsing with a mode |

### Enums

| `RomanStyle` | Description |
|--------------|-------------|
| `Strict` | Strict parsing (default): canonical form only |
| `Lenient` | Lenient parsing: accepts non-canonical forms |

### Instance methods

| Method | Description |
|--------|-------------|
| `ToInt()` | Returns the Arabic value |
| `ToString()` | Returns the canonical Roman string |
| `CompareTo(Roman? other)` | Compares with another value |
| `CompareTo(int other)` | Compares with an integer value |
| `Equals(Roman? other)` | Value equality |
| `Equals(int other)` | Value equality against an integer |
| `GetHashCode()` | Hash code |

### Operators

| Operator | Description |
|----------|-------------|
| `+ - * /` | Arithmetic (integer division), `Roman` or `int` on either side |
| `> < >= <=` | Comparison (null sorts lowest), `Roman` or `int` on either side |
| `== !=` | Equality, `Roman` or `int` on either side |
| `(int)roman` | Explicit conversion `Roman -> int` |
| `(Roman)value` | Explicit conversion `int -> Roman` |

## Performance

The conversion path is allocation-light:

- The `int -> Roman` conversion writes into a `stackalloc Span<char>` buffer — no heap
  allocation beyond the resulting string.
- The type is immutable and wraps a single `int`, so instances are cheap to copy and compare.

## Testing

The library ships with a comprehensive MSTest suite covering constructors, arithmetic,
comparison, parsing (both modes), conversions, and int → Roman → string round-trips.

```bash
dotnet test                                   # run all tests
dotnet test --collect:"XPlat Code Coverage"   # with coverage
```

## Contributing

Issues and pull requests are welcome. When filing a bug, please include the library version,
the .NET version, a minimal reproduction, and the expected vs. actual behavior.

## License

Licensed under the [MIT License](https://github.com/deliciousNesquik/roman/blob/main/LICENSE.md).

## Author

**deliciousNesquik** — [@deliciousNesquik](https://github.com/deliciousNesquik)
