using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionTileUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _collectionNameText;
    [SerializeField] private Image _collectionImage;
    [SerializeField] private Button _selectCollectionButton;

    private CollectionData _collectionData;

    private void Awake() {
        _selectCollectionButton.onClick.AddListener(SelectCollection);
    }

    public void Init(CollectionData collectionData) {
        _collectionData = collectionData;
        _collectionNameText.text = collectionData.collectionName;
        _collectionImage.sprite = collectionData.collectionImage;
        gameObject.SetActive(true);
    }

    private void SelectCollection() {
        GameManager.Instance.ShowSelectFigureView(_collectionData.collectionType);
        SelectCollectionWindow.Instance.CloseWindow();
    }
}
