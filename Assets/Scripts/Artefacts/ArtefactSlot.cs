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
    [Space]

    [SerializeField] private Image iconImg;

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
    }
    public void RemoveArtefact()
    {
        isOccupied = false;
        equipedData = null;
        iconImg.gameObject.SetActive(false);
        nameTxt.text = "";
        descriptionTxt.text = "";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        artefactTooltip.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        artefactTooltip.SetActive(false);
    }
}
