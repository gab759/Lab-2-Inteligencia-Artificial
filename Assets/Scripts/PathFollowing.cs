using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    public List<Transform> paths = new List<Transform>();
    private int index = 0;
    public float valuedistance = 1f;
    public bool loop;

    public Vector3 GetCurrentTarget()
    {
        if (paths.Count == 0) return transform.position;
        return paths[index].position;
    }

    public void UpdatePath(Vector3 point)
    {
        if (paths.Count == 0) return;

        float distance = (point - paths[index].position).magnitude;

        if (distance < valuedistance)
        {
            if (loop)
            {
                index = (index + 1) % paths.Count;
            }
            else
            {
                index = Mathf.Clamp(index + 1, 0, paths.Count - 1);
            }
        }
    }
}


// Explicación:
// Esta clase PathFollowing permite a un agente seguir una serie de puntos (paths) en el espacio.
// - paths: una lista de posiciones que el agente seguirá.
// - index: el índice actual del punto al que se dirige.
// - valuedistance: la distancia mínima para considerar que llegó a un punto y avanzar al siguiente.
// - loop: si es verdadero, cuando el agente llegue al último punto volverá al primero, haciendo un ciclo.
//
// El método NextPoint recibe la posición actual del agente y devuelve el siguiente punto hacia el que debería ir.
// Si la distancia al punto actual es menor que valuedistance, cambia al siguiente punto de la lista.
// Si loop es true, el índice reinicia al llegar al final.