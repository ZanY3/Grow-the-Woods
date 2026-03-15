using UnityEngine;

public class PackShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount = 2f;
    [SerializeField] private float shakeSpeed = 6f;

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.localEulerAngles;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        transform.localEulerAngles = new Vector3(
            startRotation.x,
            startRotation.y,
            startRotation.z + angle
        );
    }
}
