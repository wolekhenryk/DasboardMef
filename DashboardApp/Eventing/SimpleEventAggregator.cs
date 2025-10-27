using Contracts;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardApp.Eventing
{
    [Export(typeof(IEventAggregator))]
    [Shared]
    public sealed class SimpleEventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _subs = [];

        public void Publish<T>(T evt)
        {
            if (_subs.TryGetValue(typeof(T), out var list))
                foreach (var h in list.OfType<Action<T>>()) h(evt);
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            
            if (!_subs.TryGetValue(t, out var list)) 
                _subs[t] = list = [];
            
            list.Add(handler);
        }
    }
}
