using System.Runtime.InteropServices;

namespace Brim.Binaryen.Unsafe;

public sealed class BinaryenModuleHandle : SafeHandle
{
  public BinaryenModuleHandle() : base(IntPtr.Zero, ownsHandle: true) { }

  public override bool IsInvalid => handle == IntPtr.Zero;

  protected override bool ReleaseHandle()
  {
    Interop.Native.BinaryenModuleDispose(handle);
    return true;
  }
}

