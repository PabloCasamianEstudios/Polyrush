using UnityEngine;

public class LookAtYou : MonoBehaviour
{

   public Transform target;
    void Update()
    {
        transform.LookAt(target);
    }


}
