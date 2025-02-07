using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors.Scoring;
using UnityEngine.SceneManagement;

namespace ThisIsAnAttack.Monitors;

public class SceneChangeMonitor : MonitorBase
{
    private IPluginLogger Logger { get; }

    public SceneChangeMonitor(
        IPluginLogger logger)
    {
        this.Logger = logger;

        this.Logger.Debug($"{nameof(SceneChangeMonitor)} constructor called.");
    }

    #region Public Methods

    public IDisposable RegisterSceneBoundActions<TState>(
        Func<Scene, bool> scenePredicate,
        Func<Scene, TState> onStart,
        Action<Scene, TState> onComplete)
    {
        this.Logger.Debug($"{nameof(RegisterSceneBoundActions)}() called.");

        var disposables = new CompositeDisposable();

        var currentScene = SceneManager.GetActiveScene();
        if (scenePredicate(currentScene))
        {
            this.Logger.Info($"Active scene matches predicate: {currentScene.name}");
            var state = onStart(currentScene);
            var sceneId = currentScene.buildIndex;
            void onSceneUnloaded(Scene unloadedScene)
            {
                if (unloadedScene.buildIndex == sceneId)
                {
                    this.Logger.Info($"Unloaded scene matches predicate: {unloadedScene.name}");
                    onComplete(unloadedScene, state);
                }
            }
            SceneManager.sceneUnloaded += onSceneUnloaded;
            disposables.Register(() =>
            {
                SceneManager.sceneUnloaded -= onSceneUnloaded;
            });
        }
        else
        {
            this.Logger.Info($"Active scene does not match predicate: {currentScene.name}");

            void onSceneLoaded(Scene loadedScene, LoadSceneMode mode)
            {
                this.Logger.Debug($"Scene loaded: {loadedScene.name}, {loadedScene.buildIndex}");
                var scenePredicateResult = scenePredicate(loadedScene);

                if (scenePredicateResult)
                {
                    this.Logger.Info($"Loaded scene matches predicate: {loadedScene.name}");
                    var state = onStart(loadedScene);
                    var sceneId = loadedScene.buildIndex;

                    void onSceneUnloaded(Scene unloadedScene)
                    {
                        if (unloadedScene.buildIndex == sceneId)
                        {
                            this.Logger.Info($"Unloaded scene matches predicate: {unloadedScene.name}");
                            onComplete(unloadedScene, state);
                        }
                    }

                    SceneManager.sceneUnloaded += onSceneUnloaded;

                    disposables.Register(() =>
                    {
                        SceneManager.sceneUnloaded -= onSceneUnloaded;
                    });
                }
            };

            SceneManager.sceneLoaded += onSceneLoaded;

            disposables.Register(() =>
            {
                SceneManager.sceneLoaded -= onSceneLoaded;
            });

            this.Logger.Debug($"{nameof(RegisterSceneBoundActions)}() completed.");
        }

        return disposables;
    }

    public IDisposable RegisterSceneBoundActions<TState>(
        string sceneName,
        Func<Scene, TState> onStart,
        Action<Scene, TState> onComplete)
    {
        return this.RegisterSceneBoundActions(
            scene => scene.name == sceneName,
            onStart,
            onComplete);
    }

    public IDisposable RegisterSceneBoundActions(
        Func<Scene, bool> scenePredicate,
        Action<Scene> onStart,
        Action<Scene> onComplete)
    {
        return this.RegisterSceneBoundActions<int>(
            scenePredicate,
            scene => { onStart(scene); return default; },
            (scene, _) => onComplete(scene));
    }

    public IDisposable RegisterSceneBoundActions(
        string sceneName,
        Action<Scene> onStart,
        Action<Scene> onComplete)
    {
        return this.RegisterSceneBoundActions(
            scene => scene.name == sceneName,
            onStart,
            onComplete);
    }

    public IDisposable RegisterSceneBoundActions<T>(
        Func<Scene, bool> scenePredicate,
        Func<Scene, Task<T>> onStart,
        Func<Scene, T, Task> onComplete)
    {
        this.Logger.Debug("RegisterSceneBound() called (async overload).");

        var disposables = new CompositeDisposable();

        this.RegisterSceneBoundActions(
            scenePredicate,
            onStart,
            async (scene, task) =>
            {
                var state = await task.ConfigureAwait(false);
                await onComplete(scene, state)
                    .ConfigureAwait(false);
            });

        return disposables;
    }

    public IDisposable RegisterSceneBoundActions<T>(
        string sceneName,
        Func<Scene, Task<T>> onStart,
        Func<Scene, T, Task> onComplete)
    {
        return this.RegisterSceneBoundActions(
            scene => scene.name == sceneName,
            onStart,
            onComplete);
    }

    #endregion
}
