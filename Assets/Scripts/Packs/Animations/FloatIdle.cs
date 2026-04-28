using DG.Tweening;
using UnityEngine;

public class FloatIdle : MonoBehaviour
{
    [SerializeField] private float moveAmount = 4f;
    [SerializeField] private float duration = 2f;

    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;

        float offset = Random.Range(-moveAmount, moveAmount);

        rect.DOAnchorPosY(startPos.y + offset, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
