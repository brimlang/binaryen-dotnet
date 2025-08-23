namespace Brim.Binaryen;

/// <summary>
/// WebAssembly operations for unary and binary expressions.
/// </summary>
public static class BinaryenOp
{
  // ========== Unary Operations (i32) ==========
  
  /// <summary>i32.clz - count leading zeros</summary>
  public static int ClzInt32 => Interop.Native.BinaryenClzInt32();
  
  /// <summary>i32.ctz - count trailing zeros</summary>
  public static int CtzInt32 => Interop.Native.BinaryenCtzInt32();
  
  /// <summary>i32.popcnt - population count (number of 1 bits)</summary>
  public static int PopcntInt32 => Interop.Native.BinaryenPopcntInt32();
  
  /// <summary>i32.eqz - test if equal to zero</summary>
  public static int EqZInt32 => Interop.Native.BinaryenEqZInt32();
  
  // ========== Unary Operations (i64) ==========
  
  /// <summary>i64.clz - count leading zeros</summary>
  public static int ClzInt64 => Interop.Native.BinaryenClzInt64();
  
  /// <summary>i64.ctz - count trailing zeros</summary>
  public static int CtzInt64 => Interop.Native.BinaryenCtzInt64();
  
  /// <summary>i64.popcnt - population count (number of 1 bits)</summary>
  public static int PopcntInt64 => Interop.Native.BinaryenPopcntInt64();
  
  /// <summary>i64.eqz - test if equal to zero</summary>
  public static int EqZInt64 => Interop.Native.BinaryenEqZInt64();
  
  // ========== Unary Operations (f32) ==========
  
  /// <summary>f32.neg - negate</summary>
  public static int NegFloat32 => Interop.Native.BinaryenNegFloat32();
  
  /// <summary>f32.abs - absolute value</summary>
  public static int AbsFloat32 => Interop.Native.BinaryenAbsFloat32();
  
  /// <summary>f32.sqrt - square root</summary>
  public static int SqrtFloat32 => Interop.Native.BinaryenSqrtFloat32();
  
  // ========== Unary Operations (f64) ==========
  
  /// <summary>f64.neg - negate</summary>
  public static int NegFloat64 => Interop.Native.BinaryenNegFloat64();
  
  /// <summary>f64.abs - absolute value</summary>
  public static int AbsFloat64 => Interop.Native.BinaryenAbsFloat64();
  
  /// <summary>f64.sqrt - square root</summary>
  public static int SqrtFloat64 => Interop.Native.BinaryenSqrtFloat64();
  
  // ========== Binary Operations (i32) ==========
  
  /// <summary>i32.add - addition</summary>
  public static int AddInt32 => Interop.Native.BinaryenAddInt32();
  
  /// <summary>i32.sub - subtraction</summary>
  public static int SubInt32 => Interop.Native.BinaryenSubInt32();
  
  /// <summary>i32.mul - multiplication</summary>
  public static int MulInt32 => Interop.Native.BinaryenMulInt32();
  
  /// <summary>i32.div_s - signed division</summary>
  public static int DivSInt32 => Interop.Native.BinaryenDivSInt32();
  
  /// <summary>i32.div_u - unsigned division</summary>
  public static int DivUInt32 => Interop.Native.BinaryenDivUInt32();
  
  /// <summary>i32.rem_s - signed remainder</summary>
  public static int RemSInt32 => Interop.Native.BinaryenRemSInt32();
  
  /// <summary>i32.rem_u - unsigned remainder</summary>
  public static int RemUInt32 => Interop.Native.BinaryenRemUInt32();
  
  /// <summary>i32.and - bitwise and</summary>
  public static int AndInt32 => Interop.Native.BinaryenAndInt32();
  
  /// <summary>i32.or - bitwise or</summary>
  public static int OrInt32 => Interop.Native.BinaryenOrInt32();
  
  /// <summary>i32.xor - bitwise xor</summary>
  public static int XorInt32 => Interop.Native.BinaryenXorInt32();
  
