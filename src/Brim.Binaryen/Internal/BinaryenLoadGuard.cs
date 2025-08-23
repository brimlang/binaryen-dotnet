using System.Runtime.InteropServices;

namespace Brim.Binaryen.Internal;

/// <summary>
/// Probes the native Binaryen library once and throws a helpful error if it can't be loaded.
/// Call <see cref="EnsureLoaded"/> before touching any interop.
/// </summary>
internal static class BinaryenLoadGuard
{
  private static volatile bool _loaded;
  private static readonly object _gate = new();

  public static void EnsureLoaded()
  {
    if (_loaded) return;

    lock (_gate)
    {
      if (_loaded) return;

      try
      {
        // Minimal probe that forces the runtime to resolve the native library.
        nint m = Interop.Native.BinaryenModuleCreate();
        if (m != IntPtr.Zero)
        {
          Interop.Native.BinaryenModuleDispose(m);
        }

        _loaded = true;
      }
      catch (DllNotFoundException ex)
      {
        throw BuildLoadException("Native library not found", ex);
      }
      catch (BadImageFormatException ex)
      {
        // Usually RID/arch mismatch (e.g., x86 vs x64) or wrong glibc target.
        throw BuildLoadException("Native library has the wrong format for this process (arch/ABI mismatch)", ex);
      }
      catch (EntryPointNotFoundException ex)
      {
        // Extremely unlikely with a consistent build; indicates mismatched versions.
        throw BuildLoadException("Expected entry point missing (version skew between managed bindings and native lib)", ex);
      }
    }
  }

  private static InvalidOperationException BuildLoadException(string reason, Exception? inner)
  {
    string baseDir = AppContext.BaseDirectory;
    (string os, string arch) = (GetOs(), RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant());
    string expectedName = os switch
    {
      "windows" => "binaryen.dll",
      "linux" => "libbinaryen.so",
      "osx" => "libbinaryen.dylib",
      _ => "binaryen (unknown platform)"
    };

    // Check the obvious path first: AppContext.BaseDirectory (where NuGet runtime assets land).
    string candidate = Path.Combine(baseDir, expectedName);
    bool exists = File.Exists(candidate);

    string msg =
$"""
Failed to load Binaryen native library: {reason}.

Searched (first): {candidate}  {(exists ? "[FOUND]" : "[MISSING]")}
Process:   OS={os}, Arch={arch}
BaseDir:   {baseDir}
Expected:  {expectedName}

If you're running from a consuming app:
 - Ensure the package 'Brim.Binaryen' is referenced by the app project (not only by a transitive library).
 - Ensure you built/published for the correct RID (e.g., linux-x64, win-x64, osx-arm64).
 - For self-contained publishes, confirm the runtimes/*/native assets are included next to the app.

If you're developing this repo:
 - Run the native bringup script to stage the shared library and wasm-opt into src/Brim.Binaryen/Native/<rid>/.
""";
    // Keep the exception type simple for now; upgrade to a custom one if you prefer.
    return new InvalidOperationException(msg, inner);
  }

  private static string GetOs()
  {
    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
      ? "windows"
      : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
      ? "linux"
      : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown";
  }
}

