using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TornBubbleKey : MonoBehaviour
{
    float _turnSpeed;
    public Button button;
    public Animator animator;
    private void Start()
    {
        _turnSpeed = UnityEngine.Random.Range(-90,90);
        transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
    }
    public void SetKeyText()
    {
        transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
    }

    void Update()
    {
        transform.Rotate(0, 0, _turnSpeed * Time.deltaTime);
    }

    public void CorrectKey()
    {
        this.GetComponent<Image>().raycastTarget = false;
        button.interactable = false;
        animator.Play("Pop");
        GetComponent<AudioSource>().Play();
    }

    internal void Activate(bool v)
    {
        button.interactable = v;
    }
}
