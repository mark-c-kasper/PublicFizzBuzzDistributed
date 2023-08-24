using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using FizzBuzzShared.Models;

namespace LambdaASP.FizzBuzzApi.Processors;

public sealed class ReadResponseQueueProcessor : IReadResponseQueueProcessor
{
    public async Task<string> GetResponseFromResultQueue(string resultQueueUrl)
    {
        using AmazonSQSClient amazonSqsClient = new AmazonSQSClient();
        var receiveMessageResponse = await amazonSqsClient.ReceiveMessageAsync(GetReceiveMessageRequest(resultQueueUrl));
        return ProcessReceiveMessageResponse(receiveMessageResponse);
    }

    private static ReceiveMessageRequest GetReceiveMessageRequest(string resultQueueUrl)
    {
        return new ReceiveMessageRequest
        {
            QueueUrl = resultQueueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 20
        };
    }

    private static string ProcessReceiveMessageResponse(ReceiveMessageResponse receiveMessageResponse)
    {
        if (receiveMessageResponse.Messages is null)
        {
            return string.Empty;
        }

        var message = receiveMessageResponse.Messages.First();
        
        LambdaLogger.Log($"Message Body: {message.Body}");
        
        var result = JsonSerializer.Deserialize<FizzBuzzModel>(message.Body);
        return result?.Response ?? "Not a Fizz, Buzz, or FizzBuzz!  Too sad, try again!";
    }
}