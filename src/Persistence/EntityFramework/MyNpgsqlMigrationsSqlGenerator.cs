// <copyright file="MyNpgsqlMigrationsSqlGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

/// <summary>
/// A <see cref="NpgsqlMigrationsSqlGenerator"/> which takes care of required database roles.
/// </summary>
internal class MyNpgsqlMigrationsSqlGenerator : NpgsqlMigrationsSqlGenerator
{
    /// <summary>
    /// A set which contains the names of tables of the data-schema which are required by the config-Role.
    /// </summary>
    private static readonly IReadOnlySet<string> DataTablesRequiredByConfigRole = new HashSet<string>
    {
        nameof(Item),
        nameof(ItemOptionLink),
        nameof(ItemItemOfItemSet),
        nameof(ItemStorage),
    };

    /// <summary>
    /// A set which contains the names of tables of the data-schema which are required by the guild-Role.
    /// </summary>
    private static readonly IReadOnlySet<string> DataTablesRequiredByGuildRole = new HashSet<string>
    {
        nameof(Character),
    };

    /// <summary>
    /// A set which contains the names of tables of the data-schema which are required by the friend-Role.
    /// </summary>
    private static readonly IReadOnlySet<string> DataTablesRequiredByFriendRole = new HashSet<string>
    {
        nameof(Character),
    };

    /// <summary>
    /// A dictionary which contains the names of the schemas as keys for the corresponding <see cref="DatabaseRole"/>s.
    /// </summary>
    private static readonly IDictionary<string, DatabaseRole> DatabaseRoles = new Dictionary<string, DatabaseRole>
    {
        { SchemaNames.AccountData, DatabaseRole.Account },
        { SchemaNames.Configuration, DatabaseRole.Configuration },
        { SchemaNames.Guild, DatabaseRole.Guild },
        { SchemaNames.Friend, DatabaseRole.Friend },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="MyNpgsqlMigrationsSqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">The dependencies of the generator.</param>
    /// <param name="npgsqlOptions">The options.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "We are extending internals of the NpgsqlMigrationsSqlGenerator.")]
    public MyNpgsqlMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, npgsqlOptions)
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }
    }

    /// <summary>
    ///     <para>
    ///         Can be overridden by database providers to build commands for the given <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.EnsureSchemaOperation" />
    ///         by making calls on the given <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationCommandListBuilder" />.
    ///     </para>
    ///     <para>
    ///         Note that the default implementation of this method throws <see cref="T:System.NotImplementedException" />. Providers
    ///         must override if they are to support this kind of operation.
    ///     </para>
    /// </summary>
    /// <param name="operation"> The operation. </param>
    /// <param name="model"> The target model which may be <see langword="null" /> if the operations exist without a model. </param>
    /// <param name="builder"> The command builder to use to build the commands. </param>
    protected override void Generate(EnsureSchemaOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        base.Generate(operation, model, builder);
        var schemaName = operation.Name;
        if (DatabaseRoles.TryGetValue(schemaName, out var databaseRole))
        {
            var roleName = ConnectionConfigurator.GetRoleName(databaseRole);

            builder
                .AppendLine($"""
                    DO
                    $do$
                    BEGIN
                       IF EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = '{roleName}') THEN 
                          RAISE NOTICE 'Role "{roleName}" already exists. Skipping.';
                       ELSE
                          CREATE ROLE {roleName} WITH LOGIN PASSWORD '{ConnectionConfigurator.GetRolePassword(databaseRole)}';
                       END IF;
                    END
                    $do$;
                """)
                .EndCommand()
                .AppendLine($"GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA {schemaName} TO GROUP {roleName};")
                .EndCommand()
                .AppendLine($"ALTER DEFAULT PRIVILEGES IN SCHEMA {schemaName} GRANT ALL ON TABLES TO {roleName};")
                .EndCommand()
                .AppendLine($"GRANT USAGE ON SCHEMA {schemaName} TO GROUP {roleName};")
                .EndCommand();
        }
    }

    /// <summary>
    ///     Builds commands for the given <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.DropSchemaOperation" /> by making calls on the given
    ///     <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationCommandListBuilder" />, and then terminates the final command.
    /// </summary>
    /// <param name="operation"> The operation. </param>
    /// <param name="model"> The target model which may be <see langword="null" /> if the operations exist without a model. </param>
    /// <param name="builder"> The command builder to use to build the commands. </param>
    protected override void Generate(DropSchemaOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        base.Generate(operation, model, builder);
        var schemaName = operation.Name;
        if (DatabaseRoles.TryGetValue(schemaName, out var databaseRole))
        {
            var roleName = ConnectionConfigurator.GetRoleName(databaseRole);

            builder
                .AppendLine($"DROP ROLE IF EXISTS {roleName};")
                .EndCommand();
        }
    }

    /// <summary>
    ///     Builds commands for the given <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.CreateTableOperation" /> by making calls on the given
    ///     <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationCommandListBuilder" />.
    /// </summary>
    /// <param name="operation"> The operation. </param>
    /// <param name="model"> The target model which may be <see langword="null" /> if the operations exist without a model. </param>
    /// <param name="builder"> The command builder to use to build the commands. </param>
    /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
    protected override void Generate(CreateTableOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
    {
        base.Generate(operation, model, builder, terminate);
        if (operation.Schema != "data")
        {
            return;
        }

        if (DataTablesRequiredByConfigRole.Contains(operation.Name))
        {
            var configRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Configuration);
            builder
                .AppendLine($"GRANT SELECT ON TABLE data.\"{operation.Name}\" TO GROUP {configRoleName};")
                .EndCommand()
                .AppendLine($"GRANT USAGE ON SCHEMA data TO GROUP {configRoleName};")
                .EndCommand();
        }

        if (DataTablesRequiredByGuildRole.Contains(operation.Name))
        {
            var guildRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Guild);
            builder
                .AppendLine($"GRANT SELECT ON TABLE data.\"{operation.Name}\" TO GROUP {guildRoleName};")
                .EndCommand()
                .AppendLine($"GRANT USAGE ON SCHEMA data TO GROUP {guildRoleName};")
                .EndCommand();
        }

        if (DataTablesRequiredByFriendRole.Contains(operation.Name))
        {
            var friendRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Friend);
            builder
                .AppendLine($"GRANT SELECT ON TABLE data.\"{operation.Name}\" TO GROUP {friendRoleName};")
                .EndCommand()
                .AppendLine($"GRANT USAGE ON SCHEMA data TO GROUP {friendRoleName};")
                .EndCommand();
        }
    }
}