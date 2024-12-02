using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Utils;
using Object = UnityEngine.Object;

namespace Serializers {
    /// <summary>
    /// Serializable object's material.
    /// </summary>
    [Serializable]
    public class SMesh : SComponent {
        [JsonProperty] private List<int[]> _triangles = new();

        [JsonProperty] private SVector3[] _vertex;
        [JsonProperty] private SVector3[] _normal;
        [JsonProperty] private SVector4[] _tangent;
        [JsonProperty] private SVector2[] _uv;

        [JsonProperty] private SVector3 _boundsCenter;
        [JsonProperty] private SVector3 _boundsSize;

        /// <summary>
        /// Empty constructor required to deserialize the component.
        /// </summary>
        public SMesh() { }

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="mesh">object's mesh</param>
        public SMesh(Mesh mesh) {
            Name = mesh.name;
            Id = Ids.GetID(Name);

            for (var i = 0; i < mesh.subMeshCount; ++i) {
                _triangles.Add(mesh.GetIndices(i));
            }
            // _triangles = mesh.GetIndices(0);

            _vertex = mesh.vertices.Select(a => (SVector3) a).ToArray();
            _normal = mesh.normals.Select(a => (SVector3) a).ToArray();
            _tangent = mesh.tangents.Select(a => (SVector4) a).ToArray();
            _uv = mesh.uv.Select(a => (SVector2) a).ToArray();

            _boundsCenter = mesh.bounds.center;
            _boundsSize = mesh.bounds.size;
        }

        /// <summary>
        /// Build the mesh using the fields in the class.
        /// </summary>
        /// <returns>object's mesh</returns>
        public Mesh GetMesh() {
            var mesh = new Mesh {
                name = Name,
                bounds = new Bounds {
                    center = _boundsCenter,
                    size = _boundsSize
                }
            };

            mesh.SetVertices(_vertex.Select(a => (Vector3) a).ToArray());
            mesh.SetNormals(_normal.Select(a => (Vector3) a).ToArray());
            mesh.SetTangents(_tangent.Select(a => (Vector4) a).ToArray());
            mesh.SetUVs(0, _uv.Select(a => (Vector2) a).ToArray());

            for (var i = 0; i < _triangles.Count; i++) {
                if (i > 0) mesh.subMeshCount++;
                mesh.SetTriangles(_triangles[i], i);
            }

            return mesh;
        }


        /// <inheritdoc />
        /// Before attaching it, the mesh is built by the GetMesh() method.
        public override void AttachTo(SObject sObj) {
            var go = sObj.Obj;
            
            var filter = go.GetComponent<MeshFilter>();
            filter.mesh = GetMesh();
            
            var collider = go.GetComponent<MeshCollider>();
            collider.sharedMesh = GetMesh();
            collider.convex = sObj.IsMovable;
            
            // Workaround: if present, set Mesh Collider for interactable
            // Needed because Mesh arrives later than RPC.
            if (go.TryGetComponent<XRGrabInteractable>(out var grab))
                grab.colliders.Add(collider);
        }

        /// <inheritdoc />
        public override byte[] Serialize() {
            var json = JsonConvert.SerializeObject(this, SerialSettings);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}