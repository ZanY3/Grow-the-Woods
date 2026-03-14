using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isOccupied;
    private Color startColor;
    private Image image;

    [Space]
    [Header("Only for first cell with plant")]
    [SerializeField] private GameObject currentPlant;

    private void Start()
    {
        image = GetComponent<Image>();
        startColor = image.color;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(PackManager.Instance.waitingForClick && !isOccupied)
        {
            image.color = Color.white;
        }    
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (PackManager.Instance.waitingForClick)
        {
            image.color = startColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PackManager.Instance.waitingForClick)
        {
            image.color = startColor;
            if(!isOccupied)
            {
                Plant(PackManager.Instance.plantPrefab);
            }
            else
            {
                PackManager.Instance.ChangePlaceClueTxt("This <color=yellow>cell</color> is already occupied!", true);
            }
        }
    }

    public void Plant(GameObject plantPrefab)
    {
        if(isOccupied) return;

        currentPlant = Instantiate(plantPrefab, transform, false);

        RectTransform rect = currentPlant.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localRotation = Quaternion.identity;

        PackManager.Instance.ChangePlaceClueTxt("", false);
        isOccupied = true;
        PackManager.Instance.waitingForClick = false;
    }
}
