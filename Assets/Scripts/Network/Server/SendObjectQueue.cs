using System.Collections.Generic;
using System.Linq;
using Network.Objects;
using Priority_Queue;
using UnityEngine;

namespace Network.Server {
    /// <summary>
    /// Queues for the outgoing objects, one for each Client.
    /// </summary>
    public class SendObjectQueue : MonoBehaviour {
        private readonly Dictionary<ulong, SimplePriorityQueue<GameObject>> _queues = new();

        /// <summary>
        /// Has set of the currently connected Clients.
        /// </summary>
        public HashSet<ulong> Clients { get; } = new();

        /// <summary>
        /// Enqueue new object on the Client's queue.
        /// </summary>
        /// <param name="client">client id</param>
        /// <param name="go">gameObject</param>
        /// <param name="priority">priority in queue</param>
        public void Add(ulong client, GameObject go, int priority) {
            // Debug.LogError($"{go.name} : {priority}");
            if (!_queues[client].Contains(go)) {
                var x = Prefs.Singleton.priorityQueue ? priority : 0;
                _queues[client].Enqueue(go, x);
                // Debug.LogError($"P: {x}");
            }
        }

        /// <summary>
        /// Delete object from the Client's queue
        /// </summary>
        /// <param name="client">client id</param>
        /// <param name="go">gameObject</param>
        public void Delete(ulong client, GameObject go) {
            if (_queues[client].Contains(go))
                _queues[client].Remove(go);
        }

        /// <summary>
        /// Get object with the highest priority from the Client's queue.
        /// </summary>
        /// <param name="client">client id</param>
        /// <returns>gameObject with the highest priority</returns>
        public GameObject Get(ulong client) {
            return _queues[client]?.Dequeue();
        }

        public void UpdatePriority(ulong client, GameObject go, int newPriority) 
        {
            if (_queues[client].Contains(go))
            {
                _queues[client].UpdatePriority(go, newPriority);
            }
        }

        /// <summary>
        /// Lenght of the Client's queue.
        /// </summary>
        /// <param name="client">client id</param>
        /// <returns>lenght of the queue, 0 if not existing</returns>
        public int Size(ulong client) {
            return !_queues.ContainsKey(client)
                ? 0
                : _queues[client].Count;
        }

        /// <summary>
        /// Add new empty Client's queue.
        /// </summary>
        /// <param name="client">client id</param>
        public void AddClient(ulong client) {
            Clients.Add(client);
            _queues[client] = new SimplePriorityQueue<GameObject>(
                (f1, f2) => f1.CompareTo(f2) // inverse order comparison?
            );
        }

        /// <summary>
        /// Remove Client's queue.
        /// </summary>
        /// <param name="client">client id</param>
        public void RemoveClient(ulong client) {
            Clients.Remove(client);
            _queues.Remove(client);
        }
    }
}