using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAutomationExecutionRuleNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationExecutions_AutomationRules_RuleId",
                table: "AutomationExecutions");

            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "AutomationExecutions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationExecutions_AutomationRules_RuleId",
                table: "AutomationExecutions",
                column: "RuleId",
                principalTable: "AutomationRules",
                principalColumn: "RuleId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationExecutions_AutomationRules_RuleId",
                table: "AutomationExecutions");

            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "AutomationExecutions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationExecutions_AutomationRules_RuleId",
                table: "AutomationExecutions",
                column: "RuleId",
                principalTable: "AutomationRules",
                principalColumn: "RuleId");
        }
    }
}
