using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("UI Компоненты")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup panelCanvasGroup; // Добавь этот компонент на панель для альфы
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Space]
    [SerializeField] private float showTime;
    [SerializeField] private AudioClip achievementSound;

    private Sequence animSequence;
    private Vector2 originalPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        originalPosition = panelRect.anchoredPosition;
        panelCanvasGroup.alpha = 0;
    }
    /*
    private void Start()
    {
        ShowAchievement("test achievement", null);
    }
    */
    public void ShowAchievement(string description, Sprite icon)
    {
        descriptionText.text = description;
        iconImage.sprite = icon;
        panelCanvasGroup.blocksRaycasts = true;

        if (animSequence != null && animSequence.IsActive()) animSequence.Kill();

        panelRect.anchoredPosition = originalPosition;
        panelCanvasGroup.alpha = 1f;

        AudioManager.Instance.PlaySfxSound(achievementSound, 0.25f, 0.95f, 1.05f);

        animSequence = DOTween.Sequence();
        animSequence
            .Append(panelRect.DOAnchorPosY(originalPosition.y + 300f, 0.5f).From().SetEase(Ease.OutBack))
            .Join(panelRect.DOScale(0.8f, 0.5f).From().SetEase(Ease.OutBack))
            .Join(panelCanvasGroup.DOFade(0f, 0.3f).From())

            .AppendInterval(showTime)

            .Append(panelRect.DOAnchorPosY(originalPosition.y + 300f, 0.4f).SetEase(Ease.InBack))
            .Join(panelRect.DOScale(0.8f, 0.4f).SetEase(Ease.InBack))
            .Join(panelCanvasGroup.DOFade(0f, 0.3f));

        panelCanvasGroup.blocksRaycasts = false;
    }
}