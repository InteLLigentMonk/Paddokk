using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Paddokk.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarMakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarMakes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CarMakeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarModels_CarMakes_CarMakeId",
                        column: x => x.CarMakeId,
                        principalTable: "CarMakes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndYear = table.Column<int>(type: "integer", nullable: true),
                    CarModelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarGenerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarGenerations_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    JourneyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyLikes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPostImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JourneyPostId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPostImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JourneyPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JourneyId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TextContent = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Journeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserCarId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubscriptionTier = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    DefaultActiveJourneyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Journeys_DefaultActiveJourneyId",
                        column: x => x.DefaultActiveJourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JourneySubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    JourneyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneySubscriptions_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JourneySubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JourneyPostId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostComments_JourneyPosts_JourneyPostId",
                        column: x => x.JourneyPostId,
                        principalTable: "JourneyPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CarMakeId = table.Column<int>(type: "integer", nullable: false),
                    CarModelId = table.Column<int>(type: "integer", nullable: false),
                    CarGenerationId = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PrimaryImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCars_CarGenerations_CarGenerationId",
                        column: x => x.CarGenerationId,
                        principalTable: "CarGenerations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCars_CarMakes_CarMakeId",
                        column: x => x.CarMakeId,
                        principalTable: "CarMakes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCars_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCarImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserCarId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MediumUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCarImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCarImages_UserCars_UserCarId",
                        column: x => x.UserCarId,
                        principalTable: "UserCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CarMakes",
                columns: new[] { "Id", "Country", "Group", "Name" },
                values: new object[,]
                {
                    { 1, "Japan", 1, "Toyota" },
                    { 2, "Japan", 1, "Honda" },
                    { 3, "Japan", 1, "Nissan" },
                    { 4, "Japan", 1, "Mazda" },
                    { 5, "Japan", 1, "Subaru" },
                    { 6, "Germany", 2, "BMW" },
                    { 7, "Germany", 2, "Mercedes-Benz" },
                    { 8, "Germany", 2, "Audi" },
                    { 9, "Germany", 2, "Volkswagen" },
                    { 10, "Germany", 2, "Porsche" },
                    { 11, "USA", 3, "Ford" },
                    { 12, "USA", 3, "Chevrolet" },
                    { 13, "USA", 3, "Dodge" }
                });

            migrationBuilder.InsertData(
                table: "CarModels",
                columns: new[] { "Id", "CarMakeId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Supra" },
                    { 2, 1, "Corolla" },
                    { 3, 1, "86" },
                    { 4, 1, "MR2" },
                    { 5, 2, "Civic" },
                    { 6, 2, "S2000" },
                    { 7, 2, "NSX" },
                    { 8, 2, "Integra" },
                    { 9, 3, "240SX" },
                    { 10, 3, "GT-R" },
                    { 11, 3, "350Z" },
                    { 12, 3, "370Z" },
                    { 13, 6, "M3" },
                    { 14, 6, "M4" },
                    { 15, 6, "335i" },
                    { 16, 11, "Mustang" },
                    { 17, 11, "Focus" }
                });

            migrationBuilder.InsertData(
                table: "CarGenerations",
                columns: new[] { "Id", "CarModelId", "EndYear", "Name", "StartYear" },
                values: new object[,]
                {
                    { 1, 1, null, "A90", 2019 },
                    { 2, 1, 2002, "A80", 1993 },
                    { 3, 5, 2000, "EK", 1996 },
                    { 4, 5, 2005, "EP3", 2001 },
                    { 5, 9, 1994, "S13", 1989 },
                    { 6, 9, 1998, "S14", 1995 },
                    { 7, 13, 2006, "E46", 2000 },
                    { 8, 13, 2013, "E92", 2007 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarGenerations_CarModelId",
                table: "CarGenerations",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarMakes_Name",
                table: "CarMakes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarMakeId_Name",
                table: "CarModels",
                columns: new[] { "CarMakeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyLikes_JourneyId",
                table: "JourneyLikes",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyLikes_UserId_JourneyId",
                table: "JourneyLikes",
                columns: new[] { "UserId", "JourneyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPostImages_JourneyPostId",
                table: "JourneyPostImages",
                column: "JourneyPostId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPosts_JourneyId",
                table: "JourneyPosts",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyPosts_UserId",
                table: "JourneyPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_UserCarId",
                table: "Journeys",
                column: "UserCarId");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_UserId",
                table: "Journeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneySubscriptions_JourneyId",
                table: "JourneySubscriptions",
                column: "JourneyId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneySubscriptions_UserId_JourneyId",
                table: "JourneySubscriptions",
                columns: new[] { "UserId", "JourneyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_JourneyPostId",
                table: "PostComments",
                column: "JourneyPostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_UserId",
                table: "PostComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCarImages_UserCarId_IsPrimary",
                table: "UserCarImages",
                columns: new[] { "UserCarId", "IsPrimary" },
                unique: true,
                filter: "\"IsPrimary\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_CarGenerationId",
                table: "UserCars",
                column: "CarGenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_CarMakeId",
                table: "UserCars",
                column: "CarMakeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_CarModelId",
                table: "UserCars",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCars_UserId",
                table: "UserCars",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultActiveJourneyId",
                table: "Users",
                column: "DefaultActiveJourneyId");

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyLikes_Journeys_JourneyId",
                table: "JourneyLikes",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyLikes_Users_UserId",
                table: "JourneyLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyPostImages_JourneyPosts_JourneyPostId",
                table: "JourneyPostImages",
                column: "JourneyPostId",
                principalTable: "JourneyPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyPosts_Journeys_JourneyId",
                table: "JourneyPosts",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JourneyPosts_Users_UserId",
                table: "JourneyPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_UserCars_UserCarId",
                table: "Journeys",
                column: "UserCarId",
                principalTable: "UserCars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Users_UserId",
                table: "Journeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarGenerations_CarModels_CarModelId",
                table: "CarGenerations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_CarModels_CarModelId",
                table: "UserCars");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCars_CarMakes_CarMakeId",
                table: "UserCars");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Journeys_DefaultActiveJourneyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "JourneyLikes");

            migrationBuilder.DropTable(
                name: "JourneyPostImages");

            migrationBuilder.DropTable(
                name: "JourneySubscriptions");

            migrationBuilder.DropTable(
                name: "PostComments");

            migrationBuilder.DropTable(
                name: "UserCarImages");

            migrationBuilder.DropTable(
                name: "JourneyPosts");

            migrationBuilder.DropTable(
                name: "CarModels");

            migrationBuilder.DropTable(
                name: "CarMakes");

            migrationBuilder.DropTable(
                name: "Journeys");

            migrationBuilder.DropTable(
                name: "UserCars");

            migrationBuilder.DropTable(
                name: "CarGenerations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
