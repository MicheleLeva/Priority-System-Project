using System;
using System.Collections.Generic;
using System.Linq;
using Network.Objects;
using Network.Sync;
using Unity.Netcode;
using UnityEngine;

namespace Serializers {
    // public enum ComponentType {
    //     Default,
    //     Mesh,
    //     Texture,
    //     Material,
    //     Pivot
    // }

    public class SObject : INetworkSerializable {
        [NonSerialized] private GameObject _obj;
        
        private int _id;
        private string _name;
        private string _tag;
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _localScale;
        private int _priority;
        private int _parent;
        private bool _isActive;

        private bool _syncTransform;
        private bool _syncColor;
        private bool _syncLight;

        // Components
        private int[] _componentIds = new int[10];

        private int _componentNum;

        // Movable
        private bool _isMovable;
        private int _layer;

        public SObject() { }

        public string Name => _name;
        public int Id => _id;
        public GameObject Obj => _obj;
        public bool IsMovable => _isMovable;
        public int Priority => _priority;
        public int Parent => _parent;
        public bool IsActive => _isActive;
        public int Layer => _layer;

        public List<int> GetComponentIds() => _componentIds.ToList();

        public void AddComponent(SComponent component) {
            _componentIds[_componentNum] = component.Id;
            _componentNum++;
        }


        public SObject(GameObject obj /*, int priority, bool movable = false, bool isActive = false*/) {
            var netObj = obj.GetComponent<NetObject>();

            var parent = obj.transform.parent;
            _parent = parent != null
                ? parent.GetComponent<NetObject>().id
                : 0;

            _name = obj.name;
            _obj = obj;
            _id = netObj.id;
            _tag = obj.tag;
            _layer = obj.layer;
            var t = obj.transform;
            _position = t.position;
            _rotation = t.rotation;
            _localScale = t.localScale;
            _isMovable = netObj.isMovable;
            _priority = netObj.priority;

            _syncTransform = obj.TryGetComponent<SyncTransform>(out _);
            _syncColor = obj.TryGetComponent<SyncColor>(out _);
            _syncLight = obj.TryGetComponent<SyncLight>(out _);
        }

        public GameObject BuildObject() {
            if (_obj == null) {
                _obj = new GameObject {
                    name = Name,
                    tag = _tag,
                    transform = {
                        position = _position,
                        rotation = _rotation,
                        localScale = _localScale
                    }
                };
            }

            if (_syncTransform) _obj.AddComponent<SyncTransformClient>();
            if (_syncColor) _obj.AddComponent<SyncColorClient>();
            if (_syncLight) _obj.AddComponent<SyncLightClient>();

            return _obj;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _name);
            serializer.SerializeValue(ref _tag);
            serializer.SerializeValue(ref _position);
            serializer.SerializeValue(ref _rotation);
            serializer.SerializeValue(ref _localScale);
            // serializer.SerializeValue(ref _componentTypes);
            serializer.SerializeValue(ref _componentIds);
            serializer.SerializeValue(ref _componentNum);
            serializer.SerializeValue(ref _isMovable);
            serializer.SerializeValue(ref _priority);
            serializer.SerializeValue(ref _parent);
            serializer.SerializeValue(ref _layer);
            serializer.SerializeValue(ref _isActive);
            serializer.SerializeValue(ref _syncTransform);
            serializer.SerializeValue(ref _syncColor);
            serializer.SerializeValue(ref _syncLight);
        }

        public override string ToString() {
            return
                $"[{GetType()}: {_name}, {_tag}, {_position}, {_rotation}, {_localScale}, {_isMovable}, {_isActive}]";
        }
    }
}