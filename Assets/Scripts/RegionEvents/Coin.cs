using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private float fadeWarningAt = 2.5f;
    [SerializeField] private TMP_Text incomeText;

    [Header("Sound")]
    [SerializeField] private AudioClip pickSound;
    [SerializeField] private float pickSoundVolume;

    private CanvasGroup canvasGroup;
    private RectTransform incomeTextRect;
    private bool clicked = false;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(0f, lifetime - fadeWarningAt).SetDelay(fadeWarningAt).SetEase(Ease.InQuad);

        if (incomeText != null)
        {
            incomeTextRect = incomeText.rectTransform;
            incomeText.transform.SetParent(transform.parent);
            incomeText.gameObject.SetActive(false);
        }

        Destroy(gameObject, lifetime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!clicked)
        {
            clicked = true;
            DOTween.Kill(transform);
            AudioManager.Instance.PlaySfxSound(pickSound, pickSoundVolume, 0.9f, 1.1f);

            ShowIncomeText();

            transform.DOScale(1.4f, 0.1f).SetEase(Ease.OutQuad)
                     .OnComplete(() => transform.DOScale(0f, 0.15f).SetEase(Ease.InBack)
                     .OnComplete(() =>
                     {
                         CoinManager.Instance.AddCoins(1);
                         Destroy(transform.parent.gameObject);
                     }));
        }
    }

    private void ShowIncomeText()
    {
        if (incomeText == null) return;

        incomeTextRect.position = transform.position;
        incomeText.gameObject.SetActive(true);

        Color baseColor = incomeText.color;
        incomeText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        incomeTextRect.DOAnchorPosY(incomeTextRect.anchoredPosition.y + 60f, 0.5f).SetEase(Ease.OutQuad);

        incomeText.DOFade(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() => Destroy(incomeText.gameObject));
    }
}
