namespace ThisIsAnAttack.Lifecycles;

public interface IPluginLifecycleAwareEventProxy
{
    void TriggerOnEnable();
    void TriggerOnDisable();

    event EventHandler? OnEnable;
    event EventHandler? OnDisable;
}
