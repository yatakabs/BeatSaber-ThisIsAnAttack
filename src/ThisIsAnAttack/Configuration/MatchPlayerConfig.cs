using IPA.Config.Stores.Attributes;

namespace ThisIsAnAttack.Configuration;

public class MatchPlayerConfig
{
    [NonNullable]
    public string? ScoreSaberId { get; set; }
}
