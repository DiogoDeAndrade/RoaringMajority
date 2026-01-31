using UnityEngine;

public class LocationObject : MonoBehaviour
{
    [SerializeField] 
    private Location        location;
    [SerializeField]
    private SpriteRenderer  backgroundImage;
    [SerializeField]
    private PolygonCollider2D _leftSpawnArea;
    [SerializeField]
    private PolygonCollider2D _leftProtestArea;
    [SerializeField]
    private PolygonCollider2D _rightSpawnArea;
    [SerializeField]
    private PolygonCollider2D _rightProtestArea;
    [SerializeField]
    private Vector2         scaleArea;
    [SerializeField]
    private Vector2         scale;
    [SerializeField]
    private Vector2         zPos;

    static LocationObject instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        backgroundImage.sprite = location.backgroundImage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-360.0f, scaleArea.x, 0.0f), new Vector3(360.0f, scaleArea.x, 0.0f));

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(-360.0f, scaleArea.y, 0.0f), new Vector3(360.0f, scaleArea.y, 0.0f));
    }

    public static float GetScaleFactor(float worldY)
    {
        float py = Mathf.Clamp01((worldY - instance.scaleArea.x) / (instance.scaleArea.y - instance.scaleArea.x));

        return Mathf.Lerp(instance.scale.x, instance.scale.y, py);
    }

    public static float GetZ(float worldY)
    {
        float py = Mathf.Clamp01((worldY - instance.scaleArea.x) / (instance.scaleArea.y - instance.scaleArea.x));

        return Mathf.Lerp(instance.zPos.x, instance.zPos.y, py);
    }

    public static PolygonCollider2D leftSpawnArea => instance._leftSpawnArea;
    public static PolygonCollider2D rightSpawnArea => instance._rightSpawnArea;
    public static PolygonCollider2D leftProtestArea => instance._leftProtestArea;
    public static PolygonCollider2D rightProtestArea => instance._rightProtestArea;
}
