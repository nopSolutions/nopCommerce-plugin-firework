using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Firework.Domain;

namespace Nop.Plugin.Widgets.Firework.Data
{
    [NopMigration("2023/01/01 12:00:00", "Widgets.Firework base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<FireworkEmbedWidget>();
        }

        #endregion
    }
}