using System.Collections.Generic;
using UnityEngine;

public class StarsControllerUI : MonoBehaviour {
    [SerializeField] private List<StarUI> _stars;

    private const float MIN_RESULT_FOR_1_STAR = 0.4f;
    private const float MIN_RESULT_FOR_2_STARS = 0.6f;
    private const float MIN_RESULT_FOR_3_STARS = 0.9f;
    private const float SHOW_STAR_DELAY = 1f;

    public void HideStars() {
        foreach (var star in _stars) {
            star.Hide();
        }
    }

    public void ShowStars(float levelResult, bool withAnimation) {
        var starsToShow = GetCountOfStarsToShow(levelResult);
        for (int i = 0; i < starsToShow; i++) {
            _stars[i].Show(SHOW_STAR_DELAY * i, withAnimation);
        }
    }

    private int GetCountOfStarsToShow(float levelResult) {
        if (levelResult > MIN_RESULT_FOR_3_STARS) {
            return 3;
        }

        if (levelResult > MIN_RESULT_FOR_2_STARS) {
            return 2;
        }

        if (levelResult > MIN_RESULT_FOR_1_STAR) {
            return 1;
        }

        return 0;
    }
}
