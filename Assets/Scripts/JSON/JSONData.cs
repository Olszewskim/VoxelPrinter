using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class PlayerSaveJSON {
    public Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> voxelsData =
        new Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>>();

    public PlayerSaveJSON() {
    }

    public PlayerSaveJSON(Dictionary<CollectionType, Dictionary<string, VoxelFigureInfoData>> voxelsData) {
        foreach (var voxelData in voxelsData) {
            this.voxelsData.Add(voxelData.Key, new Dictionary<string, VoxelFigureInfoData>());
            var vData = voxelData.Value.Values.ToList();
            foreach (var data in vData) {
                this.voxelsData[voxelData.Key].Add(data.figureID, data);
            }
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
