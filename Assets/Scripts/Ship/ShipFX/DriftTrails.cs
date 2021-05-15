using UnityEngine;

namespace Racing.Ship.ShipFX
{
    public class DriftTrails : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _driftTrails;

        private ShipMovement _shipMovement;

        private void Start()
        {
            _shipMovement = GetComponentInParent<ShipMovement>();
        }

        private void Update()
        {
            var emissionModule = _driftTrails[0].emission;
            emissionModule.enabled = _shipMovement.CurrentState == _shipMovement.availableStates[2];
            var emission = _driftTrails[1].emission;
            emission.enabled = _shipMovement.CurrentState == _shipMovement.availableStates[2];
        
        }
    }
}
