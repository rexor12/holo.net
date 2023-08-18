using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Holo.Module.General.Reactions.Configurations;
using Holo.Module.General.Reactions.Enums;
using Holo.Sdk.DI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.Bulkhead;
using Polly.CircuitBreaker;

namespace Holo.Module.General.Reactions.ApiClients;

[Service(typeof(IWaifuPicsClient))]
public sealed class WaifuPicsClient : IWaifuPicsClient
{
    private static readonly IReadOnlyDictionary<ReactionType, string> ReactionCategories
        = new Dictionary<ReactionType, string>
        {
            { ReactionType.Awoo, "awoo" },
            { ReactionType.Bite, "bite" },
            { ReactionType.Blush, "blush" },
            { ReactionType.Bonk, "bonk" },
            { ReactionType.Bully, "bully" },
            { ReactionType.Cry, "cry" },
            { ReactionType.Cuddle, "cuddle" },
            { ReactionType.Dance, "dance" },
            { ReactionType.Handhold, "handhold" },
            { ReactionType.Happy, "happy" },
            { ReactionType.Highfive, "highfive" },
            { ReactionType.Hug, "hug" },
            { ReactionType.Kill, "kill" },
            { ReactionType.Kiss, "kiss" },
            { ReactionType.Lick, "lick" },
            { ReactionType.Neko, "neko" },
            { ReactionType.Nom, "nom" },
            { ReactionType.Pat, "pat" },
            { ReactionType.Poke, "poke" },
            { ReactionType.Slap, "slap" },
            { ReactionType.Smile, "smile" },
            { ReactionType.Smug, "smug" },
            { ReactionType.Waifu, "waifu" },
            { ReactionType.Wave, "wave" },
            { ReactionType.Wink, "wink" }
        };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WaifuPicsClient> _logger;
    private readonly IOptions<ReactionOptions> _options;

    public WaifuPicsClient(
        IHttpClientFactory httpClientFactory,
        ILogger<WaifuPicsClient> logger,
        IOptions<ReactionOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options;
    }

    public async Task<IReadOnlyList<string>> GetUrlsAsync(ReactionType reactionType, CancellationToken token)
    {
        if (!ReactionCategories.TryGetValue(reactionType, out var reactionCategory))
            throw new ArgumentOutOfRangeException(
                nameof(reactionType),
                reactionType,
                "Unknown reaction type cannot be mapped to category.");

        using var httpClient = _httpClientFactory.CreateClient("WaifuPics");
        try
        {
            var response = await httpClient.PostAsync(
                string.Format(_options.Value.SfwBatchApiRoute, reactionCategory),
                new StringContent("{}", Encoding.UTF8, "application/json"),
                token);
            var result = JsonConvert.DeserializeObject<WaifuPicsBatchResult>(await response.Content.ReadAsStringAsync());

            return result == null ? Array.Empty<string>() : result.Files;
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit broken for waifu.pics");
            throw;
        }
        catch (BulkheadRejectedException)
        {
            _logger.LogWarning("Rate limit hit for waifu.pics");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred during a waifu.pics request");
            throw;
        }
    }

    private sealed record WaifuPicsBatchResult(string[] Files);
}