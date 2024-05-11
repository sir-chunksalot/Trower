using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [SerializeField] private List<float> waves;
    [SerializeField] private GameObject flag;
    [SerializeField] private float sliderWidth;
    private Slider slider;

    public float fillSpeed = 0.5f;
    private float targetProgress = 0;
    public event EventHandler OnWaveStart;
    void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    private void Start()
    {
        foreach (float wave in waves)
        {
            Debug.Log(wave + "wave");
            Vector3 spawnPos = new Vector3(gameObject.transform.position.x - (((sliderWidth * wave) - 80) * -2), gameObject.transform.position.y + 15, gameObject.transform.position.z + 1);
            Instantiate(flag, spawnPos, Quaternion.identity, transform);
        }
    }
    void Update()
    {
        foreach (float wave in waves)
        {
            if (slider.value >= wave)
            {
                OnWaveStart?.Invoke(gameObject, EventArgs.Empty);
                Debug.Log("WAVE BEGIN");
                waves.Remove(wave);
                break;
            }
        }

        if (slider.value < targetProgress)
        {
            slider.value += fillSpeed * Time.deltaTime;
        }
    }

    public void IncrementProgress(float newProgress, bool instant = false)
    {
        targetProgress = slider.value + newProgress;
    }

    public float GetProgress()
    {
        return slider.value;
    }
}
