namespace ThisIsAnAttack.Monitors.Scoring;

public class ComboMonitor : MonitorBase
{
    public IComboController ComboController { get; }

    public ComboSnapshot LatestSnapshot { get; private set; } = new ComboSnapshot
    {
        DateTimeOffset = DateTimeOffset.Now,
        MaxCombo = 0,
        Combo = 0,
    };

    public ComboMonitor(IComboController comboController)
    {
        this.ComboController = comboController;

        this.Register(
            this.ComboController,
            cc => cc.comboDidChangeEvent += this.OnComboDidChange,
            cc => cc.comboDidChangeEvent -= this.OnComboDidChange);

        if (this.ComboController is ComboController instance)
        {
            var initialSnapshot = new ComboSnapshot
            {
                DateTimeOffset = DateTimeOffset.Now,
                MaxCombo = instance.maxCombo,
                Combo = 0,
            };

            this.LatestSnapshot = initialSnapshot;
        }
    }

    private void OnComboDidChange(int combo)
    {
        var maxCombo = this.ComboController is ComboController instance ? instance.maxCombo : 0;

        var newSnapshot = new ComboSnapshot
        {
            DateTimeOffset = DateTimeOffset.Now,
            MaxCombo = maxCombo,
            Combo = combo,
        };

        var previousSnapshot = this.LatestSnapshot;

        var changed = newSnapshot.WithoutTimestamp() != previousSnapshot.WithoutTimestamp();
        if (changed)
        {
            this.LatestSnapshot = newSnapshot;

            var args = new ComboChangedEventArgs(newSnapshot, previousSnapshot);
            this.ComboChanged?.Invoke(this, args);
        }
    }
    public int CurrentCombo { get; private set; }

    public event EventHandler<ComboChangedEventArgs>? ComboChanged;
}

