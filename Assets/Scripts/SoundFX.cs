using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioClip
        bostezo,
        burbuja,
        fail,
        ganaste,
        panImpact,
        tostadora
        ;
    public AudioSource audioSource;

    public void Bostezo()
    {
        audioSource.clip = bostezo;
        audioSource.Play();
    }
    public void Burbuja()
    {
        audioSource.clip = burbuja;
        audioSource.Play();
    }
    public void Fail()
    {
        audioSource.clip = fail;
        audioSource.Play();
    }
    public void Ganaste()
    {
        audioSource.clip = ganaste;
        audioSource.Play();
    }
    public void PanImpact()
    {
        audioSource.clip = panImpact;
        audioSource.Play();
    }
    public void Tostadora()
    {
        audioSource.clip = tostadora;
        audioSource.Play();
    }
    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }
}
