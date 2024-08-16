using System;
using UnityEditor;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float speed = 5.0f;
    public TrailRenderer trailRenderer;
    public TrailRenderer trailRenderer2;
    public float timeToTrail = .5f;
    float _time = 0;
    Animator _anim;
    private bool _inGame = true;

    void Start()
    {
        // Asigna el componente Rigidbody según el tipo de juego
        rb2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (_inGame == false)
            return;
        MovementCar();
    }

    private void MovementCar()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Mantén la misma posición Z

        if (Vector2.Distance(rb2D.position, mousePosition) < .1f)
        {
            rb2D.velocity = Vector2.zero;
            _time = 0;
            trailRenderer.emitting = true;
            trailRenderer2.emitting = true;
            _anim.speed = 0;
            return;
        }
        RotationCar();
        _time += Time.deltaTime;
        if(_time  > timeToTrail && trailRenderer.emitting)
        {
            _anim.speed = 1;
            trailRenderer.emitting = false;
            trailRenderer2.emitting = false;
        }
        // Calcula la dirección hacia el mouse
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Mueve el Rigidbody en esa dirección
        rb2D.velocity = direction * speed;
    }

    private void RotationCar()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;
    }
    public void Die()
    {
        _inGame = false;
        GetComponent<Collider2D>().enabled = false;
        rb2D.velocity = Vector2.zero;
        _anim.speed = 0;
    }
}