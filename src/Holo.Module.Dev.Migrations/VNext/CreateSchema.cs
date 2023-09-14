using FluentMigrator;

namespace Holo.Module.Dev.Migrations;

[Migration(1694589867)]
public sealed class CreateSchema : Migration
{
    public override void Up()
    {
        Create.Schema(Constants.SchemaName);
    }

    public override void Down()
        => throw new NotSupportedException();
}
