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

  [Fact]
  public void Can_Use_High_Level_Module()
  {
    using var module = new BinaryenModule();
    Assert.True(module.Validate());
  }

  [Fact] 
  public void Can_Create_Types()
  {
    var i32Type = BinaryenType.Int32;
    var i64Type = BinaryenType.Int64;
    var f32Type = BinaryenType.Float32;
    var f64Type = BinaryenType.Float64;
    
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
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    var i32Const = expr.I32Const(42);
    var i64Const = expr.I64Const(123456789L);
    var f32Const = expr.F32Const(3.14f);
    var f64Const = expr.F64Const(2.71828);
    
    Assert.NotEqual(IntPtr.Zero, i32Const.Handle);
    Assert.NotEqual(IntPtr.Zero, i64Const.Handle);
    Assert.NotEqual(IntPtr.Zero, f32Const.Handle);
    Assert.NotEqual(IntPtr.Zero, f64Const.Handle);
  }

  [Fact]
  public void Can_Create_Binary_Operations()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    var left = expr.I32Const(10);
    var right = expr.I32Const(20);
    var add = expr.I32Add(left, right);
    var sub = expr.I32Sub(left, right);
    var mul = expr.I32Mul(left, right);
    
    Assert.NotEqual(IntPtr.Zero, add.Handle);
    Assert.NotEqual(IntPtr.Zero, sub.Handle);
    Assert.NotEqual(IntPtr.Zero, mul.Handle);
  }

  [Fact]
  public void Can_Create_Control_Flow()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    var condition = expr.I32Const(1);
    var thenBranch = expr.I32Const(42);
    var elseBranch = expr.I32Const(0);
    
    var ifExpr = expr.If(condition, thenBranch, elseBranch);
    Assert.NotEqual(IntPtr.Zero, ifExpr.Handle);
    
    var block = expr.Block("test", thenBranch, elseBranch);
    Assert.NotEqual(IntPtr.Zero, block.Handle);
  }

  [Fact]
  public void Can_Create_Simple_Function()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Create a simple function that returns 42
    var body = expr.I32Const(42);
    var func = module.AddFunction("get42", BinaryenType.None, BinaryenType.Int32, body);
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.Equal(1u, module.FunctionCount);
    
    var retrieved = module.GetFunction("get42");
    Assert.NotNull(retrieved);
    Assert.Equal(func.Handle, retrieved.Value.Handle);
  }

  [Fact]
  public void Can_Create_Function_With_Parameters()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Create function that adds two i32 parameters
    var param1 = expr.LocalGet(0, BinaryenType.Int32);
    var param2 = expr.LocalGet(1, BinaryenType.Int32);
    var body = expr.I32Add(param1, param2);
    
    var paramTypes = BinaryenType.Create(BinaryenType.Int32, BinaryenType.Int32);
    var func = module.AddFunction("add", paramTypes, BinaryenType.Int32, body);
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Add_Exports()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Create a simple function
    var body = expr.I32Const(42);
    module.AddFunction("internal_func", BinaryenType.None, BinaryenType.Int32, body);
    
    // Export it
    var export = module.AddFunctionExport("internal_func", "exported_func");
    Assert.NotEqual(IntPtr.Zero, export.Handle);
    
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Add_Globals()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Add a global i32 with initial value 100
    var init = expr.I32Const(100);
    var global = module.AddGlobal("myGlobal", BinaryenType.Int32, true, init);
    
    Assert.NotEqual(IntPtr.Zero, global.Handle);
    
    var retrieved = module.GetGlobal("myGlobal");
    Assert.NotNull(retrieved);
    Assert.Equal(global.Handle, retrieved.Value.Handle);
    
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Write_Text_Format()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    var body = expr.I32Const(42);
    module.AddFunction("test", BinaryenType.None, BinaryenType.Int32, body);
    module.AddFunctionExport("test", "test");
    
    var text = module.WriteText();
    Assert.NotEmpty(text);
    Assert.Contains("test", text);
    Assert.Contains("i32.const 42", text);
  }

  [Fact]
  public void Can_Set_Memory()
  {
    using var module = new BinaryenModule();
    
    module.SetMemory(1, 10); // 1 initial page, 10 max pages
    Assert.True(module.HasMemory);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Use_Operations_Constants()
  {
    // Test that operation constants are accessible
    var addOp = BinaryenOp.AddInt32;
    var subOp = BinaryenOp.SubInt32;
    var clzOp = BinaryenOp.ClzInt32;
    
    Assert.NotEqual(0, addOp);
    Assert.NotEqual(0, subOp);  
    Assert.NotEqual(0, clzOp);
    Assert.NotEqual(addOp, subOp);
  }
}

