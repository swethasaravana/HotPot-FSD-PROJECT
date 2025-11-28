namespace HotPotAPI.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(int id, string name, string role);
    }
}
