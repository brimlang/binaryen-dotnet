using System.Diagnostics;

namespace Brim.Binaryen.Tools;

public interface IBinaryenToolLocator
{
  string Resolve(string toolName); // e.g. "wasm-opt" -> absolute path
}

public sealed class BinaryenToolLocator : IBinaryenToolLocator
{
  public string Resolve(string toolName)
  {
    string exe = OperatingSystem.IsWindows() ? $"{toolName}.exe" : toolName;
    string path = Path.Combine(AppContext.BaseDirectory, exe);
    return !File.Exists(path)
      ? throw new FileNotFoundException($"Binaryen tool not found: {path}")
      : path;
  }
}

public static class WasmOpt
{
  public static int Run(IBinaryenToolLocator locator, string args, out string stdout, out string stderr)
  {
    ProcessStartInfo psi = new(locator.Resolve("wasm-opt"))
    {
      Arguments = args,
      RedirectStandardOutput = true,
      RedirectStandardError = true
    };

    using Process p = Process.Start(psi)!;
    stdout = p.StandardOutput.ReadToEnd();
    stderr = p.StandardError.ReadToEnd();
    p.WaitForExit();
    return p.ExitCode;
  }
}
