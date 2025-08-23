using System.Runtime.InteropServices;
using System.Text;
using Brim.Binaryen.Internal;

namespace Brim.Binaryen;

public sealed class BinaryenModule : IDisposable
{
  internal IntPtr _m;
  private ExpressionBuilder? _expressions;

  public BinaryenModule()
  {
    BinaryenLoadGuard.EnsureLoaded();
    _m = Interop.Native.BinaryenModuleCreate();
    if (_m == IntPtr.Zero) throw new BinaryenException("Failed to create module.");
  }

  /// <summary>
  /// Gets the expression builder for creating expressions in this module.
  /// </summary>
  public ExpressionBuilder Expressions {
    get
    {
      ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
      return _expressions ??= new ExpressionBuilder(_m);
    }
  }

  /// <summary>
  /// Validates the module.
  /// </summary>
  /// <returns>True if the module is valid, false otherwise.</returns>
  public bool Validate()
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    return Interop.Native.BinaryenModuleValidate(_m) != 0;
  }

  public void Optimize(OptimizeOptions? options = null)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    OptimizeOptions o = options ?? OptimizeOptions.O2();
    _ = Interop.Native.BinaryenSetOptimizeLevel(o.OptimizeLevel);
    _ = Interop.Native.BinaryenSetShrinkLevel(o.ShrinkLevel);

    if (o.Passes?.Count > 0)
      Interop.Native.BinaryenModuleRunPasses(_m, [.. o.Passes!], (nuint)o.Passes!.Count);
    else
      Interop.Native.BinaryenModuleRunPasses(_m, [], 0);
  }

  /// <summary>
  /// Runs standard optimization passes.
  /// </summary>
  public void OptimizeDefault()
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenModuleOptimize(_m);
  }

  /// <summary>
  /// Runs specific optimization passes.
  /// </summary>
  public void RunPasses(params string[] passes)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenModuleRunPasses(_m, passes, (nuint)passes.Length);
  }

  /// <summary>
  /// Prints the module in text format (for debugging).
  /// </summary>
  public void Print()
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenModulePrint(_m);
  }

  public byte[] WriteBinary()
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nuint size = 0;
    _ = Interop.Native.BinaryenModuleWrite(_m, IntPtr.Zero, ref size); // query size
    nint buf = Marshal.AllocHGlobal((IntPtr)size);
    try
    {
      _ = Interop.Native.BinaryenModuleWrite(_m, buf, ref size);
      byte[] managed = new byte[(int)size];
      Marshal.Copy(buf, managed, 0, (int)size);
      return managed;
    }
    finally
    {
      Marshal.FreeHGlobal(buf);
    }
  }

  /// <summary>
  /// Writes the module in text format.
  /// </summary>
  public string WriteText()
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);

    // Try with a reasonable buffer size first (like the C example)
    const int BufferSize = 8192;
    nint buf = Marshal.AllocHGlobal(BufferSize);
    try
    {
      nuint actualSize = Interop.Native.BinaryenModuleWriteText(_m, buf, BufferSize);
      if (actualSize == 0) return string.Empty;

      // If the result is truncated, we might need a larger buffer
      if (actualSize >= BufferSize)
      {
        // Query the exact size needed
        Marshal.FreeHGlobal(buf);
        nuint size = Interop.Native.BinaryenModuleWriteText(_m, IntPtr.Zero, 0);
        if (size == 0) return string.Empty;

        buf = Marshal.AllocHGlobal((nint)size);
        actualSize = Interop.Native.BinaryenModuleWriteText(_m, buf, size);
      }

      return Marshal.PtrToStringUTF8(buf, (int)actualSize) ?? string.Empty;
    }
    finally
    {
      Marshal.FreeHGlobal(buf);
    }
  }

  // ========== Function Management ==========

  /// <summary>
  /// Adds a function to the module.
  /// </summary>
  public BinaryenFunction AddFunction(string name, UIntPtr paramTypes, UIntPtr resultTypes,
    UIntPtr[] localTypes, BinaryenExpression body)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);

    nint localTypesPtr = IntPtr.Zero;
    if (localTypes.Length > 0)
    {
      localTypesPtr = Marshal.AllocHGlobal(localTypes.Length * IntPtr.Size);
      try
      {
        Marshal.Copy(localTypes.Select(t => (IntPtr)t).ToArray(), 0, localTypesPtr, localTypes.Length);
        nint funcPtr = Interop.Native.BinaryenAddFunction(_m, name, paramTypes, resultTypes,
          localTypesPtr, (uint)localTypes.Length, body.Handle);
        return new BinaryenFunction(funcPtr);
      }
      finally
      {
        Marshal.FreeHGlobal(localTypesPtr);
      }
    }
    else
    {
      nint funcPtr = Interop.Native.BinaryenAddFunction(_m, name, paramTypes, resultTypes,
        IntPtr.Zero, 0, body.Handle);
      return new BinaryenFunction(funcPtr);
    }
  }

  /// <summary>
  /// Adds a function to the module with no local variables.
  /// </summary>
  public BinaryenFunction AddFunction(string name, UIntPtr paramTypes, UIntPtr resultTypes, BinaryenExpression body) => AddFunction(name, paramTypes, resultTypes, [], body);

  /// <summary>
  /// Gets a function by name.
  /// </summary>
  public BinaryenFunction? GetFunction(string name)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint funcPtr = Interop.Native.BinaryenGetFunction(_m, name);
    return funcPtr == IntPtr.Zero ? null : new BinaryenFunction(funcPtr);
  }

  /// <summary>
  /// Removes a function by name.
  /// </summary>
  public void RemoveFunction(string name)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenRemoveFunction(_m, name);
  }

  /// <summary>
  /// Gets the number of functions in the module.
  /// </summary>
  public uint FunctionCount {
    get
    {
      ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
      return Interop.Native.BinaryenGetNumFunctions(_m);
    }
  }

  // ========== Export Management ==========

  /// <summary>
  /// Adds a function export.
  /// </summary>
  public BinaryenExport AddFunctionExport(string internalName, string externalName)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint exportPtr = Interop.Native.BinaryenAddFunctionExport(_m, internalName, externalName);
    return new BinaryenExport(exportPtr);
  }

  /// <summary>
  /// Adds a global export.
  /// </summary>
  public BinaryenExport AddGlobalExport(string internalName, string externalName)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint exportPtr = Interop.Native.BinaryenAddGlobalExport(_m, internalName, externalName);
    return new BinaryenExport(exportPtr);
  }

  /// <summary>
  /// Adds a memory export.
  /// </summary>
  public BinaryenExport AddMemoryExport(string internalName, string externalName)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint exportPtr = Interop.Native.BinaryenAddMemoryExport(_m, internalName, externalName);
    return new BinaryenExport(exportPtr);
  }

  // ========== Global Management ==========

  /// <summary>
  /// Adds a global variable to the module.
  /// </summary>
  public BinaryenGlobal AddGlobal(string name, UIntPtr type, bool mutable, BinaryenExpression init)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint globalPtr = Interop.Native.BinaryenAddGlobal(_m, name, type, mutable, init.Handle);
    return new BinaryenGlobal(globalPtr);
  }

  /// <summary>
  /// Gets a global by name.
  /// </summary>
  public BinaryenGlobal? GetGlobal(string name)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    nint globalPtr = Interop.Native.BinaryenGetGlobal(_m, name);
    return globalPtr == IntPtr.Zero ? null : new BinaryenGlobal(globalPtr);
  }

  /// <summary>
  /// Removes a global by name.
  /// </summary>
  public void RemoveGlobal(string name)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenRemoveGlobal(_m, name);
  }

  // ========== Memory Management ==========

  /// <summary>
  /// Sets up memory for the module.
  /// </summary>
  public void SetMemory(uint initial, uint maximum, string? exportName = null, bool shared = false, bool memory64 = false, string? name = null)
  {
    ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
    Interop.Native.BinaryenSetMemory(_m, initial, maximum, exportName, IntPtr.Zero, IntPtr.Zero,
      IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, shared, memory64, name);
  }

  /// <summary>
  /// Checks if the module has memory defined.
  /// </summary>
  public bool HasMemory {
    get
    {
      ObjectDisposedException.ThrowIf(_m == IntPtr.Zero, this);
      return Interop.Native.BinaryenHasMemory(_m);
    }
  }

  public void Dispose()
  {
    nint m = Interlocked.Exchange(ref _m, IntPtr.Zero);
    if (m != IntPtr.Zero) Interop.Native.BinaryenModuleDispose(m);
    GC.SuppressFinalize(this);
  }
}

