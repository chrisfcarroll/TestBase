using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegalBricks.Matters.Contracts.Events;
using LegalBricks.Matters.Domain.Model;
using LegalBricks.Matters.Infrastructure;
using Moq;

namespace Tests.WebApi.TestFwk
{
    public static class GivenMockEventStore
    {
        static Mock<IEventStore> eventStoreMock;

        public static Mock<IEventStore> WithStream(IEventStream eventStream)
        {
            eventStoreMock = new Mock<IEventStore>();
            eventStoreMock
                .Setup(x => x.LoadStream(It.IsAny<string>()))
                .Returns(() => Task.FromResult(eventStream));
            return eventStoreMock;
        }

        public static Mock<IEventStore> WithStreamForMatterCreated(string faketenant, string fakeUserid, Guid matterid)
        {
            return WithStream(FakeEventStream.ForMatterCreated(faketenant, fakeUserid, matterid));
        }

        public static Mock<IEventStore> WithEmptyStream()
        {
            return WithStream(FakeEventStream.Empty);
        }
    }

    public class FakeEventStream : List<IEvent>, IEventStream
    {
        public long? Version { get; } = 0;

        public FakeEventStream(params IEvent[] events) { AddRange(events); }

        public static FakeEventStream ForMatterCreated(string tenantid, string userid, Guid matterid)
        {
            var matterCreated = new MatterCreated(tenantid, matterid, userid);
            return new FakeEventStream(matterCreated);
        }

        public static readonly FakeEventStream Empty = new FakeEventStream();
    }
}