using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Disposables;

/// <summary>
/// Represents a disposable object that executes a delegate when disposed.
/// </summary>
public sealed class DelegateDisposable : IDisposable
{
    private Action? disposeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateDisposable"/> class.
    /// </summary>
    /// <param name="disposeAction">The action to execute when the object is disposed.</param>
    public DelegateDisposable(Action disposeAction)
    {
        this.disposeAction = disposeAction;
    }

    /// <summary>
    /// Cancels the dispose action and returns it.
    /// </summary>
    /// <returns>
    /// The dispose action that was cancelled.
    /// </returns>
    public Action? CancelDispose()
    {
        this.ThrowIfDisposed();
        return Interlocked.Exchange(ref this.disposeAction, null);
    }

    /// <summary>
    /// Attempts to dispose the object and returns the cancelled dispose action if any.
    /// </summary>
    /// <param name="cancelled">The cancelled dispose action if the dispose action was not null.</param>
    /// <returns>True if the dispose action was cancelled; otherwise, false.</returns>
    public bool TryDispose(out Action? cancelled)
    {
        cancelled = Interlocked.Exchange(ref this.disposeAction, null);
        return cancelled != null;
    }

    #region IDisposable

    private volatile bool isDisposed;

    /// <summary>
    /// Disposes the object and suppresses finalization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">A value indicating whether the object is being disposed explicitly.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                this.disposeAction?.Invoke();
                this.disposeAction = null;
            }

            this.isDisposed = true;
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the object is disposed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(DelegateDisposable));
        }
    }

    #endregion IDisposable
}
