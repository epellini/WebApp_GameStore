using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualGameStore.Migrations
{
    public partial class Address_Default_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "ShippingAddresses",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "ShippingAddresses");
        }
    }
}
