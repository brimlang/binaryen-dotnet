using System.Runtime.InteropServices;

namespace Brim.Binaryen.Unsafe;

/// <summary>
/// SafeHandle for Binaryen function references.
/// </summary>
public sealed class BinaryenFunctionHandle : SafeHandle
{
  public BinaryenFunctionHandle() : base(IntPtr.Zero, ownsHandle: false) { }

  public BinaryenFunctionHandle(IntPtr handle) : base(handle, ownsHandle: false) => SetHandle(handle);

  public override bool IsInvalid => handle == IntPtr.Zero;

  protected override bool ReleaseHandle() =>
    // Functions are owned by the module and don't need explicit disposal
    true;
}

/// <summary>
/// SafeHandle for Binaryen expression references.
/// </summary>
public sealed class BinaryenExpressionHandle : SafeHandle
{
  public BinaryenExpressionHandle() : base(IntPtr.Zero, ownsHandle: false) { }

  public BinaryenExpressionHandle(IntPtr handle) : base(handle, ownsHandle: false) => SetHandle(handle);

  public override bool IsInvalid => handle == IntPtr.Zero;

  protected override bool ReleaseHandle() =>
    // Expressions are owned by the module and don't need explicit disposal
    true;
}

/// <summary>
/// SafeHandle for allocated buffers from Binaryen.
/// </summary>
public sealed class BinaryenBufferHandle : SafeHandle
{
  public BinaryenBufferHandle() : base(IntPtr.Zero, ownsHandle: true) { }

  public override bool IsInvalid => handle == IntPtr.Zero;

  protected override bool ReleaseHandle()
  {
    if (handle != IntPtr.Zero)
    {
      Interop.Native.BinaryenModuleDisposeAllocatedBuffer(handle);
    }
    return true;
  }
}
