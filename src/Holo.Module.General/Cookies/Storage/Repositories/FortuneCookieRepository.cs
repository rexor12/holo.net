using System;
using System.Linq.Expressions;
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
    NumericIdentifierBasedRepositoryBase<FortuneCookieId, FortuneCookie, CookiesDbContext>,
    IFortuneCookieRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="FortuneCookieRepository"/>.
    /// </summary>
    /// <param name="databaseServices">
    /// The <see cref="IDatabaseServices"/> used to access units of work.
    /// </param>
    public FortuneCookieRepository(IDatabaseServices databaseServices)
        : base(databaseServices)
    {
    }

    /// <inheritdoc cref="IFortuneCookieRepository.GetRandomFortuneCookieAsync"/>
    public async Task<FortuneCookie> GetRandomFortuneCookieAsync()
    {
        await using var dbContextWrapper = GetDbContextWrapper();
        var queryable = dbContextWrapper.DbContext.GetRandomFortuneCookie();

        return await queryable.FirstAsync();
    }

    /// <inheritdoc cref="RepositoryBase{TAggregateRoot, TDbContext}.GetDbSet(TDbContext)"/>
    protected override DbSet<FortuneCookie> GetDbSet(CookiesDbContext dbContext)
        => dbContext.FortuneCookies;

    protected override Expression<Func<FortuneCookie, bool>> GetEqualByIdExpression(FortuneCookieId identifier)
        => entity => entity.Identifier == identifier;
}