using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0197d227-ed75-7ac5-af55-31c0e1b6000f", "0197d227-ed75-7ac5-af55-31c1246abd3a", true, false, "Member", "MEMBER" },
                    { "0197d227-ed75-7ac5-af55-31c2d55797c4", "0197d227-ed75-7ac5-af55-31c3691fbcee", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0197d227-ed75-7ac5-af55-31ba464a746d", 0, "0197d227-ed75-7ac5-af55-31bbf90fe5c9", "admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEPRNLdJ9K5egf32RiGlgY3TlB9FMiBqUJyVH3vP5AinCO0txjSbgKSPRnjzbuuOU1w==", null, false, "55BF92C9EF0249CDA210D85D1A851BC9", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 2, "permissions", "polls:add", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 3, "permissions", "polls:update", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 4, "permissions", "polls:delete", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 5, "permissions", "questions:read", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 6, "permissions", "questions:add", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 7, "permissions", "questions:update", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 8, "permissions", "users:read", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 9, "permissions", "users:add", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 10, "permissions", "users:update", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 11, "permissions", "roles:read", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 12, "permissions", "roles:add", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 13, "permissions", "roles:update", "0197d227-ed75-7ac5-af55-31c2d55797c4" },
                    { 14, "permissions", "results:read", "0197d227-ed75-7ac5-af55-31c2d55797c4" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0197d227-ed75-7ac5-af55-31c2d55797c4", "0197d227-ed75-7ac5-af55-31ba464a746d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0197d227-ed75-7ac5-af55-31c2d55797c4", "0197d227-ed75-7ac5-af55-31ba464a746d" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d");
        }
    }
}
