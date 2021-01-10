using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

[Serializable]
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

[Serializable]
public class ColorData {
    public Vector3 pos;
    public float rValue;
    public float gValue;
    public float bValue;

    public ColorData() {
    }

    public ColorData(Vector3 pos, Color color) {
        this.pos = pos;
        rValue = color.r;
        gValue = color.g;
        bValue = color.b;
    }
}

[Serializable]
public class VoxelFigureInfoData {
    public string figureID;
    public float completionPercent;
    public bool isCompleted;
    public bool isUnlocked;
    public List<ColorData> voxelColors = new List<ColorData>();

    public VoxelFigureInfoData() {
    }

    public VoxelFigureInfoData(string figureID) {
        this.figureID = figureID;
    }

    public bool FinishModel(VoxelFigure finishedModel, float completionPercent) {
        isCompleted = true;

        if (completionPercent > this.completionPercent) {
            this.completionPercent = completionPercent;
            voxelColors = finishedModel.GetFigureColorsMap();
            return true;
        }

        return false;
    }

    public Dictionary<Vector3, Color> GetVoxelsColorDataDictionary() {
        var colorsData = new Dictionary<Vector3, Color>();
        foreach (var voxelColor in voxelColors) {
            colorsData.Add(voxelColor.pos, new Color(voxelColor.rValue, voxelColor.gValue, voxelColor.bValue));
        }

        return colorsData;
    }
}

[Serializable]
public class UnlockedShopItemsJSON {
    public HashSet<string> unlockedShopItems = new HashSet<string>();

    public UnlockedShopItemsJSON() {
    }

    public UnlockedShopItemsJSON(HashSet<string> unlockedShopItems) {
        this.unlockedShopItems = unlockedShopItems;
    }
}
