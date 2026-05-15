using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ArtefactSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip")]
    [SerializeField] private GameObject artefactTooltip;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private GameObject deleteBtn;

    [Header("Confirmation Panel")]
    [SerializeField] private GameObject deleteConfirmPanel;
    [SerializeField] private CanvasGroup confirmCanvasGroup;
    [SerializeField] private RectTransform panelRect;

    [Space]
    [SerializeField] private Image iconImg;
    [SerializeField] private ArtefactsManager artefactsManager;
    [Space]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip deleteSound;

    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public ArtefactData equipedData;
    private bool canShowTooltip = true;

    public void ChangeConfirmPanelState(bool state)
    {
        AudioManager.Instance.PlaySfxSound(clickSound, 0.65f, 0.9f, 1.1f);
        // Останавливаем старые анимации перед началом новых
        confirmCanvasGroup.DOKill();
        panelRect.DOKill();

        if (state)
        {
            canShowTooltip = false;
            artefactTooltip.SetActive(false);
            deleteConfirmPanel.SetActive(true);

            // Начальное состояние для появления
            confirmCanvasGroup.alpha = 0;
            panelRect.localScale = Vector3.one * 0.7f;

            // Анимация появления
            confirmCanvasGroup.DOFade(1, 0.3f).SetUpdate(true);
            panelRect.DOScale(1, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        else
        {
            // Анимация исчезновения
            canShowTooltip = true;
            confirmCanvasGroup.DOFade(0, 0.2f).SetUpdate(true);
            panelRect.DOScale(0.8f, 0.2f).SetUpdate(true).OnComplete(() => {
                deleteConfirmPanel.SetActive(false);
            });
        }
    }

    public void RemoveArtefact()
    {
        if (equipedData != null)
        {
            StatsManager.Instance.RemoveArtefact(equipedData);
            artefactsManager.OnArtefactRemoved(equipedData);
        }

        ChangeConfirmPanelState(false);

        // Очищаем слот
        AudioManager.Instance.PlaySfxSound(deleteSound, 1f, 0.9f, 1.1f);
        artefactTooltip.SetActive(false);
        deleteBtn.SetActive(false);
        isOccupied = false;
        equipedData = null;
        iconImg.gameObject.SetActive(false);
        nameTxt.text = "";
        descriptionTxt.text = "";
    }

    public void SetArtefact(ArtefactData artefact)
    {
        isOccupied = true;
        equipedData = artefact;
        iconImg.sprite = artefact.icon;
        iconImg.gameObject.SetActive(true);
        nameTxt.text = artefact.name;
        descriptionTxt.text = artefact.description;
        deleteBtn.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOccupied && canShowTooltip)
            artefactTooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOccupied && canShowTooltip)
            artefactTooltip.SetActive(false);
    }
}