using System;
using FluentMigrator;

namespace Holo.Module.ServiceHost.Migrations;

[Migration(1699539947)]
public sealed class AddBackgroundProcessingItemsTable : Migration
{
    private const string TableName = "background_processing_items";

    public override void Up()
    {
        Create
            .Table(TableName).InSchema(Constants.SchemaName)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("correlation_id").AsString().NotNullable()
            .WithColumn("item_type").AsString().NotNullable()
            .WithColumn("serialized_item_data").AsString().NotNullable();

        Create
            .Index("ix_background_processing_items_ordering")
            .OnTable(TableName).InSchema(Constants.SchemaName)
            .WithOptions().NonClustered()
            .OnColumn("correlation_id").Ascending()
            .OnColumn("created_at").Ascending();
    }

    public override void Down()
        => throw new NotSupportedException();
}