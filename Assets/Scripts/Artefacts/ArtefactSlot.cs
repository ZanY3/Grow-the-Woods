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

    // Cached references to avoid repeated singleton lookups
    private AudioManager _audioManager;
    private StatsManager _statsManager;
    // Cached gameObject reference from iconImg to avoid repeated property access
    private GameObject _iconImgGO;

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _statsManager = StatsManager.Instance;
        _iconImgGO = iconImg.gameObject;
    }

    public void ChangeConfirmPanelState(bool state)
    {
        _audioManager.PlaySfxSound(clickSound, 0.65f, 0.9f, 1.1f);
        confirmCanvasGroup.DOKill();
        panelRect.DOKill();

        if (state)
        {
            canShowTooltip = false;
            // Only deactivate tooltip if it's actually active
            if (artefactTooltip.activeSelf) artefactTooltip.SetActive(false);
            deleteConfirmPanel.SetActive(true);

            confirmCanvasGroup.alpha = 0;
            panelRect.localScale = Vector3.one * 0.7f;

            confirmCanvasGroup.DOFade(1, 0.3f).SetUpdate(true);
            panelRect.DOScale(1, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        else
        {
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
            _statsManager.RemoveArtefact(equipedData);
            artefactsManager.OnArtefactRemoved(equipedData);
        }

        ChangeConfirmPanelState(false);

        _audioManager.PlaySfxSound(deleteSound, 1f, 0.9f, 1.1f);

        // Only call SetActive if the state needs to change
        if (artefactTooltip.activeSelf) artefactTooltip.SetActive(false);
        if (deleteBtn.activeSelf) deleteBtn.SetActive(false);
        if (_iconImgGO.activeSelf) _iconImgGO.SetActive(false);

        isOccupied = false;
        equipedData = null;
        nameTxt.text = string.Empty;
        descriptionTxt.text = string.Empty;
    }

    public void SetArtefact(ArtefactData artefact)
    {
        isOccupied = true;
        equipedData = artefact;
        iconImg.sprite = artefact.icon;
        // Only activate if not already active
        if (!_iconImgGO.activeSelf) _iconImgGO.SetActive(true);
        nameTxt.text = artefact.name;
        descriptionTxt.text = artefact.description;
        if (!deleteBtn.activeSelf) deleteBtn.SetActive(true);
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