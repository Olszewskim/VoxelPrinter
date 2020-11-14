using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class GameResourcesDatabase : Singleton<GameResourcesDatabase> {
    [SerializeField] private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;

    public Material _lockedFigureMaterial;

    public static List<VoxelFigureData> GetVoxelFiguresCollection(CollectionType collectionType) {
        if (Instance._voxelFiguresDataDictionary.ContainsKey(collectionType)) {
            return Instance._voxelFiguresDataDictionary[collectionType];
        }

        return null;
    }
}
