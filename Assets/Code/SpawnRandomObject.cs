using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnRandomObject : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject orbitGameobject;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Hanya MasterClient yang melakukan spawning
        {
            // Start coroutine once
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
            float randomZ = 0f;  // Adjust if needed for 3D space

            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // Wait for a random time between 5 and 10 seconds
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            // Master client spawns the object and sends the data to others
            photonView.RPC("SpawnObject", RpcTarget.AllBuffered, randomPosition);
        }
    }

    // RPC function to spawn object
    [PunRPC]
    void SpawnObject(Vector3 position)
    {
        // Instantiate object at the random position
        PhotonNetwork.Instantiate(orbitGameobject.name, position, Quaternion.identity);
    }
}
