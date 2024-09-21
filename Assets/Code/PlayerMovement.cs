using Photon.Pun;  // Import Photon PUN
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun, IPunObservable  // Tambahkan IPunObservable untuk sinkronisasi manual
{
    float speed = 0.1f;
    float dirX;
    float dirY;

    [SerializeField] Transform cameraFollow;
    [SerializeField] Transform sprite_karakter;
    [SerializeField] Animator animator_karakter;

    private PhotonView photonViews;

    private GameObject sceneCamera;
    [SerializeField] GameObject playerCamera;

    void Start()
    {
        photonViews = GetComponent<PhotonView>();

        // Kamera hanya mengikuti pemain lokal 
        if (photonView.IsMine)
        {
            sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);
            playerCamera.SetActive(true);
            cameraFollow.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
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

        transform.Translate(dirX, dirY, 0);
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
