using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IWidget
    {
        public string Name { get; }
        public object View { get; }
    }

    public sealed record DataSubmittedEvent(string Data);

    public interface IEventAggregator
    {
        void Publish<TEvent>(TEvent eventToPublish);
        void Subscribe<TEvent>(Action<TEvent> eventHandler);
    }
}
