using UnityEngine;

public class VelocityDirection : MonoBehaviour
{
    public Rigidbody rb;

   
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
}
