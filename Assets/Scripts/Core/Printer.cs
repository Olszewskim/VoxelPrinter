using DG.Tweening;
using UnityEngine;

public class Printer : MonoBehaviour {
    [SerializeField] private Transform _nozzle;

    private const float PRINT_HEIGHT = 1.5f;
    private const float MOVE_TIME = 5f;

    public Tween MoveNoozle(Vector3 position) {
        position.y += PRINT_HEIGHT;
        var anim = _nozzle.DOMove(position, MOVE_TIME).SetEase(Ease.Linear).SetSpeedBased();
        return anim;
    }
}
