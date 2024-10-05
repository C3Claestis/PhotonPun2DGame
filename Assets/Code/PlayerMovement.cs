using Photon.Pun;  // Import Photon PUN
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPun, IPunObservable  // Tambahkan IPunObservable untuk sinkronisasi manual
{
    float speed = 2f;
    float dirX;
    float dirY;

    [SerializeField] Transform cameraFollow;
    [SerializeField] Transform sprite_karakter;
    [SerializeField] Animator animator_karakter;
    [SerializeField] Text nickPlayer;

    private Rigidbody2D rb;

    private GameObject sceneCamera;
    [SerializeField] GameObject playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Kamera hanya mengikuti pemain lokal 
        if (photonView.IsMine)
        {
            // Set nickname for the local player
            nickPlayer.text = PhotonNetwork.NickName;
            photonView.RPC("SyncNickname", RpcTarget.Others, PhotonNetwork.NickName);

            sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);
            playerCamera.SetActive(true);
            cameraFollow.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
        else
        {
            // Set nickname for other players
            nickPlayer.text = photonView.Owner.NickName;
        }
    }

    void Update()
    {
        if (photonView.IsMine)  // Hanya untuk pemain lokal
        {
            HandleMovement();
            HandleAnimation();
            HandleCameraFollow();
        }
    }

    void HandleMovement()
    {
        dirX = Input.GetAxis("Horizontal") * speed;
        dirY = Input.GetAxis("Vertical") * speed;

        // Menggunakan Rigidbody2D untuk mengatur kecepatan
        rb.velocity = new Vector2(dirX * speed, dirY * speed);
        //transform.Translate(dirX, dirY, 0);
    }

    void HandleAnimation()
    {
        bool isWalking = (dirX != 0 || dirY != 0);
        animator_karakter.SetBool("Walk", isWalking);

        // Kirim RPC untuk sinkronisasi animasi
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
    // Sinkronisasi posisi dan rotasi secara otomatis melalui Photon PUN
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // Pemain lokal menulis data
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else  // Pemain remote membaca data
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
