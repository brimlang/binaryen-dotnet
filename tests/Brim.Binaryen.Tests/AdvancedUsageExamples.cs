using Brim.Binaryen.Tests;

namespace Brim.Binaryen.Tests;

/// <summary>
/// Advanced usage examples showing how to create more complex WebAssembly modules.
/// </summary>
public class AdvancedUsageExamples
{
  [Fact]
  public void Can_Create_Factorial_Function()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Create a factorial function: factorial(n) = n <= 1 ? 1 : n * factorial(n-1)
    
    // Parameters: i32 n (index 0)
    // Locals: none
    var n = expr.LocalGet(0, BinaryenType.Int32);
    var one = expr.I32Const(1);
    
    // Check if n <= 1
    var condition = expr.I32LtS(n, expr.I32Const(2));
    
    // Recursive case: n * factorial(n-1)
    var nMinus1 = expr.I32Sub(n, one);
    var recursiveCall = expr.Call("factorial", [nMinus1], BinaryenType.Int32);
    var recursive = expr.I32Mul(n, recursiveCall);
    
    // if (n <= 1) return 1; else return n * factorial(n-1)
    var body = expr.If(condition, one, recursive);
    
    var paramTypes = BinaryenType.Create(BinaryenType.Int32);
    var func = module.AddFunction("factorial", paramTypes, BinaryenType.Int32, body);
    
    // Export the function
    module.AddFunctionExport("factorial", "factorial");
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
    
    // Check the generated text contains our function
    var text = module.WriteText();
    Assert.Contains("factorial", text);
    Assert.Contains("call $factorial", text);
  }

  [Fact] 
  public void Can_Create_Module_With_Memory_And_Globals()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Set up memory (1 initial page, 16 max pages)
    module.SetMemory(1, 16, "memory");
    
    // Add a global counter
    var counterInit = expr.I32Const(0);
    var counter = module.AddGlobal("counter", BinaryenType.Int32, true, counterInit);
    
    // Create increment function that increments the global counter
    var currentValue = expr.GlobalGet("counter", BinaryenType.Int32);
    var incremented = expr.I32Add(currentValue, expr.I32Const(1));
    var setGlobal = expr.GlobalSet("counter", incremented);
    var returnValue = expr.GlobalGet("counter", BinaryenType.Int32);
    var body = expr.Block("increment", setGlobal, returnValue);
    
    var func = module.AddFunction("increment", BinaryenType.None, BinaryenType.Int32, body);
    
    // Export the function and global
    module.AddFunctionExport("increment", "increment");
    module.AddGlobalExport("counter", "counter");
    module.AddMemoryExport("memory", "memory");
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.NotEqual(IntPtr.Zero, counter.Handle);
    Assert.True(module.HasMemory);
    Assert.True(module.Validate());
    
    var text = module.WriteText();
    Assert.Contains("increment", text);
    Assert.Contains("counter", text);
    Assert.Contains("memory", text);
  }

  [Fact]
  public void Can_Create_Complex_Control_Flow()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Create a function that demonstrates loops and breaks
    // while (n > 0) { result = result + n; n = n - 1; }
    
    var paramTypes = BinaryenType.Create(BinaryenType.Int32);
    var localTypes = new[] { BinaryenType.Int32 }; // result variable
    
    var n = expr.LocalGet(0, BinaryenType.Int32);       // parameter n
    var result = expr.LocalGet(1, BinaryenType.Int32);  // local result
    
    // Loop body
    var zero = expr.I32Const(0);
    var one = expr.I32Const(1);
    
    // Check if n > 0
    var condition = expr.I32GtS(n, zero);
    
    // Break if n <= 0
    var breakIf = expr.Br("loop", condition);
    
    // Add n to result
    var newResult = expr.I32Add(result, n);
    var setResult = expr.LocalSet(1, newResult);
    
    // Decrement n
    var newN = expr.I32Sub(n, one);
    var setN = expr.LocalSet(0, newN);
    
    // Continue loop
    var continue_ = expr.Br("loop");
    
    var loopBody = expr.Block(null, breakIf, setResult, setN, continue_);
    var loop = expr.Loop("loop", loopBody);
    
    // Initialize result to 0
    var initResult = expr.LocalSet(1, zero);
    
    // Return result
    var returnResult = expr.LocalGet(1, BinaryenType.Int32);
    
    var body = expr.Block("main", initResult, loop, returnResult);
    
    var func = module.AddFunction("sum_to_n", paramTypes, BinaryenType.Int32, localTypes, body);
    module.AddFunctionExport("sum_to_n", "sum_to_n");
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
    
    var text = module.WriteText();
    Assert.Contains("sum_to_n", text);
    Assert.Contains("loop", text);
  }

  [Fact]
  public void Can_Use_Multiple_Operations()
  {
    using var module = new BinaryenModule();
    var expr = module.Expressions;
    
    // Test various operations
    var a = expr.I32Const(42);
    var b = expr.I32Const(7);
    
    // Test unary operations
    var clz = expr.I32Clz(a);
    var abs = expr.F32Abs(expr.F32Const(-3.14f));
    
    // Test binary operations
    var add = expr.I32Add(a, b);
    var sub = expr.I32Sub(a, b);
    var mul = expr.I32Mul(a, b);
    var div = expr.I32DivS(a, b);
    var eq = expr.I32Eq(a, b);
    
    // Create a function that uses these operations
    var body = expr.Block("test_ops", 
      clz, abs, add, sub, mul, div, eq,
      expr.I32Const(1) // return 1
    );
    
    var func = module.AddFunction("test_operations", BinaryenType.None, BinaryenType.Int32, body);
    module.AddFunctionExport("test_operations", "test_operations");
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Copy_And_Manipulate_Expressions()
  {
    using var module1 = new BinaryenModule();
    using var module2 = new BinaryenModule();
    
    var expr1 = module1.Expressions;
    var expr2 = module2.Expressions;
    
    // Create an expression in module1
    var original = expr1.I32Add(expr1.I32Const(10), expr1.I32Const(20));
    
    // Copy it to module2
    var copied = original.Copy(module2);
    
    // Use the copied expression
    var func = module2.AddFunction("test", BinaryenType.None, BinaryenType.Int32, 
      new BinaryenExpression(copied.Handle));
    
    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module2.Validate());
    
    // Verify both expressions have the same structure but different handles
    Assert.NotEqual(original.Handle, copied.Handle);
    Assert.Equal(original.GetId(), copied.GetId());
  }
}