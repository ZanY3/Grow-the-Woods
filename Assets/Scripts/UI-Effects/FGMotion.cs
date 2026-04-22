using DG.Tweening;
using UnityEngine;

public class FGMotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveAmount = 10f;
    [SerializeField] private float moveDuration = 3f;

    [Header("Scale")]
    [SerializeField] private float scaleAmount = 0.02f;
    [SerializeField] private float scaleDuration = 4f;

    private RectTransform rect;
    private Vector2 startPos;
    private Vector3 startScale;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        startScale = rect.localScale;

        PlayMotion();
    }

    private void PlayMotion()
    {
        rect.DOAnchorPosY(startPos.y + moveAmount, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        rect.DOScale(startScale * (1f + scaleAmount), scaleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
