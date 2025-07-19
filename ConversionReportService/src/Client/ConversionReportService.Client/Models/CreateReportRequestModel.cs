namespace ConversionReportService.Client.Models;

public class CreateReportRequestModel
{
    public long ItemId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}