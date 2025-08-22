using System.Runtime.InteropServices;
using Brim.Binaryen.Internal;

namespace Brim.Binaryen;

public sealed class BinaryenModule : IDisposable
{
  private IntPtr _m;

  public BinaryenModule()
  {
    BinaryenLoadGuard.EnsureLoaded();
    _m = Interop.Native.BinaryenModuleCreate();
    if (_m == IntPtr.Zero) throw new BinaryenException("Failed to create module.");
  }

  public void Optimize(OptimizeOptions? options = null)
  {
    OptimizeOptions o = options ?? OptimizeOptions.O2();
    _ = Interop.Native.BinaryenSetOptimizeLevel(o.OptimizeLevel);
    _ = Interop.Native.BinaryenSetShrinkLevel(o.ShrinkLevel);

    if (o.Passes?.Count > 0)
      Interop.Native.BinaryenModuleRunPasses(_m, [.. o.Passes!], (nuint)o.Passes!.Count);
    else
      Interop.Native.BinaryenModuleRunPasses(_m, [], 0);
  }

  public byte[] WriteBinary()
  {
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

  public void Dispose()
  {
    nint m = Interlocked.Exchange(ref _m, IntPtr.Zero);
    if (m != IntPtr.Zero) Interop.Native.BinaryenModuleDispose(m);
    GC.SuppressFinalize(this);
  }
}

