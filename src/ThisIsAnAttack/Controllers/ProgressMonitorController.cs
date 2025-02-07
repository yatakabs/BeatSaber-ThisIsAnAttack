using System.Collections.Concurrent;
using UnityEngine.SceneManagement;

namespace ThisIsAnAttack.Controllers;

public class ProgressMonitorController
{
    private IPA.Logging.Logger Logger { get; }
    private ConcurrentDictionary<Scene, ConcurrentQueue<Action<Scene>>> SceneUnloadedHandlers { get; } = new ConcurrentDictionary<Scene, ConcurrentQueue<Action<Scene>>>();

    public ProgressMonitorController(IPA.Logging.Logger logger)
    {
        this.Logger = logger;
    }

    private void Awake()
    {
        this.Logger.Debug("ProgressMonitorController awake.");
        SceneManager.sceneLoaded += this.OnSceneLoaded;
        SceneManager.sceneUnloaded += this.OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        this.Logger.Debug("ProgressMonitorController destroyed.");

        SceneManager.sceneLoaded -= this.OnSceneLoaded;
        SceneManager.sceneUnloaded -= this.OnSceneUnloaded;

        foreach (var scene in this.SceneUnloadedHandlers.Keys)
        {
            this.Logger.Debug($"Unregistering scene unloaded handlers for scene: {scene.name}");
            this.OnSceneUnloaded(scene);
        }

        this.SceneUnloadedHandlers.Clear();

        this.Logger.Debug("ProgressMonitorController destroyed.");
    }

    private void RegiterSceneUnloadedHandler(Scene scene, Action<Scene> action)
    {
        this.Logger.Debug($"Registering scene unloaded handler for scene: {scene.name}");

        var actions = this.SceneUnloadedHandlers.GetOrAdd(
            scene,
            _ => new ConcurrentQueue<Action<Scene>>());

        actions.Enqueue(action);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.Logger.Debug($"Scene loaded: {scene.name}");

        if (scene.name == "GameCore")
        {
            this.Logger.Debug("GameCore scene loaded. Registering monitors.");

            throw new NotImplementedException("GameCore scene loaded. Registering monitors.");

            //var monitorCompositor = new MonitorCompositor(
            //    null,
            //    null,
            //    null,
            //    null);

            //this.RegiterSceneUnloadedHandler(scene, _ =>
            //{
            //    this.Logger.Debug("GameCore scene unloaded. Disposing monitors.");
            //    monitorCompositor.Dispose();
            //});
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        this.Logger.Debug($"Scene unloaded: {scene.name}");

        if (this.SceneUnloadedHandlers.TryRemove(scene, out var actions))
        {
            while (actions.TryDequeue(out var action))
            {
                action(scene);
            }
        }
    }
}
