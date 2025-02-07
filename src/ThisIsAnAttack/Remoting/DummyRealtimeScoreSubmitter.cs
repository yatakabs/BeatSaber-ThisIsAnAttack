using ThisIsAnAttack.Monitors.Scoring.Entities;

namespace ThisIsAnAttack.Remoting;

public class DummyRealtimeScoreSubmitter : IRealtimeScoreSubmitter
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public Task SubmitFinishScoreAsync(PlayerGameProgress progress)
    {
        return Task.CompletedTask;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public Task SubmitScoreAsync(PlayerGameProgress score)
    {
        return Task.CompletedTask;
    }
}
