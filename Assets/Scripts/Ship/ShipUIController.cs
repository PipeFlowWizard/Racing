using Doozy.Engine.Progress;
using UnityEngine;

namespace Racing.Ship
{
    [RequireComponent(typeof(ShipMovement))]
    public class ShipUIController : MonoBehaviour
    {

        private ShipMovement _shipMovement;
        private ProgressorGroup _movementProgressors;
        private Progressor _velocityProgressor, _accelerationProgressor, _boostProgressor;

        // Start is called before the first frame update
        void Start()
        {
            _shipMovement = GetComponent<ShipMovement>();
            _movementProgressors = GetComponent<ProgressorGroup>();
        
            _boostProgressor = _movementProgressors.Progressors[2];
            _velocityProgressor = _movementProgressors.Progressors[1];
            _accelerationProgressor = _movementProgressors.Progressors[0];
        
            _boostProgressor.SetMax(_shipMovement.maxBoost);
            _velocityProgressor.SetMax(_shipMovement.stats.maxVelocity);
            _accelerationProgressor.SetMax(_shipMovement.stats.maxAcceleration);
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            _velocityProgressor.InstantSetValue(_shipMovement.CurrentVelocity);
            _accelerationProgressor.InstantSetValue(_shipMovement.CurrentAcceleration);
            _boostProgressor.SetValue(_shipMovement.CurrentBoost);
            foreach (var progressor in _movementProgressors.Progressors)
            {
                progressor.UpdateProgress();
                progressor.UpdateProgressTargets();
            }
        
        }
    }
}
