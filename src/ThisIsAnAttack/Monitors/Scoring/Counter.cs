namespace ThisIsAnAttack.Monitors.Scoring;

public class Counter
{
    private int value;
    public int Value => this.value;

    public Counter() { }

    public void Increment()
    {
        Interlocked.Increment(ref this.value);
    }

    public void Decrement()
    {
        Interlocked.Decrement(ref this.value);
    }

    public void Reset()
    {
        Interlocked.Exchange(ref this.value, 0);
    }
}
