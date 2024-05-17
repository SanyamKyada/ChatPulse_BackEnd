using CP.Models.Models;
using System.Collections.Concurrent;

namespace CP.SignalR.DataService
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connections = new();

        public ConcurrentDictionary<string, UserConnection> connections => _connections;
    }
}
