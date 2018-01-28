using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class ConnectGameAndJoin : Photon.MonoBehaviour
{
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;

    public string GameName;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    private double ServerGameTime;
    private double oneSecond = 1f;
    private double previousTimeCheck;
            
    private double WaitTime = 5f;
    private double timeRemainingTillStart;

    private double RoundTimeSpan = 180f;

    string newGameKeyString = "NewGameStartTime";
    string roundStartedKeyString = "RoundStarted";

    private bool roundStarted = false;

    public virtual void Start()
    {
        timeRemainingTillStart = 60f;
        Debug.Log("Game start!");
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
        PhotonNetwork.sendRate = 21;
        PhotonNetwork.sendRateOnSerialize = 21;

    }

    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(GameName);
        }

        if(Time.time - previousTimeCheck > oneSecond)
        {
            CBUG.Do("Time Remaining: " + timeRemainingTillStart);
            timeRemainingTillStart--;
            previousTimeCheck = Time.time;
            if(timeRemainingTillStart < 0)
            {
                StartRound();
            }
        }
    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 100}, null);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

        var roomProperties = PhotonNetwork.room.CustomProperties;

        if(roomProperties.ContainsKey(roundStartedKeyString) == false)
        {
            roomProperties.Add(roundStartedKeyString, false);
        }

        if (roomProperties.ContainsKey(newGameKeyString) == false)
        {
            roomProperties.Add(newGameKeyString, PhotonNetwork.time + WaitTime);
        }
        PhotonNetwork.room.SetCustomProperties(roomProperties);
        timeRemainingTillStart = (double)roomProperties[newGameKeyString] - PhotonNetwork.time;
    }

    public void StartRound()
    {
        var roomProperties = PhotonNetwork.room.CustomProperties;

        UnityEngine.Random.InitState(Convert.ToInt32((double)roomProperties[newGameKeyString]));
        var IAmBATMAN = PhotonNetwork.playerList[UnityEngine.Random.Range(0, PhotonNetwork.room.PlayerCount)].ID == PhotonNetwork.player.ID;
        
         
        roomProperties[newGameKeyString] = PhotonNetwork.time + WaitTime + RoundTimeSpan;
        roomProperties[roundStartedKeyString] =  true;
        PhotonNetwork.room.SetCustomProperties(roomProperties);
        
        timeRemainingTillStart = (double)roomProperties[newGameKeyString] - PhotonNetwork.time;
        CBUG.Do("Next Game Start time time is in: " + timeRemainingTillStart);

        if (IAmBATMAN)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlaneControls>().Batify();
        }
    }
}
