using System.Threading.Tasks;
using Holo.Module.General.Cookies.Models;
using Holo.Sdk.Storage.Repositories;

namespace Holo.Module.General.Cookies.Storage.Repositories;

/// <summary>
/// Interface for a repository used for accessing <see cref="FortuneCookie"/> entities.
/// </summary>
public interface IFortuneCookieRepository : IRepository<ulong, FortuneCookie, CookiesDbContext>
{
    /// <summary>
    /// Gets a randomly selected fortune cookie.
    /// </summary>
    /// <returns>A randomly selected <see cref="FortuneCookie"/>.</returns>
    Task<FortuneCookie> GetRandomFortuneCookieAsync();
}
