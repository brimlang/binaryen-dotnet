using Brim.Binaryen.Internal;

namespace Brim.Binaryen;

/// <summary>
/// WebAssembly value types.
/// </summary>
public static class BinaryenType
{
  /// <summary>Gets the 'none' type (void).</summary>
  public static UIntPtr None {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeNone();
    }
  }

  /// <summary>Gets the i32 type.</summary>
  public static UIntPtr Int32 {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeInt32();
    }
  }

  /// <summary>Gets the i64 type.</summary>
  public static UIntPtr Int64 {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeInt64();
    }
  }

  /// <summary>Gets the f32 type.</summary>
  public static UIntPtr Float32 {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeFloat32();
    }
  }

  /// <summary>Gets the f64 type.</summary>
  public static UIntPtr Float64 {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeFloat64();
    }
  }

  /// <summary>Gets the v128 type.</summary>
  public static UIntPtr Vec128 {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeVec128();
    }
  }

  /// <summary>Gets the funcref type.</summary>
  public static UIntPtr Funcref {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeFuncref();
    }
  }

  /// <summary>Gets the externref type.</summary>
  public static UIntPtr Externref {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeExternref();
    }
  }

  /// <summary>Gets the anyref type.</summary>
  public static UIntPtr Anyref {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeAnyref();
    }
  }

  /// <summary>Gets the unreachable type.</summary>
  public static UIntPtr Unreachable {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeUnreachable();
    }
  }

  /// <summary>Gets the auto type (for automatic type inference).</summary>
  public static UIntPtr Auto {
    get
    {
      BinaryenLoadGuard.EnsureLoaded();
      return Interop.Native.BinaryenTypeAuto();
    }
  }

  /// <summary>Creates a tuple type from the given component types.</summary>
  /// <param name="types">The component types.</param>
  /// <returns>A tuple type.</returns>
  public static unsafe UIntPtr Create(params UIntPtr[] types)
  {
    BinaryenLoadGuard.EnsureLoaded();
    fixed (UIntPtr* ptr = types)
    {
      return Interop.Native.BinaryenTypeCreate((IntPtr)ptr, (uint)types.Length);
    }
  }

  /// <summary>Gets the arity (number of components) of a type.</summary>
  /// <param name="type">The type.</param>
  /// <returns>The number of components.</returns>
  public static uint GetArity(UIntPtr type)
  {
    BinaryenLoadGuard.EnsureLoaded();
    return Interop.Native.BinaryenTypeArity(type);
  }
}
