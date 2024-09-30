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
        NilaiTambah = Random.Range(1, 10);

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

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Orbit"))
        {
            OrbitManager orbit = other.GetComponent<OrbitManager>();
            orbit.AddOrbitingObject(NilaiTambah);
            orbit.UpdateOrbitingObjectAngles();
            Destroy(gameObject);
        }
    }
}
