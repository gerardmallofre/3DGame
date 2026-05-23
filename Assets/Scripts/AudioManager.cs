using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Altaveus (AudioSources)")]
    public AudioSource musicaFonsSource; 
    public AudioSource efectesSource;    

    [Header("Arxius de So (AudioClips)")]
    public AudioClip musicaNivell;
    public AudioClip soPortaOberta;
    public AudioClip soClicBoto;
    public AudioClip soSelectBoto; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (musicaNivell != null)
        {
            musicaFonsSource.clip = musicaNivell;
            musicaFonsSource.loop = true;
            StartCoroutine(FadeIn(musicaFonsSource, 2f, 0.2f));
        }
    }

    public void PlayOpenDoor()
    {
        efectesSource.PlayOneShot(soPortaOberta, 0.5f); 
    }

    public void PlayButtonSelect()
    {
        efectesSource.PlayOneShot(soSelectBoto, 0.3f);
    }

    public void PlayButtonClick()
    {
        efectesSource.PlayOneShot(soClicBoto, 0.3f);
    }

    public IEnumerator FadeIn(AudioSource source, float duracio, float volumMaxim)
    {
        source.volume = 0f;
        source.Play();
        float temps = 0f;

        while (temps < duracio)
        {
            temps += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, volumMaxim, temps / duracio);
            yield return null; 
        }
        source.volume = volumMaxim;
    }

    public IEnumerator FadeOut(AudioSource source, float duracio)
    {
        float volumInicial = source.volume;
        float temps = 0f;

        while (temps < duracio)
        {
            temps += Time.deltaTime;
            source.volume = Mathf.Lerp(volumInicial, 0f, temps / duracio);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }
}