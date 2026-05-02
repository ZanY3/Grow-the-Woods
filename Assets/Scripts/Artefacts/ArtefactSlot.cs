using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtefactSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip")]
    [SerializeField] private GameObject artefactTooltip;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private GameObject deleteBtn;

    [Space]
    [SerializeField] private Image iconImg;
    [SerializeField] private ArtefactsManager artefactsManager;

    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public ArtefactData equipedData;

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

    public void RemoveArtefact()
    {
        if (equipedData != null)
        {
            StatsManager.Instance.RemoveArtefact(equipedData);

            // Notify manager so it can refresh prices if needed
            artefactsManager.OnArtefactRemoved(equipedData);
        }

        artefactTooltip.SetActive(false);
        deleteBtn.SetActive(false);
        isOccupied = false;
        equipedData = null;
        iconImg.gameObject.SetActive(false);
        nameTxt.text = "";
        descriptionTxt.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOccupied)
            artefactTooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOccupied)
            artefactTooltip.SetActive(false);
    }
}