namespace Brim.Binaryen;

/// <summary>
/// Represents a WebAssembly function.
/// </summary>
public readonly struct BinaryenFunction
{
  internal readonly IntPtr Handle;

  internal BinaryenFunction(IntPtr handle) => Handle = handle;

  public static implicit operator IntPtr(BinaryenFunction func) => func.Handle;
}

/// <summary>
/// Represents a WebAssembly export.
/// </summary>
public readonly struct BinaryenExport
{
  internal readonly IntPtr Handle;

  internal BinaryenExport(IntPtr handle) => Handle = handle;

  public static implicit operator IntPtr(BinaryenExport export) => export.Handle;
}

/// <summary>
/// Represents a WebAssembly global variable.
/// </summary>
public readonly struct BinaryenGlobal
{
  internal readonly IntPtr Handle;

  internal BinaryenGlobal(IntPtr handle) => Handle = handle;

  public static implicit operator IntPtr(BinaryenGlobal global) => global.Handle;
}
