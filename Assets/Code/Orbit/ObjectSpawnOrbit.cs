using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectSpawnOrbit : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Orbit"))
        {
            OrbitManager orbit = other.GetComponent<OrbitManager>();
            orbit.AddOrbitingObject();
            orbit.UpdateOrbitingObjectAngles();
            Destroy(gameObject);
        }
    }
}
