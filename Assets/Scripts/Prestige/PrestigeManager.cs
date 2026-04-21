using DG.Tweening;
using UnityEngine;

public class PrestigeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsForRegions;
    [SerializeField] private GameObject nextRegionConfirmPanel;
    [Space]
    [Header("For tweens")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    private int currentRegion = 0;
    public void UpdateRegion()
    {
        currentRegion++;
        CoinManager.Instance.ResetCoins();

        // 1. Сначала деактивируем и удаляем всё старое
        for (int i = 0; i < objectsForRegions.Length; i++)
        {
            if (i != currentRegion && objectsForRegions[i] != null)
            {
                objectsForRegions[i].SetActive(false);
                Destroy(objectsForRegions[i]);
            }
        }

        // 2. Включаем новый регион
        // В этот момент сработает OnEnable() в скрипте Grid на новом объекте
        // и Grid.Instance обновится автоматически!
        if (currentRegion < objectsForRegions.Length)
        {
            objectsForRegions[currentRegion].SetActive(true);
        }

        CloseNextRegionPanel();
    }
    public void OpenNextRegionPanel()
    {
        nextRegionConfirmPanel.SetActive(true);

        InteractionManager.Instance.canZoomCam = false;

        panelRect.anchoredPosition = new Vector2(0, -800);
        panelRect.localScale = Vector3.one * 0.9f;
        panelCanvasGroup.alpha = 0;

        // Animations
        panelRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        panelRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        panelCanvasGroup.DOFade(1, 0.3f);
    }
    public void CloseNextRegionPanel()
    {
        InteractionManager.Instance.canZoomCam = true;

        panelRect.DOAnchorPosY(-800, 0.4f).SetEase(Ease.InBack);
        panelRect.DOScale(0.9f, 0.3f);

        panelCanvasGroup.DOFade(0, 0.25f).OnComplete(() =>
        {
            nextRegionConfirmPanel.SetActive(false);
        });
    }
}
