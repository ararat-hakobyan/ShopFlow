using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopFlow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = '0' WHERE [Status] = 'Pending'");
            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = '1' WHERE [Status] = 'Out for Delivery'");
            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = '2' WHERE [Status] = 'Delivered'");
            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = '3' WHERE [Status] = 'Rejected'");

           
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