  /// <summary>i32.shl - shift left</summary>
  public static int ShlInt32 => Interop.Native.BinaryenShlInt32();
  
  /// <summary>i32.shr_u - logical shift right</summary>
  public static int ShrUInt32 => Interop.Native.BinaryenShrUInt32();
  
  /// <summary>i32.shr_s - arithmetic shift right</summary>
  public static int ShrSInt32 => Interop.Native.BinaryenShrSInt32();
  
  /// <summary>i32.eq - equal</summary>
  public static int EqInt32 => Interop.Native.BinaryenEqInt32();
  
  /// <summary>i32.ne - not equal</summary>
  public static int NeInt32 => Interop.Native.BinaryenNeInt32();
  
  /// <summary>i32.lt_s - signed less than</summary>
  public static int LtSInt32 => Interop.Native.BinaryenLtSInt32();
  
  /// <summary>i32.lt_u - unsigned less than</summary>
  public static int LtUInt32 => Interop.Native.BinaryenLtUInt32();
  
  /// <summary>i32.le_s - signed less than or equal</summary>
  public static int LeSInt32 => Interop.Native.BinaryenLeSInt32();
  
  /// <summary>i32.le_u - unsigned less than or equal</summary>
  public static int LeUInt32 => Interop.Native.BinaryenLeUInt32();
  
  /// <summary>i32.gt_s - signed greater than</summary>
  public static int GtSInt32 => Interop.Native.BinaryenGtSInt32();
  
  /// <summary>i32.gt_u - unsigned greater than</summary>
  public static int GtUInt32 => Interop.Native.BinaryenGtUInt32();
  
  /// <summary>i32.ge_s - signed greater than or equal</summary>
  public static int GeSInt32 => Interop.Native.BinaryenGeSInt32();
  
  /// <summary>i32.ge_u - unsigned greater than or equal</summary>
  public static int GeUInt32 => Interop.Native.BinaryenGeUInt32();
  
  // ========== Binary Operations (i64) ==========
  
  /// <summary>i64.add - addition</summary>
  public static int AddInt64 => Interop.Native.BinaryenAddInt64();
  
  /// <summary>i64.sub - subtraction</summary>
  public static int SubInt64 => Interop.Native.BinaryenSubInt64();
  
  /// <summary>i64.mul - multiplication</summary>
  public static int MulInt64 => Interop.Native.BinaryenMulInt64();
  
  /// <summary>i64.div_s - signed division</summary>
  public static int DivSInt64 => Interop.Native.BinaryenDivSInt64();
  
  /// <summary>i64.div_u - unsigned division</summary>
  public static int DivUInt64 => Interop.Native.BinaryenDivUInt64();
  
  // ========== Binary Operations (f32) ==========
  
  /// <summary>f32.add - addition</summary>
  public static int AddFloat32 => Interop.Native.BinaryenAddFloat32();
  
  /// <summary>f32.sub - subtraction</summary>
  public static int SubFloat32 => Interop.Native.BinaryenSubFloat32();
  
  /// <summary>f32.mul - multiplication</summary>
  public static int MulFloat32 => Interop.Native.BinaryenMulFloat32();
  
  /// <summary>f32.div - division</summary>
  public static int DivFloat32 => Interop.Native.BinaryenDivFloat32();
  
  // ========== Binary Operations (f64) ==========
  
  /// <summary>f64.add - addition</summary>
  public static int AddFloat64 => Interop.Native.BinaryenAddFloat64();
  
  /// <summary>f64.sub - subtraction</summary>
  public static int SubFloat64 => Interop.Native.BinaryenSubFloat64();
  
  /// <summary>f64.mul - multiplication</summary>
  public static int MulFloat64 => Interop.Native.BinaryenMulFloat64();
  
  /// <summary>f64.div - division</summary>
  public static int DivFloat64 => Interop.Native.BinaryenDivFloat64();
}