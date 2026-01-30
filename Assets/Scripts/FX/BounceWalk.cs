using UC;
using UnityEngine;

public class BounceWalk : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float      stepTime = 0.1f;
    [SerializeField] private float      stepHeight = 5.0f;
    [SerializeField] private float      teleportDistance = 50.0f;
    [SerializeField] private float      minDist = 1e-3f;

    Vector3 basePos;
    Vector3 prevPos;
    float angle;
    float prevAngularSpeed;

    void Start()
    {
        prevPos = transform.position;

        basePos = targetTransform?.localPosition ?? Vector3.zero;
    }

    void FixedUpdate()
    {
        if (targetTransform == null) return;

        Vector3 deltaPos = transform.position.xy0() - prevPos.xy0();
        float distance = deltaPos.magnitude;

        if (distance > teleportDistance)
        {
            prevPos = transform.position;
            angle = 0.0f;
        }
        else
        {
            if (distance < minDist)
            {
                // Stopped - return to rest
                if ((angle > 0.0f) && (angle < Mathf.PI * 0.5f))
                {
                    angle -= prevAngularSpeed;
                    if (angle < 0.0f) angle = 0.0f;
                }
                else if (angle >= Mathf.PI * 0.5f)
                {
                    angle += prevAngularSpeed;
                    if (angle > Mathf.PI) angle = 0.0f;
                }
            }
            else
            {
                prevAngularSpeed = Time.fixedDeltaTime * Mathf.PI / stepTime;
                angle += prevAngularSpeed;
                if (angle > Mathf.PI) angle -= Mathf.PI;
            }
        }

        targetTransform.localPosition = basePos + Vector3.up * Mathf.Sin(angle) * stepHeight;
        prevPos = transform.position;
    }
}
