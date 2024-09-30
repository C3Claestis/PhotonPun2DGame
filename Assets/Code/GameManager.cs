using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Generate random spawn position
        float randomX = Random.Range(-50f, 50f);
        float randomY = Random.Range(-50f, 50f);
        
        // You might want to set z to a specific value, assuming it's 0 for a 2D plane
        Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomY);

        // Instantiate player at random position
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
