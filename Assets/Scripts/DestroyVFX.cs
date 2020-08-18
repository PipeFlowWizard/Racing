using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVfx : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 1;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
