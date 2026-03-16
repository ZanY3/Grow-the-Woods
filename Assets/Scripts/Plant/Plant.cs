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
            int coins = plantData.coinsAmount;

            if (plantData.name == "Cactus")
            {
                if (!Grid.Instance.HasAdjacentPlants(GetComponentInParent<Cell>()))
                {
                    coins += 2;
                }
            }
            if(plantData.name == "Royal Flower")
            {
                coins += Grid.Instance.CountAdjacentPlants(GetComponentInParent<Cell>());
            }

            CoinManager.Instance.AddCoins(coins);

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
