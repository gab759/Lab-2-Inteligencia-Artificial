using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeSteeringBehavior
{
    Seek,
    Flee,
    Evade,
    Arrive,
    Pursuit,
    Wander,
    PathFollowing,
    ObstacleAvoidance,
}

public class Agent : MonoBehaviour
{
    public TypeSteeringBehavior type;
    public Transform target;
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float arriveRadius = 1f;
    public float wanderRadius = 5f;
    public float wanderDistance = 10f;
    public float wanderJitter = 1f;
    public PathFollowing pathFollowing;
    private Vector3 wanderTarget;

    void Start()
    {
        wanderTarget = transform.position + Random.insideUnitSphere * wanderRadius;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) type = TypeSteeringBehavior.Seek;
        else if (Input.GetKey(KeyCode.Alpha2)) type = TypeSteeringBehavior.Flee;
        else if (Input.GetKey(KeyCode.Alpha3)) type = TypeSteeringBehavior.Evade;
        else if (Input.GetKey(KeyCode.Alpha4)) type = TypeSteeringBehavior.Arrive;
        else if (Input.GetKey(KeyCode.Alpha5)) type = TypeSteeringBehavior.Pursuit;
        else if (Input.GetKey(KeyCode.Alpha6)) type = TypeSteeringBehavior.Wander;
        else if (Input.GetKey(KeyCode.Alpha7)) type = TypeSteeringBehavior.ObstacleAvoidance;
        else if (Input.GetKey(KeyCode.Alpha8)) type = TypeSteeringBehavior.PathFollowing;

        switch (type)
        {
            case TypeSteeringBehavior.Seek:
                Seek(target.position);
                break;
            case TypeSteeringBehavior.Flee:
                Flee(target.position);
                break;
            case TypeSteeringBehavior.Evade:
                Evade(target.position);
                break;
            case TypeSteeringBehavior.Arrive:
                Arrive(target.position);
                break;
            case TypeSteeringBehavior.Pursuit:
                Pursuit(target.position);
                break;
            case TypeSteeringBehavior.Wander:
                Wander();
                break;
            case TypeSteeringBehavior.ObstacleAvoidance:
                ObstacleAvoidance();
                break;
            case TypeSteeringBehavior.PathFollowing:
                if (pathFollowing != null)
                {
                    Vector3 point = pathFollowing.GetCurrentTarget();
                    Seek(point);
                    pathFollowing.UpdatePath(transform.position);
                }
                break;
        }
    }

    void Seek(Vector3 goal)
    {
        Vector3 direction = goal - transform.position;
        direction.y = 0;
        Vector3 desired = direction.normalized * speed;
        transform.position += desired * Time.deltaTime;
        if (desired != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desired), rotationSpeed * Time.deltaTime);
    }

    void Flee(Vector3 goal)
    {
        Vector3 direction = transform.position - goal;
        direction.y = 0;
        Vector3 desired = direction.normalized * speed;
        transform.position += desired * Time.deltaTime;
        if (desired != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desired), rotationSpeed * Time.deltaTime);
    }

    void Evade(Vector3 goal)
    {
        Vector3 futurePosition = goal + target.forward * 2f;
        Flee(futurePosition);
    }

    void Arrive(Vector3 goal)
    {
        Vector3 toTarget = goal - transform.position;
        toTarget.y = 0;
        float distance = toTarget.magnitude;
        float ramped = speed * (distance    / arriveRadius);
        float clamped = Mathf.Min(ramped, speed);
        Vector3 desired = toTarget.normalized * clamped;
        transform.position += desired * Time.deltaTime;
        if (desired != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desired), rotationSpeed * Time.deltaTime);
    }

    void Pursuit(Vector3 goal)
    {
        Vector3 futurePosition = goal + target.forward * 2f;
        Seek(futurePosition);
    }

    void Wander()
    {
        wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0, Random.Range(-1f, 1f) * wanderJitter);
        wanderTarget = wanderTarget.normalized * wanderRadius;
        Vector3 targetLocal = wanderTarget + transform.forward * wanderDistance;
        Vector3 targetWorld = transform.position + targetLocal;
        Seek(targetWorld);
    }

    void ObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
        {
            Vector3 avoidance = Vector3.Reflect(transform.forward, hit.normal);
            avoidance.y = 0;
            transform.position += avoidance.normalized * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(avoidance), rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
// Explicación de cada algoritmo:

// Seek: Mueve el agente directamente hacia la posición objetivo.
// Flee: Mueve el agente alejándose del objetivo.
// Evade: Se anticipa a la posición futura del objetivo y huye de esa posición.
// Arrive: Va hacia el objetivo pero desacelera a medida que se acerca para llegar suavemente.
// Pursuit: Predice hacia dónde se moverá el objetivo y lo persigue.
// Wander: Realiza un movimiento aleatorio pero suavizado para simular un vagar natural.
// PathFollowing: No implementado en este código, pero normalmente sigue una serie de puntos en un camino.
// ObstacleAvoidance: Detecta obstáculos al frente y cambia de dirección para evitarlos.