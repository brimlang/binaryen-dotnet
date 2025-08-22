namespace Brim.Binaryen;

/// <summary>
/// Optimization settings for Binaryen. Mirrors the CLI concepts:
/// - OptimizeLevel: 0..3 (aka -O0..-O3)
/// - ShrinkLevel:   0..2 (0 = speed, 1 = -Os, 2 = -Oz)
/// - Passes:        optional explicit pass list (e.g., "dce", "inlining-optimizing")
/// </summary>
public sealed class OptimizeOptions
{
  /// <summary>0..3 (maps to BinaryenSetOptimizeLevel)</summary>
  public int OptimizeLevel { get; init; } = 2;

  /// <summary>0..2 (maps to BinaryenSetShrinkLevel). 1 ≈ -Os, 2 ≈ -Oz.</summary>
  public int ShrinkLevel { get; init; }

  /// <summary>Optional explicit pass list. If empty, Binaryen will use the level presets.</summary>
  public List<string> Passes { get; set; } = [];

  /// <summary>Add a single pass (returns this for chaining).</summary>
  public OptimizeOptions AddPass(string pass)
  {
    if (string.IsNullOrWhiteSpace(pass)) throw new ArgumentException("Pass name must be non-empty.", nameof(pass));
    Passes.Add(pass);
    return this;
  }

  /// <summary>Add multiple passes (returns this for chaining).</summary>
  public OptimizeOptions AddPasses(IEnumerable<string> passes)
  {
    foreach (string p in passes) _ = AddPass(p);
    return this;
  }

  /// <summary>Clamp levels into supported ranges; call before invoking native APIs.</summary>
  public OptimizeOptions Normalize()
  {
    return new OptimizeOptions
    {
      OptimizeLevel = Clamp(OptimizeLevel, 0, 3),
      ShrinkLevel = Clamp(ShrinkLevel, 0, 2),
      // Create a new list copy to keep this instance immutable-ish after Normalize
      Passes = [.. Passes],
    };

    static int Clamp(int v, int lo, int hi) => v < lo ? lo : (v > hi ? hi : v);
  }

  // Presets — mirrors common CLI switches

  public static OptimizeOptions O0() => new() { OptimizeLevel = 0, ShrinkLevel = 0 };
  public static OptimizeOptions O1() => new() { OptimizeLevel = 1, ShrinkLevel = 0 };
  public static OptimizeOptions O2() => new() { OptimizeLevel = 2, ShrinkLevel = 0 };
  public static OptimizeOptions O3() => new() { OptimizeLevel = 3, ShrinkLevel = 0 };

  /// <summary>Size-focused (~ -Os): moderate speed opts, prefer smaller code.</summary>
  public static OptimizeOptions Os() => new() { OptimizeLevel = 2, ShrinkLevel = 1 };

  /// <summary>Smallest size (~ -Oz): more aggressive size reductions.</summary>
  public static OptimizeOptions Oz() => new() { OptimizeLevel = 2, ShrinkLevel = 2 };
}

