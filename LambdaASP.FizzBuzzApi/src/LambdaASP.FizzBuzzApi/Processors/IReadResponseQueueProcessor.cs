namespace LambdaASP.FizzBuzzApi.Processors;

public interface IReadResponseQueueProcessor
{
    Task<string> GetResponseFromResultQueue(string resultQueueUrl);
}