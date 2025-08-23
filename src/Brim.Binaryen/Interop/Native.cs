using System.Runtime.InteropServices;

namespace Brim.Binaryen.Interop;

internal static partial class Native
{
  // ========== Basic Types ==========

  // Core types
  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeNone();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeInt32();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeInt64();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeFloat32();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeFloat64();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeVec128();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeFuncref();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeExternref();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeAnyref();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeUnreachable();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeAuto();

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenTypeCreate(IntPtr valueTypes, uint numTypes);

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenTypeArity(UIntPtr t);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenTypeExpand(UIntPtr t, IntPtr buf);

  // External kinds
  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExternalFunction();

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExternalTable();

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExternalMemory();

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExternalGlobal();

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExternalTag();

  // ========== Module Operations ==========

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenModuleCreate();

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModuleDispose(IntPtr m);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenModuleValidate(IntPtr m);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModuleOptimize(IntPtr module);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModulePrint(IntPtr module);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenModuleRunPasses(
      IntPtr module, string[] passes, nuint numPasses);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSetOptimizeLevel(int level);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSetShrinkLevel(int level);

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGetOptimizeLevel();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGetShrinkLevel();

  // Writers
  [LibraryImport("binaryen")]
  internal static partial nuint BinaryenModuleWrite(IntPtr m, IntPtr buffer, ref nuint size);

  [LibraryImport("binaryen")]
  internal static partial nuint BinaryenModuleAllocateAndWrite(IntPtr m, out IntPtr buffer);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenModuleDisposeAllocatedBuffer(IntPtr buffer);

  [LibraryImport("binaryen")]
  internal static partial nuint BinaryenModuleWriteText(IntPtr module, IntPtr output, nuint outputSize);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenModuleAllocateAndWriteText(IntPtr module);

  // ========== Literals ==========

  [StructLayout(LayoutKind.Explicit)]
  internal struct BinaryenLiteral
  {
    [FieldOffset(0)]
    internal UIntPtr type;

    [FieldOffset(8)]
    internal int i32;

    [FieldOffset(8)]
    internal long i64;

    [FieldOffset(8)]
    internal float f32;

    [FieldOffset(8)]
    internal double f64;

    [FieldOffset(8)]
    internal unsafe fixed byte v128[16];

    [FieldOffset(8)]
    internal IntPtr func;
  }

  [LibraryImport("binaryen")]
  internal static partial BinaryenLiteral BinaryenLiteralInt32(int x);

  [LibraryImport("binaryen")]
  internal static partial BinaryenLiteral BinaryenLiteralInt64(long x);

  [LibraryImport("binaryen")]
  internal static partial BinaryenLiteral BinaryenLiteralFloat32(float x);

  [LibraryImport("binaryen")]
  internal static partial BinaryenLiteral BinaryenLiteralFloat64(double x);

  [LibraryImport("binaryen")]
  internal static partial BinaryenLiteral BinaryenLiteralVec128(IntPtr x);

  // ========== Basic Operations ==========

  // Unary operations
  [LibraryImport("binaryen")]
  internal static partial int BinaryenClzInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenCtzInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenPopcntInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenNegFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenAbsFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSqrtFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenEqZInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenClzInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenCtzInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenPopcntInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenNegFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenAbsFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSqrtFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenEqZInt64();

  // Binary operations
  [LibraryImport("binaryen")]
  internal static partial int BinaryenAddInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSubInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenMulInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenRemSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenRemUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenAndInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenOrInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenXorInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenShlInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenShrUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenShrSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenEqInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenNeInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenLtSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenLtUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenLeSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenLeUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGtSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGtUInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGeSInt32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenGeUInt32();

  // 64-bit binary operations
  [LibraryImport("binaryen")]
  internal static partial int BinaryenAddInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSubInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenMulInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivSInt64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivUInt64();

  // Float operations
  [LibraryImport("binaryen")]
  internal static partial int BinaryenAddFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSubFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenMulFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivFloat32();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenAddFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenSubFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenMulFloat64();

  [LibraryImport("binaryen")]
  internal static partial int BinaryenDivFloat64();

