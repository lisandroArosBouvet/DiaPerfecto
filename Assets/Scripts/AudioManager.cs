using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Busca la instancia en la escena si no ha sido asignada
                _instance = FindAnyObjectByType<AudioManager>();

                if (_instance == null)
                {
                    // Si no la encuentra, crea un nuevo GameObject con el Singleton
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AudioManager>();
                    singletonObject.name = typeof(AudioManager).ToString() + " (Singleton)";

                    // Asegúrate de que el singleton no se destruya al cargar una nueva escena
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    private void Start()
    {                // Busca la instancia en la escena si no ha sido asignada
        if (FindObjectsByType<AudioManager>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public AudioClip
        menuMusic,
        gameMusic
        ;
    public AudioClip
        bostezo,
        burbuja,
        fail,
        ganaste,
        panImpact,
        tostadora
        ;
    public AudioSource 
        fxSource,
        musicSource
        ;

    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic)
            return;
        musicSource.clip = menuMusic;
        musicSource.Play();
    }
    public void PlayGameMusic()
    {
        if (musicSource.clip == gameMusic)
            return;
        musicSource.clip = gameMusic;
        musicSource.Play();
    }
    public void Bostezo()
    {
        fxSource.clip = bostezo;
        fxSource.Play();
    }
    public void Burbuja()
    {
        fxSource.clip = burbuja;
        fxSource.Play();
    }
    public void Fail()
    {
        fxSource.clip = fail;
        fxSource.Play();
    }
    public void Ganaste()
    {
        fxSource.clip = ganaste;
        fxSource.Play();
    }
    public void PanImpact()
    {
        fxSource.clip = panImpact;
        fxSource.Play();
    }
    public void Tostadora()
    {
        fxSource.clip = tostadora;
        fxSource.Play();
    }
    public void SetPitch(float pitch)
    {
        fxSource.pitch = pitch;
    }
}
