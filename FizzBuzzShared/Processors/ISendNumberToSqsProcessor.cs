using FizzBuzzShared.Models;

namespace FizzBuzzShared.Processors;

public interface ISendNumberToSqsProcessor
{
    Task Send(FizzBuzzModel model, string queueUrl);
}