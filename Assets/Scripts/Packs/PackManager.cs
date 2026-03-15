using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    public static PackManager Instance;

    public GameObject plantPrefab;
    [SerializeField] private Cell[] cells;

    [SerializeField] private GameObject allPanel;
    [SerializeField] private GameObject openPanel;
    [SerializeField] private GameObject plantChoosePanel;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TMP_Text placeClueTxt;

    [HideInInspector] public bool waitingForClick = false;

    private PlantData selectedPlant;
    private GameObject selectedPlantObject;

    // сохраняем оригинальный scale каждого растения
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private const float selectedMultiplier = 1.15f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void OpenPack()
    {
        openPanel.SetActive(false);
        plantChoosePanel.SetActive(true);

        confirmBtn.interactable = false;
        selectedPlant = null;
        selectedPlantObject = null;

        CachePlantScales();
        AnimatePlants();
    }

    void CachePlantScales()
    {
        originalScales.Clear();

        RectTransform panel = plantChoosePanel.GetComponent<RectTransform>();

        for (int i = 0; i < panel.childCount; i++)
        {
            RectTransform card = panel.GetChild(i).GetComponent<RectTransform>();
            originalScales.Add(card.gameObject, card.localScale);
        }
    }

    public bool IsExistEmptyCell()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (!cells[i].isOccupied && cells[i].isBuyied)
                return true;
        }

        return false;
    }

    public void SelectPlant(GameObject plant)
    {
        if (selectedPlantObject == plant)
        {
            ResetScale(plant);

            selectedPlantObject = null;
            selectedPlant = null;
            confirmBtn.interactable = false;

            return;
        }

        if (selectedPlantObject != null)
        {
            ResetScale(selectedPlantObject);
        }

        selectedPlantObject = plant;
        selectedPlant = plant.GetComponent<PlantVisualizer>().Data;

        SetScale(plant);

        confirmBtn.interactable = true;
    }

    public void ConfirmChoice()
    {
        if (selectedPlantObject != null)
        {
            ResetScale(selectedPlantObject);
        }

        openPanel.SetActive(true);
        plantChoosePanel.SetActive(false);
        allPanel.SetActive(false);

        waitingForClick = true;
        InteractionManager.Instance.canZoomCam = true;
        InteractionManager.Instance.canPressBtns = false;

        ChangePlaceClueTxt(
            "Click on the <color=yellow>cell</color> where you want to place the <color=green>plant</color>",
            true
        );
    }

    public void ChangePlaceClueTxt(string text, bool state)
    {
        placeClueTxt.text = text;
        placeClueTxt.gameObject.SetActive(state);

        if (state)
            InteractionManager.Instance.canZoomCam = true;
    }

    void SetScale(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();

        Vector3 original = originalScales[obj];

        rect.localScale = new Vector3(
            original.x * selectedMultiplier,
            original.y * selectedMultiplier,
            1f
        );
    }

    void ResetScale(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();

        Vector3 original = originalScales[obj];

        rect.localScale = new Vector3(
            original.x,
            original.y,
            1f
        );
    }

    void AnimatePlants()
    {
        RectTransform panel = plantChoosePanel.GetComponent<RectTransform>();

        for (int i = 0; i < panel.childCount; i++)
        {
            RectTransform card = panel.GetChild(i).GetComponent<RectTransform>();

            Vector3 originalScale = originalScales[card.gameObject];
            Vector2 targetPos = card.anchoredPosition;

            card.localScale = new Vector3(0f, 0f, 1f);
            card.anchoredPosition = targetPos + Vector2.up * 20f;

            DG.Tweening.Sequence seq = DOTween.Sequence();

            seq.Append(card.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack));
            seq.Join(card.DOAnchorPos(targetPos, 0.35f).SetEase(Ease.OutCubic));

            seq.SetDelay(i * 0.05f);
        }
    }
}
