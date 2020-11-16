using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class GameResourcesDatabase : Singleton<GameResourcesDatabase> {
    [SerializeField] private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;

    public Material _lockedFigureMaterial;

    private Dictionary<Color, Material> _voxelColorsMap = new Dictionary<Color, Material>();

    public static List<VoxelFigureData> GetVoxelFiguresCollection(CollectionType collectionType) {
        if (Instance._voxelFiguresDataDictionary.ContainsKey(collectionType)) {
            return Instance._voxelFiguresDataDictionary[collectionType];
        }

        return null;
    }

    public static Material GetMaterialOfColor(Color voxelColor) {
        if (!Instance._voxelColorsMap.ContainsKey(voxelColor)) {
            var newMaterial = new Material(Instance._lockedFigureMaterial);
            newMaterial.color = voxelColor;
            Instance._voxelColorsMap.Add(voxelColor, newMaterial);
        }

        return Instance._voxelColorsMap[voxelColor];
    }
}
