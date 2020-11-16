using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class PlayerSaveJSON {
    public Dictionary<CollectionType, List<VoxelFigureInfoData>> voxelsData =
        new Dictionary<CollectionType, List<VoxelFigureInfoData>>();

    public PlayerSaveJSON() {
    }

    public PlayerSaveJSON(Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> voxelsData) {
        foreach (var voxelData in voxelsData) {
            this.voxelsData.Add(voxelData.Key, new List<VoxelFigureInfoData>());
            this.voxelsData[voxelData.Key].AddRange(voxelData.Value.Values.ToList());
        }
    }
}

public class VoxelFigureInfoData {
    public string figureID;
    public float completionPercent;
    public bool isCompleted;
    public bool isUnlocked;
    public Dictionary<Vector3, Color> voxelColors = new Dictionary<Vector3, Color>();

    public VoxelFigureInfoData() {
    }

    public VoxelFigureInfoData(string figureID) {
        this.figureID = figureID;
    }
}
