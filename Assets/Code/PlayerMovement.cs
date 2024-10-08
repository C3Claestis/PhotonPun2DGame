using Photon.Pun;  // Import Photon PUN
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPun, IPunObservable  // Tambahkan IPunObservable untuk sinkronisasi manual
{
    float speed = 5f;
    float dirX;
    float dirY;
    int currentHP;  // HP saat ini
    [SerializeField] Transform sprite_karakter;
    [SerializeField] Animator animator_karakter;
    [SerializeField] Text nickPlayer;
    [SerializeField] Image fillBarHP;
    [SerializeField] int maxHP = 100;  // HP Maksimum
    [SerializeField] GameManager gameManager;
    private Rigidbody2D rb;
    private GameObject cameraFollow;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;  // Set HP awal

        // Kamera hanya mengikuti pemain lokal 
        if (photonView.IsMine)
        {
            // Set nickname untuk pemain lokal
            nickPlayer.text = PhotonNetwork.NickName;
            photonView.RPC("SyncNickname", RpcTarget.Others, PhotonNetwork.NickName);

            gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
            cameraFollow = GameObject.Find("Main Camera");
            cameraFollow.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
        else
        {
            // Set nickname untuk pemain lain
            nickPlayer.text = photonView.Owner.NickName;
        }

        UpdateHPBar();  // Inisialisasi tampilan HP bar
    }

    void Update()
    {
        if (photonView.IsMine)  // Hanya untuk pemain lokal
        {
            HandleMovement();
            HandleAnimation();
            HandleCameraFollow();

            if (Input.GetKeyDown(KeyCode.P))
            {
                TakeDamage(110);
            }
            // Jika HP 0 atau kurang, hancurkan player
            if (currentHP <= 0)
            {
                // Sinkronkan bahwa player ini telah dihancurkan ke semua pemain
                photonView.RPC("DestroyPlayer", RpcTarget.AllBuffered);
                gameManager.isDefeat = true;
            }
        }
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);  // Pastikan HP tidak lebih dari max atau kurang dari 0
        UpdateHPBar();  // Perbarui tampilan bar HP

        // Sinkronisasi perubahan HP ke pemain lain
        photonView.RPC("SyncHP", RpcTarget.Others, currentHP);
    }


    // Fungsi untuk memperbarui tampilan bar HP
    void UpdateHPBar()
    {
        fillBarHP.fillAmount = (float)currentHP / maxHP;  // Perbarui UI bar HP
    }

    void HandleMovement()
    {
        dirX = Input.GetAxis("Horizontal");
        dirY = Input.GetAxis("Vertical");

        if (dirX == 0 && dirY == 0)
        {
            // Tidak ada input, tingkatkan drag agar karakter tetap diam
            rb.velocity = Vector2.zero;  // Set kecepatan ke nol
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.drag = 10f;  // Tingkatkan drag untuk mengurangi gesekan dari gaya eksternal (dorongan dari pemain lain)

            // Kirim RPC untuk menyinkronkan status diam ke pemain lain
            photonView.RPC("SyncMovement", RpcTarget.Others, Vector2.zero, 0, true);
        }
        else
        {
            // Ada input, gerakkan pemain dan atur drag kembali ke nilai normal
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.drag = 0f;  // Atur drag ke nol agar pemain bisa bergerak bebas
            Vector2 velocity = new Vector2(dirX * speed, dirY * speed);
            rb.velocity = velocity;  // Gerakkan karakter sesuai input

            // Kirim RPC untuk menyinkronkan gerakan ke pemain lain
            photonView.RPC("SyncMovement", RpcTarget.Others, velocity, Mathf.RoundToInt(speed), false);
        }
    }

    void HandleAnimation()
    {
        bool isWalking = (dirX != 0 || dirY != 0);
        animator_karakter.SetBool("Walk", isWalking);

        photonView.RPC("SyncAnimation", RpcTarget.Others, isWalking);

        FlipSprite();
    }

    // Kamera hanya mengikuti pemain lokal
    void HandleCameraFollow()
    {
        cameraFollow.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    void FlipSprite()
    {
        if (dirX < 0)
        {
            sprite_karakter.localScale = new Vector3(-1f, 1f, 1f);
            // Sinkronisasi flip dengan RPC
            photonView.RPC("SyncFlip", RpcTarget.Others, sprite_karakter.localScale);
        }
        if (dirX > 0)
        {
            sprite_karakter.localScale = new Vector3(1f, 1f, 1f);
            // Sinkronisasi flip dengan RPC
            photonView.RPC("SyncFlip", RpcTarget.Others, sprite_karakter.localScale);
        }
    }

    [PunRPC]
    void SyncMovement(Vector2 syncedVelocity, int syncedSpeed, bool isIdle)
    {
        if (!photonView.IsMine)  // Pastikan hanya pemain lain yang menerima pembaruan
        {
            if (isIdle)
            {
                // Pemain lain dalam keadaan diam
                rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                rb.drag = 10f;  // Sinkronisasi drag untuk pemain diam
            }
            else
            {
                // Pemain lain sedang bergerak
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.drag = 0f;
                rb.velocity = syncedVelocity;  // Terapkan kecepatan yang disinkronkan
            }
        }
    }

    // RPC untuk sinkronisasi animasi
    [PunRPC]
    void SyncAnimation(bool isWalking)
    {
        animator_karakter.SetBool("Walk", isWalking);
    }

    // RPC untuk sinkronisasi flip (localScale)
    [PunRPC]
    void SyncFlip(Vector3 flipScale)
    {
        sprite_karakter.localScale = flipScale;
    }

    [PunRPC]
    void SyncNickname(string nickname)
    {
        nickPlayer.text = nickname;
    }
    // Sinkronisasi HP dengan pemain lain melalui RPC
    [PunRPC]
    void SyncHP(int syncedHP)
    {
        currentHP = syncedHP;
        UpdateHPBar();  // Perbarui tampilan HP bar ketika HP disinkronkan
    }
    [PunRPC]
    void DestroyPlayer()
    {
        // Hancurkan semua child termasuk objek orbit
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);  // Hancurkan semua child
        }

        // Hancurkan game object player ini
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);  // Hancurkan objek player di Photon Network
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // Pemain lokal menulis data
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(currentHP);  // Sinkronisasi HP juga
        }
        else  // Pemain remote membaca data
        {
            transform.position = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
            currentHP = (int)stream.ReceiveNext();  // Terima sinkronisasi HP
            UpdateHPBar();  // Perbarui tampilan HP bar
        }
    }
}
