namespace FizzBuzzShared.Models;

public class FizzBuzzModel
{
    public int Number { get; init; }

    public string ResultQueue { get; init; } = null!;

    public string Response { get; set; } = null!;
}