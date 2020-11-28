using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public enum Direction {
    Left,
    Down,
    Right,
    Up,
    Other
}

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

    public bool IsCompleted { get; private set; } //tODO: check if completed
    public VoxelData CurrentElement { get; private set; }

    private int _currentLayer;
    private Dictionary<Vector3, VoxelElement> _voxelElementsPositionsDictionary =
        new Dictionary<Vector3, VoxelElement>();

    private List<VoxelData> _currentLayerElements = new List<VoxelData>();

    private Direction _currentDirection = Direction.Left;
    private float _extraScoreAmount = 0.1f;

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
    }

    public void Print(float printTime, Color printColor, Action onFinish) {
        CurrentElement?.voxelElement.Print(printTime, printColor, onFinish);
    }

    public void ShowCurrentElementAndLayer() {
        if (IsCompleted) {
            return;
        }

        CurrentElement = GetNextElement();
        CurrentElement?.voxelElement.ShowCurrentElement();
    }

    private VoxelData GetNextElement() {
        while (_currentLayerElements.Count == 0 && !IsCompleted) {
            InitAndShowNextLayer();
        }

        if (CurrentElement == null) {
            return GetFirstElementFromLayer();
        }

        if (!GetNeighbour(_currentDirection, CurrentElement.voxelPosition)) {
            ChangeDirection();
        }

        Debug.Log("========================");
        Debug.Log("Obecny element " + CurrentElement.voxelElement.name);
        Debug.Log("Idę w " + _currentDirection);
        var shorterDistance = float.MaxValue;
        var closestElement = CurrentElement;
        foreach (var layerElement in _currentLayerElements) {
            var direction = layerElement.voxelPosition - CurrentElement.voxelPosition;
            var directionOfCheckedElement = GetDirectionType(direction);
            var extraScore = directionOfCheckedElement == _currentDirection ? _extraScoreAmount : 0;

            if (extraScore > 0) {
                Debug.Log(layerElement.voxelElement.name + " spelnia wymagania");
            }

            var distance = direction.sqrMagnitude - extraScore;
            if (distance < shorterDistance) {
                shorterDistance = distance;
                closestElement = layerElement;
            }
        }

        _currentLayerElements.Remove(closestElement);
        return closestElement;
    }

    private Direction GetDirectionType(Vector3 direction) {
        if (direction.x == 0) {
            if (direction.z > 0) {
                return Direction.Up;
            }

            return Direction.Down;
        }

        if (direction.z == 0) {
            if (direction.x > 0) {
                return Direction.Right;
            }

            return Direction.Left;
        }

        return Direction.Other;
    }

    private void ChangeDirection() {
        switch (_currentDirection) {
            case Direction.Left:
                _currentDirection = Direction.Down;
                break;
            case Direction.Down:
                _currentDirection = Direction.Right;
                break;
            case Direction.Right:
                _currentDirection = Direction.Up;
                break;
            case Direction.Up:
                _currentDirection = Direction.Left;
                break;
        }
    }

    private VoxelData GetFirstElementFromLayer() {
        var element = _currentLayerElements.OrderByDescending(v => v.voxelPosition.z)
            .ThenByDescending(v => v.voxelPosition.x).First();
        _currentLayerElements.Remove(element);
        return element;
    }

    public List<Color> GetFigureColors() {
        return _voxels.Select(v => v.voxelColor).Distinct().ToList();
    }

    private void InitAndShowNextLayer() {
        if (GetPrintProgress() >= 1f) {
            IsCompleted = true;
            return;
        }

        _currentLayer++;
        _currentLayerElements = _voxels.Where(v => v.voxelPosition.y == _currentLayer).ToList();
        foreach (var voxel in _currentLayerElements) {
            voxel.voxelElement.PrepareToPrint();
        }

        CurrentElement = null;
        OnLayerChanged?.Invoke(_currentLayer);
    }

    public void TurnOffAllVoxels() {
        _currentLayer = -1;
        foreach (var v in _voxels) {
            v.voxelElement.Hide();
        }
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

    public void SetFigureState(VoxelFigureInfoData voxelFigureInfoData) {
        if (!voxelFigureInfoData.isCompleted) {
            DisableFigure();
        } else {
            InitFigureColors(voxelFigureInfoData);
        }

        HideInvisibleVoxels();
    }

    private void DisableFigure() {
        foreach (var voxel in _voxels) {
            voxel.voxelElement.HideFrame();
            voxel.voxelElement.SetMaterial(GameResourcesDatabase.Instance._lockedFigureMaterial);
        }
    }

    private void InitFigureColors(VoxelFigureInfoData voxelFigureInfoData) {
        foreach (var voxel in _voxelElementsPositionsDictionary) {
            voxel.Value.HideFrame();
            var voxelsColorDataDictionary = voxelFigureInfoData.GetVoxelsColorDataDictionary();
            if (voxelsColorDataDictionary.ContainsKey(voxel.Key)) {
                var material = GameResourcesDatabase.GetMaterialOfColor(voxelsColorDataDictionary[voxel.Key]);
                voxel.Value.SetMaterial(material);
            }
        }
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

    private VoxelElement GetNeighbour(Direction dir, Vector3 voxelPosition) {
        switch (dir) {
            case Direction.Left:
                return GetNeighbour(NeighbourType.Left, voxelPosition);
            case Direction.Down:
                return GetNeighbour(NeighbourType.Front, voxelPosition);
            case Direction.Right:
                return GetNeighbour(NeighbourType.Right, voxelPosition);
            case Direction.Up:
                return GetNeighbour(NeighbourType.Back, voxelPosition);
        }

        return null;
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

    public List<ColorData> GetFigureColorsMap() {
        var colorMap = new List<ColorData>();
        foreach (var voxel in _voxelElementsPositionsDictionary) {
            colorMap.Add(new ColorData(voxel.Key, voxel.Value.MyPrintedColor));
        }

        return colorMap;
    }
}
