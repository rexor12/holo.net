namespace Holo.Module.General.Reactions.Configurations;

public sealed class ReactionOptions
{
    public const string SectionName = "Extensions:General:ReactionOptions";

    public required string ApiBaseUrl { get; set; }
    public required string SfwBatchApiRoute { get; set; }
    public int CircuitBreakerFailureThreshold { get; set; } = 1;
    public int CircuitBreakerRecoveryTimeInSeconds { get; set; } = 300;
    public int RateLimiterRequestsPerInterval { get; set; } = 2;
    public int RateLimiterIntervalInSeconds { get; set; } = 5;
}