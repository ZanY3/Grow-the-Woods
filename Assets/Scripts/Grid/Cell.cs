using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool isOccupied = false;

    [SerializeField] private Sprite closedCellSprite;
    [SerializeField] private Sprite openedCellSprite;

    private Color startColor;
    private Image image;

    [Space]
    [Header("Only for first cell with plant")]
    [SerializeField] private GameObject currentPlant;
    public bool isBuyied = false;

    private void Start()
    {
        image = GetComponent<Image>();
        startColor = image.color;
        if(isBuyied == false)
        {
            ChangeState(false);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(PackManager.Instance.waitingForClick && !isOccupied && isBuyied)
        {
            image.color = Color.white;
        }    
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (PackManager.Instance.waitingForClick && isBuyied)
        {
            image.color = startColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PackManager.Instance.waitingForClick && isBuyied)
        {
            image.color = startColor;
            if(!isOccupied)
            {
                Plant(PackManager.Instance.plantPrefab);
                InteractionManager.Instance.canPressBtns = true;
            }
            else
            {
                PackManager.Instance.ChangePlaceClueTxt("This <color=yellow>cell</color> is already occupied!", true);
            }
        }
    }
    public void ChangeState(bool state)
    {
        if (state)
        {
            image.sprite = openedCellSprite;
            image.color = startColor;
            transform.localScale = Vector3.one;
        }
        else
        {
            image.sprite = closedCellSprite;

            var a = image.color.a;

            transform.localScale = Vector3.one * 0.75f;
        }

        isBuyied = state;
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
