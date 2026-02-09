using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.Audio;

public class CrowdAudio : MonoBehaviour
{
    [Serializable]
    struct AudioLayer
    {
        public int          layerCount;
        public AudioClip    clip;
    }

    [SerializeField] private List<AudioLayer>   layers;
    [SerializeField] private AudioMixerGroup    mixer;
    [SerializeField] private float              rotationPeriod = 6.0f;
    [SerializeField] private float              fadeSpeed = 2.5f;
    [SerializeField] private int                shuffleSeed = 12345;
    [SerializeField] private float              loudnessGamma = 2.2f;
    [SerializeField] private bool               cooldownEnable = false;
    [SerializeField, ShowIf(nameof(cooldownEnable)), MinMaxSlider(0.0f, 20.0f)] 
    private Vector2 cooldownTime = Vector2.zero;
    [SerializeField, ShowIf(nameof(cooldownEnable)), MinMaxSlider(0.0f, 20.0f)] 
    private Vector2 playTime = Vector2.zero;

    private int[] _order;
    private int _lastOrderN = -1;
    private int _lastStep = int.MinValue;
    [SerializeField, Range(0.0f, 1.0f)] 
    private float   _intensity;
    private float cooldownTimer;
    private float playTimer;
    private float lastIntensityPlayed = 0.0f;
    private float currentVolume = 0.0f;

    public float intensity
    {
        get { return _intensity; }
        set 
        { 
            float delta = Mathf.Abs(value - lastIntensityPlayed);
            _intensity = value; 
            if (delta > 0.15f)
            {
                // Skip cooldown if the intensity changed a lot since the last time it played
                cooldownTimer = 0.0f;
                lastIntensityPlayed = value;
            }
        }
    }

    private List<AudioSource> audioSources;

    void Start()
    {
        audioSources = new();
        foreach (var layer in layers)
        {
            for (int i = 0; i < layer.layerCount; i++)
            {
                audioSources.Add(AddLayer(layer.clip));
            }
        }
        currentVolume = intensity;
    }

    void Update()
    {
        if (audioSources == null || audioSources.Count == 0)
            return;

        int n = audioSources.Count;

        // Ensure we have a permutation array for this N
        if ((_order == null) || (_lastOrderN != n))
        {
            _order = new int[n];
            for (int i = 0; i < n; i++) _order[i] = i;
            _lastOrderN = n;
            _lastStep = int.MinValue;
        }

        // Periodically reshuffle which sources get the "loud slots"
        int step = rotationPeriod > 0.0001f ? Mathf.FloorToInt(Time.time / rotationPeriod) : 0;
        if (step != _lastStep)
        {
            _lastStep = step;
            ShuffleDeterministic(_order, shuffleSeed ^ step);
        }

        // intensity [0..1] -> [0..n] layers worth of loudness
        float   shaped = Mathf.Pow(Mathf.Clamp01(_intensity), loudnessGamma);

        // Check cooldown 
        if ((cooldownEnable) && (shaped > float.Epsilon))
        {
            if (playTimer > 0.0f)
            {
                playTimer -= Time.deltaTime;
                if (playTimer <= 0.0f)
                {
                    cooldownTimer = cooldownTime.Random();
                    playTimer = 0.0f;
                    shaped = 0.0f;
                }
            }
            else if (cooldownTimer > 0.0f)
            {
                // On cooldown, don't play 
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0.0f)
                {
                    playTimer = playTime.Random();
                }
                shaped = 0.0f;
            }
            else if (cooldownTimer == 0.0f)
            {
                playTimer = playTime.Random();
            }
        }

        currentVolume = Mathf.MoveTowards(currentVolume, shaped, Time.deltaTime);

        float   x = currentVolume * n;
        int     full = Mathf.FloorToInt(x);
        float   frac = x - full;

        // Apply volumes in the shuffled order:
        // first 'full' are 1, next is 'frac', rest 0
        for (int rank = 0; rank < n; rank++)
        {
            int idx = _order[rank];
            var src = audioSources[idx];

            float target = 0f;
            if (rank < full) target = 1f;
            else if (rank == full) target = frac;

            // Smooth transitions to avoid audible stepping
            src.volume = Mathf.MoveTowards(src.volume, target, fadeSpeed * Time.deltaTime);

            // Optional: pause when effectively silent
            const float eps = 0.0005f;
            if (src.volume > eps)
            {
                if (!src.isPlaying) src.UnPause();
            }
            else
            {
                if (src.isPlaying) src.Pause();
            }
        }
    }

    private static void ShuffleDeterministic(int[] arr, int seed)
    {
        // Fisher-Yates with deterministic LCG
        uint state = (uint)seed;
        for (int i = arr.Length - 1; i > 0; i--)
        {
            state = state * 1664525u + 1013904223u;
            int j = (int)(state % (uint)(i + 1));
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    AudioSource AddLayer(AudioClip clip)
    {
        GameObject go = new GameObject();
        go.name = $"Layer {clip.name}";
        go.transform.SetParent(transform);
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = 0.0f;
        audioSource.outputAudioMixerGroup = mixer;
        audioSource.priority = 50;
        audioSource.time = UnityEngine.Random.Range(0.0f, clip.length);
        audioSource.Play();

        return audioSource;
    }
}
