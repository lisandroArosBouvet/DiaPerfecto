using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TornBubbleKey : MonoBehaviour
{
    float _turnSpeed;
    public TMP_Text text;
    public Button button;
    private void Start()
    {
        _turnSpeed = UnityEngine.Random.Range(-90,90);   
    }
    public void SetKeyText(KeyCode key)
    {
        text.text = key.ToString();
        transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
    }

    void Update()
    {
        transform.Rotate(0, 0, _turnSpeed * Time.deltaTime);
    }

    internal void CorrectKey()
    {
        Destroy(gameObject);
    }
}
