using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class ConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerIDGroup
    {
        public List<string> playerIDList = new List<string>();
    }

    [System.Serializable]
    public class RoomIDGroup
    {
        public List<string> roomIDList = new List<string>();
    }

    public class PlayerData
    {
        public string uid;
        public MoveRacket playerObj;
        public Vector3 correctPos;
    }

    [System.Serializable]
    public class PlayerUpdateData
    {
        public float x, y, z;
    }

    public enum ConnectionState
    {
        Disconnected, Connected
    }

    public ConnectionState connectionState;
    public MoveRacket playerObjPref;
    public string ownerID;
    public PlayerIDGroup playerIDGroup;
    public PlayerIDGroup cachePlayerIDGroup;
    public RoomIDGroup roomIDGroup;
    private List<PlayerData> characterList = new List<PlayerData>();
    private PlayerData playerDataOwner;
    private SocketIOComponent socket;
    public string roomName;
    private bool isRoom;

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();

        socket.On("OnOwnerClientConnect", OnOwnerClientConnect);
        socket.On("OnClientConnect", OnClientConnect);
        socket.On("OnClientFetchPlayerList", OnClientFetchPlayerList);
        socket.On("OnClientDisconnect", OnClientDisconnect);
        socket.On("GameIsFull", GameIsFull);

        socket.On("OnClientLeaveRoom", OnClientLeaveRoom);

        socket.On("OnClientFetchRoomList", OnClientFetchRoomList);

        socket.On("OnClientUpdateMoveList", OnClientUpdateMoveList);

        cachePlayerIDGroup = new PlayerIDGroup();
    }

    // Update is called once per frame
    void Update()
    {
        switch (connectionState)
        {
            case ConnectionState.Disconnected:
            {
                    if (socket.IsConnected)
                    {
                        connectionState = ConnectionState.Connected;
                    }

                    break;
            }

            case ConnectionState.Connected:
                {
                    /*if ()
                    {

                    }*/

                    break;
                }
            
        }

        DetectPlayerConnect();
        UpdateAllCharacter();
        UpdateBall();
    }

    public void OnJoinGameClick ()
    {
        socket.Emit("ClientJoinGame");
    }

    void UpdateBall()
    {

    }

    void UpdateAllCharacter()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].uid == ownerID)
                continue;

            Vector3 currentPos = characterList[i].playerObj.transform.position;
            currentPos = Vector3.Lerp(currentPos, characterList[i].correctPos, 5.0f * Time.deltaTime);

            characterList[i].playerObj.transform.position = currentPos;
        }
    }

    IEnumerator UpdateOwnerPlayerData()
    {
        while (connectionState == ConnectionState.Connected)
        {
            if (playerDataOwner != null && playerDataOwner.playerObj != null)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();

                Vector3 playerPos = playerDataOwner.playerObj.transform.position;
                data.Add("roomName", roomName);
                data.Add("uid", ownerID);
                data.Add("x", playerPos.x.ToString());
                data.Add("y", playerPos.y.ToString());
                data.Add("z", playerPos.z.ToString());


                JSONObject jsonObj = new JSONObject(data);

                socket.Emit("OnClientUpdateMove", jsonObj);

                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }
    }

    private void DetectPlayerConnect()
    {
        if (cachePlayerIDGroup.playerIDList.Count != playerIDGroup.playerIDList.Count)
        {
            bool checkConnect;
            List<string> firstList;
            List<string> secondList;

            if (playerIDGroup.playerIDList.Count > cachePlayerIDGroup.playerIDList.Count)
            {
                firstList = playerIDGroup.playerIDList;
                secondList = cachePlayerIDGroup.playerIDList;
                checkConnect = true;
            }
            else
            {
                firstList = cachePlayerIDGroup.playerIDList;
                secondList = playerIDGroup.playerIDList;
                checkConnect = false;
            }

            foreach (var fID in firstList)
            {
                bool isFound = false;
                foreach (var sID in secondList)
                {
                    if (fID == sID)
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    if (checkConnect)//Check player connect
                    {
                        //Debug.Log("Player connected : " + fID);
                        CreateCharacter(fID);
                    }
                    else//Check player disconnect
                    {
                        //Debug.Log("Player disconnected : " + fID);
                        DestroyCharacter(fID);
                    }
                }
            }
        }

        cachePlayerIDGroup.playerIDList = playerIDGroup.playerIDList;
    }

    private void CreateCharacter(string uid)
    {
        PlayerData newPlayerData = new PlayerData();

        newPlayerData.uid = uid;
        newPlayerData.playerObj = Instantiate(playerObjPref, Vector3.zero, Quaternion.identity);

        newPlayerData.playerObj.name = "Player : " + uid;

        if (uid == ownerID)
        {
            newPlayerData.playerObj.canControl = true;
            playerDataOwner = newPlayerData;
        }

        characterList.Add(newPlayerData);
    }

    private void DestroyCharacter(string uid)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].uid == uid)
            {
                Destroy(characterList[i].playerObj.gameObject);
                characterList.RemoveRange(i, 1);
                break;
            }
        }
    }

    public void CreateRoom(string newRoomName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomName", newRoomName);
        JSONObject jsonObj = new JSONObject(data);

        socket.Emit("OnClientCreateRoom", jsonObj);
    }

    public void JoinRoom(string newRoomName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomName", newRoomName);
        JSONObject jsonObj = new JSONObject(data);

        socket.Emit("OnClientJoinRoom", jsonObj);
    }

    public void LeaveRoom()
    {
        connectionState = ConnectionState.Connected;
        roomName = "";
        socket.Emit("OnClientLeaveRoom");
    }

    private void FetchPlayerList()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomName", roomName);
        JSONObject jsonObj = new JSONObject(data);
        socket.Emit("OnClientFetchPlayerList", jsonObj);
    }

    #region Callback Group
    void OnClientConnect(SocketIOEvent evt)
    {
        Debug.Log("OnClientConnect : " + evt.data.ToString());
        //socket.Emit("OnClientFetchPlayerList");
    }

    void OnClientDisconnect(SocketIOEvent evt)
    {
        Debug.Log("OnClientDisconnect : " + evt.data.ToString());
        //socket.Emit("OnClientFetchPlayerList");
    }

    void OnOwnerClientConnect(SocketIOEvent evt)
    {
        Debug.Log("OnOwnerClientConnect : " + evt.data.ToString());
    }

    void OnClientFetchPlayerList(SocketIOEvent evt)
    {
        Debug.Log("OnClientFetchPlayerList : " + evt.data.ToString());

        playerIDGroup = JsonUtility.FromJson<PlayerIDGroup>(evt.data.ToString());
    }

    void GameIsFull(SocketIOEvent evt)
    {
        Debug.Log("Bruh");
    }

    //======================== Room ===========================
    void OnClientCreateRoomSuccess(SocketIOEvent evt)
    {
        Debug.Log("OnClientCreateRoomSuccess : " + evt.data.ToString());

        //connectionState = ConnectionState.InRoom;

        var dictData = evt.data.ToDictionary();

        ownerID = dictData["uid"];

        StartCoroutine(UpdateOwnerPlayerData());

        FetchPlayerList();
    }

    void OnClientCreateRoomFail(SocketIOEvent evt)
    {
        Debug.Log("OnClientCreateRoomFail : " + evt.data.ToString());
    }

    void OnOwnerClientJoinRoomSuccess(SocketIOEvent evt)
    {
        Debug.Log("OnOwnerClientJoinRoomSuccess : " + evt.data.ToString());

        //connectionState = ConnectionState.InRoom;

        var dictData = evt.data.ToDictionary();

        ownerID = dictData["uid"];

        StartCoroutine(UpdateOwnerPlayerData());

        FetchPlayerList();
    }

    void OnClientJoinRoomSuccess(SocketIOEvent evt)
    {
        Debug.Log("OnClientJoinRoomSuccess : " + evt.data.ToString());

        FetchPlayerList();
    }

    void OnClientJoinRoomFail(SocketIOEvent evt)
    {
        Debug.Log("OnClientJoinRoomFail : " + evt.data.ToString());
    }

    void OnClientLeaveRoom(SocketIOEvent evt)
    {
        Debug.Log("OnClientLeaveRoom : " + evt.data.ToString());

        FetchPlayerList();
    }

    void OnClientFetchRoomList(SocketIOEvent evt)
    {
        Debug.Log("OnClientFetchRoomList : " + evt.data.ToString());

        roomIDGroup = JsonUtility.FromJson<RoomIDGroup>(evt.data.ToString());
    }

    void OnClientUpdateMoveList(SocketIOEvent evt)
    {
        var dataDict = evt.data.ToDictionary();

        for (int i = 0; i < characterList.Count; i++)
        {
            var newPlayerUpdateData = JsonUtility.FromJson<PlayerUpdateData>(dataDict[characterList[i].uid]);
            Vector3 newPos = new Vector3(newPlayerUpdateData.x, newPlayerUpdateData.y, newPlayerUpdateData.z);

            if (characterList[i].playerObj.transform.position == Vector3.zero)
            {
                characterList[i].playerObj.transform.position = newPos;
            }

            characterList[i].correctPos = newPos;
        }
    }
    #endregion
}

