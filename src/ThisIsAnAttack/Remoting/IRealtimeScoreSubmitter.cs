using ThisIsAnAttack.Monitors.Scoring.Entities;

namespace ThisIsAnAttack.Remoting;

public interface IRealtimeScoreSubmitter
{
    Task SubmitFinishScoreAsync(PlayerGameProgress progress);
    Task SubmitScoreAsync(PlayerGameProgress score);

}
