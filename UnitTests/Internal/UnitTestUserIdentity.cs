using Infrastructure.Services.Identity;

namespace UnitTests.Internal
{
    public class UnitTestUserIdentity : IAuthenticatedIdentity
    {
        public override string ToString() => $"{Domain}\\{UserName}";

        public string UserName => "UnitTest";

        public string Domain => "test";
    }
}
