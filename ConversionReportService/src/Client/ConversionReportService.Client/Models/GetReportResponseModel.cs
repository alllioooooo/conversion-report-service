namespace ConversionReportService.Client.Models;

public class GetReportResponseModel
{
    public string Status { get; set; } = null!;
    public double? Ratio { get; set; }
    public long? PayedAmount { get; set; }
}