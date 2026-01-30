using NaughtyAttributes;
using System;
using UC;
using UnityEngine;

public class BounceBody: MonoBehaviour
{
    [SerializeField, MinMaxSlider(-90.0f, 90.0f)]
    private Vector2     amplitudeRange = new Vector2(0.0f, 5.0f);
    [SerializeField, MinMaxSlider(0.0f, 20.0f)]
    private Vector2     moveDuration = new Vector2(5.0f, 5.0f);
    [SerializeField, MinMaxSlider(0.0f, 20.0f)]
    private Vector2     pauseDuration = new Vector2(5.0f, 5.0f);

    float amplitude;
    float timeToMove;
    float angleInc;
    float angle;

    void Start()
    {
        timeToMove = pauseDuration.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToMove > 0.0f)
        {
            timeToMove -= Time.deltaTime;
            if (timeToMove <= 0.0f)
            {
                angleInc = 2.0f * Mathf.PI / moveDuration.Random();
                angle = 0.0f;
                amplitude = amplitudeRange.Random();
            }
        }
        if (angleInc > 0.0f)
        {
            angle += angleInc * Time.deltaTime;
            if (angle > Mathf.PI)
            {
                Stop();
            }
        }
        var p = transform.localPosition;
        p.y = amplitude * Mathf.Sin(angle);
        transform.localPosition = p;
    }

    public void Stop()
    {
        angle = 0.0f;
        timeToMove = pauseDuration.Random();
        angleInc = 0.0f;
    }
}
