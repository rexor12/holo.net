using System.ComponentModel.DataAnnotations.Schema;
using Holo.Sdk.Storage;

namespace Holo.Module.General.Cookies.Models;

public sealed class FortuneCookie : AggregateRoot<FortuneCookieId>
{
    public const string TableName = "fortune_cookies";

    [Column("message")]
    public required string Message { get; set; }
}