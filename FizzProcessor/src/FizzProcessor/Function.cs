using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using FizzBuzzShared;
using FizzBuzzShared.Models;
using FizzBuzzShared.Processors;
using Microsoft.Extensions.DependencyInjection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FizzProcessor;

public class Function
{
    private ServiceCollection _serviceCollection = null!;
    
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        ConfigureServices();
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach (var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");

        var model = JsonSerializer.Deserialize<FizzBuzzModel>(message.Body);
        AddFizzIfNeeded(model);
        await SendMessageToNextQueue(model);
    }
    
    private void ConfigureServices()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddSharedServices();
    }

    private void AddFizzIfNeeded(FizzBuzzModel fizzBuzzModel)
    {
        if (fizzBuzzModel.Number % 3 != 0)
        {
            return;
        }
        
        fizzBuzzModel.Response = "Fizz";
    }

    private async Task SendMessageToNextQueue(FizzBuzzModel fizzBuzzModel)
    {
        await using var serviceProvider = _serviceCollection.BuildServiceProvider();
        var sendNumberToSqsProcessor = serviceProvider.GetRequiredService<ISendNumberToSqsProcessor>();
        await sendNumberToSqsProcessor.Send(fizzBuzzModel, GetQueueUrl());
    }
    
    private static string GetQueueUrl()
    {
        string? queueUrl = Environment.GetEnvironmentVariable(Constants.FizzBuzzBuzzQueue);

        if (string.IsNullOrEmpty(queueUrl))
        {
            throw new ApplicationException("Queue URL Not Found!");
        }

        return queueUrl;
    }
}