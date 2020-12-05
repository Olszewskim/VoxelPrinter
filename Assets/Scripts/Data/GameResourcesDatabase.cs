using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class GameResourcesDatabase : Singleton<GameResourcesDatabase> {
    [SerializeField] private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;
    [SerializeField] private List<CollectionData> _collectionsData = new List<CollectionData>();

    public Material _lockedFigureMaterial;

    private Dictionary<Color, Material> _voxelColorsMap = new Dictionary<Color, Material>();
    private Dictionary<string, VoxelFigureData> _voxelFiguresDataByNameDictionary =
        new Dictionary<string, VoxelFigureData>();

    protected override void Awake() {
        base.Awake();
        foreach (var data in _voxelFiguresDataDictionary) {
            foreach (var voxelFigure in data.Value) {
                _voxelFiguresDataByNameDictionary.Add(voxelFigure.figureID, voxelFigure);
            }
        }
    }

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

    public static string GetVoxelFigureName(string figureID) {
        if (Instance._voxelFiguresDataByNameDictionary.ContainsKey(figureID)) {
            return Instance._voxelFiguresDataByNameDictionary[figureID].figureName;
        }

        return "";
    }

    public static List<CollectionData> GetCollectionsData() {
        return Instance._collectionsData;
    }

    public static string GetCollectionName(CollectionType collectionType) {
        var collection = Instance._collectionsData.FirstOrDefault(c => c.collectionType == collectionType);
        if(collection != null) {
            return collection.collectionName;
        }

        return "";
    }
}
