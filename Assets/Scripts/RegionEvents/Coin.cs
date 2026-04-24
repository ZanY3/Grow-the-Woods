using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour, IPointerClickHandler
{
    private void Start()
    {
        Destroy(gameObject,5);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CoinManager.Instance.AddCoins(1);
        Destroy(gameObject);
    }
}
