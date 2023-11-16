using System;
using FluentMigrator;
using Holo.Sdk.Storage;

namespace Holo.Module.Dev.Migrations;

[Migration(1694589868)]
public sealed class AddFeatureStatesTable : Migration
{
    private const string TableName = "feature_states";

    public override void Up()
    {
        Create
            .Table(TableName).InSchema(Constants.SchemaName)
            .WithColumn("id").AsString(DataSizes.StringId).NotNullable().PrimaryKey()
            .WithColumn("is_enabled").AsBoolean().NotNullable();
    }

    public override void Down()
        => throw new NotSupportedException();
}