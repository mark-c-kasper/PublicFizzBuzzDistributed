using Amazon.Lambda.Core;
using FizzBuzzShared.Models;
using FizzBuzzShared.Processors;
using LambdaASP.FizzBuzzApi.Processors;
using Microsoft.AspNetCore.Mvc;

namespace LambdaASP.FizzBuzzApi.Controllers;

[Route("api/[controller]")]
public class FizzBuzzController : ControllerBase
{
    private readonly ISendNumberToSqsProcessor _sendNumberToSqsProcessor;
    private readonly ICreateResultQueueProcessor _createResultQueueProcessor;
    private readonly IReadResponseQueueProcessor _readResponseQueueProcessor;
    
    public FizzBuzzController(ISendNumberToSqsProcessor sendNumberToSqsProcessor,
        ICreateResultQueueProcessor createResultQueueProcessor,
        IReadResponseQueueProcessor readResponseQueueProcessor)
    {
        _sendNumberToSqsProcessor = sendNumberToSqsProcessor;
        _createResultQueueProcessor = createResultQueueProcessor;
        _readResponseQueueProcessor = readResponseQueueProcessor;
    }
    
    // GET
    [HttpGet("{number}")]
    public async Task<IActionResult> Get(int number)
    {
        string queueUrl = GetQueueUrl();
        string resultQueueUrl = await _createResultQueueProcessor.CreateResultQueue();
        var model = new FizzBuzzModel
        {
            Number = number,
            ResultQueue = resultQueueUrl
        };
        
        // string result = JsonSerializer.Serialize(model);
        await _sendNumberToSqsProcessor.Send(model, queueUrl);
        LambdaLogger.Log($"Result Queue URL: {resultQueueUrl}");
        string result = await _readResponseQueueProcessor.GetResponseFromResultQueue(resultQueueUrl);
        await _createResultQueueProcessor.DeleteResultQueue(resultQueueUrl);
        
        return this.Ok(result);
    }

    private static string GetQueueUrl()
    {
        string? queueUrl = Environment.GetEnvironmentVariable(Constants.FizzBuzzFizzQueue);

        if (string.IsNullOrEmpty(queueUrl))
        {
            throw new ApplicationException("Queue URL Not Found!");
        }

        return queueUrl;
    }
}