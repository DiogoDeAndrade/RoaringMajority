using NaughtyAttributes;
using UC;
using UnityEngine;

public class RotateMask : MonoBehaviour
{
    [SerializeField, MinMaxSlider(-90.0f, 90.0f)]
    private Vector2     minMaxAngle = new Vector2(-10.0f, 10.0f);
    [SerializeField, MinMaxSlider(0.0f, 20.0f)]
    private Vector2     duration = new Vector2(5.0f, 5.0f);
    [SerializeField]
    private bool        randomStart;

    float angle;
    float angleInc;

    void Start()
    {
        if (randomStart) angle = Random.Range(-Mathf.PI, Mathf.PI);
        else angle = 0.0f;

        angleInc = 2.0f * Mathf.PI / duration.Random();
    }

    // Update is called once per frame
    void Update()
    {
        angle += angleInc * Time.deltaTime;
        while (angle > 2.0f * Mathf.PI) angle -= 2.0f * Mathf.PI;

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(minMaxAngle.x, minMaxAngle.y, Mathf.Sin(angle) * 0.5f + 0.5f));
    }
}
