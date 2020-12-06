using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionTileUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _collectionNameText;
    [SerializeField] private Image _collectionImage;
    [SerializeField] private Button _selectCollectionButton;
    [SerializeField] private RectTransform _lockedCollectionArea;
    [SerializeField] private TextMeshProUGUI _lockedCollectionText;

    private CollectionData _collectionData;

    private void Awake() {
        _selectCollectionButton.onClick.AddListener(SelectCollection);
    }

    public void Init(CollectionData collectionData, int currentAmountOfStars) {
        _collectionData = collectionData;
        _collectionNameText.text = collectionData.collectionName;
        _collectionImage.sprite = collectionData.collectionImage;
        gameObject.SetActive(true);
        _lockedCollectionArea.gameObject.SetActive(currentAmountOfStars < collectionData.starsToUnlock);
        var requiredStars = collectionData.starsToUnlock - currentAmountOfStars;
        _lockedCollectionText.text = GameTexts.GetMoreStarsToUnlockText(requiredStars);
    }

    private void SelectCollection() {
        GameManager.Instance.ShowSelectFigureView(_collectionData.collectionType);
        SelectCollectionWindow.Instance.CloseWindow();
    }
}
