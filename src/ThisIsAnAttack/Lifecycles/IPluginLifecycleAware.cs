namespace ThisIsAnAttack.Lifecycles;

public interface IPluginLifecycleAware
{
    void OnEnable();

    void OnDisable();
}
