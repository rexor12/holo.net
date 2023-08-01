using System;
using FluentMigrator;
using Holo.Sdk.Storage;

namespace Holo.Module.General.Migrations;

[Migration(1690233675)]
public sealed class AddFortuneCookiesTable : Migration
{
    private const string TableName = "fortune_cookies";

    public override void Up()
    {
        Create
            .Table(TableName).InSchema(Constants.SchemaName)
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("message").AsString(DataSizes.UserText).NotNullable();

        Execute.EmbeddedScript("Holo.Module.General.Migrations.VNext.AddFortuneCookiesMetadataView.sql");
        Execute.EmbeddedScript("Holo.Module.General.Migrations.VNext.AddFortuneCookies.sql");
    }

    public override void Down()
        => throw new NotSupportedException();
}