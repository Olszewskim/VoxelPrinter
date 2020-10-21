using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class VoxelData {
    public Vector3 voxelPosition;
    public VoxelElement voxelElement;
    public Color voxelColor;

    public VoxelData(Vector3 voxelPosition, VoxelElement voxelElement, Color voxelColor) {
        this.voxelPosition = voxelPosition;
        this.voxelElement = voxelElement;
        this.voxelColor = voxelColor;
    }
}

public class VoxelFigure : MonoBehaviour {
    private const float PRINT_TIME = 0.2f;

    [SerializeField] private List<VoxelData> _voxels;

    private Printer _printer;
    private int _currentLayer;

    public void AddVoxel(Vector3 position, VoxelElement voxelElement, Color voxelColor) {
        if (_voxels == null) {
            _voxels = new List<VoxelData>();
        }

        _voxels.Add(new VoxelData(position, voxelElement, voxelColor));
    }

    public IEnumerator Start() {
        _printer = FindObjectOfType<Printer>();
        SortVoxels();
        yield return null;
        StartCoroutine(Print());
    }

    private void SortVoxels() {
        _voxels = _voxels.OrderBy(v => v.voxelPosition.y).ThenBy(v => v.voxelPosition.z)
            .ThenBy(v => v.voxelPosition.x)
            .ToList();
    }

    private IEnumerator Print() {
        TurnOffAllVoxels();
        var myColors = GetFigureColors();
        _printer.SetButtonsColors(myColors);

        _currentLayer = -1;
        foreach (var v in _voxels) {
            if (v.voxelPosition.y != _currentLayer) {
                _currentLayer = (int) v.voxelPosition.y;
                ShowCurrentLayer();
            }

            var moveAnim = _printer.MoveNoozle(v.voxelElement.transform.position, v.voxelColor);
            yield return null;
            var moveAnimTime = moveAnim.Duration();
            yield return new WaitForSeconds(moveAnimTime);
            v.voxelElement.Print(PRINT_TIME);
            _printer.Print(PRINT_TIME);
            yield return new WaitForSeconds(PRINT_TIME);
        }
    }

    private List<Color> GetFigureColors() {
        var colors = new HashSet<Color>();
        foreach (var v in _voxels) {
            colors.Add(v.voxelColor);
        }

        return colors.ToList();
    }

    private void ShowCurrentLayer() {
        var thisLayerVoxels = _voxels.Where(v => v.voxelPosition.y == _currentLayer).ToList();
        foreach (var voxel in thisLayerVoxels) {
            voxel.voxelElement.PrepareToPrint();
        }
    }

    private void TurnOffAllVoxels() {
        foreach (var v in _voxels) {
            v.voxelElement.Hide();
        }
    }
}
