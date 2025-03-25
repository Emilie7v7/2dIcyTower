using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace _Scripts.Lights
{
    [RequireComponent(typeof(Light2D))]
    public class LightFlickerController : MonoBehaviour
    {
        public enum FlickerMode { Off, SmoothFire, Lantern }

        [Header("General")]
        [SerializeField] private FlickerMode mode = FlickerMode.Off;

        [Header("Smooth Fire Settings")]
        [SerializeField] private float baseIntensity = 5f;
        [SerializeField] private float flickerRange = 1f;
        [SerializeField] private float flickerSpeed = 0.3f;
        [SerializeField] private float baseRadius = 8.0f;
        [SerializeField] private float radiusRange = 8.0f;
        [SerializeField] private bool flickerRadius = true;

        [Header("Lantern Flicker Settings")]
        [SerializeField] private float flickerIntervalMin = 1f;
        [SerializeField] private float flickerIntervalMax = 5f;
        [SerializeField] private float lanternIntensityMin = 1f;
        [SerializeField] private float lanternIntensityMax = 5f;
        [SerializeField] private bool randomBurst = true;

        private Light2D _light2D;
        private float _noiseOffset;
        private Coroutine _lanternRoutine;

        private void Awake()
        {
            _light2D = GetComponent<Light2D>();
            _noiseOffset = Random.Range(0f, 100f);
        }

        private void OnEnable()
        {
            if (mode == FlickerMode.Lantern)
            {
                _lanternRoutine = StartCoroutine(LanternFlickerLoop());
            }
        }

        private void OnDisable()
        {
            if (_lanternRoutine != null)
            {
                StopCoroutine(_lanternRoutine);
            }
        }

        private void Update()
        {
            if (mode == FlickerMode.SmoothFire)
            {
                var time = Time.time * flickerSpeed + _noiseOffset;
                var noise = Mathf.PerlinNoise(time, 0f);
                _light2D.intensity = baseIntensity + (noise - 0.5f) * flickerRange;
                
                if (flickerRadius)
                {
                    _light2D.pointLightOuterRadius = baseRadius + (noise - 0.5f) * radiusRange;
                }
            }
        }

        private IEnumerator LanternFlickerLoop()
        {
            while (true)
            {
                var waitTime = Random.Range(flickerIntervalMin, flickerIntervalMax);

                // Lantern "burst" flickering logic
                if (randomBurst)
                {
                    // 50/50 chance to flicker or stay steady
                    _light2D.intensity = Random.value > 0.5f
                        ? Random.Range(lanternIntensityMin, lanternIntensityMax)
                        : baseIntensity;
                }
                else
                {
                    _light2D.intensity = Random.Range(lanternIntensityMin, lanternIntensityMax);
                }

                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
