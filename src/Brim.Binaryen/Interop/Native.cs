using System.Runtime.InteropServices;

namespace Brim.Binaryen.Interop;

internal static partial class Native
{
  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenModuleCreate();

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModuleDispose(IntPtr m);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenModuleValidate(IntPtr m);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenModuleRunPasses(
      IntPtr module, string[] passes, nuint numPasses);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSetOptimizeLevel(int level);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSetShrinkLevel(int level);

  // Writers
  [LibraryImport("binaryen")]
  internal static partial nuint BinaryenModuleWrite(IntPtr m, IntPtr buffer, ref nuint size);

  [LibraryImport("binaryen")]
  internal static partial nuint BinaryenModuleAllocateAndWrite(IntPtr m, out IntPtr buffer);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModuleDisposeAllocatedBuffer(IntPtr buffer);
}

