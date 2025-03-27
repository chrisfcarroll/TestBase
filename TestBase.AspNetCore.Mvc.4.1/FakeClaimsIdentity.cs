using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace TestBase
{
    public class FakeClaimsIdentity : ClaimsIdentity
    {
        public readonly List<Claim> ClaimsValue = new List<Claim>();
        public bool IsAuthenticatedSet;

        public FakeClaimsIdentity() : base(new GenericIdentity(typeof(FakeClaimsIdentity).Name)) { }

        public FakeClaimsIdentity(string name) : base(new GenericIdentity(name)) { }

        public FakeClaimsIdentity(GenericIdentity identity) : base(identity) { }

        public override bool IsAuthenticated => IsAuthenticatedSet;

        public override IEnumerable<Claim> Claims => ClaimsValue;

        public FakeClaimsIdentity WithClaim(Claim claim)
        {
            AddClaim(claim);
            return this;
        }

        public override void AddClaim(Claim claim) { ClaimsValue.Add(claim); }

        public override void AddClaims(IEnumerable<Claim> claims) { ClaimsValue.AddRange(claims); }

        public override bool TryRemoveClaim(Claim claim) { return ClaimsValue.Remove(claim); }
    }
}
