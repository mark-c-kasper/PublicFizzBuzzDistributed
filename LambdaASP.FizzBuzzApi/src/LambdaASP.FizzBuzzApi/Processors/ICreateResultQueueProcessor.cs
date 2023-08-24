namespace LambdaASP.FizzBuzzApi.Processors;

public interface ICreateResultQueueProcessor
{
    Task<string> CreateResultQueue();

    Task DeleteResultQueue(string queueUrl);
}