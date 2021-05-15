using UnityEngine;

namespace Racing.Ship
{
    public class ThrustParticleController : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponentInParent<Rigidbody>();
            _particleSystem = GetComponent<ParticleSystem>();
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }



        void SetEmissionDirection(Vector3 inputVelocity)
        {
            //_particleSystem.; = -1 * inputVelocity.normalized;
        }
    
    
    }
}
