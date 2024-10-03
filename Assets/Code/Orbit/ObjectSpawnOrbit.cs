using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectSpawnOrbit : MonoBehaviourPun
{
    [SerializeField] SpriteRenderer sprite;
    private int NilaiTambah;

    // This function is called by the spawning script to set the random value
    public void SetNilaiTambah(int nilai)
    {
        // Set the random value
        NilaiTambah = nilai;

        // Sync the color based on the value
        SyncColorAndValue(NilaiTambah);
    }

    // Function to sync the color based on the value
    void SyncColorAndValue(int nilai)
    {
        // Set color based on the value
        if (NilaiTambah >= 1 && NilaiTambah < 4)
        {
            sprite.color = Color.white;
        }
        else if (NilaiTambah >= 4 && NilaiTambah < 7)
        {
            sprite.color = Color.blue;
        }
        else if (NilaiTambah >= 7 && NilaiTambah <= 10)
        {
            sprite.color = Color.yellow;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Orbit"))
        {
            OrbitManager orbit = other.GetComponent<OrbitManager>();

            if (orbit != null)
            {
                // Handle interaction with the orbit
                HandleOrbitTrigger(NilaiTambah, orbit);
            }
        }
    }

    // Function to handle interaction with the orbit
    void HandleOrbitTrigger(int nilai, OrbitManager orbit)
    {
        if (orbit != null)
        {
            // Add orbiting object with the synced value
            orbit.AddOrbitingObject(nilai);
            orbit.UpdateOrbitingObjectAngles();
        }

        // Destroy the spawned object after the trigger (also in all clients)
        PhotonNetwork.Destroy(gameObject);
    }
}
