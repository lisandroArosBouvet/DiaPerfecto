using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semaforo : MonoBehaviour
{
    public float tiempoEnRojo = 2f;
    public float tiempoEnVerde = .65f;
    public bool estaEnRojo = true;
    private float _tiempo = 0f;
    public Collider2D collision;
    public Sprite verdeSprite;
    public Sprite rojoSprite;
    public SpriteRenderer rendererSemaforo;

    void Start()
    {
        CambioARojo(estaEnRojo);
    }
    void Update()
    {
        _tiempo += Time.deltaTime;
        if(estaEnRojo)
        {
            if(_tiempo >= tiempoEnRojo)
            {
                _tiempo = 0f;
                CambioARojo(false);
            }
        }else
        {
            if(_tiempo >= tiempoEnVerde)
            {
                _tiempo = 0f;
                CambioARojo(true);
            }
        }
    }
    private void CambioARojo(bool rojo)
    {
        estaEnRojo = rojo;
        collision.enabled = estaEnRojo;
        rendererSemaforo.sprite = estaEnRojo ? rojoSprite : verdeSprite;
    }
}
