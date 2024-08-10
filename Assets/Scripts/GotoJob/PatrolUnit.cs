using UnityEngine;

public class PatrolUnit : MonoBehaviour
{
    public Transform[] waypoints;  // Array de waypoints
    public float speed = 2f;       // Velocidad de patrullaje
    private int currentWaypointIndex = 0; // Índice del waypoint actual

    private Rigidbody2D rb2D;
    private Animator anim;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obtener el Rigidbody según el tipo de juego
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Obtener el Animator del obje
        anim.speed = speed;

        // Iniciar la patrulla hacia el primer waypoint
        MoveToNextWaypoint();
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        gameObject.SetActive(true); 
        MoveToNextWaypoint();
    }

    void Update()
    {
        // Revisar si el personaje ha llegado al waypoint actual
        if (HasReachedWaypoint())
        {
            // Moverse al siguiente waypoint de manera cíclica
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            MoveToNextWaypoint();
        }
    }

    private bool HasReachedWaypoint()
    {
        // Comprobar la distancia al waypoint actual
        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.02f)
        {
            return true;
        }
        return false;
    }

    private void MoveToNextWaypoint()
    {
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        rb2D.velocity = direction * speed;
        spriteRenderer.flipX = transform.position.x < waypoints[currentWaypointIndex].position.x;

    }
}
