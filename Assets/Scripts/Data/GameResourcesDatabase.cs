using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class GameResourcesDatabase : Singleton<GameResourcesDatabase> {
    [SerializeField] private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;
    [SerializeField] private List<CollectionData> _collectionsData = new List<CollectionData>();

    [SerializeField] private Material _lockedFigureMaterial;
    [SerializeField] private float _grayScale;

    [SerializeField] private Transform _bookcaseFiguresRoot;

    private Dictionary<Color, Material> _voxelColorsMap = new Dictionary<Color, Material>();
    private Dictionary<string, VoxelFigureData> _voxelFiguresDataByNameDictionary =
        new Dictionary<string, VoxelFigureData>();

    private Dictionary<string, VoxelFigure> _bookcaseFiguresPool = new Dictionary<string, VoxelFigure>();

    protected override void Awake() {
        base.Awake();
        if (Instance != this) {
            return;
        }

        foreach (var data in _voxelFiguresDataDictionary) {
            foreach (var voxelFigure in data.Value) {
                _voxelFiguresDataByNameDictionary.Add(voxelFigure.figureID, voxelFigure);
            }
        }

        DontDestroyOnLoad(_bookcaseFiguresRoot);
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

    public static Material GetGrayscaledMaterial(Color voxelColor) {
        var grayScaleColor = new Color(Instance._grayScale * voxelColor.r, Instance._grayScale * voxelColor.b,
            Instance._grayScale * voxelColor.b);
        return GetMaterialOfColor(grayScaleColor);
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
        if (collection != null) {
            return collection.collectionName;
        }

        return "";
    }

    public static VoxelFigure GetBookcaseFigure(VoxelFigureData voxelFigureData) {
        if (!Instance._bookcaseFiguresPool.ContainsKey(voxelFigureData.figureID)) {
            Instance._bookcaseFiguresPool.Add(voxelFigureData.figureID,
                Instantiate(voxelFigureData.voxelFigure, Instance._bookcaseFiguresRoot));
        }

        return Instance._bookcaseFiguresPool[voxelFigureData.figureID];
    }
}
