using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
    string baseDirPath = Path.Combine(AppContext.BaseDirectory, exe);
    if (File.Exists(baseDirPath)) return baseDirPath;

    string? pathEnv = Environment.GetEnvironmentVariable("PATH");
    if (!string.IsNullOrEmpty(pathEnv))
    {
      foreach (string dir in pathEnv.Split(Path.PathSeparator))
      {
        if (string.IsNullOrWhiteSpace(dir)) continue;
        string candidate = Path.Combine(dir, exe);
        if (File.Exists(candidate)) return candidate;
      }
    }

    throw new FileNotFoundException($"Binaryen tool not found: {exe}");
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

  public static async Task<(int exitCode, string stdout, string stderr)> RunAsync(
    IBinaryenToolLocator locator,
    string args,
    CancellationToken cancellationToken)
  {
    ProcessStartInfo psi = new(locator.Resolve("wasm-opt"))
    {
      Arguments = args,
      RedirectStandardOutput = true,
      RedirectStandardError = true
    };

    using Process p = Process.Start(psi)!;
    Task<string> stdoutTask = p.StandardOutput.ReadToEndAsync(cancellationToken);
    Task<string> stderrTask = p.StandardError.ReadToEndAsync(cancellationToken);
    await p.WaitForExitAsync(cancellationToken);
    string stdout = await stdoutTask;
    string stderr = await stderrTask;
    return (p.ExitCode, stdout, stderr);
  }
}
