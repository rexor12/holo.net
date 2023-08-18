using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Holo.Module.General.Reactions.ApiClients;
using Holo.Module.General.Reactions.Enums;
using Holo.Sdk.DI;

namespace Holo.Module.General.Reactions.DataProviders;

[Service(typeof(IReactionDataProvider))]
public sealed class ReactionDataProvider : IReactionDataProvider
{
    private readonly ConcurrentDictionary<ReactionType, ConcurrentQueue<string>> _cache = new();
    private readonly IReadOnlyDictionary<ReactionType, SemaphoreSlim> _semaphores;
    private readonly IWaifuPicsClient _apiClient;

    public ReactionDataProvider(IWaifuPicsClient apiClient)
    {
        _apiClient = apiClient;
        _semaphores = Enum.GetValues<ReactionType>().ToDictionary(i => i, i => new SemaphoreSlim(1, 1));
    }

    public async Task<string?> TryGetPictureUrlAsync(ReactionType reactionType, CancellationToken token)
    {
        var queue = _cache.GetOrAdd(reactionType, _ => new ConcurrentQueue<string>());
        if (queue.TryDequeue(out var pictureUrl))
            return pictureUrl;

        if (!_semaphores.TryGetValue(reactionType, out var semaphore))
            throw new ArgumentOutOfRangeException(nameof(reactionType), reactionType, "Unknown reaction type.");

        await semaphore.WaitAsync(token);
        try
        {
            if (queue.TryDequeue(out pictureUrl))
                return pictureUrl;

            var pictureUrls = await _apiClient.GetUrlsAsync(reactionType, token);
            if (pictureUrls.Count == 0)
                return null;

            var result = pictureUrls[0];
            for (var index = 1; index < pictureUrls.Count; index++)
                queue.Enqueue(pictureUrls[index]);

            return result;
        }
        finally
        {
            semaphore.Release();
        }
    }
}