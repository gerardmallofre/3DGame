using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Altaveus (AudioSources)")]
    public AudioSource musicaFonsSource;
    public AudioSource efectesSource;

    [Header("Arxius de So (AudioClips)")]
    public AudioClip soSalt;
    public AudioClip musicaNivell;
    public AudioClip soPortaOberta;
    public AudioClip soClicBoto;
    public AudioClip soSelectBoto;
    public AudioClip soMort;
    public AudioClip soDamage;
    public AudioClip soSpikeTrap;
    public AudioClip soAxeTrap;
    public AudioClip soCoin;
    public AudioClip explosionClip;
    public AudioClip slimeImpact;
    public AudioClip soDispararFletxa;
    public AudioClip soImpactarFletxa;
    public AudioClip soTremolorTerra;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            ValidarAudioSources();
        }
        else
        {
            instance.ActualitzarTot(this);

            if (musicaNivell != null)
            {
                instance.CanviarMusica(musicaNivell);
            }
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        if (musicaNivell != null && musicaFonsSource != null && musicaFonsSource.clip == null)
        {
            musicaFonsSource.clip = musicaNivell;
            musicaFonsSource.loop = true;
            StartCoroutine(FadeIn(musicaFonsSource, 2f, 0.4f));
        }
    }

    private void ValidarAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (musicaFonsSource == null)
        {
            if (sources.Length > 0) musicaFonsSource = sources[0];
            else musicaFonsSource = gameObject.AddComponent<AudioSource>();
        }

        if (efectesSource == null)
        {
            if (sources.Length > 1) efectesSource = sources[1];
            else
            {
                efectesSource = gameObject.AddComponent<AudioSource>();
                efectesSource.playOnAwake = false;
            }
        }
    }

    private void PlaySo(AudioClip clip, float volum)
    {
        if (clip == null || efectesSource == null) return;
        efectesSource.PlayOneShot(clip, volum);
    }


    public void ActualitzarTot(AudioManager nouManager)
    {
        this.musicaNivell = nouManager.musicaNivell;
        this.soPortaOberta = nouManager.soPortaOberta;
        this.soClicBoto = nouManager.soClicBoto;
        this.soSelectBoto = nouManager.soSelectBoto;
        this.soMort = nouManager.soMort;
        this.soDamage = nouManager.soDamage;
        this.soTremolorTerra = nouManager.soTremolorTerra;
        this.soSpikeTrap = nouManager.soSpikeTrap;
        this.soAxeTrap = nouManager.soAxeTrap;
        this.soCoin = nouManager.soCoin;
        this.explosionClip = nouManager.explosionClip;
        this.slimeImpact = nouManager.slimeImpact;
        this.soDispararFletxa = nouManager.soDispararFletxa;
        this.soImpactarFletxa = nouManager.soImpactarFletxa;
        this.soSalt = nouManager.soSalt;

        if (this.musicaFonsSource == null || this.efectesSource == null)
        {
            this.musicaFonsSource = nouManager.musicaFonsSource;
            this.efectesSource = nouManager.efectesSource;
        }

        ValidarAudioSources();
    }

    public void CanviarMusica(AudioClip novaMusica)
    {
        ValidarAudioSources();
        if (musicaFonsSource == null) return;

        if (musicaFonsSource.clip == novaMusica && musicaFonsSource.isPlaying) return;

        musicaNivell = novaMusica;

        StopAllCoroutines();

        musicaFonsSource.volume = 0f;
        musicaFonsSource.Stop();
        musicaFonsSource.clip = novaMusica;
        musicaFonsSource.loop = true;

        StartCoroutine(FadeIn(musicaFonsSource, 2f, 0.1f));
    }

    public void PlayMort() { PlaySo(soMort, 2.5f); }
    public void PlayDamage() { PlaySo(soDamage, 0.5f); }
    public void PlaySpikeTrap() { PlaySo(soSpikeTrap, 0.5f); }
    public void PlayAxeTrap() { PlaySo(soAxeTrap, 0.5f); }
    public void PlayCoin() { PlaySo(soCoin, 0.5f); }
    public void PlayOpenDoor() { PlaySo(soPortaOberta, 0.5f); }
    public void PlayButtonSelect() { PlaySo(soSelectBoto, 0.1f); }
    public void PlayButtonClick() { PlaySo(soClicBoto, 0.1f); }
    public void PlayExplosion() { PlaySo(explosionClip, 0.5f); }
    public void PlaySlimeImpact() { PlaySo(slimeImpact, 0.5f); }
    public void PlayShootArrow() { PlaySo(soDispararFletxa, 0.5f); }
    public void PlayHitArrow() { PlaySo(soImpactarFletxa, 0.5f); }
    public void PlayJump() { PlaySo(soSalt, 1f); }
    public void PlayCrumble() { PlaySo(soTremolorTerra, 1f);  }

    public IEnumerator FadeIn(AudioSource source, float duracio, float volumMaxim)
    {
        if (source == null) yield break;
        source.volume = 0f;
        source.Play();
        float temps = 0f;

        while (temps < duracio)
        {
            if (source == null) yield break;
            temps += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, volumMaxim, temps / duracio);
            yield return null;
        }
        if (source != null) source.volume = volumMaxim;
    }

    public IEnumerator FadeOut(AudioSource source, float duracio)
    {
        if (source == null) yield break;
        float temps = 0f;
        float volumInicial = source.volume;

        while (temps < duracio)
        {
            if (source == null) yield break;
            temps += Time.deltaTime;
            source.volume = Mathf.Lerp(volumInicial, 0f, temps / duracio);
            yield return null;
        }
        if (source != null)
        {
            source.volume = 0f;
            source.Stop();
        }
    }
}