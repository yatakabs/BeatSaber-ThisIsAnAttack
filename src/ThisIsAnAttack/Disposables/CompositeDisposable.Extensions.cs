namespace ThisIsAnAttack.Disposables;

/// <summary>
/// Provides extension methods for the CompositeDisposable class.
/// </summary>
public static class CompositeDisposableExtensions
{
    /// <summary>
    /// Adds the specified disposable to the composite disposable.
    /// </summary>
    /// <typeparam name="T">The type of the disposable.</typeparam>
    /// <param name="disposable">The disposable to add.</param>
    /// <param name="compositeDisposable">The composite disposable to add to.</param>
    /// <returns>The added disposable.</returns>
    public static T AddTo<T>(
        this T disposable,
        CompositeDisposable compositeDisposable)
        where T : IDisposable
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }

    /// <summary>
    /// Registers an action to be called when the composite disposable is disposed.
    /// </summary>
    /// <param name="compositeDisposable">The composite disposable to register with.</param>
    /// <param name="onDispose">The action to call on dispose.</param>
    /// <returns>The composite disposable.</returns>
    public static CompositeDisposable Register(
        this CompositeDisposable compositeDisposable,
        Action onDispose)
    {
        compositeDisposable.Add(new DelegateDisposable(onDispose));
        return compositeDisposable;
    }

    /// <summary>
    /// Registers an action to be called with the specified instance when the composite disposable is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="compositeDisposable">The composite disposable to register with.</param>
    /// <param name="instance">The instance to pass to the action.</param>
    /// <param name="onDispose">The action to call on dispose.</param>
    /// <returns>The composite disposable.</returns>
    public static CompositeDisposable Register<T>(
        this CompositeDisposable compositeDisposable,
        T instance,
        Action<T> onDispose)
        where T : class
    {
        compositeDisposable.Add(new DelegateDisposable(() => onDispose(instance)));
        return compositeDisposable;
    }

    /// <summary>
    /// Registers an action to be called with the specified instance and disposing flag when the composite disposable is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="compositeDisposable">The composite disposable to register with.</param>
    /// <param name="instance">The instance to pass to the action.</param>
    /// <param name="onDispose">The action to call on dispose.</param>
    /// <param name="disposing">The disposing flag to pass to the action.</param>
    /// <returns>The composite disposable.</returns>
    public static CompositeDisposable Register<T>(
        this CompositeDisposable compositeDisposable,
        T instance,
        Action<T, bool> onDispose,
        bool disposing)
        where T : class
    {
        compositeDisposable.Add(new DelegateDisposable(() => onDispose(instance, disposing)));
        return compositeDisposable;
    }

    /// <summary>
    /// Registers an action to be called with the specified instance, disposing flag, and isDisposed flag when the composite disposable is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="compositeDisposable">The composite disposable to register with.</param>
    /// <param name="instance">The instance to pass to the action.</param>
    /// <param name="onDispose">The action to call on dispose.</param>
    /// <param name="disposing">The disposing flag to pass to the action.</param>
    /// <param name="isDisposed">The isDisposed flag to pass to the action.</param>
    /// <returns>The composite disposable.</returns>
    public static CompositeDisposable Register<T>(
        this CompositeDisposable compositeDisposable,
        T instance,
        Action<T, bool> onDispose,
        bool disposing,
        bool isDisposed)
        where T : class
    {
        compositeDisposable.Add(new DelegateDisposable(() => onDispose(instance, disposing)));
        return compositeDisposable;
    }
}
