using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "CollectionData", menuName = "Data/CollectionData")]
public class CollectionData : ScriptableObject {
    public CollectionType collectionType;
    public string collectionName;
    public Sprite collectionImage;
    public int starsToUnlock;
}
