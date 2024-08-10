using UnityEditor;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float speed = 5.0f;

    void Start()
    {
        // Asigna el componente Rigidbody según el tipo de juego
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Mantén la misma posición Z

        if (Vector2.Distance(rb2D.position, mousePosition) < .1f)
        {
            rb2D.velocity = Vector2.zero;
            return;
        }
        // Calcula la dirección hacia el mouse
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Mueve el Rigidbody en esa dirección
        rb2D.velocity = direction * speed;
    }
}