using UnityEngine;
using System.Collections.Generic;

public class OrbitManager : MonoBehaviour
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
    }

    void Update()
    {
        // Contoh menambah atau menghapus objek dengan tombol input
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AddOrbitingObject();
            UpdateOrbitingObjectAngles();  // Atur sudut objek baru
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RemoveOrbitingObject();
            UpdateOrbitingObjectAngles();  // Atur sudut ulang setelah objek dihapus
        }
    }

    public void AddOrbitingObject()
    {
        // Buat objek baru
        GameObject newObject = Instantiate(orbitingObjectPrefab);
        newObject.transform.position = player.position;

        // Dapatkan komponen OrbitingObject
        OrbitingObject orbitScript = newObject.GetComponent<OrbitingObject>();
        orbitScript.player = player;
        orbitScript.orbitRadius = orbitRadius;
        orbitScript.orbitSpeed = orbitSpeed;

        // Tambahkan ke list
        orbitingObjects.Add(newObject);

        // Update sudut distribusi objek orbit
        UpdateOrbitingObjectAngles();
    }

    public void RemoveOrbitingObject()
    {
        if (orbitingObjects.Count > 0)
        {
            GameObject lastObject = orbitingObjects[orbitingObjects.Count - 1];
            orbitingObjects.Remove(lastObject);
            Destroy(lastObject);  // Hapus objek dari scene

            // Update sudut distribusi objek orbit
            UpdateOrbitingObjectAngles();
        }
    }

    // Fungsi untuk memperbarui sudut distribusi setiap objek
    private void UpdateOrbitingObjectAngles()
    {
        // Hitung sudut distribusi per objek
        float angleStep = 360f / orbitingObjects.Count;

        for (int i = 0; i < orbitingObjects.Count; i++)
        {
            // Dapatkan script OrbitingObject pada setiap objek
            OrbitingObject orbitScript = orbitingObjects[i].GetComponent<OrbitingObject>();

            // Atur sudut distribusi awal untuk setiap objek
            orbitScript.SetInitialAngle(i * angleStep);
        }
    }
}
