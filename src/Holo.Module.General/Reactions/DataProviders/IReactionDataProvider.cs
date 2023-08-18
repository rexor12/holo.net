using System.Threading;
using System.Threading.Tasks;
using Holo.Module.General.Reactions.Enums;

namespace Holo.Module.General.Reactions.DataProviders;

public interface IReactionDataProvider
{
    Task<string?> TryGetPictureUrlAsync(ReactionType reactionType, CancellationToken token = default);
}