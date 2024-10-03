using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnRandomObject : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject orbitGameobject;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Hanya MasterClient yang melakukan spawning
        {
            StartCoroutine(RandomSpawn());
        }
    }

    // Coroutine for random spawning
    IEnumerator RandomSpawn()
    {
        while (true)
        {
            // Random range for X and Y coordinates
            float randomX = Random.Range(-40f, 40f);
            float randomY = Random.Range(-40f, 40f);
            float randomZ = 0f;

            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // Randomize value (NilaiTambah) between 1 and 10
            int randomValue = Random.Range(1, 10);

            // Wait for a random time between 5 and 10 seconds
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            // Master client spawns the object with the random value and sends it to others
            photonView.RPC("SpawnObjectWithValue", RpcTarget.AllBuffered, randomPosition, randomValue);
        }
    }

    // RPC function to spawn object with a random value
    [PunRPC]
    void SpawnObjectWithValue(Vector3 position, int value)
    {
        // Instantiate object at the random position
        GameObject newObject = PhotonNetwork.Instantiate(orbitGameobject.name, position, Quaternion.identity);

        // Get the ObjectSpawnOrbit component and assign the random value to it
        ObjectSpawnOrbit orbitScript = newObject.GetComponent<ObjectSpawnOrbit>();
        if (orbitScript != null)
        {
            orbitScript.SetNilaiTambah(value); // Pass the value to the spawned object
        }
    }
}
