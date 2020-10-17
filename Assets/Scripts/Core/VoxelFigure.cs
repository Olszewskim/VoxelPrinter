using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void AddVoxel(Vector3 position, VoxelElement voxelElement) {
        if (_voxels == null) {
            _voxels = new List<VoxelData>();
        }

        _voxels.Add(new VoxelData(position, voxelElement));
    }

    public void Start() {
        SortVoxels();
        StartCoroutine(Print());
    }

    private void SortVoxels() {
        _voxels = _voxels.OrderBy(v => v.voxelPosition.y).ThenByDescending(v => v.voxelPosition.x)
            .ThenBy(v => v.voxelPosition.z)
            .ToList();
    }

    private IEnumerator Print() {
        TurnOffAllVoxels();
        foreach (var v in _voxels) {
            yield return new WaitForSeconds(0.2f);
            v.voxelElement.Print(0.2f);
        }
    }

    private void TurnOffAllVoxels() {
        foreach (var v in _voxels) {
            v.voxelElement.Hide();
        }
    }
}
