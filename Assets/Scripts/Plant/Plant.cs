using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private GameObject tooltip;

    public PlantData PlantData => plantData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
