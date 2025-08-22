namespace Brim.Binaryen;

public sealed class BinaryenException : ApplicationException
{
  public BinaryenException()
  {
  }

  public BinaryenException(string message)
      : base(message)
  {
  }

  public BinaryenException(string message, Exception inner)
      : base(message, inner)
  {
  }
}
