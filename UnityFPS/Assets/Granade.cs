using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public GameObject fxParticle;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject fx = Instantiate(fxParticle);
        fx.transform.position = transform.position;

        Destroy(gameObject);
    }
}