  // ========== Expression Creation ==========

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenBlock(IntPtr module, string? name,
    IntPtr[] children, uint numChildren, UIntPtr type);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenIf(IntPtr module, IntPtr condition,
    IntPtr ifTrue, IntPtr ifFalse);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenLoop(IntPtr module, string? name, IntPtr body);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenBreak(IntPtr module, string name,
    IntPtr condition, IntPtr value);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenCall(IntPtr module, string target,
    IntPtr[] operands, uint numOperands, UIntPtr returnType);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenLocalGet(IntPtr module, uint index, UIntPtr type);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenLocalSet(IntPtr module, uint index, IntPtr value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenLocalTee(IntPtr module, uint index, IntPtr value, UIntPtr type);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenGlobalGet(IntPtr module, string name, UIntPtr type);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenGlobalSet(IntPtr module, string name, IntPtr value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenConst(IntPtr module, BinaryenLiteral value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenUnary(IntPtr module, int op, IntPtr value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenBinary(IntPtr module, int op, IntPtr left, IntPtr right);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenSelect(IntPtr module, IntPtr condition,
    IntPtr ifTrue, IntPtr ifFalse);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenDrop(IntPtr module, IntPtr value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenReturn(IntPtr module, IntPtr value);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenNop(IntPtr module);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenUnreachable(IntPtr module);

  // ========== Function Management ==========

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddFunction(IntPtr module, string name,
    UIntPtr paramTypes, UIntPtr resultTypes, IntPtr varTypes, uint numVarTypes, IntPtr body);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenGetFunction(IntPtr module, string name);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenRemoveFunction(IntPtr module, string name);

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenGetNumFunctions(IntPtr module);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenGetFunctionByIndex(IntPtr module, uint index);

  // ========== Export Management ==========

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddFunctionExport(IntPtr module,
    string internalName, string externalName);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddTableExport(IntPtr module,
    string internalName, string externalName);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddMemoryExport(IntPtr module,
    string internalName, string externalName);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddGlobalExport(IntPtr module,
    string internalName, string externalName);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenGetExport(IntPtr module, string externalName);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenRemoveExport(IntPtr module, string externalName);

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenGetNumExports(IntPtr module);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenGetExportByIndex(IntPtr module, uint index);

  // ========== Global Management ==========

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenAddGlobal(IntPtr module, string name,
    UIntPtr type, [MarshalAs(UnmanagedType.Bool)] bool mutable, IntPtr init);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenGetGlobal(IntPtr module, string name);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenRemoveGlobal(IntPtr module, string name);

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenGetNumGlobals(IntPtr module);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenGetGlobalByIndex(IntPtr module, uint index);

  // ========== Memory Management ==========

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial void BinaryenSetMemory(IntPtr module, uint initial, uint maximum,
    string? exportName, IntPtr segmentNames, IntPtr segmentDatas, IntPtr segmentPassives,
    IntPtr segmentOffsets, IntPtr segmentSizes, uint numSegments,
    [MarshalAs(UnmanagedType.Bool)] bool shared, [MarshalAs(UnmanagedType.Bool)] bool memory64, string? name);

  [LibraryImport("binaryen")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static partial bool BinaryenHasMemory(IntPtr module);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenMemorySize(IntPtr module, string? memoryName, [MarshalAs(UnmanagedType.Bool)] bool memoryIs64);

  [LibraryImport("binaryen", StringMarshalling = StringMarshalling.Utf8)]
  internal static partial IntPtr BinaryenMemoryGrow(IntPtr module, IntPtr delta,
    string? memoryName, [MarshalAs(UnmanagedType.Bool)] bool memoryIs64);

  // ========== Expression Introspection ==========

  [LibraryImport("binaryen")]
  internal static partial uint BinaryenExpressionGetId(IntPtr expr);

  [LibraryImport("binaryen")]
  internal static partial UIntPtr BinaryenExpressionGetType(IntPtr expr);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenExpressionSetType(IntPtr expr, UIntPtr type);

  [LibraryImport("binaryen")]
  internal static partial void BinaryenExpressionPrint(IntPtr expr);

  [LibraryImport("binaryen")]
  internal static partial IntPtr BinaryenExpressionCopy(IntPtr expr, IntPtr module);
}

