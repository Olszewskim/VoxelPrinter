using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class VoxelData {
    public Vector3 voxelPosition;
    public GameObject voxelGameobject;

    public VoxelData(Vector3 voxelPosition, GameObject voxelGameobject) {
        this.voxelPosition = voxelPosition;
        this.voxelGameobject = voxelGameobject;
    }
}

public class VoxelFigure : MonoBehaviour {
    [SerializeField] private List<VoxelData> _voxels;

    public void AddVoxel(Vector3 position, GameObject voxelObject) {
        if (_voxels == null) {
            _voxels = new List<VoxelData>();
        }

        _voxels.Add(new VoxelData(position, voxelObject));
    }

    public void Start() {
        SortVoxels();
       StartCoroutine(Print());
    }

    private void SortVoxels() {
        _voxels = _voxels.OrderBy(v => v.voxelPosition.y).ThenByDescending(v => v.voxelPosition.x).ThenBy(v => v.voxelPosition.z)
            .ToList();
    }

    private IEnumerator Print() {
        TurnOffAllVoxels();
        foreach (var v in _voxels) {
            yield return new WaitForSeconds(0.2f);
            v.voxelGameobject.SetActive(true);
        }
    }

    private void TurnOffAllVoxels() {
        foreach (var v in _voxels) {
            v.voxelGameobject.SetActive(false);
        }
    }
}
