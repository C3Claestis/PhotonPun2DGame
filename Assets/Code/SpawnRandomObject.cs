using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObject : MonoBehaviour
{
    [SerializeField] GameObject orbitGameobject;

    // Start is called before the first frame update
    void Start()
    {
        // Start coroutine once
        StartCoroutine(RandomSpawn());
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

            // Instantiate object at the random position
            Instantiate(orbitGameobject, randomPosition, Quaternion.identity);
        }
    }
}
