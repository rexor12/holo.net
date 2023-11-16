using System;
using FluentMigrator;

namespace Holo.Module.ServiceHost.Migrations;

[Migration(1699432432)]
public sealed class AddHiLoSequencesTable : Migration
{
    private const string TableName = "hilo_sequences";

    public override void Up()
    {
        Create
            .Table(TableName).InSchema(Constants.SchemaName)
            .WithColumn("id").AsString().NotNullable().PrimaryKey()
            .WithColumn("current_hi").AsInt64().NotNullable().WithDefaultValue(0);

        Execute.EmbeddedScript("Holo.Module.ServiceHost.Migrations.VNext.AddHiLoFunctions.sql");
    }

    public override void Down()
        => throw new NotSupportedException();
}