namespace Infrastructure.Services.Identity
{
    public interface IAuthenticatedIdentity
    {
        string UserName { get; }
        string Domain { get; }
    }
}
