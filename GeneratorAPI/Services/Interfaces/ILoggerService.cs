namespace GeneratorAPI.Services.Interfaces
{
    public interface ILoggerService
    {
        Task Log(string message, bool insertNewLine = false);
    }
}