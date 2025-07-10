namespace Domain.Configs;

public record BehaviourLoggingConfig
{
    public const string SectionName = "BehaviourLogging";
    public bool Enable { get; set; } = false;
}