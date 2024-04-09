using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Serializers;

namespace Network.Client {
    /// <summary>
    /// Cache on Client to store already downloaded object's components.
    /// </summary>
    public class ComponentsCache : Dictionary<int, ComponentDesc> {
        private static ComponentsCache _instance;
        public static ComponentsCache Singleton => _instance ??= new ComponentsCache();

        /// <summary>
        /// Initialize new component as not downloaded.
        /// </summary>
        /// <param name="id">component id</param>
        public void InitComp(int id) => Add(id, new ComponentDesc());

        /// <summary>
        /// Set component as downloaded.
        /// </summary>
        /// <param name="id">component id</param>
        /// <param name="sComp">serialized component</param>
        public void SetComp(int id, SComponent sComp) {
            this[id].sComponent = sComp;
            this[id].downloaded = true;
            this[id].awaiting.ToList().ForEach(sComp.AttachTo);
        }

        /// <summary>
        /// Add object to component's awaiters.
        /// </summary>
        /// <param name="id">component id</param>
        /// <param name="awaiter">serialized object that awaits</param>
        public void AddAwaiter(int id, SObject awaiter) {
            this[id].awaiting.Add(awaiter);
        }

        /// <summary>
        /// Get component if present in cache.
        /// </summary>
        /// <param name="id">component id</param>
        /// <returns>component</returns>
        public SComponent GetComp(int id) => this[id].sComponent;

        /// <summary>
        /// Check if the components has been downloaded.
        /// </summary>
        /// <param name="id">component id</param>
        /// <returns>is component downloaded</returns>
        public bool IsDownloaded(int id) => this[id].downloaded;
    }

    /// <summary>
    /// Description of the component's state.
    /// </summary>
    public class ComponentDesc {
        /// <summary>
        /// Serialized component.
        /// </summary>
        [CanBeNull] public SComponent sComponent;
        
        /// <summary>
        /// If the asset has been downloaded or not.
        /// </summary>
        public bool downloaded;
        
        /// <summary>
        /// List of serialized objects awaiting for the download's completion.
        /// </summary>
        public readonly List<SObject> awaiting = new();
        
        public ComponentDesc() {
            sComponent = null;
            downloaded = false;
        }
    }
}