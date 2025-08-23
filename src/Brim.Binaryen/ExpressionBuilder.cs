using System.Runtime.InteropServices;

namespace Brim.Binaryen;

/// <summary>
/// Provides a high-level interface for building WebAssembly expressions.
/// </summary>
public sealed class ExpressionBuilder
{
  private readonly IntPtr _module;

  internal ExpressionBuilder(IntPtr module) => _module = module;

  // ========== Constants ==========

  /// <summary>Creates an i32.const expression.</summary>
  public BinaryenExpression I32Const(int value)
  {
    Interop.Native.BinaryenLiteral literal = Interop.Native.BinaryenLiteralInt32(value);
    nint expr = Interop.Native.BinaryenConst(_module, literal);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an i64.const expression.</summary>
  public BinaryenExpression I64Const(long value)
  {
    Interop.Native.BinaryenLiteral literal = Interop.Native.BinaryenLiteralInt64(value);
    nint expr = Interop.Native.BinaryenConst(_module, literal);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an f32.const expression.</summary>
  public BinaryenExpression F32Const(float value)
  {
    Interop.Native.BinaryenLiteral literal = Interop.Native.BinaryenLiteralFloat32(value);
    nint expr = Interop.Native.BinaryenConst(_module, literal);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an f64.const expression.</summary>
  public BinaryenExpression F64Const(double value)
  {
    Interop.Native.BinaryenLiteral literal = Interop.Native.BinaryenLiteralFloat64(value);
    nint expr = Interop.Native.BinaryenConst(_module, literal);
    return new BinaryenExpression(expr);
  }

  // ========== Local Operations ==========

  /// <summary>Creates a local.get expression.</summary>
  public BinaryenExpression LocalGet(uint index, UIntPtr type)
  {
    nint expr = Interop.Native.BinaryenLocalGet(_module, index, type);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a local.set expression.</summary>
  public BinaryenExpression LocalSet(uint index, BinaryenExpression value)
  {
    nint expr = Interop.Native.BinaryenLocalSet(_module, index, value.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a local.tee expression.</summary>
  public BinaryenExpression LocalTee(uint index, BinaryenExpression value, UIntPtr type)
  {
    nint expr = Interop.Native.BinaryenLocalTee(_module, index, value.Handle, type);
    return new BinaryenExpression(expr);
  }

  // ========== Global Operations ==========

  /// <summary>Creates a global.get expression.</summary>
  public BinaryenExpression GlobalGet(string name, UIntPtr type)
  {
    nint expr = Interop.Native.BinaryenGlobalGet(_module, name, type);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a global.set expression.</summary>
  public BinaryenExpression GlobalSet(string name, BinaryenExpression value)
  {
    nint expr = Interop.Native.BinaryenGlobalSet(_module, name, value.Handle);
    return new BinaryenExpression(expr);
  }

  // ========== Unary Operations ==========

  /// <summary>Creates a unary expression.</summary>
  public BinaryenExpression Unary(int op, BinaryenExpression value)
  {
    nint expr = Interop.Native.BinaryenUnary(_module, op, value.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an i32.clz expression.</summary>
  public BinaryenExpression I32Clz(BinaryenExpression value) =>
    Unary(BinaryenOp.ClzInt32, value);

  /// <summary>Creates an i32.ctz expression.</summary>
  public BinaryenExpression I32Ctz(BinaryenExpression value) =>
    Unary(BinaryenOp.CtzInt32, value);

  /// <summary>Creates an i32.popcnt expression.</summary>
  public BinaryenExpression I32Popcnt(BinaryenExpression value) =>
    Unary(BinaryenOp.PopcntInt32, value);

  /// <summary>Creates an i32.eqz expression.</summary>
  public BinaryenExpression I32Eqz(BinaryenExpression value) =>
    Unary(BinaryenOp.EqZInt32, value);

  /// <summary>Creates an f32.neg expression.</summary>
  public BinaryenExpression F32Neg(BinaryenExpression value) =>
    Unary(BinaryenOp.NegFloat32, value);

  /// <summary>Creates an f32.abs expression.</summary>
  public BinaryenExpression F32Abs(BinaryenExpression value) =>
    Unary(BinaryenOp.AbsFloat32, value);

  /// <summary>Creates an f32.sqrt expression.</summary>
  public BinaryenExpression F32Sqrt(BinaryenExpression value) =>
    Unary(BinaryenOp.SqrtFloat32, value);

  // ========== Binary Operations ==========

  /// <summary>Creates a binary expression.</summary>
  public BinaryenExpression Binary(int op, BinaryenExpression left, BinaryenExpression right)
  {
    nint expr = Interop.Native.BinaryenBinary(_module, op, left.Handle, right.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an i32.add expression.</summary>
  public BinaryenExpression I32Add(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.AddInt32, left, right);

  /// <summary>Creates an i32.sub expression.</summary>
  public BinaryenExpression I32Sub(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.SubInt32, left, right);

  /// <summary>Creates an i32.mul expression.</summary>
  public BinaryenExpression I32Mul(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.MulInt32, left, right);

  /// <summary>Creates an i32.div_s expression.</summary>
  public BinaryenExpression I32DivS(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.DivSInt32, left, right);

  /// <summary>Creates an i32.eq expression.</summary>
  public BinaryenExpression I32Eq(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.EqInt32, left, right);

  /// <summary>Creates an i32.lt_s expression.</summary>
  public BinaryenExpression I32LtS(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.LtSInt32, left, right);

  /// <summary>Creates an i32.gt_s expression.</summary>
  public BinaryenExpression I32GtS(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.GtSInt32, left, right);

  /// <summary>Creates an f32.add expression.</summary>
  public BinaryenExpression F32Add(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.AddFloat32, left, right);

  /// <summary>Creates an f64.add expression.</summary>
  public BinaryenExpression F64Add(BinaryenExpression left, BinaryenExpression right) =>
    Binary(BinaryenOp.AddFloat64, left, right);

  // ========== Control Flow ==========

  /// <summary>Creates a block expression.</summary>
  public BinaryenExpression Block(string? name, BinaryenExpression[] children, UIntPtr type)
  {
    nint[] handles = children.Select(e => e.Handle).ToArray();
    nint expr = Interop.Native.BinaryenBlock(_module, name, handles, (uint)handles.Length, type);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a block expression with auto type.</summary>
  public BinaryenExpression Block(string? name, params BinaryenExpression[] children) =>
    Block(name, children, BinaryenType.Auto);

  /// <summary>Creates an if expression.</summary>
  public BinaryenExpression If(BinaryenExpression condition, BinaryenExpression ifTrue, BinaryenExpression? ifFalse = null)
  {
    nint expr = Interop.Native.BinaryenIf(_module, condition.Handle, ifTrue.Handle,
      ifFalse?.Handle ?? IntPtr.Zero);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a loop expression.</summary>
  public BinaryenExpression Loop(string? name, BinaryenExpression body)
  {
    nint expr = Interop.Native.BinaryenLoop(_module, name, body.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a br (break) expression.</summary>
  public BinaryenExpression Br(string name, BinaryenExpression? condition = null, BinaryenExpression? value = null)
  {
    nint expr = Interop.Native.BinaryenBreak(_module, name,
      condition?.Handle ?? IntPtr.Zero, value?.Handle ?? IntPtr.Zero);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a call expression.</summary>
  public BinaryenExpression Call(string target, BinaryenExpression[] operands, UIntPtr returnType)
  {
    nint[] handles = operands.Select(e => e.Handle).ToArray();
    nint expr = Interop.Native.BinaryenCall(_module, target, handles, (uint)handles.Length, returnType);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a call expression with no operands.</summary>
  public BinaryenExpression Call(string target, UIntPtr returnType) =>
    Call(target, [], returnType);

  /// <summary>Creates a select expression.</summary>
  public BinaryenExpression Select(BinaryenExpression condition, BinaryenExpression ifTrue, BinaryenExpression ifFalse)
  {
    nint expr = Interop.Native.BinaryenSelect(_module, condition.Handle, ifTrue.Handle, ifFalse.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a drop expression.</summary>
  public BinaryenExpression Drop(BinaryenExpression value)
  {
    nint expr = Interop.Native.BinaryenDrop(_module, value.Handle);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a return expression.</summary>
  public BinaryenExpression Return(BinaryenExpression? value = null)
  {
    nint expr = Interop.Native.BinaryenReturn(_module, value?.Handle ?? IntPtr.Zero);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates a nop expression.</summary>
  public BinaryenExpression Nop()
  {
    nint expr = Interop.Native.BinaryenNop(_module);
    return new BinaryenExpression(expr);
  }

  /// <summary>Creates an unreachable expression.</summary>
  public BinaryenExpression Unreachable()
  {
    nint expr = Interop.Native.BinaryenUnreachable(_module);
    return new BinaryenExpression(expr);
  }
}
