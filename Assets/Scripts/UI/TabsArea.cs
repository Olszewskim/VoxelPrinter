using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TabsArea : MonoBehaviour {
    [SerializeField] private Button[] _tabButtons;
    [SerializeField] private Color _activeTabColor;
    [SerializeField] private Color _inactiveTabColor;

    private int _currentActiveTab;
    private readonly Vector3 _activeTabScale = new Vector3(1.1f, 1.1f, 1.1f);
    private readonly Vector3 _inactiveTabScale = new Vector3(1f, 1f, 1f);

    private const float ANIM_TIME = 0.5f;

    private void Awake() {
        for (int i = 0; i < _tabButtons.Length; i++) {
            var index = i;
            _tabButtons[i].onClick.AddListener(() => RefreshTabs(index));
        }

        RefreshTabs(0);
    }

    private void RefreshTabs(int index) {
        _currentActiveTab = index;
        for (int i = 0; i < _tabButtons.Length; i++) {
            var shouldBeActive = _currentActiveTab == i;
            var color = shouldBeActive ? _activeTabColor : _inactiveTabColor;
            var scale = shouldBeActive ? _activeTabScale : _inactiveTabScale;
            _tabButtons[i].transform.DOScale(scale, ANIM_TIME);
            _tabButtons[i].targetGraphic.DOColor(color, ANIM_TIME);
            if (shouldBeActive) {
                _tabButtons[i].transform.SetAsLastSibling();
            }
        }
    }
}
