using ThisIsAnAttack.Disposables;

namespace ThisIsAnAttack.Lifecycles;

public class PluginLifecycleAwareDecorator : DisposableContainer, IPluginLifecycleAware
{
    private IPluginLifecycleAware Decorated { get; }
    private IPluginLifecycleAwareEventProxy Proxy { get; }

    public PluginLifecycleAwareDecorator(
        IPluginLifecycleAware decorated,
        IPluginLifecycleAwareEventProxy proxy)
    {
        this.Decorated = decorated;
        this.Proxy = proxy;

        this.Register(
            this.Proxy,
            proxy =>
            {
                proxy.OnEnable += this.OnEnableTriggered;
                proxy.OnDisable += this.OnDisableTriggered;
            },
            proxy =>
            {
                proxy.OnEnable -= this.OnEnableTriggered;
                proxy.OnDisable -= this.OnDisableTriggered;
            });
    }

    private void OnDisableTriggered(object sender, EventArgs e)
    {
        this.Decorated.OnDisable();
    }

    private void OnEnableTriggered(object sender, EventArgs e)
    {
        this.Decorated.OnEnable();
    }

    public void OnEnable()
    {
        this.Proxy.TriggerOnEnable();
    }

    public void OnDisable()
    {
        this.Proxy.TriggerOnDisable();
    }
}
