using System;
using FluentMigrator;
using Holo.Sdk.Storage;

namespace Holo.Module.Reminders.Migrations;

[Migration(1694679804)]
public sealed class AddRemindersTable : Migration
{
    private const string TableName = "reminders";

    public override void Up()
    {
        Create
            .Table(TableName).InSchema(Constants.SchemaName)
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("message").AsString(DataSizes.UserText).Nullable()
            .WithColumn("is_repeating").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("frequency_time").AsCustom("INTERVAL").Nullable().WithDefaultValue(null)
            .WithColumn("day_of_week").AsInt16().NotNullable().WithDefaultValue(0)
            .WithColumn("until_date").AsDate().Nullable().WithDefaultValue(null)
            .WithColumn("base_trigger").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("last_trigger").AsDateTimeOffset().NotNullable()
            .WithColumn("next_trigger").AsDateTimeOffset().NotNullable()
            .WithColumn("location").AsInt16().NotNullable().WithDefaultValue(0)
            .WithColumn("server_id").AsInt64().Nullable().WithDefaultValue(null)
            .WithColumn("channel_id").AsInt64().Nullable().WithDefaultValue(null);

        // TODO https://github.com/npgsql/efcore.pg/issues/2617
        // Create
        //     .Sequence("reminders_hilo_seq").InSchema(Constants.SchemaName)
        //     .IncrementBy(100);
    }

    public override void Down()
        => throw new NotSupportedException();
}