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

        // Tambahkan objek awal ke orbit jika kita adalah master client
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < objectCount; i++)
            {
                AddOrbitingObject();
            }
            // Sinkronisasi sudut objek untuk semua pemain
            photonView.RPC("SyncOrbitAngles", RpcTarget.AllBuffered);
        }
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
        if (PhotonNetwork.IsMasterClient)
        {
            // Buat objek baru menggunakan PhotonNetwork.Instantiate agar tersinkron di seluruh klien
            GameObject newObject = PhotonNetwork.Instantiate(orbitingObjectPrefab.name, player.position, Quaternion.identity);

            // Set newObject sebagai child dari OrbitManager
            newObject.transform.SetParent(this.transform);

            // Panggil RPC untuk mengatur properti pada OrbitingObject yang baru dibuat
            photonView.RPC("InitializeOrbitingObject", RpcTarget.AllBuffered, newObject.GetComponent<PhotonView>().ViewID, player.GetComponent<PhotonView>().ViewID, orbitRadius, orbitSpeed);

            // Tambahkan ke list orbitingObjects
            orbitingObjects.Add(newObject);

            // Perbarui sudut objek yang baru
            UpdateOrbitingObjectAngles();
        }
    }

    public void AddOrbitingObject(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Buat objek baru menggunakan PhotonNetwork.Instantiate agar tersinkron di seluruh klien
            GameObject newObject = PhotonNetwork.Instantiate(orbitingObjectPrefab.name, player.position, Quaternion.identity);

            // Set newObject sebagai child dari OrbitManager
            newObject.transform.SetParent(this.transform);

            // Panggil RPC untuk mengatur properti pada OrbitingObject yang baru dibuat
            photonView.RPC("InitializeOrbitingObject", RpcTarget.AllBuffered, newObject.GetComponent<PhotonView>().ViewID, player.GetComponent<PhotonView>().ViewID, orbitRadius, orbitSpeed);

            // Tambahkan ke list orbitingObjects
            orbitingObjects.Add(newObject);
        }
        // Sinkronisasi sudut objek untuk semua pemain
        photonView.RPC("SyncOrbitAngles", RpcTarget.AllBuffered);
        UpdateOrbitingObjectAngles(); // Update sudut setelah menambahkan objek
    }


    public void RemoveOrbitingObject(GameObject obj)
    {
        if (obj != null && orbitingObjects.Contains(obj))
        {
            // Hapus objek dari list
            orbitingObjects.Remove(obj);

            // Hapus objek dari semua klien
            PhotonNetwork.Destroy(obj);

            // Perbarui sudut objek yang tersisa
            UpdateOrbitingObjectAngles();
        }
    }

    public bool IsObjectInList(GameObject obj)
    {
        return orbitingObjects.Contains(obj);
    }

    public void UpdateOrbitingObjectAngles()
    {
        // Hanya master client yang menghitung sudut dan menyinkronkannya
        if (PhotonNetwork.IsMasterClient)
        {
            float angleStep = 360f / orbitingObjects.Count;
            float[] angles = new float[orbitingObjects.Count];

            for (int i = 0; i < orbitingObjects.Count; i++)
            {
                OrbitingObject orbitScript = orbitingObjects[i].GetComponent<OrbitingObject>();
                float angle = i * angleStep;
                orbitScript.SetInitialAngle(angle);
                angles[i] = angle; // Simpan sudut untuk dikirimkan melalui RPC
            }

            // Kirim sudut ke semua klien lain
            photonView.RPC("SyncOrbitAnglesWithClients", RpcTarget.AllBuffered, angles);
        }
    }

    [PunRPC]
    void SyncOrbitAnglesWithClients(float[] angles)
    {
        for (int i = 0; i < orbitingObjects.Count; i++)
        {
            OrbitingObject orbitScript = orbitingObjects[i].GetComponent<OrbitingObject>();
            orbitScript.SetInitialAngle(angles[i]); // Set sudut yang diterima dari master
        }
    }

    [PunRPC]
    void InitializeOrbitingObject(int newObjectViewID, int playerViewID, float radius, float speed)
    {
        // Temukan PhotonView dari objek yang baru dibuat
        PhotonView newObjectPhotonView = PhotonView.Find(newObjectViewID);
        OrbitingObject orbitingObject = newObjectPhotonView.GetComponent<OrbitingObject>();

        if (orbitingObject != null)
        {
            // Temukan PhotonView dari player
            PhotonView playerPhotonView = PhotonView.Find(playerViewID);
            orbitingObject.player = playerPhotonView.transform;  // Set player
            orbitingObject.orbitRadius = radius;  // Set radius
            orbitingObject.orbitSpeed = speed;    // Set speed
            orbitingObject.orbitManager = this;   // Set orbit manager
        }
    }

    [PunRPC]
    void SyncOrbitAngles()
    {
        // Hanya klien selain master yang menyinkronkan sudutnya dari master
        if (!PhotonNetwork.IsMasterClient)
        {
            UpdateOrbitingObjectAngles();
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Panggil AddOrbitingObject sesuai jumlah objek saat ini untuk memastikan sinkronisasi
            for (int i = 0; i < objectCount; i++)
            {
                AddOrbitingObject();
            }

            photonView.RPC("SyncOrbitAnglesWithClients", RpcTarget.AllBuffered);
        }
    }

}

