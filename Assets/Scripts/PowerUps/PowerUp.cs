using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem particle;
    bool _onCooldown = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!_onCooldown)
        {
            _onCooldown = true;
            //HidePowerUp
            GetComponent<Renderer>().enabled = false;
            //ApplyEffect(other,type)

            var destroyParticle = Instantiate(particle, transform.position, transform.rotation);
            Destroy(destroyParticle, 3);
            //Cooldown
            //Reenable renderer
            StartCoroutine("Cooldown");
        }
        


    }

    

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(3);
        _onCooldown = false;
        GetComponent<Renderer>().enabled = true;
    }
}
