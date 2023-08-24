using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace LambdaASP.FizzBuzzApi.Processors;

public sealed class CreateResultQueueProcessor : ICreateResultQueueProcessor
{
    public async Task<string> CreateResultQueue()
    {
        try
        {
            using AmazonSQSClient amazonSqsClient = new AmazonSQSClient();
            var createQueueRequest = new CreateQueueRequest
            {
                QueueName = Guid.NewGuid().ToString()
            };

            var createQueueResponse = await amazonSqsClient.CreateQueueAsync(createQueueRequest);
            return createQueueResponse.QueueUrl;
        }
        catch (Exception ex)
        {
            LambdaLogger.Log("Issue trying to create queue!");
        }

        return string.Empty;
    }

    public async Task DeleteResultQueue(string queueUrl)
    {
        if (string.IsNullOrEmpty(queueUrl))
        {
            return;
        }
        
        using AmazonSQSClient amazonSqsClient = new AmazonSQSClient();
        var deleteQueueRequest = new DeleteQueueRequest
        {
            QueueUrl = queueUrl
        };

        var deleteQueueResponse = await amazonSqsClient.DeleteQueueAsync(deleteQueueRequest);
        Console.WriteLine(deleteQueueResponse.HttpStatusCode.ToString());
    }
}