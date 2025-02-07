using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Disposables;

public class DisposableContainer : IDisposable
{
    private ConcurrentBag<MonitorRegistration> MonitorRegistrations { get; } = [];

    public DisposableContainer()
    {
    }

    protected IDisposable Register<TParam, TState>(
        TParam param,
        Func<TParam, TState> onRegister,
        Action<TParam, TState> onUnregister)
    {
        this.ThrowIfDisposed();

        var state = onRegister(param);

        var registration = new MonitorRegistration(reg =>
        {
            try
            {
                onUnregister(param, state);
            }
            finally
            {
                this.MonitorRegistrations.TryTake(out _);
            }
        });

        this.MonitorRegistrations.Add(registration);
        return registration;
    }

    protected IDisposable Register<T>(
        T param,
        Action<T> onRegister,
        Action<T> onUnregister)
    {
        this.ThrowIfDisposed();

        onRegister(param);

        var registration = new MonitorRegistration(reg =>
        {
            try
            {
                onUnregister(param);
            }
            finally
            {
                this.MonitorRegistrations.TryTake(out _);
            }
        });

        this.MonitorRegistrations.Add(registration);
        return registration;
    }

    private sealed class MonitorRegistration : IDisposable
    {
        private int gate;
        private Action<MonitorRegistration> UnregisterAction { get; }

        public MonitorRegistration(Action<MonitorRegistration> onUnregister)
        {
            this.UnregisterAction = onUnregister;
        }

        private void Unregister(bool disposing)
        {
            if (Interlocked.Exchange(ref this.gate, 1) == 0)
            {
                this.UnregisterAction(this);
            }
        }

        #region IDisposable
        private bool isDisposed;
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    try
                    {
                        this.Unregister(disposing);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }

    #region IDisposable

    private volatile bool isDisposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                while (this.MonitorRegistrations.TryTake(out var registration))
                {
                    registration.Dispose();
                }
            }
            this.isDisposed = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(DisposableContainer));
        }
    }
    #endregion IDisposable

}

