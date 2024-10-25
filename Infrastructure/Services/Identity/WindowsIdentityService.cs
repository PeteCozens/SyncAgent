namespace Infrastructure.Services.Identity
{
    internal class WindowsIdentityService : IAuthenticatedIdentity
    {
        public override string ToString() => $"{Domain}\\{UserName}";

        public string UserName => Environment.UserName;

        public string Domain => Environment.UserDomainName;
    }
}
