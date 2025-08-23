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
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Create a factorial function: factorial(n) = n <= 1 ? 1 : n * factorial(n-1)

    // Parameters: i32 n (index 0)
    // Locals: none
    BinaryenExpression n = expr.LocalGet(0, BinaryenType.Int32);
    BinaryenExpression one = expr.I32Const(1);

    // Check if n <= 1
    BinaryenExpression condition = expr.I32LtS(n, expr.I32Const(2));

    // Recursive case: n * factorial(n-1)
    BinaryenExpression nMinus1 = expr.I32Sub(n, one);
    BinaryenExpression recursiveCall = expr.Call("factorial", [nMinus1], BinaryenType.Int32);
    BinaryenExpression recursive = expr.I32Mul(n, recursiveCall);

    // if (n <= 1) return 1; else return n * factorial(n-1)
    BinaryenExpression body = expr.If(condition, one, recursive);

    nuint paramTypes = BinaryenType.Create(BinaryenType.Int32);
    BinaryenFunction func = module.AddFunction("factorial", paramTypes, BinaryenType.Int32, body);

    // Export the function
    module.AddFunctionExport("factorial", "factorial");

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());

    // Check the generated text contains our function
    string text = module.WriteText();
    Assert.Contains("factorial", text);
    Assert.Contains("call $factorial", text);
  }

  [Fact]
  public void Can_Create_Module_With_Memory_And_Globals()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Set up memory (1 initial page, 16 max pages) and export it as "memory"
    module.SetMemory(1, 16, "memory");

    // Add a global counter (immutable to avoid requiring mutable-globals feature)
    BinaryenExpression counterInit = expr.I32Const(42);
    BinaryenGlobal counter = module.AddGlobal("counter", BinaryenType.Int32, false, counterInit);

    // Create a simple function that returns the global counter value
    BinaryenExpression body = expr.GlobalGet("counter", BinaryenType.Int32);

    BinaryenFunction func = module.AddFunction("getCounter", BinaryenType.None, BinaryenType.Int32, body);

    // Export the function and global (memory already exported via SetMemory)
    module.AddFunctionExport("getCounter", "getCounter");
    module.AddGlobalExport("counter", "counter");

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.NotEqual(IntPtr.Zero, counter.Handle);
    Assert.True(module.HasMemory);
    Assert.True(module.Validate());

    string text = module.WriteText();
    Assert.Contains("getCounter", text);
    Assert.Contains("counter", text);
    Assert.Contains("memory", text);
  }

  [Fact]
  public void Can_Create_Complex_Control_Flow()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Create a function that demonstrates loops and breaks
    // while (n > 0) { result = result + n; n = n - 1; }

    nuint paramTypes = BinaryenType.Create(BinaryenType.Int32);
    nuint[] localTypes = new[] { BinaryenType.Int32 }; // result variable

    BinaryenExpression n = expr.LocalGet(0, BinaryenType.Int32);       // parameter n
    BinaryenExpression result = expr.LocalGet(1, BinaryenType.Int32);  // local result

    // Loop body
    BinaryenExpression zero = expr.I32Const(0);
    BinaryenExpression one = expr.I32Const(1);

    // Check if n > 0
    BinaryenExpression condition = expr.I32GtS(n, zero);

    // Break if n <= 0
    BinaryenExpression breakIf = expr.Br("loop", condition);

    // Add n to result
    BinaryenExpression newResult = expr.I32Add(result, n);
    BinaryenExpression setResult = expr.LocalSet(1, newResult);

    // Decrement n
    BinaryenExpression newN = expr.I32Sub(n, one);
    BinaryenExpression setN = expr.LocalSet(0, newN);

    // Continue loop
    BinaryenExpression continue_ = expr.Br("loop");

    BinaryenExpression loopBody = expr.Block(null, breakIf, setResult, setN, continue_);
    BinaryenExpression loop = expr.Loop("loop", loopBody);

    // Initialize result to 0
    BinaryenExpression initResult = expr.LocalSet(1, zero);

    // Return result
    BinaryenExpression returnResult = expr.LocalGet(1, BinaryenType.Int32);

    BinaryenExpression body = expr.Block("main", initResult, loop, returnResult);

    BinaryenFunction func = module.AddFunction("sum_to_n", paramTypes, BinaryenType.Int32, localTypes, body);
    module.AddFunctionExport("sum_to_n", "sum_to_n");

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());

    string text = module.WriteText();
    Assert.Contains("sum_to_n", text);
    Assert.Contains("loop", text);
  }

  [Fact]
  public void Can_Use_Multiple_Operations()
  {
    using BinaryenModule module = new BinaryenModule();
    ExpressionBuilder expr = module.Expressions;

    // Test various operations
    BinaryenExpression a = expr.I32Const(42);
    BinaryenExpression b = expr.I32Const(7);

    // Test unary operations
    BinaryenExpression clz = expr.I32Clz(a);
    BinaryenExpression abs = expr.F32Abs(expr.F32Const(-3.14f));

    // Test binary operations
    BinaryenExpression add = expr.I32Add(a, b);
    BinaryenExpression sub = expr.I32Sub(a, b);
    BinaryenExpression mul = expr.I32Mul(a, b);
    BinaryenExpression div = expr.I32DivS(a, b);
    BinaryenExpression eq = expr.I32Eq(a, b);

    // Create a function that uses these operations
    // Drop intermediate values since they're not used
    BinaryenExpression body = expr.Block("test_ops",
      expr.Drop(clz), 
      expr.Drop(abs), 
      expr.Drop(add), 
      expr.Drop(sub), 
      expr.Drop(mul), 
      expr.Drop(div), 
      expr.Drop(eq),
      expr.I32Const(1) // return 1
    );

    BinaryenFunction func = module.AddFunction("test_operations", BinaryenType.None, BinaryenType.Int32, body);
    module.AddFunctionExport("test_operations", "test_operations");

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module.Validate());
  }

  [Fact]
  public void Can_Copy_And_Manipulate_Expressions()
  {
    using BinaryenModule module1 = new BinaryenModule();
    using BinaryenModule module2 = new BinaryenModule();

    ExpressionBuilder expr1 = module1.Expressions;
    ExpressionBuilder expr2 = module2.Expressions;

    // Create an expression in module1
    BinaryenExpression original = expr1.I32Add(expr1.I32Const(10), expr1.I32Const(20));

    // Copy it to module2
    BinaryenExpression copied = original.Copy(module2);

    // Use the copied expression
    BinaryenFunction func = module2.AddFunction("test", BinaryenType.None, BinaryenType.Int32,
      new BinaryenExpression(copied.Handle));

    Assert.NotEqual(IntPtr.Zero, func.Handle);
    Assert.True(module2.Validate());

    // Verify both expressions have the same structure but different handles
    Assert.NotEqual(original.Handle, copied.Handle);
    Assert.Equal(original.GetId(), copied.GetId());
  }
}
