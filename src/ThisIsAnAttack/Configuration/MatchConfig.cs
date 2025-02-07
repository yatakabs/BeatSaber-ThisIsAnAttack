using IPA.Config.Stores.Attributes;

namespace ThisIsAnAttack.Configuration;

public class MatchConfig
{
    [NonNullable]
    public Uri? ServerUri { get; set; }

    [NonNullable]
    public string? MatchId { get; set; }

    public string? MatchKey { get; set; }
}
