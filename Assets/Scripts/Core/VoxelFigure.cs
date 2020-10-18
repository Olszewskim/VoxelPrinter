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

    public VoxelData(Vector3 voxelPosition, VoxelElement voxelElement) {
        this.voxelPosition = voxelPosition;
        this.voxelElement = voxelElement;
    }
}

public class VoxelFigure : MonoBehaviour {
    [SerializeField] private List<VoxelData> _voxels;

    private Printer _printer;
    private const float PRINT_TIME = 0.3f;

    public void AddVoxel(Vector3 position, VoxelElement voxelElement) {
        if (_voxels == null) {
            _voxels = new List<VoxelData>();
        }

        _voxels.Add(new VoxelData(position, voxelElement));
    }

    public void Start() {
        _printer = FindObjectOfType<Printer>();
        SortVoxels();
        StartCoroutine(Print());
    }

    private void SortVoxels() {
        _voxels = _voxels.OrderBy(v => v.voxelPosition.y).ThenBy(v => v.voxelPosition.z)
            .ThenBy(v => v.voxelPosition.x)
            .ToList();
    }

    private IEnumerator Print() {
        TurnOffAllVoxels();
        foreach (var v in _voxels) {
            var moveAnim = _printer.MoveNoozle(v.voxelElement.transform.position);
            yield return null;
            var moveAnimTime = moveAnim.Duration();
            yield return new WaitForSeconds(moveAnimTime);
            v.voxelElement.Print(PRINT_TIME);
            yield return new WaitForSeconds(PRINT_TIME);
        }
    }

    private void TurnOffAllVoxels() {
        foreach (var v in _voxels) {
            v.voxelElement.Hide();
        }
    }
}
