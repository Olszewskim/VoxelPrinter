using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FiguresBookcase : MonoBehaviour {
    [SerializeField] private FigureSlot[] _figureSlots;
    [SerializeField] private TextMeshProUGUI _collectionNameText;

    public void InitFigureSlots(List<VoxelFigureData> voxelFiguresData,
        List<VoxelFigureInfoData> currentCollectionData) {
        ClearAllSlots();
        _collectionNameText.text = voxelFiguresData[0].collectionType.ToString();
        for (int i = 0; i < voxelFiguresData.Count; i++) {
            _figureSlots[i].Init(voxelFiguresData[i], currentCollectionData[i]);
        }
    }

    private void ClearAllSlots() {
        foreach (var figureSlot in _figureSlots) {
            figureSlot.Clear();
        }
    }
}
