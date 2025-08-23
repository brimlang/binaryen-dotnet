using Brim.Binaryen.Tools;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Brim.Binaryen.Tests;

public class BinaryenToolLocatorTests
{
  [Fact]
  public void Resolve_Finds_Tool_On_Path()
  {
    if (OperatingSystem.IsWindows())
    {
      BinaryenToolLocator locator = new();
      string path = locator.Resolve("dotnet");
      Assert.True(File.Exists(path));
      return;
    }

    string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    Directory.CreateDirectory(tempDir);
    string toolName = "dummy";
    string toolPath = Path.Combine(tempDir, toolName);
    File.WriteAllText(toolPath, "#!/bin/sh\nexit 0\n");
    Process.Start("chmod", $"+x {toolPath}")!.WaitForExit();
    string? originalPath = Environment.GetEnvironmentVariable("PATH");
    Environment.SetEnvironmentVariable("PATH", tempDir);
    try
    {
      BinaryenToolLocator locator = new();
      string resolved = locator.Resolve(toolName);
      Assert.Equal(toolPath, resolved);
    }
    finally
    {
      Environment.SetEnvironmentVariable("PATH", originalPath);
      Directory.Delete(tempDir, true);
    }
  }

  [Fact]
  public async Task RunAsync_Executes_Command()
  {
    IBinaryenToolLocator locator;
    string args;
    if (OperatingSystem.IsWindows())
    {
      locator = new StubLocator(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
      args = "/c \"echo stdout && echo stderr 1>&2\"";
    }
    else
    {
      locator = new StubLocator("/bin/sh");
      args = "-c \"echo stdout; echo stderr 1>&2\"";
    }

    (int exitCode, string stdout, string stderr) = await WasmOpt.RunAsync(locator, args, CancellationToken.None);
    Assert.Equal(0, exitCode);
    Assert.Contains("stdout", stdout);
    Assert.Contains("stderr", stderr);
  }

  private sealed class StubLocator : IBinaryenToolLocator
  {
    private readonly string _path;
    public StubLocator(string path) => _path = path;
    public string Resolve(string toolName) => _path;
  }
}
