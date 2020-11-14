using UnityEngine;

public class FigureSlot : MonoBehaviour {
    [SerializeField] private StarsControllerUI _starsControllerUI;

    private VoxelFigureData _currentVoxelFigureData;
    private VoxelFigure _currentSpawnedVoxelFigure;

    public void Init(VoxelFigureData voxelFigureData) {
        _currentVoxelFigureData = voxelFigureData;
        _currentSpawnedVoxelFigure = Instantiate(_currentVoxelFigureData.voxelFigure, transform);
        _currentSpawnedVoxelFigure.DisableFigure();
    }

    public void Clear() {
        if (_currentSpawnedVoxelFigure != null) {
            //TODO: Optimize spawning figures
            Destroy(_currentSpawnedVoxelFigure.gameObject);
            _currentSpawnedVoxelFigure = null;
        }
    }
}
