namespace Brim.Binaryen;

/// <summary>
/// Represents a WebAssembly expression.
/// </summary>
public readonly struct BinaryenExpression
{
  internal readonly IntPtr Handle;

  internal BinaryenExpression(IntPtr handle) => Handle = handle;

  /// <summary>
  /// Gets the expression ID (type of expression).
  /// </summary>
  public uint GetId() => Interop.Native.BinaryenExpressionGetId(Handle);

  /// <summary>
  /// Gets the type of this expression.
  /// </summary>
  public UIntPtr GetExpressionType() => Interop.Native.BinaryenExpressionGetType(Handle);

  /// <summary>
  /// Sets the type of this expression.
  /// </summary>
  public void SetType(UIntPtr type) => Interop.Native.BinaryenExpressionSetType(Handle, type);

  /// <summary>
  /// Prints this expression to stdout (for debugging).
  /// </summary>
  public void Print() => Interop.Native.BinaryenExpressionPrint(Handle);

  /// <summary>
  /// Creates a deep copy of this expression in the given module.
  /// </summary>
  public BinaryenExpression Copy(BinaryenModule module)
  {
    nint copied = Interop.Native.BinaryenExpressionCopy(Handle, module._m);
    return new BinaryenExpression(copied);
  }

  public static implicit operator IntPtr(BinaryenExpression expr) => expr.Handle;
}
