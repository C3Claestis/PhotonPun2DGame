using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnRandomObject : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject orbitGameobject;
    [SerializeField] Transform[] pointSpawn; // Array of spawn points

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
            // Cek apakah semua pointSpawn sudah penuh (memiliki child)
            if (AllSpawnPointsFull())
            {
                Debug.Log("Semua pointSpawn sudah penuh. Berhenti spawn.");
                yield break; // Hentikan coroutine jika semua penuh
            }

            // Randomize value (NilaiTambah) between 1 and 10
            int randomValue = Random.Range(1, 10);

            // Wait for a random time between 5 and 10 seconds
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            // Dapatkan random pointSpawn yang kosong
            Transform spawnPoint = GetRandomAvailableSpawnPoint();

            if (spawnPoint != null) // Pastikan ada pointSpawn yang tersedia
            {
                int spawnIndex = System.Array.IndexOf(pointSpawn, spawnPoint); // Dapatkan index pointSpawn yang dipilih
                
                // Master client spawns the object with the random value and sends it to others
                photonView.RPC("SpawnObjectWithValue", RpcTarget.AllBuffered, spawnIndex, randomValue);
            }
        }
    }

    // RPC function to spawn object with a random value
    [PunRPC]
    void SpawnObjectWithValue(int spawnIndex, int value)
    {
        // Dapatkan pointSpawn berdasarkan index yang dikirim
        Transform parentSpawnPoint = pointSpawn[spawnIndex];

        // Instantiate object di posisi pointSpawn yang dipilih
        GameObject newObject = PhotonNetwork.Instantiate(orbitGameobject.name, parentSpawnPoint.position, Quaternion.identity);

        // Jadikan object sebagai child dari spawnPoint yang dipilih
        newObject.transform.SetParent(parentSpawnPoint);

        // Get the ObjectSpawnOrbit component and assign the random value to it
        ObjectSpawnOrbit orbitScript = newObject.GetComponent<ObjectSpawnOrbit>();
        if (orbitScript != null)
        {
            orbitScript.SetNilaiTambah(value); // Pass the value to the spawned object
        }
    }

    // Function to get a random available spawn point that has no child
    Transform GetRandomAvailableSpawnPoint()
    {
        List<Transform> availableSpawnPoints = new List<Transform>();

        // Cek semua pointSpawn dan tambahkan yang tidak memiliki child ke dalam list
        foreach (Transform point in pointSpawn)
        {
            if (point.childCount == 0) // Hanya tambahkan jika point tidak memiliki child
            {
                availableSpawnPoints.Add(point);
            }
        }

        if (availableSpawnPoints.Count > 0)
        {
            // Ambil spawn point secara acak dari list yang tersedia
            return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        }

        return null; // Tidak ada spawn point yang tersedia
    }

    // Function to check if all spawn points are full (have a child)
    bool AllSpawnPointsFull()
    {
        foreach (Transform point in pointSpawn)
        {
            if (point.childCount == 0) // Jika ada point yang tidak memiliki child, return false
            {
                return false;
            }
        }
        return true; // Semua point memiliki child
    }
}
