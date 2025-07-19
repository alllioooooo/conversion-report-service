using System.Text;
using ConversionReportService.Application.Contracts.Dtos;
using ConversionReportService.Client.Clients;
using ConversionReportService.Client.Models;
using ConversionReportService.Infrastructure.Kafka.Producer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConversionReportService.Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversionReportController : ControllerBase
{
    private readonly IKafkaMessageProducer<long, CreateReport.Request> _producer;
    private readonly ConversionReportGrpcClient _grpcClient;

    public ConversionReportController(
        IKafkaMessageProducer<long, CreateReport.Request> producer,
        ConversionReportGrpcClient grpcClient)
    {
        _producer = producer;
        _grpcClient = grpcClient;
    }

    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequestModel request, CancellationToken cancellationToken)
    {
        var registrationId = GenerateRegistrationId(request.ItemId);

        var kafkaMessage = new CreateReport.Request(
            RegistrationId: registrationId,
            ItemId: request.ItemId,
            DateFrom: request.DateFrom,
            DateTo: request.DateTo
        );

        var message = new KafkaProducerMessage<long, CreateReport.Request>(registrationId, kafkaMessage);

        await _producer.ProduceAsync(message, cancellationToken);

        return Accepted(new { RegistrationId = registrationId });
    }

    [HttpGet("{registrationId:long}")]
    [ProducesResponseType(typeof(GetReportResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReport(long registrationId, CancellationToken cancellationToken)
    {
        if (registrationId <= 0)
            return BadRequest("Invalid registrationId");

        var grpcResponse = await _grpcClient.GetReportAsync(registrationId, cancellationToken);

        var response = new GetReportResponseModel
        {
            Status = grpcResponse.Status.ToString(),
            Ratio = grpcResponse.Ratio,
            PayedAmount = grpcResponse.PayedAmount
        };

        return Ok(response);
    }

    private static long GenerateRegistrationId(long itemId)
    {
        var timestamp = DateTime.UtcNow.ToString("HHmmssfff");
        var combined = $"{itemId}{timestamp}";
        var registrationId = long.Parse(combined);
        return registrationId;
    }
}