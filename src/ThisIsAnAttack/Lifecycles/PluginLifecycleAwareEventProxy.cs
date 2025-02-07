namespace ThisIsAnAttack.Lifecycles;

public class PluginLifecycleAwareEventProxy : IPluginLifecycleAwareEventProxy
{
    public event EventHandler? OnEnable;
    public event EventHandler? OnDisable;

    public void TriggerOnEnable()
    {
        this.OnEnable?.Invoke(this, EventArgs.Empty);
    }
    public void TriggerOnDisable()
    {
        this.OnDisable?.Invoke(this, EventArgs.Empty);
    }
}
