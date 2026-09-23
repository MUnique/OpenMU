// <copyright file="20260922183000_CleanUpOrphanedAggregateMembers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260922183000_CleanUpOrphanedAggregateMembers")]
    public partial class CleanUpOrphanedAggregateMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Removes the rows which were orphaned while EntityFrameworkContextBase.ForEachAggregate
            // did not recurse into the members of a member: deleting an account removed its characters,
            // but not their inventories - and an inventory is referenced BY its character, so no delete
            // cascade of the database ever reached it. The same happened to every member which is
            // referenced by its owner and sits two levels below the deleted object.
            // The children of a removed row follow through its own cascade.
            migrationBuilder.Sql(
                @"do $$
declare
    target record;
    reference record;
    conditions text;
begin
    -- A row of these tables is only reachable through a reference held by its owner, so a row which
    -- nothing points at is unreachable. Tables whose rows can also be owned through a foreign key of
    -- their own are deliberately not listed (Item, PowerUpDefinition, MonsterSpawnArea,
    -- CastleSiegeZoneDefinition, MagicEffectDefinition): an item in a storage holds that reference
    -- itself, so nothing points at it is true for almost every item of every player.
    for target in
        select * from (values
                ('config', 'AreaSkillSettings', '{}'::text[]),
                ('config', 'BattleZoneDefinition', '{}'::text[]),
                ('config', 'CastleSiegeConfiguration', array['CastleSiegeNpcDefinition', 'CastleSiegeStateScheduleEntry', 'CastleSiegeUpgradeDefinition', 'CastleSiegeZoneDefinition']::text[]),
                ('config', 'DuelConfiguration', array['DuelArea']::text[]),
                ('config', 'MasterSkillDefinition', '{}'::text[]),
                ('config', 'PowerUpDefinitionValue', array['AttributeRelationship']::text[]),
                ('config', 'Rectangle', '{}'::text[]),
                ('config', 'SimpleCraftingSettings', array['ItemCraftingRequiredItem', 'ItemCraftingResultItem']::text[]),
                ('config', 'SkillComboDefinition', array['SkillComboStep']::text[]),
                ('data', 'AppearanceData', array['ItemAppearance']::text[]),
                ('data', 'ItemStorage', array['Item']::text[])
        ) as v(schema_name, table_name, owned_tables)
    loop
        conditions := '';

        -- Every foreign key which points at the target, read from the catalog instead of a written
        -- list: one which is missed would delete rows which are in use. The tables of the target's
        -- own aggregate are skipped - they point at their owner, and an owner which only its own
        -- members still reference is exactly what is unreachable.
        for reference in
            select source_schema.nspname as source_schema,
                   source_table.relname as source_table,
                   source_column.attname as source_column,
                   target_column.attname as target_column
            from pg_constraint c
                join pg_class source_table on source_table.oid = c.conrelid
                join pg_namespace source_schema on source_schema.oid = source_table.relnamespace
                join pg_class referenced_table on referenced_table.oid = c.confrelid
                join pg_namespace referenced_schema on referenced_schema.oid = referenced_table.relnamespace
                join lateral unnest(c.conkey, c.confkey) as key_columns(source_attnum, target_attnum) on true
                join pg_attribute source_column
                    on source_column.attrelid = source_table.oid and source_column.attnum = key_columns.source_attnum
                join pg_attribute target_column
                    on target_column.attrelid = referenced_table.oid and target_column.attnum = key_columns.target_attnum
            where c.contype = 'f'
              and referenced_schema.nspname::text = target.schema_name
              and referenced_table.relname::text = target.table_name
              and not (source_table.relname::text = any (target.owned_tables))
        loop
            conditions := conditions || format(
                ' and not exists (select 1 from %I.%I r where r.%I = x.%I)',
                reference.source_schema, reference.source_table, reference.source_column, reference.target_column);
        end loop;

        if conditions = '' then
            -- Nothing points at this table at all, so an orphan cannot be told from a row in use.
            continue;
        end if;

        execute format('delete from %I.%I x where true%s', target.schema_name, target.table_name, conditions);
    end loop;
end $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to do - the removed rows were unreachable and can't be restored.
        }
    }
}
