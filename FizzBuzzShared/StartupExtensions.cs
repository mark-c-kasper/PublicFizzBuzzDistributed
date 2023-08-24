using FizzBuzzShared.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace FizzBuzzShared;

public static class StartupExtensions
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<ISendNumberToSqsProcessor, SendNumberToSqsProcessor>();
        services.AddScoped<ICreateResultQueueProcessor, CreateResultQueueProcessor>();
    }
}