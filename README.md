# Roman

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![NuGet](https://img.shields.io/nuget/v/Roman.svg)](https://www.nuget.org/packages/Roman/)
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
  - [Parsing modes (lenient / strict)](#parsing-modes-lenient--strict)
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
- Lenient (default) and strict (`RomanStyle.Strict`) parsing modes
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
<PackageReference Include="Roman" Version="1.1.1" />
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

// TryParse — never throws
if (Roman.TryParse(42, out var r3)) { /* ... */ }
if (Roman.TryParse("invalid", out var r4)) { /* not reached */ }

// Implicit Roman -> int
int value = new Roman(42);     // 42

// Explicit int -> Roman (can throw)
var r5 = (Roman)99;            // XCIX
```

### Arithmetic

Operations compute on the underlying integer and re-validate the result against the 1–3999
range, throwing `ArgumentOutOfRangeException` on overflow/underflow. Division is integer
division.

```csharp
new Roman(10) + new Roman(5);   // XV  (15)
new Roman(50) - new Roman(20);  // XXX (30)
new Roman(7)  * new Roman(8);   // LVI (56)
new Roman(20) / new Roman(4);   // V   (5)
new Roman(10) / new Roman(3);   // III (3, integer division)

new Roman(3999) + new Roman(1); // throws: result > 3999
new Roman(5)    - new Roman(5); // throws: result < 1
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

### Parsing modes (lenient / strict)

By default parsing is **lenient** — it accepts non-canonical forms such as `"IIII"` for `4`.
This applies to the constructors, `Parse(string)` and `TryParse(string, …)`:

```csharp
var roman = new Roman("IIII");  // parses as 4
Console.WriteLine(roman);       // IV (output is always canonical)
```

For **strict** validation (canonical form only), pass `RomanStyle.Strict` to the `Parse` /
`TryParse` overloads — modeled after `int.Parse(string, NumberStyles)`:

```csharp
Roman.Parse("IV", RomanStyle.Strict);    // OK -> 4
Roman.Parse("IIII", RomanStyle.Strict);  // FormatException: not canonical

if (Roman.TryParse("IIII", RomanStyle.Strict, out var r))
    Console.WriteLine(r);
else
    Console.WriteLine("Non-canonical form!");
```

`RomanStyle.Lenient` (the default for the overloads) reproduces the lenient behavior. Strict
mode still rejects garbage characters (`ArgumentException`) and out-of-range values
(`ArgumentOutOfRangeException`); an otherwise valid but non-canonical form yields
`FormatException`.

## Limitations

- **Range is 1–3999** (`MMMCMXCIX`). Values outside this range throw
  `ArgumentOutOfRangeException`. There is no representation for `0` or negatives.
- **Round-trip is value-preserving, not string-preserving.** `new Roman("IIII").ToString()`
  returns `"IV"`. Use `RomanStyle.Strict` if you need to reject non-canonical input.
- Arithmetic results must stay within 1–3999, otherwise they throw.

## API reference

### Constructors

| Constructor | Description |
|-------------|-------------|
| `Roman(int value)` | Creates a value from an integer (1–3999) |
| `Roman(string roman)` | Creates a value from a string (lenient) |
| `Roman(Roman other)` | Copy constructor |

### Static methods

| Method | Description |
|--------|-------------|
| `Parse(int value)` | Parses an `int`; throws on error |
| `Parse(string roman)` | Parses a string (lenient); throws on error |
| `Parse(string roman, RomanStyle style)` | Parses with a mode; `Strict` throws `FormatException` on non-canonical input |
| `TryParse(int value, out Roman? result)` | Exception-free `int` parsing |
| `TryParse(string roman, out Roman? result)` | Exception-free string parsing (lenient) |
| `TryParse(string roman, RomanStyle style, out Roman? result)` | Exception-free string parsing with a mode |

### Enums

| `RomanStyle` | Description |
|--------------|-------------|
| `Lenient` | Lenient parsing (default): accepts non-canonical forms |
| `Strict` | Strict parsing: canonical form only |

### Instance methods

| Method | Description |
|--------|-------------|
| `ToInt()` | Returns the Arabic value |
| `ToString()` | Returns the canonical Roman string |
| `CompareTo(Roman? other)` | Compares with another value |
| `Equals(Roman? other)` | Value equality |
| `GetHashCode()` | Hash code |

### Operators

| Operator | Description |
|----------|-------------|
| `+ - * /` | Arithmetic (integer division) |
| `> < >= <=` | Comparison (null sorts lowest) |
| `== !=` | Equality |
| `(int)roman` | Implicit conversion `Roman -> int` |
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

Licensed under the [MIT License](https://github.com/deliciousNesquik/roman/blob/main/LICENSE).

## Author

**deliciousNesquik** — [@deliciousNesquik](https://github.com/deliciousNesquik)
