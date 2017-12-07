// <copyright file="00000000000002_AddFriendView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System.Text;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Migration which adds the FriendView which is used by the <see cref="FriendViewItemRepository"/> and <see cref="FriendContext"/>.
    /// </summary>
    [DbContext(typeof(EntityDataContext))]
    [Migration("00000000000002_AddFriendView")]
    public partial class AddFriendView : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var viewText = new StringBuilder();
            viewText.Append("CREATE OR REPLACE VIEW data.\"FriendView\" AS ");
            viewText.Append("SELECT \"Id\", \"CharacterId\", \"FriendId\", \"Accepted\", \"RequestOpen\", ");
            viewText.Append("    (select \"Name\" From data.\"Character\" where \"Id\" = \"CharacterId\") AS \"CharacterName\", ");
            viewText.Append("    (select \"Name\" From data.\"Character\" where \"Id\" = \"FriendId\") AS \"FriendName\" ");
            viewText.Append("FROM data.\"FriendViewItem\" ");
            migrationBuilder.Sql(viewText.ToString());
            migrationBuilder.Sql("ALTER TABLE data.\"FriendView\" OWNER TO account");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW data.\"FriendView\"");
        }
    }
}