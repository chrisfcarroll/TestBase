using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace TestBase
{
    public class FakeIdentity : ClaimsIdentity
    {
        public readonly List<Claim> ClaimsValue = new List<Claim>();
        public bool IsAuthenticatedValue;

        public FakeIdentity() : base(new GenericIdentity(typeof(FakeIdentity).Name)) { }

        public FakeIdentity(string name) : base(new GenericIdentity(name)) { }

        public FakeIdentity(GenericIdentity identity) : base(identity) { }

        public override bool IsAuthenticated => IsAuthenticatedValue;

        public override IEnumerable<Claim> Claims => ClaimsValue;

        public FakeIdentity WithClaim(Claim claim)
        {
            AddClaim(claim);
            return this;
        }

        public override void AddClaim(Claim claim) { ClaimsValue.Add(claim); }

        public override void AddClaims(IEnumerable<Claim> claims) { ClaimsValue.AddRange(claims); }

        public override bool TryRemoveClaim(Claim claim) { return ClaimsValue.Remove(claim); }
    }
}
