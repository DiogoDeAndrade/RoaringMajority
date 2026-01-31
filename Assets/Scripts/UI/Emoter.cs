using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UC;
using UnityEngine;

public class Emoter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer emotePrefab;

    public void Emote(List<Sprite> images, int nSpawns)
    {
        Emote(images, nSpawns, 10.0f, Vector2.one, 25.0f, new Vector2(15.0f, 35.0f), new Vector2(0.1f, 0.3f), new Vector2(0.3f, 0.8f), 0.1f);
    }

public void Emote(List<Sprite> images, int nSpawns, float spawnRadius, Vector2 spawnDirection, float angleVariance, Vector2 moveSpeedRange, Vector2 spawnIntervalRange, Vector2 durationRange, float fadeInOutPercentage)
    {
        StartCoroutine(EmoteCR(images, nSpawns, spawnRadius, spawnDirection, angleVariance, moveSpeedRange, spawnIntervalRange, durationRange, fadeInOutPercentage));
    }

    IEnumerator EmoteCR(List<Sprite> images, int nSpawns, float spawnRadius, Vector2 spawnDirection, float angleVariance, Vector2 moveSpeedRange, Vector2 spawnIntervalRange, Vector2 durationRange, float fadeInOutPercentage)
    {
        for (int i = 0; i < nSpawns; i++)
        {
            // Create one emote
            float duration = durationRange.Random();

            var newObj = Instantiate(emotePrefab, transform);
            {
                // Need to create this so that we can capture it without it being overwritten 
                var obj = newObj;

                obj.sprite = images.Random();
                obj.transform.position = transform.position + (Random.insideUnitCircle * spawnRadius).xyz(0.0f);
                obj.color = newObj.color.ChangeAlpha(0.0f);
                obj.FadeTo(1.0f, duration * fadeInOutPercentage).Done(() =>
                {
                    obj.FadeTo(0.0f, duration * fadeInOutPercentage).DelayStart(duration * (1.0f - 2.0f * fadeInOutPercentage)).Done(() =>
                    {
                        Destroy(obj.gameObject);
                    });
                });

            }

            Vector3 direction = spawnDirection;
            direction.RotateZ(Random.Range(-angleVariance, angleVariance));

            newObj.transform.Move(direction * moveSpeedRange.Random(), duration);

            yield return new WaitForSeconds(spawnIntervalRange.Random());
        }
    }
}
