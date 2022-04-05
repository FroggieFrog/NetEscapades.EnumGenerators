﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NetEscapades.EnumGenerators;
using NetEscapades.EnumGenerators.Benchmarks;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

[EnumExtensions]
public enum TestEnum
{
    First = 0,

    [Display(Name = "2nd")]
    Second = 1,
    Third = 2,
}

[MemoryDiagnoser]
public class ToStringBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToString()
    {
        return _enum.ToString();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToStringDisplayNameWithReflection()
    {
        return EnumHelper<TestEnum>.GetDisplayName(_enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string ToStringFast()
    {
        return _enum.ToStringFast();
    }
}

[MemoryDiagnoser]
public class IsDefinedBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedName()
    {
        return Enum.IsDefined(typeof(TestEnum), _enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedName()
    {
        return TestEnumExtensions.IsDefined(_enum);
    }
}

[MemoryDiagnoser]
public class IsDefinedNameBenchmark
{
    private static readonly string _enum = nameof(TestEnum.Second);
    private static readonly string _enumDisplaName = "2nd";

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined()
    {
        return Enum.IsDefined(typeof(TestEnum), _enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
    {
        return TestEnumExtensions.IsDefined(_enum);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection()
    {
        return EnumHelper<TestEnum>.TryParseByDisplayName(_enumDisplaName, out _);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedNameDisplayName()
    {
        return TestEnumExtensions.IsDefined(_enumDisplaName, allowMatchingDisplayAttribute: true);
    }
}

[MemoryDiagnoser]
public class IsDefinedNameFromSpanBenchmark
{
    private static readonly char[] _enum = new char[] { 'S', 'e', 'c', 'o', 'n', 'd' };
    private static readonly char[] _enumDisplayName = new char[] { '2', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return Enum.IsDefined(typeof(TestEnum), _enumAsSpan.ToString());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefinedNameDisplayNameWithReflection()
    {
        ReadOnlySpan<char> _enumAsSpan = _enumDisplayName;
        return EnumHelper<TestEnum>.TryParseByDisplayName(_enumAsSpan.ToString(), out _);
    }


    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefined()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return TestEnumExtensions.IsDefined(_enumAsSpan.ToString());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedSpan()
    {
        return TestEnumExtensions.IsDefined(_enum.AsSpan());
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool ExtensionsIsDefinedDisplayNameSpan()
    {
        return TestEnumExtensions.IsDefined(_enumDisplayName.AsSpan(), allowMatchingDisplayAttribute: true);
    }
}

[MemoryDiagnoser]
public class GetValuesBenchmark
{
#if NETFRAMEWORK
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues()
    {
        return (TestEnum[])Enum.GetValues(typeof(TestEnum));
    }
#else
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues()
    {
        return Enum.GetValues<TestEnum>();
    }
#endif

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] ExtensionsGetValues()
    {
        return TestEnumExtensions.GetValues();
    }
}

[MemoryDiagnoser]
public class GetNamesBenchmark
{
#if NETFRAMEWORK
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames()
    {
        return Enum.GetNames(typeof(TestEnum));
    }
#else
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames()
    {
        return Enum.GetNames<TestEnum>();
    }
#endif

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] ExtensionsGetNames()
    {
        return TestEnumExtensions.GetNames();
    }
}

[MemoryDiagnoser]
public class TryParseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse()
    {
        return Enum.TryParse("Second", ignoreCase: false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse()
    {
        return TestEnumExtensions.TryParse("Second", ignoreCase: false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection()
    {
        return EnumHelper<TestEnum>.TryParseByDisplayName("2nd", out TestEnum result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName()
    {
        return TestEnumExtensions.TryParse("2nd", ignoreCase: false, out TestEnum result, allowMatchingDisplayAttribute: true)
            ? result
            : default;
    }
}

[MemoryDiagnoser]
public class TryParseFromSpanBenchmark
{
    private static readonly char[] _enum = new char[] { 'S', 'e', 'c', 'o', 'n', 'd' };
    private static readonly char[] _enumDisplayName = new char[] { '2', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return Enum.TryParse(_enumAsSpan.ToString(), ignoreCase: false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParse()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return TestEnumExtensions.TryParse(_enumAsSpan.ToString(), ignoreCase: false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseSpan()
    {
        return TestEnumExtensions.TryParse(_enum.AsSpan(), ignoreCase: false, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseDisplayNameWithReflection()
    {
        ReadOnlySpan<char> _enumAsSpan = _enumDisplayName;
        return EnumHelper<TestEnum>.TryParseByDisplayName(_enumAsSpan.ToString(), out TestEnum result) ? result : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayName()
    {
        ReadOnlySpan<char> _enumAsSpan = _enumDisplayName;
        return TestEnumExtensions.TryParse(_enumAsSpan.ToString(), ignoreCase: false, out TestEnum result, allowMatchingDisplayAttribute: true)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseDisplayNameSpan()
    {
        return TestEnumExtensions.TryParse(_enumDisplayName.AsSpan(), ignoreCase: false, out TestEnum result, allowMatchingDisplayAttribute: true)
            ? result
            : default;
    }
}

[MemoryDiagnoser]
public class TryParseIgnoreCaseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase()
    {
        return Enum.TryParse("second", ignoreCase: true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase()
    {
        return TestEnumExtensions.TryParse("second", ignoreCase: true, out TestEnum result)
            ? result
            : default;
    }
}


[MemoryDiagnoser]
public class TryParseIgnoreCaseFromSpanBenchmark
{
    private static readonly char[] _enum = new char[] { 's', 'e', 'c', 'o', 'n', 'd' };

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParseIgnoreCase()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return Enum.TryParse(_enumAsSpan.ToString(), ignoreCase: true, out TestEnum result)
            ? result
            : default;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCase()
    {
        ReadOnlySpan<char> _enumAsSpan = _enum;
        return TestEnumExtensions.TryParse(_enumAsSpan.ToString(), ignoreCase: true, out TestEnum result)
            ? result
            : default;
    }


    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ExtensionsTryParseIgnoreCaseSpan()
    {
        return TestEnumExtensions.TryParse(_enum.AsSpan(), ignoreCase: true, out TestEnum result)
            ? result
            : default;
    }
}