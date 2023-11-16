using System;
using FluentMigrator;

namespace Holo.Module.ServiceHost.Migrations;

[Migration(1699432431)]
public sealed class CreateSchema : Migration
{
    public override void Up()
    {
        Create.Schema(Constants.SchemaName);
    }

    public override void Down()
        => throw new NotSupportedException();
}
