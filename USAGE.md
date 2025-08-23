# Binaryen .NET Bindings Usage Guide

This document explains how to use the expanded Binaryen .NET bindings to create and manipulate WebAssembly modules.

## Quick Start

```csharp
using Brim.Binaryen;

// Create a module
using var module = new BinaryenModule();
var expr = module.Expressions;

// Create a simple function that returns 42
var body = expr.I32Const(42);
var func = module.AddFunction("get42", BinaryenType.None, BinaryenType.Int32, body);

// Export the function
module.AddFunctionExport("get42", "get42");

// Generate WebAssembly binary
byte[] wasmBytes = module.WriteBinary();

// Or generate text format for debugging
string wasmText = module.WriteText();
Console.WriteLine(wasmText);
```

## Core Concepts

### BinaryenModule
The main entry point for creating WebAssembly modules. Provides methods for:
- Creating functions, globals, memory, and exports
- Running optimization passes
- Writing binary or text output
- Module validation

### ExpressionBuilder
Fluent API for creating WebAssembly expressions, accessible via `module.Expressions`:
- **Constants**: `I32Const(42)`, `F64Const(3.14)`, etc.
- **Local operations**: `LocalGet(0, BinaryenType.Int32)`, `LocalSet(1, value)`
- **Binary operations**: `I32Add(left, right)`, `F32Mul(a, b)`, etc.
- **Control flow**: `If(condition, thenBranch, elseBranch)`, `Loop(name, body)`

### BinaryenType
Static class providing all WebAssembly types:
```csharp
var i32 = BinaryenType.Int32;
var f64 = BinaryenType.Float64;
var params = BinaryenType.Create(BinaryenType.Int32, BinaryenType.Int32); // tuple
```

### BinaryenOp
Static class providing all WebAssembly operations for use with `Binary()` and `Unary()`:
```csharp
var addExpr = expr.Binary(BinaryenOp.AddInt32, left, right);
var clzExpr = expr.Unary(BinaryenOp.ClzInt32, value);
```

## Examples

### Simple Function with Parameters

```csharp
using var module = new BinaryenModule();
var expr = module.Expressions;

// Create add(a: i32, b: i32) -> i32
var param1 = expr.LocalGet(0, BinaryenType.Int32);
var param2 = expr.LocalGet(1, BinaryenType.Int32);
var body = expr.I32Add(param1, param2);

var paramTypes = BinaryenType.Create(BinaryenType.Int32, BinaryenType.Int32);
var func = module.AddFunction("add", paramTypes, BinaryenType.Int32, body);
module.AddFunctionExport("add", "add");
```

### Function with Local Variables

```csharp
using var module = new BinaryenModule();
var expr = module.Expressions;

// Function with one parameter and one local variable
var localTypes = new[] { BinaryenType.Int32 }; // local at index 1

var param = expr.LocalGet(0, BinaryenType.Int32);
var doubled = expr.I32Mul(param, expr.I32Const(2));
var setLocal = expr.LocalSet(1, doubled);
var getLocal = expr.LocalGet(1, BinaryenType.Int32);

var body = expr.Block("main", setLocal, getLocal);

var paramTypes = BinaryenType.Create(BinaryenType.Int32);
var func = module.AddFunction("double", paramTypes, BinaryenType.Int32, localTypes, body);
```

### Control Flow

```csharp
using var module = new BinaryenModule();
var expr = module.Expressions;

// if-then-else
var condition = expr.I32Eqz(expr.LocalGet(0, BinaryenType.Int32));
var thenBranch = expr.I32Const(1);
var elseBranch = expr.I32Const(0);
var ifExpr = expr.If(condition, thenBranch, elseBranch);

// loops with breaks
var loopBody = expr.Block("loop_block",
    expr.Br("loop", condition), // break if condition
    expr.Br("outer_loop")       // continue loop
);
var loop = expr.Loop("outer_loop", loopBody);
```

### Memory and Globals

```csharp
using var module = new BinaryenModule();
var expr = module.Expressions;

// Set up memory (1 page initial, 10 pages max)
module.SetMemory(1, 10, "memory");

// Add a global counter
var init = expr.I32Const(0);
var global = module.AddGlobal("counter", BinaryenType.Int32, true, init);

// Function that increments the counter
var current = expr.GlobalGet("counter", BinaryenType.Int32);
var incremented = expr.I32Add(current, expr.I32Const(1));
var setGlobal = expr.GlobalSet("counter", incremented);
var body = expr.Block("increment", setGlobal, current);

var func = module.AddFunction("increment", BinaryenType.None, BinaryenType.Int32, body);

// Export everything
module.AddFunctionExport("increment", "increment");
module.AddGlobalExport("counter", "counter"); 
module.AddMemoryExport("memory", "memory");
```

### Optimization

```csharp
using var module = new BinaryenModule();
// ... create functions ...

// Use built-in optimization levels
module.Optimize(OptimizeOptions.O2());  // -O2 equivalent
module.Optimize(OptimizeOptions.Os());  // size-focused
module.Optimize(OptimizeOptions.Oz());  // maximum size reduction

// Or run specific passes
module.RunPasses("dce", "inlining-optimizing", "memory-packing");

// Validate after optimization
if (!module.Validate()) {
    throw new Exception("Module validation failed");
}
```

### Working with Expression Objects

```csharp
using var module = new BinaryenModule();
var expr = module.Expressions;

var expression = expr.I32Add(expr.I32Const(10), expr.I32Const(20));

// Get information about expressions
uint id = expression.GetId();
var type = expression.GetExpressionType();

// Print expression for debugging
expression.Print(); // outputs to console

// Copy expressions between modules
using var module2 = new BinaryenModule();
var copied = expression.Copy(module2);
```

## Type System

The Binaryen .NET bindings expose the full WebAssembly type system:

- **Basic types**: `Int32`, `Int64`, `Float32`, `Float64`, `Vec128`
- **Reference types**: `Funcref`, `Externref`, `Anyref`
- **Special types**: `None` (void), `Unreachable`, `Auto` (inference)
- **Tuple types**: Created with `BinaryenType.Create(type1, type2, ...)`

## Error Handling

- `BinaryenException` is thrown for module creation failures
- `ObjectDisposedException` is thrown when using disposed modules
- Always call `module.Validate()` to check module correctness
- Use `using` statements or explicit `Dispose()` for proper cleanup

## Thread Safety

- Module creation and disposal are thread-safe
- Expression creation within a module is thread-safe
- Other operations on the same module are not thread-safe
- Use separate module instances per thread for concurrent work

## Performance Tips

1. **Reuse modules** when creating multiple functions
2. **Batch operations** instead of making many small calls
3. **Run optimization passes** to improve generated code
4. **Validate modules** during development but consider skipping in production
5. **Use `WriteText()`** for debugging, `WriteBinary()` for production

## Best Practices

1. **Always dispose modules** using `using` statements
2. **Validate modules** after construction and before use
3. **Use meaningful names** for functions, globals, and locals
4. **Structure code** with helper methods for complex expressions
5. **Test WebAssembly output** with actual WASM runtimes
6. **Use type constants** from `BinaryenType` instead of magic values
7. **Leverage the expression builder** for readable code

## Debugging

- Use `module.WriteText()` to see the generated WebAssembly text format
- Use `expression.Print()` to debug individual expressions
- Use `module.Validate()` to catch structural errors
- Enable debug information if available in your build configuration