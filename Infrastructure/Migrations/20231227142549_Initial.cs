using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "dbo",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    Decimals = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    MinorUnits = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    MajorSingle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    MajorPlural = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    MinorSingle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    MinorPlural = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysUpdatedByUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: "")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysUpdateHostMachine = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: "")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysIsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysRowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart");

            migrationBuilder.CreateTable(
                name: "Enums",
                schema: "dbo",
                columns: table => new
                {
                    SchemaName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enums", x => new { x.SchemaName, x.TableName, x.ColumnName, x.Value });
                });

            migrationBuilder.CreateTable(
                name: "FxRates",
                schema: "dbo",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    Rate = table.Column<decimal>(type: "decimal(20,8)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysUpdatedByUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: "")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysUpdateHostMachine = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: "")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysIsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysRowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart"),
                    SysPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FxRates", x => new { x.CurrencyId, x.FromDate });
                    table.ForeignKey(
                        name: "FK_FxRates_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enums",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FxRates",
                schema: "dbo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "FxRatesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "dbo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CurrenciesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysPeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysPeriodStart");
        }
    }
}
