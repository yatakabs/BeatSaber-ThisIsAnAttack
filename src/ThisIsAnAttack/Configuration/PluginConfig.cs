using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using ThisIsAnAttack.Logging;
using Zenject;

//[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ThisIsAnAttack.Configuration;

public class PluginConfig
{

    [Inject]
    public IPluginLogger Logger { get; } = new DummyPluginLogger();

    #region Settings

    [NonNullable]
    public virtual MatchPlayerConfig Player { get; set; } = new MatchPlayerConfig();

    [UseConverter(typeof(ListConverter<MatchConfig>))]
    public virtual List<MatchConfig> Matches { get; set; } = [];

    public string? GrpcServerAddress { get; set; }

    #endregion

    /// <summary>
    /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
    /// </summary>
    public virtual void OnReload()
    {
        this.Logger.TraceFormat(
            "OnReload() called. Probably the config was changed on disk.");
        try
        {
            this.Reloaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Error in OnReload()");
            throw;
        }

        this.Logger.Debug("OnReload() completed.");
    }

    /// <summary>
    /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
    /// </summary>
    public virtual void Changed()
    {
        try
        {
            this.Logger.Debug("Changed() called. Probably the config was changed in-game.");
            this.ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Error in Changed()");
            throw;
        }

        this.Logger.Debug("Changed() completed.");
    }

    /// <summary>
    /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
    /// </summary>
    public virtual void CopyFrom(PluginConfig other)
    {
        // This instance's members populated from other
        try
        {
            this.Logger.Debug("CopyFrom() called.");
            this.Player = other.Player;
            this.Matches = other.Matches;
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Error in CopyFrom()");
            throw;
        }

        this.Logger.Debug("Copied from other. Calling OnReload() to ensure internal consistency.");

        // Call OnReload to ensure internal consistency
        //try
        //{
        //    this.Logger.Debug("Calling OnReload() to ensure internal consistency.");
        //    this.OnReload();

        //    this.Logger.Debug("OnReload() completed. (Called from CopyFrom()");
        //}
        //catch (Exception ex)
        //{
        //    this.Logger.Error("Error in OnReload()");
        //    this.Logger.Error(ex);
        //    throw;
        //}

        this.Logger.Debug("CopyFrom() completed.");
    }

    #region Events

    public event EventHandler? Reloaded;
    public event EventHandler? ConfigChanged;

    #endregion
}
