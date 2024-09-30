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
    [SerializeField] InputField namaRoomInputfield;
    [SerializeField] InputField MaxPlayerInputfield;

    [Header("Game Panel")]
    [SerializeField] GameObject GamePanel;
    [SerializeField] Text roomInfoText;
    [SerializeField] GameObject TombolMulaiGame;

    [Header("Create Room Panel")]
    [SerializeField] GameObject CreateRoomPanel;

    [Header("Show Room Panel")]
    [SerializeField] GameObject DaftarRoomPanel;
    [SerializeField] GameObject daftarEntriRoomPrefab;
    [SerializeField] GameObject daftarRoomUtamaGameobject;

    [Header("Join Room")]
    [SerializeField] GameObject JoinRoom;
    private Dictionary<string, RoomInfo> cachedDaftarRoom;
    private Dictionary<string, GameObject> daftarRoomGameobjects;

    // Start is called before the first frame update
    void Start()
    {
        // Pastikan panel login terlihat di awal
        ActivePanel(GamePanel.name);
        cachedDaftarRoom = new Dictionary<string, RoomInfo>();
        daftarRoomGameobjects = new Dictionary<string, GameObject>();
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
    public void OnRoomCreateButtonClicked()
    {
        string namaRoom = namaRoomInputfield.text;

        if (string.IsNullOrEmpty(namaRoom))
        {
            namaRoom = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(MaxPlayerInputfield.text);

        PhotonNetwork.CreateRoom(namaRoom, roomOptions);
    }
    public void OnCancelButtonClicked()
    {
        ActivePanel(RoomPanel.name);
    }
    public override void OnConnected()
    {
        Debug.Log("Connecting Complete");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " Berhasil Membuat Room");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Bergabung dengan " + PhotonNetwork.CurrentRoom.Name);
        ActivePanel(GamePanel.name);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            TombolMulaiGame.SetActive(true);
        }
        else
        {
            TombolMulaiGame.SetActive(false);
        }

        roomInfoText.text = "Nama Room : " + PhotonNetwork.CurrentRoom.Name + " " + "Maks.players : " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Terhubung Ke Server");
        ActivePanel(RoomPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedDaftarRoom.ContainsKey(room.Name))
                {
                    cachedDaftarRoom.Remove(room.Name);
                }
            }
            else
            {
                if (cachedDaftarRoom.ContainsKey(room.Name))
                {
                    cachedDaftarRoom[room.Name] = room;
                }
                else
                {
                    cachedDaftarRoom.Add(room.Name, room);
                }
            }
        }

        foreach (RoomInfo roomInfo in cachedDaftarRoom.Values)
        {
            GameObject daftarEntriRoomGameobject = Instantiate(daftarEntriRoomPrefab);
            daftarEntriRoomGameobject.transform.SetParent(daftarRoomUtamaGameobject.transform);
            daftarEntriRoomGameobject.transform.localScale = Vector3.one;

            daftarEntriRoomGameobject.transform.Find("NamaRoomText").GetComponent<Text>().text = roomInfo.Name;
            daftarEntriRoomGameobject.transform.Find("MaxPlayerText").GetComponent<Text>().text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
            daftarEntriRoomGameobject.transform.Find("TombolGabung").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(roomInfo.Name));

            daftarRoomGameobjects.Add(roomInfo.Name, daftarEntriRoomGameobject);
        }
    }
    public void OnShowRoomListButton()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivePanel(DaftarRoomPanel.name);
    }
    void OnJoinRoomButtonClicked(string _namaroom)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(_namaroom);
    }
    void ClearRoomListView()
    {
        foreach (var daftarRoomGameobjects in daftarRoomGameobjects.Values)
        {
            Destroy(daftarRoomGameobjects);
        }

        daftarRoomGameobjects.Clear();
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
