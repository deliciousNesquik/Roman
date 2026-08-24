# Roman

[![NuGet Downloads](https://img.shields.io/nuget/dt/:Roman.svg)](https://www.nuget.org/packages/Roman/)
[![NuGet Version](https://img.shields.io/nuget/v/Roman.svg)](https://www.nuget.org/packages/Roman/)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/deliciousNesquik/roman/blob/main/LICENSE)

Небольшая C#-библиотека без зависимостей для работы с римскими числами: преобразование
в/из арабских чисел, арифметика и сравнение. Иммутабельная, с минимумом аллокаций и полным
покрытием тестами.

**🌐 Язык:** [English](https://github.com/deliciousNesquik/roman/blob/main/README.md) · **Русский**

## Содержание

- [Возможности](#возможности)
- [Установка](#установка)
- [Быстрый старт](#быстрый-старт)
- [Использование](#использование)
  - [Создание значений](#создание-значений)
  - [Преобразования](#преобразования)
  - [Арифметика](#арифметика)
  - [Сравнение](#сравнение)
  - [Режимы парсинга (лояльный / строгий)](#режимы-парсинга-лояльный--строгий)
- [Ограничения](#ограничения)
- [Справочник API](#справочник-api)
- [Производительность](#производительность)
- [Тестирование](#тестирование)
- [Участие в разработке](#участие-в-разработке)
- [Лицензия](#лицензия)

## Возможности

- Преобразование арабских чисел (1–3999) в римские и обратно
- Арифметика: сложение, вычитание, умножение, деление
- Операторы сравнения: `>`, `<`, `>=`, `<=`, `==`, `!=`
- Реализация `IComparable<Roman>` и `IEquatable<Roman>`
- Иммутабельный тип — каждая операция возвращает новое значение
- Чёткая семантика `null` в операторах сравнения
- Регистронезависимый парсинг с обрезкой пробелов
- Строгий (по умолчанию) и лояльный (`RomanStyle.Lenient`) режимы парсинга
- `TryParse` для разбора без исключений
- Преобразование с минимумом аллокаций через `stackalloc Span<char>`
- Без внешних зависимостей

## Установка

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

## Быстрый старт

```csharp
using RomanNumerals;

// Из целого числа
var a = new Roman(42);
Console.WriteLine(a);            // XLII

// Из строки
var b = new Roman("XIX");
Console.WriteLine(b.ToInt());    // 19

// Арифметика
Console.WriteLine(a + b);        // LXI  (61)

// Сравнение
if (a > b)
    Console.WriteLine("42 > 19");

// Парсинг без исключений
if (Roman.TryParse("MCMXCIV", out var year))
    Console.WriteLine(year.ToInt());   // 1994
```

> **Важно:** тип называется `Roman`, namespace — `RomanNumerals`. Добавьте `using RomanNumerals;`
> и используйте `new Roman(...)`.

## Использование

### Создание значений

```csharp
var fromInt    = new Roman(1984);        // MCMLXXXIV
var fromString = new Roman("MMXXIII");   // 2023
var lower      = new Roman("  xlii  ");  // 42 (обрезка пробелов, регистронезависимо)
var copy       = new Roman(fromInt);     // конструктор копирования
```

### Преобразования

```csharp
// Parse — бросает исключение при ошибке
var r1 = Roman.Parse(500);
var r2 = Roman.Parse("D");

// TryParse — никогда не бросает
if (Roman.TryParse(42, out var r3)) { /* ... */ }
if (Roman.TryParse("invalid", out var r4)) { /* не выполнится */ }

// Явное Roman -> int (неявное позволяло смешанным выражениям обходить проверку диапазона)
var value = (int)new Roman(42);   // 42
var same  = new Roman(42).ToInt(); // 42

// Явное int -> Roman (может бросить)
var r5 = (Roman)99;            // XCIX
```

### Арифметика

Операции считаются на нижележащем целом и затем повторно проверяют результат на диапазон
1–3999, бросая `OverflowException` при выходе за верхнюю (> 3999) или нижнюю (< 1) границу.
Деление — целочисленное.

```csharp
new Roman(10) + new Roman(5);   // XV  (15)
new Roman(50) - new Roman(20);  // XXX (30)
new Roman(7)  * new Roman(8);   // LVI (56)
new Roman(20) / new Roman(4);   // V   (5)
new Roman(10) / new Roman(3);   // III (3, целочисленное деление)

new Roman(3999) + new Roman(1); // исключение: результат > 3999
new Roman(5)    - new Roman(5); // исключение: результат < 1
```

Операнд типа `int` работает с любой стороны и проверяется точно так же — он читается как
арабское количество, поэтому представимым должен быть только *результат*:

```csharp
new Roman(10) + 5;              // XV  (15)
50 - new Roman(20);             // XXX (30)
new Roman(5)  + -3;             // II  (2, операнд вне диапазона, результат внутри)
new Roman(3999) + 1;            // бросает OverflowException
new Roman(10) / 0;              // бросает DivideByZeroException
```

Сравнение и равенство с `int` не конструируют `Roman`, поэтому непредставимая граница —
допустимый вопрос:

```csharp
new Roman(3999) < 5000;         // true
new Roman(42) == 42;            // true  (согласовано с .Equals(42))
```

### Сравнение

```csharp
var a = new Roman(50);
var b = new Roman(30);

a > b;            // true
a == b;           // false
a.CompareTo(b);   // > 0
a.Equals(b);      // false
```

Семантика `null` повторяет `Comparer<T>` (null — наименьший):

```csharp
Roman x = new Roman(5);
Roman y = null;

x > y;    // true   (значение больше null)
y < x;    // true   (null меньше любого значения)
x == y;   // false
x != y;   // true

Roman p = null, q = null;
p == q;   // true
p >= q;   // true
```

### Режимы парсинга (строгий / лояльный)

По умолчанию парсинг **строгий** — принимает только каноническую запись и отвергает
неканонические формы, например `"IIII"`. Это относится к конструкторам, `Parse(string)`
и `TryParse(string, …)`:

```csharp
new Roman("IV");    // OK -> 4
new Roman("IIII");  // FormatException: не каноническая запись
```

Если нужен **лояльный** разбор (принимать неканонические формы), передайте `RomanStyle.Lenient`
в перегрузки `Parse` / `TryParse` (по аналогии с `int.Parse(string, NumberStyles)`):

```csharp
var roman = Roman.Parse("IIII", RomanStyle.Lenient);  // разбирается как 4
Console.WriteLine(roman);                             // IV (вывод всегда канонический)

if (Roman.TryParse("IIII", RomanStyle.Lenient, out var r))
    Console.WriteLine(r);                             // IV
```

`RomanStyle.Strict` (значение по умолчанию) отвергает мусорные символы (`ArgumentException`),
значения вне диапазона (`ArgumentOutOfRangeException`) и валидную, но неканоническую запись
(`FormatException`).

Мягкий разбор вычитает каждый символ, который меньше наибольшего символа справа от него, а не
только тот, что стоит перед более крупным соседом. Это даёт чтения, засвидетельствованные в
римских надписях для многосимвольных вычитаний:

```csharp
Roman.Parse("IIX",  RomanStyle.Lenient).ToInt();   // 8
Roman.Parse("XIIX", RomanStyle.Lenient).ToInt();   // 18
Roman.Parse("IIC",  RomanStyle.Lenient).ToInt();   // 98
```

Если вычитания уводят результат ниже 1, разбор бросает `ArgumentOutOfRangeException`, а не
возвращает значение: `Roman.Parse("IIIIIIIIIIX", RomanStyle.Lenient)` дал бы 0. Мягкий режим не
является валидатором — он по-прежнему принимает формы, недопустимые ни в одном соглашении,
например `"IC"` для 99.

## Ограничения

- **Диапазон 1–3999** (`MMMCMXCIX`). Значения вне диапазона бросают
  `ArgumentOutOfRangeException`. Нет представления для `0` и отрицательных чисел.
- **Round-trip сохраняет значение, а не строку.**
  `Roman.Parse("IIII", RomanStyle.Lenient).ToString()` вернёт `"IV"`. Строгий разбор
  (по умолчанию) отвергает неканонический ввод сразу.
- Результаты арифметики должны оставаться в 1–3999, иначе бросается исключение.

## Справочник API

### Конструкторы

| Конструктор | Описание |
|-------------|----------|
| `Roman(int value)` | Создаёт значение из целого числа (1–3999) |
| `Roman(string roman)` | Создаёт значение из строки (строго) |
| `Roman(Roman other)` | Конструктор копирования |

### Статические методы

| Метод | Описание |
|-------|----------|
| `Parse(int value)` | Разбирает `int`; бросает при ошибке |
| `Parse(string roman)` | Разбирает строку (строго); бросает при ошибке |
| `Parse(string roman, RomanStyle style)` | Разбор с режимом; `Strict` бросает `FormatException` на неканонической записи |
| `TryParse(int value, out Roman? result)` | Разбор `int` без исключений |
| `TryParse(string roman, out Roman? result)` | Разбор строки без исключений (строго) |
| `TryParse(string roman, RomanStyle style, out Roman? result)` | Разбор строки без исключений с режимом |

### Перечисления

| `RomanStyle` | Описание |
|--------------|----------|
| `Strict` | Строгий разбор (по умолчанию): только каноническая запись |
| `Lenient` | Лояльный разбор: принимает неканонические формы |

### Методы экземпляра

| Метод | Описание |
|-------|----------|
| `ToInt()` | Возвращает арабское значение |
| `ToString()` | Возвращает каноническую римскую строку |
| `CompareTo(Roman? other)` | Сравнение с другим значением |
| `CompareTo(int other)` | Сравнение с целым значением |
| `Equals(Roman? other)` | Равенство по значению |
| `Equals(int other)` | Равенство по значению с целым |
| `GetHashCode()` | Хеш-код |

### Операторы

| Оператор | Описание |
|----------|----------|
| `+ - * /` | Арифметика (целочисленное деление), `Roman` или `int` с любой стороны |
| `> < >= <=` | Сравнение (null — наименьший), `Roman` или `int` с любой стороны |
| `== !=` | Равенство, `Roman` или `int` с любой стороны |
| `(int)roman` | Явное преобразование `Roman -> int` |
| `(Roman)value` | Явное преобразование `int -> Roman` |

## Производительность

Путь преобразования экономен на аллокациях:

- Преобразование `int -> Roman` пишет в буфер `stackalloc Span<char>` — нет аллокаций в куче,
  кроме итоговой строки.
- Тип иммутабелен и оборачивает один `int`, поэтому экземпляры дёшево копировать и сравнивать.

## Тестирование

Библиотека поставляется с обширным набором тестов на MSTest: конструкторы, арифметика,
сравнение, парсинг (оба режима), преобразования и round-trip int → Roman → string.

```bash
dotnet test                                   # все тесты
dotnet test --collect:"XPlat Code Coverage"   # с покрытием
```

## Участие в разработке

Issues и pull request'ы приветствуются. При баг-репорте укажите версию библиотеки, версию
.NET, минимальный воспроизводимый пример, ожидаемое и фактическое поведение.

## Лицензия

Распространяется под лицензией [MIT](https://github.com/deliciousNesquik/roman/blob/main/LICENSE).

## Автор

**deliciousNesquik** — [@deliciousNesquik](https://github.com/deliciousNesquik)
