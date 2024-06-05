namespace Messages.CLI.Interfaces
{
    public interface ICliExecutor
    {
        Task<int> ExecuteAsync(string[] args);

    }
}
