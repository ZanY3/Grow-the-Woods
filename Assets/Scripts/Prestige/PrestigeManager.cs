using DG.Tweening;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsForRegions;
    [SerializeField] private GameObject nextRegionConfirmPanel;
    [SerializeField] private GameObject nextRegionPanel;
    [SerializeField] private GameObject nextRegionBtn;
    [SerializeField] private AudioClip btnSound;
    [SerializeField] private AudioClip newRegionSound;
    [SerializeField] private AudioClip rewardSound;

    [Space]
    [Header("Confirm Panel Tweens")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Announce Panel Tweens (Next Region)")]
    [SerializeField] private RectTransform nextRegionRect;
    [SerializeField] private CanvasGroup nextRegionCanvasGroup;

    [Header("Artefact Panel Tweens")]
    [SerializeField] private GameObject artefactOpenPanel;
    [SerializeField] private RectTransform artefactRect; // Добавь RectTransform панели артефактов
    [SerializeField] private CanvasGroup artefactCanvasGroup; // Добавь CanvasGroup панели артефактов

    [Header("Other")]
    [SerializeField] private ShopManager shopManager;

    [HideInInspector] public int currentRegion = 0;
    [HideInInspector] public int maxRegion = 1;

    public void UpdateRegion()
    {
        nextRegionBtn.SetActive(false);

        if (currentRegion < maxRegion)
        {
            currentRegion++;

            CoinManager.Instance.ResetCoins();
            shopManager.ReturnPrices();
            EndingManager.Instance.ResetProgress();

            for (int i = 0; i < objectsForRegions.Length; i++)
            {
                if (i != currentRegion && objectsForRegions[i] != null)
                {
                    objectsForRegions[i].SetActive(false);
                    Destroy(objectsForRegions[i]);
                }
            }

            if (currentRegion < objectsForRegions.Length)
            {
                objectsForRegions[currentRegion].SetActive(true);
            }

            CloseNextRegionPanel(isInstant: true);
            ShowRegionAnnounce();
        }
        else
        {
            CloseNextRegionPanel();
            EndingManager.Instance.PlayEnding();
            Debug.Log("THE END!");
        }
    }

    private void ShowRegionAnnounce()
    {
        AudioManager.Instance.PlaySfxSound(newRegionSound, 0.2f, 0.95f, 1.05f);
        nextRegionPanel.SetActive(true);
        nextRegionCanvasGroup.alpha = 0;
        nextRegionRect.localScale = Vector3.one * 0.7f;
        nextRegionRect.anchoredPosition = new Vector2(0, 50);

        Sequence announceSeq = DOTween.Sequence();

        announceSeq.Append(nextRegionCanvasGroup.DOFade(1, 0.6f));
        announceSeq.Join(nextRegionRect.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
        announceSeq.Join(nextRegionRect.DOAnchorPosY(0, 0.6f).SetEase(Ease.OutCubic));

        announceSeq.AppendInterval(2.0f);

        announceSeq.Append(nextRegionCanvasGroup.DOFade(0, 0.5f));
        announceSeq.Join(nextRegionRect.DOScale(1.2f, 0.5f).SetEase(Ease.InBack));

        announceSeq.OnComplete(() =>
        {
            nextRegionPanel.SetActive(false);
            OpenArtefactPanelWithAnim(); // Вызываем новую анимацию
        });
    }

    private void OpenArtefactPanelWithAnim()
    {
        AudioManager.Instance.PlaySfxSound(rewardSound, 0.3f, 0.95f, 1.05f);

        // Подготовка панели перед показом
        artefactOpenPanel.SetActive(true);
        artefactCanvasGroup.alpha = 0;
        artefactRect.localScale = Vector3.one * 0.8f;

        // Быстрая и сочная анимация появления
        artefactCanvasGroup.DOFade(1, 0.4f);
        artefactRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

    public void OpenNextRegionPanel()
    {
        AudioManager.Instance.PlaySfxSound(btnSound, 0.35f);
        nextRegionConfirmPanel.SetActive(true);

        InteractionManager.Instance.canZoomCam = false;

        panelRect.anchoredPosition = new Vector2(0, -800);
        panelRect.localScale = Vector3.one * 0.9f;
        panelCanvasGroup.alpha = 0;

        panelRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        panelRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        panelCanvasGroup.DOFade(1, 0.3f);
    }

    public void CloseNextRegionPanel(bool isInstant = false)
    {
        if (currentRegion >= maxRegion)
        {
            nextRegionBtn.SetActive(false);
        }

        if (!isInstant)
            AudioManager.Instance.PlaySfxSound(btnSound, 0.35f);

        InteractionManager.Instance.canZoomCam = true;

        panelRect.DOAnchorPosY(-800, 0.4f).SetEase(Ease.InBack);
        panelRect.DOScale(0.9f, 0.3f);

        panelCanvasGroup.DOFade(0, 0.25f).OnComplete(() =>
        {
            nextRegionConfirmPanel.SetActive(false);
        });
    }
}
