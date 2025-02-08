using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using ThisIsAnAttack.Configuration;
using ThisIsAnAttack.Installers;
using ThisIsAnAttack.Logging;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace ThisIsAnAttack;

[Plugin(RuntimeOptions.SingleStartInit)]
public partial class Plugin
{
    private IPluginLogger Logger { get; set; } = new DummyPluginLogger();

    public const string HarmonyId = "com.github.yatakabs.ThisIsAnAttack";
    internal static readonly HarmonyLib.Harmony harmony = new(HarmonyId);

    /// <summary>
    /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
    /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
    /// Only use [Init] with one Constructor.
    /// </summary>
    [Init]
    public Plugin(
        Zenjector zenjector,
        PluginMetadata metadata,
        Config ipaConfig,
        IPALogger ipaLogger)
    {
        this.Logger = new IpaPluginLogger(ipaLogger);
        this.Logger.Debug("Plugin ctor called.");

        var config = this.InitializePluginConfig(ipaConfig);

        zenjector.Install(
            Location.App,
            container =>
            {
                // Plugin logger
                container
                    .BindInstance<IPluginLogger>(this.Logger)
                    .AsSingle()
                    .NonLazy();

                // Plugin config
                container
                    .BindInstance(config)
                    .AsSingle()
                    .NonLazy();

                // Plugin metadata
                container
                    .BindInstance(metadata)
                    .AsSingle()
                    .NonLazy();

                // Plugin instance (just for reference)
                container
                    .BindInterfacesAndSelfTo<Plugin>()
                    .FromInstance(this)
                    .AsSingle()
                    .NonLazy();
            });

        zenjector.Install<MainInstaller>(Location.App);
        zenjector.Install<GrpcInstaller>(Location.App);
        zenjector.Install<ScoringPlayerInstaller>(Location.Player);
    }

    private PluginConfig InitializePluginConfig(Config ipaConfig)
    {
        try
        {
            this.Logger.Debug("Loading config...");
            var config = ipaConfig.Generated<PluginConfig>();
            this.Logger.Info("Config loaded.");
            return config;
        }
        catch (Exception ex)
        {
            this.Logger.Warn(ex, "Failed to read config. Using default config");
            return ipaConfig.Generated<PluginConfig>();
        }
    }

    #region BSIPA Start/Stop

    /// <summary>
    /// Called when the game is started.
    /// This is where the plugin should do all of its initialization (e.g. patching, creating GameObjects, etc.).
    /// </summary>
    [OnStart]
    public void OnApplicationStart()
    {
        this.Logger.InfoFormat(
            "OnApplicationStart() called. GameVersion: {0}, UnityVersion: {1}, PluginVersion: {2}",
            Application.version,
            Application.unityVersion,
            "N/A");
    }

    /// <summary>
    /// Called when the game is quitting.
    /// This is where the plugin should clean up any resources.
    /// </summary>
    [OnExit]
    public void OnApplicationQuit()
    {
        this.Logger.InfoFormat(
            "OnApplicationQuit() called. GameVersion: {0}, UnityVersion: {1}, PluginVersion: {2}",
            Application.version,
            Application.unityVersion,
            "N/A");
    }

    #endregion BSIPA Start/Stop

    // Uncomment the methods in this section if using Harmony
    #region Harmony
    /*
    /// <summary>
    /// Attempts to apply all the Harmony patches in this assembly.
    /// </summary>
    internal static void ApplyHarmonyPatches()
    {
        try
        {
            Plugin.Log?.Debug("Applying Harmony patches.");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            Plugin.Log?.Error("Error applying Harmony patches: " + ex.Message);
            Plugin.Log?.Debug(ex);
        }
    }

    /// <summary>
    /// Attempts to remove all the Harmony patches that used our HarmonyId.
    /// </summary>
    internal static void RemoveHarmonyPatches()
    {
        try
        {
            // Removes all patches with this HarmonyId
            harmony.UnpatchAll(HarmonyId);
        }
        catch (Exception ex)
        {
            Plugin.Log?.Error("Error removing Harmony patches: " + ex.Message);
            Plugin.Log?.Debug(ex);
        }
    }
    */
    #endregion
}
