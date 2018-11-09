//   Copyright Nexus Kharsun Sergey
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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