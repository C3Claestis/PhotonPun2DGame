using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManajer : MonoBehaviourPunCallbacks
{
    [Header("Status Koneksi")]
    [SerializeField] Text statusKoneksi;

    [Header("Login UI Panel")]
    [SerializeField] InputField inputField;
    [SerializeField] GameObject panelLogin;

    [Header("Room Panel")]
    [SerializeField] GameObject RoomPanel;
    
    [Header("Game Panel")]
    [SerializeField] GameObject GamePanel;

    [Header("Create Room Panel")]
    [SerializeField] GameObject CreateRoomPanel;

    [Header("Show Room Panel")]
    [SerializeField] GameObject DaftarRoomPanel;

    [Header("Join Room")]
    [SerializeField] GameObject JoinRoom;

    // Start is called before the first frame update
    void Start()
    {
        // Pastikan panel login terlihat di awal
        ActivePanel(panelLogin.name);
    }

    // Update is called once per frame
    void Update()
    {
        statusKoneksi.text = "Status Koneksi: " + PhotonNetwork.NetworkClientState;
    }

    // Fungsi untuk menangani login saat tombol ditekan
    public void LoginButton()
    {
        string playerName = inputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            // Set nama pemain di Photon
            PhotonNetwork.LocalPlayer.NickName = playerName;

            // Menghubungkan ke server Photon
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected()
    {
        Debug.Log("Connecting Complete");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Terhubung Ke Server");
        ActivePanel(RoomPanel.name);
    }

    public void ActivePanel(string panelToBeActived)
    {
        panelLogin.SetActive(panelToBeActived.Equals(panelLogin.name));
        RoomPanel.SetActive(panelToBeActived.Equals(RoomPanel.name));
        CreateRoomPanel.SetActive(panelToBeActived.Equals(CreateRoomPanel.name));
        GamePanel.SetActive(panelToBeActived.Equals(GamePanel.name));
        DaftarRoomPanel.SetActive(panelToBeActived.Equals(DaftarRoomPanel.name));
        JoinRoom.SetActive(panelToBeActived.Equals(JoinRoom.name));
    }
}
