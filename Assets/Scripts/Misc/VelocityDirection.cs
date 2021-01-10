using UnityEngine;

public class VelocityDirection : MonoBehaviour
{
    public Rigidbody rb;

   
    // Update is called once per frame
    void Update()
    {
        if(rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
}
