using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private VoxelFigure _voxelFigure;
    [SerializeField] private Printer _printer;

    public void Start() {
        Input.multiTouchEnabled = false;
        _printer.SetupPrintModel(_voxelFigure);
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
