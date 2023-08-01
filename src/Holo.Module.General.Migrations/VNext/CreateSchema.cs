using System;
using FluentMigrator;

namespace Holo.Module.General.Migrations;

[Migration(1690233674)]
public sealed class CreateSchema : Migration
{
    public override void Up()
    {
        Create.Schema(Constants.SchemaName);
    }

    public override void Down()
        => throw new NotSupportedException();
}
