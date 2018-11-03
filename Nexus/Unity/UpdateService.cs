using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Unity
{
    [HideInInspector]
    public class UpdateService : MonoBehaviour
    {
        private readonly HashSet<IUpdated> _hashSet = new HashSet<IUpdated>();
        private readonly List<IUpdated> _updateds = new List<IUpdated>();
        private static UpdateService _updateService;
        private static UpdateService Instance => _updateService ?? (_updateService = Create());

        private static UpdateService Create()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.name = "UpdateService";
            obj.GetComponent<Renderer>().enabled = false;
            obj.GetComponent<Collider>().enabled = false;
            var updateService = obj.AddComponent<UpdateService>();
            return updateService;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static void Add(IUpdated updated)
        {
            if (!Instance._hashSet.Contains(updated))
            {
                Instance._hashSet.Add(updated);
                Instance._updateds.Add(updated);
            }
        }

        public static void Remove(IUpdated updated)
        {
            if (Instance._hashSet.Contains(updated))
            {
                Instance._hashSet.Remove(updated);
                Instance._updateds.Remove(updated);
            }
        }

        protected virtual void Update()
        {
            for (var index = 0; index < _updateds.Count; index++)
                _updateds[index].Update();
        }
    }
}