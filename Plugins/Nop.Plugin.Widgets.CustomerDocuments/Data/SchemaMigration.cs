using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.CustomerDocuments.Domain;

namespace Nop.Plugin.Widgets.CustomerDocuments.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/12/04 09:30:17:6455422", "Widgets.CustomerDocuments base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<CustomerDocumentsItem>(Create);
        }
    }
}