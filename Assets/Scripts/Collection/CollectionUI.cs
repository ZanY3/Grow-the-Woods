using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup collectionWindowPanel;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Tab Buttons")]
    [SerializeField] private Button plantsTabButton;
    [SerializeField] private Button artefactsTabButton;
    [SerializeField] private CanvasGroup plantsUnderline;
    [SerializeField] private CanvasGroup artefactsUnderline;

    [Header("Progress Counter")]
    [SerializeField] private TMP_Text counterText;

    [Header("Grids")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform plantsGridParent;
    [SerializeField] private Transform artefactsGridParent;

    [Header("Sounds")]
    [SerializeField] private AudioClip panelOpenSound;
    [SerializeField] private AudioClip clickSound;

    private List<CollectionSlot> plantSlots = new List<CollectionSlot>();
    private List<CollectionSlot> artefactSlots = new List<CollectionSlot>();
    private Tween fadeTween;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        InitPlantGrid();
        InitArtefactGrid();

        plantsTabButton.onClick.AddListener(() => ShowTab(isPlants: true));
        artefactsTabButton.onClick.AddListener(() => ShowTab(isPlants: false));

        ShowTab(isPlants: true, animate: false);

        collectionWindowPanel.alpha = 0f;
        collectionWindowPanel.gameObject.SetActive(false);
    }

    // ── Grid init ─────────────────────────────────────────────────────────────

    private void InitPlantGrid()
    {
        var plants = CollectionManager.Instance.GetAllPlants();
        if (plants == null) return;
        foreach (var plant in plants)
        {
            if (plant == null) continue;
            var slot = SpawnSlot(plantsGridParent);
            if (slot == null) continue;
            plantSlots.Add(slot);

            // ИСПРАВЛЕНО: Сразу подгружаем реальный статус из сохранений менеджера
            bool isUnlocked = CollectionManager.Instance.IsPlantUnlocked(plant.id);
            slot.Setup(plant, isUnlocked);
        }
    }

    private void InitArtefactGrid()
    {
        var artefacts = CollectionManager.Instance.GetAllArtefacts();
        if (artefacts == null) return;
        foreach (var artefact in artefacts)
        {
            if (artefact == null) continue;
            int runtimeId = CollectionManager.Instance.GetArtefactRuntimeId(artefact);
            var slot = SpawnSlot(artefactsGridParent);
            if (slot == null) continue;
            artefactSlots.Add(slot);

            // ИСПРАВЛЕНО: Сразу подгружаем статус и передаем верный runtimeId
            bool isUnlocked = CollectionManager.Instance.IsArtefactUnlocked(runtimeId);
            slot.Setup(artefact, runtimeId, isUnlocked);
        }
    }

    private CollectionSlot SpawnSlot(Transform parent)
    {
        var go = Instantiate(slotPrefab, parent);
        return go.GetComponent<CollectionSlot>();
    }

    // ── Tab switching ─────────────────────────────────────────────────────────

    private void ShowTab(bool isPlants, bool animate = true)
    {
        AudioManager.Instance.PlaySfxSound(panelOpenSound, 0.65f, 0.95f, 1.05f);

        plantsGridParent.gameObject.SetActive(isPlants);
        artefactsGridParent.gameObject.SetActive(!isPlants);

        SetUnderline(plantsUnderline, isPlants, animate);
        SetUnderline(artefactsUnderline, !isPlants, animate);

        if (isPlants) RefreshPlantSlots();
        else RefreshArtefactSlots();
    }

    private void SetUnderline(CanvasGroup underline, bool visible, bool animate)
    {
        if (underline == null) return;
        underline.gameObject.SetActive(true);
        underline.DOKill();
        float target = visible ? 1f : 0f;
        if (animate)
            underline.DOFade(target, 0.2f).SetUpdate(true);
        else
            underline.alpha = target;
    }

    // ── Panel open/close ──────────────────────────────────────────────────────

    public void ChangeBookState(bool state)
    {
        fadeTween?.Kill();
        AudioManager.Instance.PlaySfxSound(clickSound, 0.15f, 0.9f, 1.1f);
        if (state)
        {
            if (plantsGridParent.gameObject.activeSelf) RefreshPlantSlots();
            else RefreshArtefactSlots();

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

    // ── Slot refresh ──────────────────────────────────────────────────────────

    private void RefreshPlantSlots()
    {
        int unlockedCount = 0;
        foreach (var slot in plantSlots)
        {
            if (slot == null) continue;
            bool isUnlocked = CollectionManager.Instance.IsPlantUnlocked(slot.CurrentItemId);
            slot.RefreshUI(isUnlocked);
            if (isUnlocked) unlockedCount++;
        }
        UpdateCounter(unlockedCount, plantSlots.Count);
    }

    private void RefreshArtefactSlots()
    {
        int unlockedCount = 0;
        foreach (var slot in artefactSlots)
        {
            if (slot == null) continue;
            bool isUnlocked = CollectionManager.Instance.IsArtefactUnlocked(slot.CurrentItemId);
            slot.RefreshUI(isUnlocked);
            if (isUnlocked) unlockedCount++;
        }
        UpdateCounter(unlockedCount, artefactSlots.Count);
    }

    private void UpdateCounter(int unlocked, int total)
    {
        if (counterText != null)
            counterText.text = $"{unlocked}/{total}";
    }
}