using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToastImpact : MonoBehaviour
{
    public ParticleSystem migas;
    public int maxPlays = 2;
    private int _currentPlay = 0;
    private Rigidbody2D _rb;
    public float stillThreshold = 0.1f;
    private void Start()
    {
        _rb  = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_currentPlay >= maxPlays || _rb.velocity.magnitude < stillThreshold)
            return;
        _currentPlay++;
        migas.Play();
    }
}
