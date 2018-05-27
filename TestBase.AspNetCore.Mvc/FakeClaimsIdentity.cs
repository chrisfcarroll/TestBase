using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace TestBase
{
    public class FakeIdentity : ClaimsIdentity
    {
        public bool IsAuthenticatedValue;
        public readonly List<Claim> ClaimsValue = new List<Claim>();

        public FakeIdentity WithClaim(Claim claim)
        {
            AddClaim(claim);
            return this;
        }

        public FakeIdentity() : base(new GenericIdentity(typeof(FakeIdentity).Name))
        {
        }

        public FakeIdentity(string name) : base(new GenericIdentity(name))
        {
        }

        public FakeIdentity(GenericIdentity identity) : base(identity)
        {
        }

        public override bool IsAuthenticated
        {
            get { return IsAuthenticatedValue; }
        }

        public override IEnumerable<Claim> Claims
        {
            get { return ClaimsValue; }
        }

        public override void AddClaim(Claim claim) => ClaimsValue.Add(claim);
        public override void AddClaims(IEnumerable<Claim> claims) => ClaimsValue.AddRange(claims);
        public override bool TryRemoveClaim(Claim claim) => ClaimsValue.Remove(claim);
    }
}