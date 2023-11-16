using System;
using FluentMigrator;

namespace Holo.Module.Reminders.Migrations;

[Migration(1694679803)]
public sealed class CreateSchema : Migration
{
    public override void Up()
    {
        Create.Schema(Constants.SchemaName);
    }

    public override void Down()
        => throw new NotSupportedException();
}
