namespace Ingenium;

/// <summary>
/// Provides a base implementation of a disposable object.
/// </summary>
public abstract class Disposable : IDisposable
{
	/// <summary>
	/// Gets whether the object is disposed.
	/// </summary>
	public bool Disposed { get; private set; }

	/// <inheritdoc />
	public void Dispose() => Dispose(true);

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	/// <param name="disposing">Flag to state whether we are explicitly disposing of the instance.</param>
	protected void Dispose(bool disposing)
	{
		if (!Disposed)
		{
			Disposed = true;

			if (disposing)
			{
				DisposeExplicit();
			}

			DisposeImplicit();

			GC.SuppressFinalize(this);
		}
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources when explicitly dispsing of the instance.
	/// </summary>
	protected virtual void DisposeExplicit() { }
	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources when implicitly dispsing of the instance.
	/// </summary>
	protected virtual void DisposeImplicit() { }
	/// <summary>
	/// Ensures the current instance is not disposed.
	/// </summary>
	/// <param name="objectName">The object name.</param>
	/// <param name="message">The exception message.</param>
	/// <exception cref="ObjectDisposedException">If the instance is already disposed.</exception>
	protected void EnsureNotDisposed(string? objectName = default, string? message = default)
	{
		if (Disposed)
		{
			objectName ??= GetType().Name;
			message ??= CommonResources.ObjectDisposedExceptionMessage;

			throw new ObjectDisposedException(objectName, message);
		}
	}

	~Disposable() => Dispose(false);
}