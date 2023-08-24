using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using FizzBuzzShared.Models;

namespace FizzBuzzShared.Processors;

public sealed class SendNumberToSqsProcessor : ISendNumberToSqsProcessor
{
    public async Task Send(FizzBuzzModel fizzBuzzModel, string queueUrl)
    {
        if (string.IsNullOrEmpty(queueUrl))
        {
            return;
        }
        
        try
        {
            var sendMessageRequest = GetSendMessageRequest(fizzBuzzModel, queueUrl);
            using AmazonSQSClient amazonSqsClient = new AmazonSQSClient();
            await amazonSqsClient.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"Exception trying to send message to queue! Exception: {ex.Message}");
        }
    }

    private static SendMessageRequest GetSendMessageRequest(FizzBuzzModel model, string queueUrl)
    {
        var body = JsonSerializer.Serialize(model);
        return new SendMessageRequest{
            QueueUrl = queueUrl,
            MessageBody = body
        };
    }
}