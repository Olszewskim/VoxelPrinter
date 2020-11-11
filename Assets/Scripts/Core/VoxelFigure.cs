using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

[Serializable]
public class VoxelData {
    public Vector3 voxelPosition;
    public VoxelElement voxelElement;
    public Color voxelColor;

    public bool IsPrinted => voxelElement.IsPrinted;

    public VoxelData(Vector3 voxelPosition, VoxelElement voxelElement, Color voxelColor) {
        this.voxelPosition = voxelPosition;
        this.voxelElement = voxelElement;
        this.voxelColor = voxelColor;
    }

    public bool IsPrintedCorrectly() {
        return voxelElement.IsPrintedCorrectly(voxelColor);
    }
}

public class VoxelFigure : MonoBehaviour {
    public event Action<int> OnLayerChanged;

    [SerializeField] private List<VoxelData> _voxels;

    public bool IsCompleted => _currentPrintedElementIndex >= _voxels.Count;

    private int _currentLayer;
    private int _currentPrintedElementIndex;
    private Dictionary<Color, Material> _materialsMap = new Dictionary<Color, Material>();
    private Dictionary<Vector3, VoxelElement> _voxelElementsPositionsDictionary =
        new Dictionary<Vector3, VoxelElement>();
    private Vector3 _minPositions, _maxPositions;

    public void AddVoxel(Vector3 position, VoxelElement voxelElement, Color voxelColor) {
        if (_voxels == null) {
            _voxels = new List<VoxelData>();
        }

        _voxels.Add(new VoxelData(position, voxelElement, voxelColor));
    }

    public void Awake() {
        SortVoxels();
    }

    private void SortVoxels() {
        _voxels = _voxels.OrderBy(v => v.voxelPosition.y).ThenByDescending(v => v.voxelPosition.z)
            .ThenByDescending(v => v.voxelPosition.x)
            .ToList();

        foreach (var voxel in _voxels) {
            _voxelElementsPositionsDictionary.Add(voxel.voxelPosition, voxel.voxelElement);
        }

        var minX = _voxels.Min(v => v.voxelPosition.x);
        var maxX = _voxels.Max(v => v.voxelPosition.x);
        var minY = _voxels.Min(v => v.voxelPosition.y);
        var maxY = _voxels.Max(v => v.voxelPosition.y);
        var minZ = _voxels.Min(v => v.voxelPosition.z);
        var maxZ = _voxels.Max(v => v.voxelPosition.z);
        _minPositions = new Vector3(minX, minY, minZ);
        _maxPositions = new Vector3(maxX, maxY, maxZ);
    }

    public void Print(float printTime, Color printColor, Action onFinish) {
        var element = _voxels[_currentPrintedElementIndex];
        var material = _materialsMap[printColor];
        element.voxelElement.Print(printTime, material, printColor, onFinish);
        _currentPrintedElementIndex++;
    }

    public void ShowCurrentElementAndLayer() {
        if (_currentPrintedElementIndex >= _voxels.Count) {
            return;
        }

        var element = _voxels[_currentPrintedElementIndex];
        if (element.voxelPosition.y != _currentLayer) {
            _currentLayer = (int) element.voxelPosition.y;
            ShowCurrentLayer();
            OnLayerChanged?.Invoke(_currentLayer);
        }

        element.voxelElement.ShowCurrentElement();
    }

    public List<Color> GetFigureColors() {
        var colors = new HashSet<Color>();
        foreach (var v in _voxels) {
            if (colors.Add(v.voxelColor)) {
                _materialsMap.Add(v.voxelColor, v.voxelElement.GetMaterial());
            }
        }

        return colors.ToList();
    }

    private void ShowCurrentLayer() {
        var thisLayerVoxels = _voxels.Where(v => v.voxelPosition.y == _currentLayer).ToList();
        foreach (var voxel in thisLayerVoxels) {
            voxel.voxelElement.PrepareToPrint();
        }
    }

    public void TurnOffAllVoxels() {
        _currentLayer = -1;
        _currentPrintedElementIndex = 0;
        foreach (var v in _voxels) {
            v.voxelElement.Hide();
        }
    }

    public VoxelData GetCurrentElement() {
        if (_currentPrintedElementIndex >= _voxels.Count) {
            return null;
        }

        return _voxels[_currentPrintedElementIndex];
    }

    public (int min, int max) GetLayersIDs() {
        var minLayer = (int) _voxels.First().voxelPosition.y;
        var maxLayer = (int) _voxels.Last().voxelPosition.y;
        return (minLayer, maxLayer);
    }

    public float GetPrintProgress() {
        var printedElements = _voxels.Count(v => v.IsPrinted);
        return printedElements / (float) _voxels.Count;
    }

    public float GetPercentageOfCorrectVoxels() {
        var elementsPrintedCorrectly = _voxels.Count(v => v.IsPrintedCorrectly());
        return elementsPrintedCorrectly / (float) _voxels.Count;
    }

    public void DisableFigure() {
        foreach (var voxel in _voxels) {
            voxel.voxelElement.HideFrame();
            //  voxel.voxelElement.SetMaterial(GameResourcesDatabase.Instance._lockedFigureMaterial);
        }

        HideInvisibleVoxels();
    }

    private void HideInvisibleVoxels() {
        foreach (var voxel in _voxels) {
            var topNeighbour = GetNeighbour(NeighbourType.Top, voxel.voxelPosition);
            var leftNeighbour = GetNeighbour(NeighbourType.Left, voxel.voxelPosition);
            var frontNeighbour = GetNeighbour(NeighbourType.Front, voxel.voxelPosition);

            if (topNeighbour != null && leftNeighbour != null && frontNeighbour != null) {
                voxel.voxelElement.Disable();
            }
        }
    }

    private VoxelElement GetNeighbour(NeighbourType neighbourType, Vector3 voxelPosition) {
        var neighbourPosition = voxelPosition;
        switch (neighbourType) {
            case NeighbourType.Top:
                neighbourPosition.y += 1;
                break;
            case NeighbourType.Left:
                neighbourPosition.x -= 1;
                break;
            case NeighbourType.Front:
                neighbourPosition.z -= 1;
                break;
            case NeighbourType.Right:
                neighbourPosition.x += 1;
                break;
            case NeighbourType.Back:
                neighbourPosition.z += 1;
                break;
            case NeighbourType.Bottom:
                neighbourPosition.y -= 1;
                break;
        }

        if (_voxelElementsPositionsDictionary.ContainsKey(neighbourPosition)) {
            return _voxelElementsPositionsDictionary[neighbourPosition];
        }

        return null;
    }
}
