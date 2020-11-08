using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "VoxelFigureData", menuName = "Data/VoxelFigureData")]
public class VoxelFigureData : ScriptableObject {
    public string figureID;
    public CollectionType collectionType;
    public string figureName;
    public VoxelFigure voxelFigure;
}
