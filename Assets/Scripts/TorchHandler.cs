using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchHandler : MonoBehaviour
{
    [SerializeField] Light light;
    [SerializeField] float startIntensity=6.25f;
    [SerializeField] float intensityVariance=1.25f;
    [SerializeField] float flickerLength=0.3f;
    float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        light.intensity = startIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        light.intensity = startIntensity + Mathf.Sin(time / flickerLength * Mathf.PI * 2) * intensityVariance;
    }
}
