using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class OrbitManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject orbitingObjectPrefab;  // Prefab objek yang akan memutari player
    private Transform player;  // Referensi ke player
    private int objectCount = 3;  // Jumlah awal objek
    private float orbitRadius = 7f;
    private float orbitSpeed = 200f;

    private List<GameObject> orbitingObjects = new List<GameObject>();  // List untuk menyimpan objek

    void Start()
    {
        player = GetComponent<Transform>();

        // Tambahkan objek awal ke orbit
        for (int i = 0; i < objectCount; i++)
        {
            AddOrbitingObject();
        }
        UpdateOrbitingObjectAngles();
    }

    private void AddOrbitingObject()
    {
        GameObject newObject = Instantiate(orbitingObjectPrefab, transform.position, Quaternion.identity);
        OrbitingObject orbitingObjectScript = newObject.GetComponent<OrbitingObject>();

        if (orbitingObjectScript != null)
        {
            newObject.transform.SetParent(this.transform);
            // Set OrbitManager ke objek orbit yang baru dibuat
            orbitingObjectScript.orbitManager = this;

            // Inisialisasi properti lain seperti radius, speed, player, dll.
            orbitingObjectScript.orbitRadius = orbitRadius;
            orbitingObjectScript.orbitSpeed = orbitSpeed;
            orbitingObjectScript.player = this.transform;
        }

        orbitingObjects.Add(newObject);
    }

    public void AddOrbitingObject(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newObject = Instantiate(orbitingObjectPrefab, transform.position, Quaternion.identity);
            OrbitingObject orbitingObjectScript = newObject.GetComponent<OrbitingObject>();

            if (orbitingObjectScript != null)
            {
                newObject.transform.SetParent(this.transform);
                // Set OrbitManager ke objek orbit yang baru dibuat
                orbitingObjectScript.orbitManager = this;

                // Inisialisasi properti lain seperti radius, speed, player, dll.
                orbitingObjectScript.orbitRadius = orbitRadius;
                orbitingObjectScript.orbitSpeed = orbitSpeed;
                orbitingObjectScript.player = this.transform;
            }

            orbitingObjects.Add(newObject);
        }
    }

    public void RemoveOrbitingObject(GameObject obj)
    {
        if (obj != null && orbitingObjects.Contains(obj))
        {
            orbitingObjects.Remove(obj);
            Destroy(obj);
            // UpdateOrbitingObjectAngles();
        }
    }

    public bool IsObjectInList(GameObject obj)
    {
        return orbitingObjects.Contains(obj);
    }

    public void UpdateOrbitingObjectAngles()
    {
        float angleStep = 360f / orbitingObjects.Count;

        for (int i = 0; i < orbitingObjects.Count; i++)
        {
            OrbitingObject orbitScript = orbitingObjects[i].GetComponent<OrbitingObject>();
            float angle = i * angleStep;
            orbitScript.SetInitialAngle(angle);
        }
    }
}
