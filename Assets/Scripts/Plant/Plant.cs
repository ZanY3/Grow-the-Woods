using UnityEngine;
using UnityEngine.EventSystems;

public class Plant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlantData plantData;
    [SerializeField] private GameObject tooltip;

    public PlantData PlantData => plantData;

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
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
