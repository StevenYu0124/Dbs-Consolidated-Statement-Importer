namespace DbsConsolidatedStatementImporter.Models;

public record Transaction
{
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}
