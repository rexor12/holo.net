using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Holo.Module.General.Reactions.Enums;

namespace Holo.Module.General.Reactions.ApiClients;

public interface IWaifuPicsClient
{
    Task<IReadOnlyList<string>> GetUrlsAsync(ReactionType reactionType, CancellationToken token = default);
}