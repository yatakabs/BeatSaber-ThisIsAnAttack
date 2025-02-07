using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Disposables;

/// <summary>
/// Represents a collection of <see cref="IDisposable"/> objects that are disposed together.
/// </summary>
public sealed class CompositeDisposable : IDisposable
{
    private ConcurrentBag<IDisposable> Disposables { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class.
    /// </summary>
    public CompositeDisposable()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class with the specified disposables.
    /// </summary>
    /// <param name="disposables">An array of <see cref="IDisposable"/> objects to add to the collection.</param>
    public CompositeDisposable(params IDisposable[] disposables)
    {
        foreach (var disposable in disposables)
        {
            this.Disposables.Add(disposable);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class with the specified disposables.
    /// </summary>
    /// <param name="disposables">An enumerable collection of <see cref="IDisposable"/> objects to add to the collection.</param>
    public CompositeDisposable(IEnumerable<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            this.Disposables.Add(disposable);
        }
    }

    /// <summary>
    /// Adds a disposable object to the collection.
    /// </summary>
    /// <param name="disposable">The <see cref="IDisposable"/> object to add.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the <see cref="CompositeDisposable"/> has been disposed.</exception>
    public void Add(IDisposable disposable)
    {
        if (this.isDisposed)
        {
            disposable.Dispose();
        }
        else
        {
            this.Disposables.Add(disposable);
        }
    }

    /// <summary>
    /// Removes a disposable object from the collection.
    /// </summary>
    /// <param name="disposable">The <see cref="IDisposable"/> object to remove.</param>
    public void Remove(IDisposable disposable)
    {
        this.Disposables.TryTake(out disposable);
    }

    /// <summary>
    /// Disposes all disposable objects in the collection and clears the collection.
    /// </summary>
    public void Clear()
    {
        while (this.Disposables.TryTake(out var disposable))
        {
            disposable.Dispose();
        }
    }

    #region IDisposable

    private volatile bool isDisposed;

    /// <summary>
    /// Disposes the <see cref="CompositeDisposable"/> and all disposable objects in the collection.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the <see cref="CompositeDisposable"/>.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the method is being called from the <see cref="Dispose"/> method.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources here.
            }

            this.isDisposed = true;
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the <see cref="CompositeDisposable"/> has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the <see cref="CompositeDisposable"/> has been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(CompositeDisposable));
        }
    }

    #endregion IDisposable
}
