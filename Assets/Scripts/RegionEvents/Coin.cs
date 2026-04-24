using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour, IPointerClickHandler
{
    private float minGravityAmout = 0.15f;
    private float maxGravityAmout = 0.25f;
    private float gravityAmout;
    private void Start()
    {
        Destroy(gameObject,5);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        gravityAmout = Random.Range(minGravityAmout, maxGravityAmout);
        GetComponent<Rigidbody2D>().gravityScale = gravityAmout;
        CoinManager.Instance.AddCoins(1);
        Destroy(gameObject);
    }
}
