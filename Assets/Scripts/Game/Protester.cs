using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

public class Protester : MonoBehaviour
{
    [SerializeField]
    private ProtesterDef    def;
    [SerializeField]
    private float           moveSpeed = 100.0f;
    [SerializeField]
    private bool            turnLeft;
    [SerializeField]
    private SpriteRenderer  bodyRenderer;
    [SerializeField]
    private SpriteRenderer  maskRenderer;
    [SerializeField]
    private Speech          speech;
    [SerializeField]
    private Emoter          emoter;

    BounceBody      bounceBody;
    BounceWalk      bounceWalk;
    ProtesterData   _protesterData;

    public ProtesterData protesterData
    {
        get
        {
            return _protesterData;
        }
        set
        {
            _protesterData = value;
            def = _protesterData.def;
        }
    }

    private void Awake()
    {
        bounceBody = bodyRenderer.GetComponent<BounceBody>();
        bounceWalk = GetComponent<BounceWalk>();
        bounceWalk.enabled = false;
    }

    void Start()
    {
        if (def)
        {
            UpdateVisuals();
            if (_protesterData == null)
            {
                _protesterData = new ProtesterData(def, GameManager.instance.currentLocationData);
                GameManager.instance.currentLocationData.AddProtester(_protesterData);
            }
        }
    }

    private void Update()
    {
        float scaleY = LocationObject.GetScaleFactor(transform.position.y);
        transform.localScale = new Vector3(scaleY, scaleY, 1.0f);

        float posZ = LocationObject.GetZ(transform.position.y);
        transform.position = transform.position.ChangeZ(posZ);
    }


    void UpdateVisuals()
    {
        var bodySprite = def.bodySprites.Random();
        bodyRenderer.sprite = bodySprite.bodySprite;
        maskRenderer.transform.localPosition = bodySprite.offset;
        maskRenderer.sprite = def.maskSprites.Random();

        bodyRenderer.flipX = turnLeft;
        if (turnLeft)
        {
            maskRenderer.transform.localPosition = bodySprite.offset.ChangeX(-bodySprite.offset.x);
            maskRenderer.flipX = turnLeft;
        }
    }

    public void MoveTo(Vector2 targetPos, Action doneFunction = null)
    {
        float distance = Vector2.Distance(transform.position.xy(), targetPos);

        turnLeft = targetPos.x < transform.position.x;
        UpdateVisuals();

        bounceBody.Stop();
        bounceBody.enabled = false;
        bounceWalk.enabled = true;
        transform.MoveToWorld(targetPos, distance / moveSpeed, "MoveProtester").Done(() =>
        {
            bounceBody.enabled = true;
            bounceWalk.enabled = false;
            doneFunction?.Invoke();
        });
    }

    public void Say(string text, float duration)
    {
        speech.Say(text, duration);
    }

    public void Emote(List<Sprite> sprites, int emoteCount)
    {
        emoter.Emote(sprites, emoteCount);
    }

    [Button("Generate")]
    void TestGeneration()
    {
        UpdateVisuals();
    }
}
