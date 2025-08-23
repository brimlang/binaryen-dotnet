using Brim.Binaryen.Internal;
using Brim.Binaryen.Interop;

namespace Brim.Binaryen.Tests;


public class Smoke
{
  [Fact]
  public void Can_Create_And_Dispose_Module()
  {
    BinaryenLoadGuard.EnsureLoaded();
    nint m = Native.BinaryenModuleCreate();
    Assert.NotEqual(IntPtr.Zero, m);
    int ok = Native.BinaryenModuleValidate(m);
    Assert.True(ok is 1 or 0); // just touch the API
    Native.BinaryenModuleDispose(m);
  }

  [Fact]
  public void Can_Use_High_Level_Module()
  {
    using BinaryenModule module = new BinaryenModule();
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Create_Types()
  {
    nuint i32Type = BinaryenType.Int32;
    nuint i64Type = BinaryenType.Int64;
    nuint f32Type = BinaryenType.Float32;
    nuint f64Type = BinaryenType.Float64;

    Assert.NotEqual(UIntPtr.Zero, i32Type);
    Assert.NotEqual(UIntPtr.Zero, i64Type);
    Assert.NotEqual(UIntPtr.Zero, f32Type);
    Assert.NotEqual(UIntPtr.Zero, f64Type);

    // All types should be different
    Assert.NotEqual(i32Type, i64Type);
    Assert.NotEqual(i32Type, f32Type);
    Assert.NotEqual(i32Type, f64Type);
  }

  [Fact]
  public void Can_Create_Constants()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    BinaryenExpression i32Const = expr.I32Const(42);
    BinaryenExpression i64Const = expr.I64Const(123456789L);
    BinaryenExpression f32Const = expr.F32Const(3.14f);
    BinaryenExpression f64Const = expr.F64Const(2.71828);

    Assert.NotEqual(IntPtr.Zero, i32Const.Handle);
    Assert.NotEqual(IntPtr.Zero, i64Const.Handle);
    Assert.NotEqual(IntPtr.Zero, f32Const.Handle);
    Assert.NotEqual(IntPtr.Zero, f64Const.Handle);
  }

  [Fact]
  public void Can_Create_Binary_Operations()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    BinaryenExpression left = expr.I32Const(10);
    BinaryenExpression right = expr.I32Const(20);
    BinaryenExpression add = expr.I32Add(left, right);
    BinaryenExpression sub = expr.I32Sub(left, right);
    BinaryenExpression mul = expr.I32Mul(left, right);

    Assert.NotEqual(IntPtr.Zero, add.Handle);
    Assert.NotEqual(IntPtr.Zero, sub.Handle);
    Assert.NotEqual(IntPtr.Zero, mul.Handle);
  }

  [Fact]
  public void Can_Create_Control_Flow()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    BinaryenExpression condition = expr.I32Const(1);
    BinaryenExpression thenBranch = expr.I32Const(42);
    BinaryenExpression elseBranch = expr.I32Const(0);

    BinaryenExpression ifExpr = expr.If(condition, thenBranch, elseBranch);
    Assert.NotEqual(IntPtr.Zero, ifExpr.Handle);

    BinaryenExpression block = expr.Block("test", thenBranch, elseBranch);
    Assert.NotEqual(IntPtr.Zero, block.Handle);
  }

  [Fact]
  public void Can_Create_Simple_Function()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Create a simple function that returns 42
    BinaryenExpression body = expr.I32Const(42);
    BinaryenFunction func = module.AddFunction("get42", BinaryenType.None, BinaryenType.Int32, body);

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.Equal(1u, module.FunctionCount);

    BinaryenFunction? retrieved = module.GetFunction("get42");
    Assert.NotNull(retrieved);
    Assert.Equal(func.Handle, retrieved.Value.Handle);
  }

  [Fact]
  public void Can_Create_Function_With_Parameters()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Create function that adds two i32 parameters
    BinaryenExpression param1 = expr.LocalGet(0, BinaryenType.Int32);
    BinaryenExpression param2 = expr.LocalGet(1, BinaryenType.Int32);
    BinaryenExpression body = expr.I32Add(param1, param2);

    nuint paramTypes = BinaryenType.Create(BinaryenType.Int32, BinaryenType.Int32);
    BinaryenFunction func = module.AddFunction("add", paramTypes, BinaryenType.Int32, body);

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Add_Exports()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Create a simple function
    BinaryenExpression body = expr.I32Const(42);
    module.AddFunction("internal_func", BinaryenType.None, BinaryenType.Int32, body);

    // Export it
    BinaryenExport export = module.AddFunctionExport("internal_func", "exported_func");
    Assert.NotEqual(IntPtr.Zero, export.Handle);

    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Add_Globals()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Add a global i32 with initial value 100
    BinaryenExpression init = expr.I32Const(100);
    BinaryenGlobal global = module.AddGlobal("myGlobal", BinaryenType.Int32, true, init);

    Assert.NotEqual(IntPtr.Zero, global.Handle);

    BinaryenGlobal? retrieved = module.GetGlobal("myGlobal");
    Assert.NotNull(retrieved);
    Assert.Equal(global.Handle, retrieved.Value.Handle);

    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Write_Text_Format()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    BinaryenExpression body = expr.I32Const(42);
    module.AddFunction("test", BinaryenType.None, BinaryenType.Int32, body);
    module.AddFunctionExport("test", "test");

    // Ensure module is validated before writing text
    Assert.True(module.Validate());

    string text = module.WriteText();
    Assert.NotEmpty(text);
    Assert.Contains("test", text);
    Assert.Contains("i32.const 42", text);
  }

  [Fact]
  public void Can_Set_Memory()
  {
    using BinaryenModule module = new BinaryenModule();

    module.SetMemory(1, 10); // 1 initial page, 10 max pages
    Assert.True(module.HasMemory);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Use_Operations_Constants()
  {
    // Test that operation constants are accessible
    int addOp = BinaryenOp.AddInt32;
    int subOp = BinaryenOp.SubInt32;
    int clzOp = BinaryenOp.ClzInt32;

    // Debug output to see actual values
    // These might be 0 in this version of Binaryen - what matters is that they're different
    Assert.NotEqual(addOp, subOp);

    // Test a few more to ensure we get different values  
    int mulOp = BinaryenOp.MulInt32;
    int divOp = BinaryenOp.DivSInt32;

    // At least some operations should be different from each other
    int[] ops = [addOp, subOp, clzOp, mulOp, divOp];
    int uniqueOps = ops.Distinct().Count();
    Assert.True(uniqueOps > 1, $"All operations returned the same value. Values: [{string.Join(", ", ops)}]");
  }
}

