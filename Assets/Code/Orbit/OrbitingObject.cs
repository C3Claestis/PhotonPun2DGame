using Photon.Pun;
using UnityEngine;

public class OrbitingObject : MonoBehaviourPunCallbacks
{
    [HideInInspector] public float orbitRadius;  // Jarak dari player
    [HideInInspector] public float orbitSpeed;   // Kecepatan rotasi
    [HideInInspector] public Transform player;   // Referensi ke player
    public OrbitManager orbitManager;            // Referensi ke OrbitManager
    private float angle;                         // Sudut rotasi saat ini
    private Quaternion targetRotation;           // Rotasi yang akan dituju

    void Start()
    {
        // Set rotasi awal menjadi rotasi objek saat ini
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (player != null)
        {
            // Hitung sudut baru untuk orbit
            angle += orbitSpeed * Time.deltaTime;

            // Konversi sudut ke radian untuk perhitungan trigonometri
            float radians = angle * Mathf.Deg2Rad;

            // Set posisi objek berdasarkan sudut
            float x = player.position.x + Mathf.Cos(radians) * orbitRadius;
            float y = player.position.y + Mathf.Sin(radians) * orbitRadius;

            // Set posisi objek
            transform.position = new Vector2(x, y);

            // Update rotasi objek agar smooth mengikuti pergerakan orbit
            UpdateRotation(radians);

            // Lerp rotasi menuju rotasi target secara smooth
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * orbitSpeed);
        }
    }

    // Fungsi untuk mengatur sudut awal setiap objek orbit
    public void SetInitialAngle(float initialAngle)
    {
        angle = initialAngle;
    }

    // Fungsi untuk update rotasi
    void UpdateRotation(float radians)
    {
        // Hitung arah rotasi yang dituju berdasarkan posisi saat ini
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        // Dapatkan rotasi yang sesuai arah orbit menggunakan arctangent (atan2)
        float zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set target rotasi menggunakan zRotation yang baru
        targetRotation = Quaternion.Euler(0f, 0f, zRotation - 90f);  // -90f untuk menyesuaikan orientasi objek agar selalu menghadap ke depan saat orbit
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OrbitingObject otherOrbitingObject = collision.gameObject.GetComponent<OrbitingObject>();
        if (otherOrbitingObject != null)
        {
            if (!orbitManager.IsObjectInList(otherOrbitingObject.gameObject))
            {
                Debug.Log("Objek orbit bertabrakan dengan objek lain: " + collision.gameObject.name);
                // Hapus objek ini saat bertabrakan, sinkron ke seluruh klien
                orbitManager.RemoveOrbitingObject(this.gameObject);
            }
        }
    }
}
