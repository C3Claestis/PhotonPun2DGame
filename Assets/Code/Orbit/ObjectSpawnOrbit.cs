using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectSpawnOrbit : MonoBehaviourPun
{
    [SerializeField] SpriteRenderer sprite;
    private int NilaiTambah;

    // Start is called before the first frame update
    void Start()
    {
        // Cek apakah PhotonView sudah terhubung sebelum melakukan RPC
        if (photonView.IsMine && PhotonNetwork.IsMasterClient)
        {
            // Set NilaiTambah dan panggil RPC untuk menyinkronkan ke semua klien
            NilaiTambah = Random.Range(1, 10);
            photonView.RPC("SyncColorAndValue", RpcTarget.AllBuffered, NilaiTambah);
        }
    }

    // RPC untuk sinkronisasi warna dan NilaiTambah ke semua klien
    [PunRPC]
    void SyncColorAndValue(int nilai)
    {
        NilaiTambah = nilai;

        // Ubah warna berdasarkan NilaiTambah
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
                PhotonView orbitPhotonView = orbit.GetComponent<PhotonView>();
                if (orbitPhotonView != null)
                {
                    // Panggil RPC dan kirimkan NilaiTambah dan ID dari PhotonView milik orbit
                    photonView.RPC("HandleOrbitTrigger", RpcTarget.AllBuffered, NilaiTambah, orbitPhotonView.ViewID);
                }
                else
                {
                    Debug.LogError("PhotonView not found on: " + other.gameObject.name);
                }
            }
            else
            {
                Debug.LogError("OrbitManager not found on: " + other.gameObject.name);
            }
        }
    }


    // RPC untuk menangani interaksi dengan Orbit di semua klien
    [PunRPC]
    void HandleOrbitTrigger(int nilai, int orbitViewID)
    {
        // Cari PhotonView berdasarkan ID yang diterima dan dapatkan OrbitManager dari objek tersebut
        PhotonView orbitPhotonView = PhotonView.Find(orbitViewID);
        OrbitManager orbit = orbitPhotonView.GetComponent<OrbitManager>();

        if (orbit != null)
        {
            orbit.AddOrbitingObject(nilai);
            orbit.UpdateOrbitingObjectAngles();
        }

        // Hancurkan objek setelah trigger, ini juga akan terlihat di semua klien
        Destroy(gameObject);
    }
}
