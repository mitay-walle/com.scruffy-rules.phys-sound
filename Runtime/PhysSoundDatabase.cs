using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhysSound
{
    [CreateAssetMenu(menuName = "Phys Sound/Database")]
    public class PhysSoundDatabase : ScriptableObject
    {
        [SerializeField] private List<PhysSoundMaterial> _materials = new List<PhysSoundMaterial>();
        [SerializeField] private List<PhysSoundKey> _keys = new List<PhysSoundKey>();

        public Physic2DModule Physic2D = new Physic2DModule();
        public Physic3DModule Physic3D = new Physic3DModule();

        private void OnEnable() => Init();

        private void Reset() => Collect();

        public void Init()
        {
            foreach (PhysSoundMaterial physSoundMaterial in _materials)
            {
                physSoundMaterial.Init();
            }

            Physic3D.Init(_keys);
            Physic2D.Init(_keys);
        }

        [ContextMenu(("Collect all"))]
        private void Collect()
        {
            _materials = Collect<PhysSoundMaterial>();
            _keys = Collect<PhysSoundKey>();
        }

        private List<T> Collect<T>() where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.FindAssets($"t:{nameof(T)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
#endif
            return null;
        }
    }

    public class Physic3DModule
    {
#if PHYS_SOUND_3D

        private Dictionary<PhysicMaterial, PhysSoundKey> _keysMap = new Dictionary<PhysicMaterial, PhysSoundKey>();

        public PhysSoundKey GetSoundMaterial(PhysicMaterial physicMaterial)
        {
            if (_keysMap.TryGetValue(physicMaterial, out PhysSoundKey physSoundKey))
            {
                return physSoundKey;
            }

            return null;
        }
#endif
        public void Init(List<PhysSoundKey> keys)
        {
#if PHYS_SOUND_3D
            _keysMap.Clear();
            foreach (PhysSoundKey key in keys)
            {
                for (int i = 0; i < key.PhysicMterials.Count; i++)
                {
                    _keysMap.Add(key.PhysicMterials[i], key);
                }
            }
#endif
        }
    }

    public class Physic2DModule
    {
#if PHYS_SOUND_2D
        private Dictionary<PhysicsMaterial2D, PhysSoundKey> _keysMap2D =
 new Dictionary<PhysicsMaterial2D, PhysSoundKey>();

            public PhysSoundKey GetSoundMaterial(PhysicsMaterial2D physicMaterial)
        {
            if (_keysMap2D.TryGetValue(physicMaterial, out PhysSoundKey physSoundKey))
            {
                return physSoundKey;
            }

            return null;
        }
#endif
        public void Init(List<PhysSoundKey> keys)
        {
#if PHYS_SOUND_2D
            _keysMap2D.Clear();
            foreach (PhysSoundKey key in keys)
            {
                for (int i = 0; i < key.PhysicMterials2D.Count; i++)
                {
                    _keysMap2D.Add(key.PhysicMterials2D[i], key);
                }
            }
#endif
        }
    }
}