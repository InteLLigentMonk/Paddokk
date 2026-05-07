using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameOwnershipFKsToPrincipalAndAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyPosts_Users_UserId",
                table: "JourneyPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Users_UserId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_Users_UserId",
                table: "UserCars");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCars",
                newName: "PrincipalId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCars_UserId",
                table: "UserCars",
                newName: "IX_UserCars_PrincipalId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PostComments",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_UserId",
                table: "PostComments",
                newName: "IX_PostComments_AuthorId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Journeys",
                newName: "PrincipalId");

            migrationBuilder.RenameIndex(
                name: "IX_Journeys_UserId",
                table: "Journeys",
                newName: "IX_Journeys_PrincipalId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "JourneyPosts",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_JourneyPosts_UserId",
                table: "JourneyPosts",
                newName: "IX_JourneyPosts_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyPosts_Users_AuthorId",
                table: "JourneyPosts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Users_PrincipalId",
                table: "Journeys",
                column: "PrincipalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Users_AuthorId",
                table: "PostComments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCars_Users_PrincipalId",
                table: "UserCars",
                column: "PrincipalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JourneyPosts_Users_AuthorId",
                table: "JourneyPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Users_PrincipalId",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Users_AuthorId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_Users_PrincipalId",
                table: "UserCars");

            migrationBuilder.RenameColumn(
                name: "PrincipalId",
                table: "UserCars",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCars_PrincipalId",
                table: "UserCars",
                newName: "IX_UserCars_UserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "PostComments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_AuthorId",
                table: "PostComments",
                newName: "IX_PostComments_UserId");

            migrationBuilder.RenameColumn(
                name: "PrincipalId",
                table: "Journeys",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Journeys_PrincipalId",
                table: "Journeys",
                newName: "IX_Journeys_UserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "JourneyPosts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JourneyPosts_AuthorId",
                table: "JourneyPosts",
                newName: "IX_JourneyPosts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyPosts_Users_UserId",
                table: "JourneyPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Users_UserId",
                table: "PostComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCars_Users_UserId",
                table: "UserCars",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
