using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToOrderAndVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProductVariants",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Orders",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Orders");
        }
    }
}
