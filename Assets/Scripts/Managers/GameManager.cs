using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private VoxelFigure _voxelFigure;
    [SerializeField] private Printer _printer;

    public void Start() {
        _printer.SetupPrintModel(_voxelFigure);
    }
}
