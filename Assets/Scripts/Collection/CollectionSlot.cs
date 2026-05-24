using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollectionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image plantIcon;
    [SerializeField] private TMP_Text closedSlotQuestionTxt;

    [Header("Tooltip")]
    [SerializeField] private CanvasGroup tooltipObject;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text rarityText;

    private PlantData currentPlant;
    private ArtefactData currentArtefact;
    private bool isUnlocked;

    public int CurrentItemId { get; private set; } = -1;

    // ── Setup ─────────────────────────────────────────────────────────────────

    public void Setup(PlantData data, bool unlockedState)
    {
        currentPlant = data;
        currentArtefact = null;
        CurrentItemId = data != null ? data.id : -1;

        if (plantIcon != null && data != null) plantIcon.sprite = data.icon;
        ResetTooltip();
        RefreshUI(unlockedState);
    }

    public void Setup(ArtefactData data, int runtimeId, bool unlockedState)
    {
        currentArtefact = data;
        currentPlant = null;
        CurrentItemId = runtimeId;

        if (plantIcon != null && data != null) plantIcon.sprite = data.icon;
        ResetTooltip();
        RefreshUI(unlockedState);
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    public void RefreshUI(bool unlockedState)
    {
        isUnlocked = unlockedState;

        if (isUnlocked)
        {
            closedSlotQuestionTxt.gameObject.SetActive(false);
            if (plantIcon != null) plantIcon.color = Color.white;

            if (currentPlant != null) ApplyPlantTooltip(currentPlant);
            else if (currentArtefact != null) ApplyArtefactTooltip(currentArtefact);
        }
        else
        {
            closedSlotQuestionTxt.gameObject.SetActive(true);
            if (plantIcon != null) plantIcon.color = Color.black;
            if (nameText != null) nameText.text = "";
            if (descriptionText != null) descriptionText.text = "";
            if (rarityText != null)
            {
                rarityText.text = "";
                rarityText.gameObject.SetActive(true);
            }
        }
    }

    private void ApplyPlantTooltip(PlantData data)
    {
        if (nameText != null) nameText.text = data.name;
        if (descriptionText != null) descriptionText.text = data.description;
        if (rarityText != null)
        {
            rarityText.gameObject.SetActive(true);
            rarityText.text = data.rarity.ToString();
            rarityText.color = data.rarity switch
            {
                PlantData.Rarity.Common => Color.green,
                PlantData.Rarity.Uncommon => Color.cyan,
                PlantData.Rarity.Rare => Color.red,
                _ => Color.white
            };
        }
    }

    private void ApplyArtefactTooltip(ArtefactData data)
    {
        if (nameText != null) nameText.text = data.name;
        if (descriptionText != null) descriptionText.text = data.description;
        if (rarityText != null) rarityText.gameObject.SetActive(false);
    }

    // ── Tooltip fade ──────────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData _)
    {
        if (!isUnlocked || tooltipObject == null) return;
        tooltipObject.gameObject.SetActive(true);
        tooltipObject.DOKill();
        tooltipObject.DOFade(1f, 0.2f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (!isUnlocked || tooltipObject == null) return;
        tooltipObject.DOKill();
        tooltipObject.DOFade(0f, 0.15f).SetUpdate(true)
            .OnComplete(() => tooltipObject.gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        if (tooltipObject == null) return;
        tooltipObject.gameObject.SetActive(false);
        tooltipObject.alpha = 0f;
    }

    private void ResetTooltip()
    {
        if (tooltipObject == null) return;
        tooltipObject.alpha = 0f;
        tooltipObject.gameObject.SetActive(false);
    }
}
