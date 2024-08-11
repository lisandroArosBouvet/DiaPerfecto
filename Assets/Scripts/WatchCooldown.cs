using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchCooldown : MonoBehaviour
{
    public Image relojImage; // Referencia al Image de la UI que muestra el reloj
    public Sprite[] segmentosSprites; // Arreglo de sprites, un sprite por cada segmento (debe tener 3 sprites)
    public RectTransform manecillaTransform; // Referencia al RectTransform de la manecilla
    public float tiempoTotal = 10f; // Tiempo total que tarda en completarse el reloj
    private float[] _timeSegments = new float[] { .5f, .75f, 1.1f };
    public Action OnTimeComplete; // Acción que se dispara cuando el tiempo se completa

    private float tiempoPorSegmento;
    private float tiempoActual;
    private int segmentoActual;

    private bool _ticTac = true;

    void Start()
    {
        gameObject.SetActive(false);
        _ticTac = false;
    }
    public void SetTimeAndFinishEvent(float totalTime, Action evFinishTime)
    {
        gameObject.SetActive(true);
        _ticTac = true;
        // Calcular el tiempo por segmento
        tiempoTotal = totalTime;
        OnTimeComplete = evFinishTime;
        segmentoActual = 0;
        tiempoPorSegmento = tiempoTotal * _timeSegments[segmentoActual];
        tiempoActual = 0f;

        // Asegurar que el reloj empiece con el primer sprite
        if (segmentosSprites.Length > 0)
        {
            relojImage.sprite = segmentosSprites[0];
        }

        // Asegurar que la manecilla esté en la posición inicial (en la cima del reloj)
        manecillaTransform.localRotation = Quaternion.Euler(0, 0, 90); // 90 grados para que la manecilla apunte hacia arriba
    }
    public void StopWatch()
    {
        _ticTac = false;
    }

    void Update()
    {
        if (_ticTac == false)
            return;
        // Actualizar el tiempo
        tiempoActual += Time.deltaTime;

        // Calcular en qué segmento estamos

        // Si cambiamos de segmento, actualizar el sprite
        if (tiempoActual >= tiempoPorSegmento)
        {
            segmentoActual++;
            tiempoPorSegmento = tiempoTotal * _timeSegments[segmentoActual];
            relojImage.sprite = segmentosSprites[segmentoActual];
        }

        // Calcular la rotación de la manecilla
        float tiempoFraccion = tiempoActual / tiempoTotal;
        float anguloRotacion = Mathf.Lerp(0, -360, tiempoFraccion); // 90 grados a -270 grados para una vuelta completa en sentido antihorario

        // Actualizar la rotación de la manecilla
        manecillaTransform.localRotation = Quaternion.Euler(0, 0, anguloRotacion);

        // Si el tiempo se ha completado
        if (tiempoActual >= tiempoTotal)
        {
            _ticTac = false;
            OnTimeComplete?.Invoke(); // Disparar la acción al completar el tiempo
            tiempoActual = 0f; // Resetear el tiempo o puedes detener el script aquí
        }
    }
}
