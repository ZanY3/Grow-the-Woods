using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CollectionUI : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private CanvasGroup collectionWindowPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridParent;

    [Header("Настройки анимации")]
    [SerializeField] private float fadeDuration = 0.25f;

    private List<CollectionSlot> spawnedSlots = new List<CollectionSlot>();
    private Tween fadeTween;

    private void Start()
    {
        InitGrid();

        collectionWindowPanel.alpha = 0f;
        collectionWindowPanel.gameObject.SetActive(false);
    }

    private void InitGrid()
    {
        List<PlantData> plants = CollectionManager.Instance.GetAllPlants();
        if (plants == null) return;

        foreach (var plant in plants)
        {
            if (plant == null) continue;

            GameObject newSlot = Instantiate(slotPrefab, gridParent);
            CollectionSlot slotScript = newSlot.GetComponent<CollectionSlot>();

            if (slotScript != null)
            {
                spawnedSlots.Add(slotScript);
                slotScript.Setup(plant, false);
            }
        }
    }

    public void ChangeBookState(bool state)
    {
        fadeTween?.Kill();

        if (state)
        {
            UpdateSlotsData();

            collectionWindowPanel.gameObject.SetActive(true);
            fadeTween = collectionWindowPanel.DOFade(1f, fadeDuration).SetUpdate(true);
        }
        else
        {
            fadeTween = collectionWindowPanel.DOFade(0f, fadeDuration)
                .SetUpdate(true)
                .OnComplete(() => collectionWindowPanel.gameObject.SetActive(false));
        }
    }

    private void UpdateSlotsData()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot == null) continue;

            bool isUnlocked = CollectionManager.Instance.IsPlantUnlocked(slot.CurrentPlantId);
            slot.RefreshUI(isUnlocked);
        }
    }
}
