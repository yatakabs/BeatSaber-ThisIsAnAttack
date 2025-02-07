namespace ThisIsAnAttack;

/// <summary>
/// Represents the progress of a song.
/// </summary>
public record SongProgress
{
    /// <summary>
    /// Gets the duration of the song.
    /// </summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets the current position in the song.
    /// </summary>
    public required TimeSpan Position { get; init; }

    /// <summary>
    /// Gets an empty song progress instance.
    /// </summary>
    public static SongProgress Empty { get; } = new SongProgress
    {
        Duration = TimeSpan.Zero,
        Position = TimeSpan.Zero,
    };
}

/// <summary>
/// Metadata for a song.
/// </summary>
public record SongMetadata
{
    /// <summary>
    /// Unique identifier for the song.
    /// </summary>
    public required string SongHash { get; init; }

    /// <summary>
    /// Name of the song.
    /// </summary>
    public required string SongName { get; init; }

    /// <summary>
    /// Subtitle of the song.
    /// </summary>
    public string? SongSubName { get; init; }

    /// <summary>
    /// Author of the song.
    /// </summary>
    public required string SongAuthorName { get; init; }

    /// <summary>
    /// Author of the level.
    /// </summary>
    public required string LevelAuthorName { get; init; }

    /// <summary>
    /// Beats per minute of the song.
    /// </summary>
    public required double BeatsPerMinute { get; init; }

    /// <summary>
    /// Start time of the song preview.
    /// </summary>
    public required TimeSpan PreviewStartTime { get; init; }

    /// <summary>
    /// Duration of the song preview.
    /// </summary>
    public required TimeSpan PreviewDuration { get; init; }

    /// <summary>
    /// Empty song metadata.
    /// </summary>
    public static SongMetadata Empty { get; } = new SongMetadata
    {
        SongHash = string.Empty,
        SongName = string.Empty,
        SongSubName = string.Empty,
        SongAuthorName = string.Empty,
        LevelAuthorName = string.Empty,
        BeatsPerMinute = 0,
        PreviewStartTime = TimeSpan.Zero,
        PreviewDuration = TimeSpan.Zero,
    };
}
