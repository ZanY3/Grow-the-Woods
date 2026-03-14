using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector] public PlantData plantData;
    [SerializeField] private GameObject tooltip;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        while (timer >= plantData.productionInterval)
        {
            CoinManager.Instance.AddCoins(plantData.seedsAmount);
            timer -= plantData.productionInterval;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!PackManager.Instance.waitingForClick)
            tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!PackManager.Instance.waitingForClick)
            tooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(ShovelSlot.Instance.waitingForAction)
        {
            GetComponentInParent<Cell>().isOccupied = false;
            Destroy(gameObject);
            ShovelSlot.Instance.ReturnShowel();
        }
    }
}
