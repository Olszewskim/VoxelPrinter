using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class GameResourcesDatabase : Singleton<GameResourcesDatabase> {
    [SerializeField] private List<VoxelFigureData> _voxelFiguresData;
    private Dictionary<CollectionType, List<VoxelFigureData>> _voxelFiguresDataDictionary;

    public Material _lockedFigureMaterial;

    protected override void Awake() {
        base.Awake();
        InitVoxelFiguresDictionary();
    }

    private void InitVoxelFiguresDictionary() {
        _voxelFiguresDataDictionary = new Dictionary<CollectionType, List<VoxelFigureData>>();
        foreach (var voxelData in _voxelFiguresData) {
            if (!_voxelFiguresDataDictionary.ContainsKey(voxelData.collectionType)) {
                _voxelFiguresDataDictionary[voxelData.collectionType] = new List<VoxelFigureData>();
            }

            _voxelFiguresDataDictionary[voxelData.collectionType].Add(voxelData);
        }
    }

    public static List<VoxelFigureData> GetVoxelFiguresCollection(CollectionType collectionType) {
        if (Instance._voxelFiguresDataDictionary.ContainsKey(collectionType)) {
            return Instance._voxelFiguresDataDictionary[collectionType];
        }

        return null;
    }
}
