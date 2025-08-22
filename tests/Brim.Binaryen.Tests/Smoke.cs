using Brim.Binaryen.Interop;

namespace Brim.Binaryen.Tests;


public class Smoke
{
  [Fact]
  public void Can_Create_And_Dispose_Module()
  {
    nint m = Native.BinaryenModuleCreate();
    Assert.NotEqual(IntPtr.Zero, m);
    int ok = Native.BinaryenModuleValidate(m);
    Assert.True(ok is 1 or 0); // just touch the API
    Native.BinaryenModuleDispose(m);
  }
}

