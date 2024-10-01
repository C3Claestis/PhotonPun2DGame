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
        photonView.RPC("SyncOrbitAngles", RpcTarget.AllBuffered);
        UpdateOrbitingObjectAngles();  // Atur sudut objek baru
    }

    // void Update()
    // {
    //     // Contoh menambah atau menghapus objek dengan tombol input
    //     if (Input.GetKeyDown(KeyCode.O))
    //     {
    //         AddOrbitingObject();
    //         UpdateOrbitingObjectAngles();  // Atur sudut objek baru
    //     }
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         RemoveOrbitingObject(orbitingObjects.Count > 0 ? orbitingObjects[orbitingObjects.Count - 1] : null);
    //         UpdateOrbitingObjectAngles();  // Atur sudut ulang setelah objek dihapus
    //     }
    // }
    private void AddOrbitingObject()
    {
        // Buat objek baru menggunakan PhotonNetwork.Instantiate agar tersinkron di seluruh klien
        GameObject newObject = PhotonNetwork.Instantiate(orbitingObjectPrefab.name, player.position, Quaternion.identity);

        // Set newObject sebagai child dari OrbitManager
        newObject.transform.SetParent(this.transform);

        // Dapatkan komponen OrbitingObject
        OrbitingObject orbitScript = newObject.GetComponent<OrbitingObject>();
        orbitScript.player = player;
        orbitScript.orbitRadius = orbitRadius;
        orbitScript.orbitSpeed = orbitSpeed;
        orbitScript.orbitManager = this;

        // Tambahkan ke list orbitingObjects
        orbitingObjects.Add(newObject);
    }

    public void AddOrbitingObject(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Buat objek baru menggunakan PhotonNetwork.Instantiate agar tersinkron di seluruh klien
            GameObject newObject = PhotonNetwork.Instantiate(orbitingObjectPrefab.name, player.position, Quaternion.identity);

            // Set newObject sebagai child dari orbitManager
            newObject.transform.SetParent(this.transform);

            // Dapatkan komponen OrbitingObject
            OrbitingObject orbitScript = newObject.GetComponent<OrbitingObject>();
            orbitScript.player = player;
            orbitScript.orbitRadius = orbitRadius;
            orbitScript.orbitSpeed = orbitSpeed;
            orbitScript.orbitManager = this;

            // Tambahkan ke list
            orbitingObjects.Add(newObject);
        }
    }

    public void RemoveOrbitingObject(GameObject obj)
    {
        if (obj != null && orbitingObjects.Contains(obj))
        {
            orbitingObjects.Remove(obj);  // Hapus objek dari list
            Destroy(obj);  // Hapus objek dari scene
            UpdateOrbitingObjectAngles();  // Atur sudut objek baru
        }
    }

    public bool IsObjectInList(GameObject obj)
    {
        return orbitingObjects.Contains(obj);
    }

    public void UpdateOrbitingObjectAngles()
    {
        // Pastikan ada objek orbit sebelum melakukan perhitungan
        if (orbitingObjects.Count > 0)
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
    [PunRPC]
    void SyncOrbitAngles()
    {
        UpdateOrbitingObjectAngles();
    }
}

