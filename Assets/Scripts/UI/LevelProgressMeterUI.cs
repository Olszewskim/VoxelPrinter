using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LevelProgressMeterUI : MonoBehaviour {
    private Slider _slider;
    private Printer _printer;

    private void Awake() {
        _slider = GetComponent<Slider>();
        ResetProgress();
        _printer = FindObjectOfType<Printer>();
        _printer.OnPrintingStarted += ResetProgress;
        _printer.OnPrintingProgress += RefreshPrintingProgress;
    }

    private void ResetProgress() {
        _slider.minValue = _slider.value = 0;
        _slider.maxValue = 1;
    }

    private void RefreshPrintingProgress(float progress) {
        _slider.value = progress;
    }
}
