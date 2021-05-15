using UnityEngine;

namespace Racing.Ship
{
    [CreateAssetMenu]
    public class ShipSounds : ScriptableObject
    {
        public AudioClip engineLoop;
    
    
        public AudioClip boostLoop;
        public AudioClip boostOneShot;
    
        public AudioClip brakeLoop;
    }
}
