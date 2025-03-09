using UnityEngine;

namespace PackageAnysound.Runtime
{
    [CreateAssetMenu(fileName = "AnysoundPack", menuName = "Anysound/AnysoundPack")]

    public class AnysoundPack : ScriptableObject
    {
        [SerializeField] private string soundPackName;
        [field: SerializeField] public Anysound[] sounds { get; private set; }
        
        
    }
}
