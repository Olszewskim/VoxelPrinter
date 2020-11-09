using System.Collections.Generic;
using UnityEngine;

public class FiguresBookcase : MonoBehaviour {
    [SerializeField] private FigureSlot[] _figureSlots;

    public void InitFigureSlots(List<VoxelFigureData> voxelFiguresData) {
        ClearAllSlots();
        for (int i = 0; i < voxelFiguresData.Count; i++) {
            _figureSlots[i].Init(voxelFiguresData[i]);
        }
    }

    private void ClearAllSlots() {
        foreach (var figureSlot in _figureSlots) {
            figureSlot.Clear();
        }
    }
}
