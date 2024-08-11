using UnityEngine;

public class PatrolUnit : MonoBehaviour
{
    public Transform[] waypoints;  // Array de waypoints
    public float speed = 2f;       // Velocidad de patrullaje
    private int currentWaypointIndex = 0; // �ndice del waypoint actual

    private Animator anim;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obtener el Rigidbody seg�n el tipo de juego
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
            // Moverse al siguiente waypoint de manera c�clica
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
            MoveToNextWaypoint();
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
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);
        spriteRenderer.flipX = transform.position.x < waypoints[currentWaypointIndex].position.x;

    }
}
