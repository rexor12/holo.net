using System.Threading.Tasks;
using Holo.Module.General.Cookies.Models;
using Holo.Sdk.DI;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Holo.Module.General.Cookies.Storage.Repositories;

/// <summary>
/// A repository used for accessing <see cref="FortuneCookie"/> entities.
/// </summary>
[Service(typeof(IFortuneCookieRepository))]
public sealed class FortuneCookieRepository :
    RepositoryBase<FortuneCookie, CookiesDbContext>,
    IFortuneCookieRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="FortuneCookieRepository"/>.
    /// </summary>
    /// <param name="dbContextFactory">
    /// The <see cref="IDbContextFactory"/> used for creating DbContexts.
    /// </param>
    /// <param name="unitOfWorkProvider">
    /// The <see cref="IUnitOfWorkProvider"/> used for accessing units of work.
    /// </param>
    public FortuneCookieRepository(
        IDbContextFactory dbContextFactory,
        IUnitOfWorkProvider unitOfWorkProvider)
        : base(dbContextFactory, unitOfWorkProvider)
    {
    }

    /// <inheritdoc cref="IFortuneCookieRepository.GetRandomFortuneCookieAsync"/>
    public async Task<FortuneCookie> GetRandomFortuneCookieAsync()
    {
        await using var dbContextWrapper = GetDbContextWrapper(false);
        var queryable = dbContextWrapper.DbContext.GetRandomFortuneCookie();

        return await queryable.FirstAsync();
    }

    /// <inheritdoc cref="RepositoryBase{TAggregateRoot, TDbContext}.GetDbSet(TDbContext)"/>
    protected override DbSet<FortuneCookie> GetDbSet(CookiesDbContext dbContext)
        => dbContext.FortuneCookies;
}