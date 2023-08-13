namespace MSU.HR.Services.Interfaces
{
    public interface ILogError
    {
        Task<int> SaveAsync(Exception ex, string body);
    }
}
