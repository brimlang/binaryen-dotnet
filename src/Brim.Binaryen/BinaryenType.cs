namespace Brim.Binaryen;

/// <summary>
/// WebAssembly value types.
/// </summary>
public static class BinaryenType
{
  /// <summary>Gets the 'none' type (void).</summary>
  public static UIntPtr None => Interop.Native.BinaryenTypeNone();
  
  /// <summary>Gets the i32 type.</summary>
  public static UIntPtr Int32 => Interop.Native.BinaryenTypeInt32();
  
  /// <summary>Gets the i64 type.</summary>
  public static UIntPtr Int64 => Interop.Native.BinaryenTypeInt64();
  
  /// <summary>Gets the f32 type.</summary>
  public static UIntPtr Float32 => Interop.Native.BinaryenTypeFloat32();
  
  /// <summary>Gets the f64 type.</summary>
  public static UIntPtr Float64 => Interop.Native.BinaryenTypeFloat64();
  
  /// <summary>Gets the v128 type.</summary>
  public static UIntPtr Vec128 => Interop.Native.BinaryenTypeVec128();
  
  /// <summary>Gets the funcref type.</summary>
  public static UIntPtr Funcref => Interop.Native.BinaryenTypeFuncref();
  
  /// <summary>Gets the externref type.</summary>
  public static UIntPtr Externref => Interop.Native.BinaryenTypeExternref();
  
  /// <summary>Gets the anyref type.</summary>
  public static UIntPtr Anyref => Interop.Native.BinaryenTypeAnyref();
  
  /// <summary>Gets the unreachable type.</summary>
  public static UIntPtr Unreachable => Interop.Native.BinaryenTypeUnreachable();
  
  /// <summary>Gets the auto type (for automatic type inference).</summary>
  public static UIntPtr Auto => Interop.Native.BinaryenTypeAuto();
  
  /// <summary>Creates a tuple type from the given component types.</summary>
  /// <param name="types">The component types.</param>
  /// <returns>A tuple type.</returns>
  public static unsafe UIntPtr Create(params UIntPtr[] types)
  {
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
    return Interop.Native.BinaryenTypeArity(type);
  }
}