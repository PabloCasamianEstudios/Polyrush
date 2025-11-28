using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform Objetivo;
    void Update()
    {
        transform.LookAt(Objetivo);
    }
}
