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

    [Header("Tooltip Settings")]
    [SerializeField] private CanvasGroup tooltipObject;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text rarityText;

    private PlantData currentPlantData;
    private bool isUnlocked;

    public int CurrentPlantId => currentPlantData != null ? currentPlantData.id : -1;

    public void Setup(PlantData data, bool unlockedState)
    {
        currentPlantData = data;

        if (currentPlantData == null) return;

        if (plantIcon != null) plantIcon.sprite = currentPlantData.icon;

        if (tooltipObject != null)
        {
            tooltipObject.alpha = 0f;
            tooltipObject.gameObject.SetActive(false);
        }

        RefreshUI(unlockedState);
    }

    public void RefreshUI(bool unlockedState)
    {
        isUnlocked = unlockedState;

        if (currentPlantData == null) return;

        if (isUnlocked)
        {
            closedSlotQuestionTxt.gameObject.SetActive(false);
            if (plantIcon != null) plantIcon.color = Color.white;
            if (nameText != null) nameText.text = currentPlantData.name;
            if (descriptionText != null) descriptionText.text = currentPlantData.description;

            if (rarityText != null)
            {
                rarityText.text = currentPlantData.rarity.ToString();

                switch (currentPlantData.rarity)
                {
                    case PlantData.Rarity.Common:
                        rarityText.color = Color.green;
                        break;
                    case PlantData.Rarity.Uncommon:
                        rarityText.color = Color.cyan;
                        break;
                    case PlantData.Rarity.Rare:
                        rarityText.color = Color.red;
                        break;
                }
            }
        }
        else
        {
            closedSlotQuestionTxt.gameObject.SetActive(true);

            if (plantIcon != null) plantIcon.color = Color.black;
            if (nameText != null) nameText.text = "";
            if (descriptionText != null) descriptionText.text = "";
            if (rarityText != null) rarityText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUnlocked && tooltipObject != null)
        {
            tooltipObject.gameObject.SetActive(true);
            tooltipObject.DOKill();
            tooltipObject.DOFade(1f, 0.2f).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUnlocked && tooltipObject != null)
        {
            tooltipObject.DOKill();
            tooltipObject.DOFade(0f, 0.15f).SetUpdate(true).OnComplete(() =>
            {
                tooltipObject.gameObject.SetActive(false);
            });
        }
    }

    private void OnDisable()
    {
        if (tooltipObject != null)
        {
            tooltipObject.gameObject.SetActive(false);
            tooltipObject.alpha = 0f;
        }
    }
}
