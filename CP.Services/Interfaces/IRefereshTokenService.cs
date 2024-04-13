namespace CP.Services.Interfaces
{
    public interface IRefereshTokenService
    {
        Task<string> GenerateToken(string username);
        bool TokenExists(string userId, string refreshtoken);
    }
}
