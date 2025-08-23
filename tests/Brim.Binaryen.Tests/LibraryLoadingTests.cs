using System;
using System.IO;
using System.Runtime.InteropServices;
using Brim.Binaryen.Internal;

namespace Brim.Binaryen.Tests;

public class LibraryLoadingTests
{
  [Fact]
  public void Native_Library_Should_Be_Present()
  {
    string baseDir = AppContext.BaseDirectory;
    string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows"
             : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux"
             : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown";

    string expectedName = os switch
    {
      "windows" => "binaryen.dll",
      "linux" => "libbinaryen.so",
      "osx" => "libbinaryen.dylib",
      _ => "binaryen (unknown platform)"
    };

    string candidate = Path.Combine(baseDir, expectedName);

    // This test should pass - the native library should be present
    Assert.True(File.Exists(candidate), $"Native library not found at: {candidate}");

    // If it exists, let's check some basic info
    FileInfo fileInfo = new FileInfo(candidate);
    Assert.True(fileInfo.Length > 0, $"Native library file is empty: {candidate}");
  }

  [Fact]
  public void LoadGuard_Should_Work_Or_Provide_Good_Error()
  {
    // This test should either work (library loads) or fail with a clear error message
    Exception? ex = Record.Exception(() => BinaryenLoadGuard.EnsureLoaded());

    // If there's an exception, it should be an InvalidOperationException with a helpful message
    if (ex != null)
    {
      Assert.IsType<InvalidOperationException>(ex);
      Assert.Contains("Failed to load Binaryen native library", ex.Message);
      Assert.Contains("BaseDir:", ex.Message); // Should contain diagnostic info
    }
    // If no exception, the library loaded successfully
  }
}
