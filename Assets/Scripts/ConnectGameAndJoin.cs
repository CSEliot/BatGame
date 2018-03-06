using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    private double oneSecond = 1f;
    private double previousTimeCheck;
            
    private double waitTime = 30f;
    private double timeSpan_Round = 180f;

    private float time_endOfRoundSplash = 5f;

    string time_currentGameEndsIn_keyString = "time_currentGameEndsIn";
    string time_nextGameStartsIn_keyString = "time_nextGameStartsIn";
    string roundStarted_keyString = "RoundStarted";




    public virtual void Start()
    {
        Debug.Log("Game start!");
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
        CBUG.Do("" + PhotonNetwork.sendRate);
        PhotonNetwork.sendRate = 21;
        PhotonNetwork.sendRateOnSerialize = 21;

        if(Application.isEditor || Debug.isDebugBuild)
        {
            timeSpan_Round = 10f;
            waitTime = 200f;
        }
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
            previousTimeCheck = Time.time;

            //CBUG.Do("Time Remaining: " + timeRemainingTillStart);
            if (PhotonNetwork.room == null)
                return;

            var roomProperties = PhotonNetwork.room.CustomProperties;
            //if round started
            if (roomProperties.ContainsKey(roundStarted_keyString) && (bool)roomProperties[roundStarted_keyString])
            {
                GameObject.Find("TimeRemainingUI").GetComponent<Text>().text = "" + 
                    Convert.ToInt32(((double)roomProperties[time_currentGameEndsIn_keyString] - PhotonNetwork.time));//countdownTimeSpan;
                GameObject.Find("PlayersOnlineUI").GetComponent<Text>().text = $"Moths Remaining: {CountAllMoths()}";

                //Win condition 1
                if (GameObject.Find("Moth(Clone)") == null)
                {
                    //Bats Win!
                    EndRound(true);
                }

                //win condition 2
                if ((double)roomProperties[time_currentGameEndsIn_keyString] - PhotonNetwork.time <= 0)
                {
                    if (roomProperties.ContainsKey(roundStarted_keyString) && (bool)roomProperties[roundStarted_keyString])
                    {
                        // Snacks win!
                        EndRound(false);
                    }
                }
            }

            //if round NOT started
            if (roomProperties.ContainsKey(roundStarted_keyString)          &&
                roomProperties.ContainsKey(time_nextGameStartsIn_keyString) &&
                (bool)roomProperties[roundStarted_keyString] == false)
            {
                GameObject.Find("TimeRemainingBGUI").GetComponent<Image>().enabled = false;
                GameObject.Find("TimeRemainingUI").GetComponent<Text>().text = "";
                GameObject.Find("RoundStartsInUI").GetComponent<Text>().text = "Round Starts In: " + 
                    Convert.ToInt32(((double)roomProperties[time_nextGameStartsIn_keyString] - PhotonNetwork.time));
                GameObject.Find("PlayersOnlineUI").GetComponent<Text>().text = $"Players Online: {PhotonNetwork.room.PlayerCount}";

                if ((double)roomProperties[time_nextGameStartsIn_keyString] - PhotonNetwork.time <= 0)
                {
                    StartRound();
                }
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
        if(PhotonNetwork.isMasterClient)
        {
            if(roomProperties.ContainsKey(roundStarted_keyString) == false)
            {
                roomProperties.Add(roundStarted_keyString, false);
            }

            if (roomProperties.ContainsKey(time_currentGameEndsIn_keyString) == false)
            {
                roomProperties.Add(time_currentGameEndsIn_keyString, PhotonNetwork.time + waitTime + timeSpan_Round);
            }

            if (roomProperties.ContainsKey(time_nextGameStartsIn_keyString) == false)
            {
                roomProperties.Add(time_nextGameStartsIn_keyString, PhotonNetwork.time + waitTime);
            }
        }
        PhotonNetwork.room.SetCustomProperties(roomProperties);
        CBUG.Do("Setting countdown time to: " + ((double)roomProperties[time_currentGameEndsIn_keyString] - PhotonNetwork.time)); 
    }

    public void StartRound()
    {
        if (PhotonNetwork.isMasterClient)
        {
            var roomProperties = PhotonNetwork.room.CustomProperties;

            //Find a rando to become the bat!
            List<GameObject> allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("NetPlayer"));
            allPlayers.Add(GameObject.FindGameObjectWithTag("Player"));
            var IAmBATMAN = UnityEngine.Random.Range(0, allPlayers.Count);
            for(int x = 0; x < allPlayers.Count; x++)
            {
                if(x == IAmBATMAN)
                {
                    allPlayers[IAmBATMAN].GetComponent<PhotonView>().RPC("Batify", PhotonTargets.All);
                } else
                {
                    allPlayers[x].GetComponent<PhotonView>().RPC("Mothify", PhotonTargets.All);
                }
            }
            
            GameObject.Find("Control Objects").GetComponent<PhotonView>().RPC("StartRoundLocal", PhotonTargets.All);
            
            //
            roomProperties[time_nextGameStartsIn_keyString] = PhotonNetwork.time + waitTime + timeSpan_Round;
            roomProperties[time_currentGameEndsIn_keyString] = PhotonNetwork.time + timeSpan_Round;
            roomProperties[roundStarted_keyString] =  true;
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }

   public void OnPhotonPlayerConnected (PhotonPlayer newPlayer) {
        var roomProperties = PhotonNetwork.room.CustomProperties;
        if (roomProperties.ContainsKey(roundStarted_keyString) == false || 
            (roomProperties.ContainsKey(roundStarted_keyString) == true && (bool)roomProperties[roundStarted_keyString] == false)) {
        }
    }

    [PunRPC]
    public void StartRoundLocal()
    {
        GameObject.Find("RoundStartsInUI").GetComponent<Text>().text = "";
        GameObject.Find("TimeRemainingBGUI").GetComponent<Image>().enabled = true;
        GameObject.Find("PreRoundHelpUI").GetComponent<Text>().enabled = false;
        GameObject.Find("PlayersOnlineUI").GetComponent<Text>().text = $"Moths Remaining: {PhotonNetwork.room.PlayerCount - 1}";
    }

    public void EndRound(bool batsWin)
    {
        if (PhotonNetwork.isMasterClient)
        {
            List<GameObject> allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("NetPlayer"));
            allPlayers.Add(GameObject.FindGameObjectWithTag("Player"));
            foreach (GameObject player in allPlayers)
            {
                player.GetComponent<PhotonView>().RPC("Mothify", PhotonTargets.All);
            }
            var roomProperties = PhotonNetwork.room.CustomProperties;
            roomProperties[time_nextGameStartsIn_keyString] = PhotonNetwork.time + waitTime;
            roomProperties[time_currentGameEndsIn_keyString] = PhotonNetwork.time + waitTime + timeSpan_Round;
            roomProperties[roundStarted_keyString] = false;
            PhotonNetwork.room.SetCustomProperties(roomProperties);
            if (batsWin)
            {
                GameObject.Find("Control Objects").GetComponent<PhotonView>().RPC("BatsWin", PhotonTargets.All);
            } else
            {
                GameObject.Find("Control Objects").GetComponent<PhotonView>().RPC("SnacksWin", PhotonTargets.All);
            }
            CBUG.Do("End Round Master");
        }
    }

    [PunRPC]
    public void SnacksWin()
    {
        CBUG.Do("End Round Local");
        GameObject.Find("BatsWinSplashUI").GetComponent<Image>().enabled = false;
        GameObject.Find("SnacksWinSplashUI").GetComponent<Image>().enabled = true;
        Tools.DelayFunction(killSplashUI, 5f);
    }

    [PunRPC]
    public void BatsWin()
    {
        CBUG.Do("End Round Local");
        GameObject.Find("BatsWinSplashUI").GetComponent<Image>().enabled = true;
        GameObject.Find("SnacksWinSplashUI").GetComponent<Image>().enabled = false;
        Tools.DelayFunction(killSplashUI, 5f);
    }

    public void killSplashUI ()
    {
        CBUG.Do("Kill splash UI");
        GameObject.Find("BatsWinSplashUI").GetComponent<Image>().enabled = false;
        GameObject.Find("SnacksWinSplashUI").GetComponent<Image>().enabled = false;
        GameObject.Find("PreRoundHelpUI").GetComponent<Text>().enabled = true;
    }

    public int CountAllMoths () {
        var totalMoths = 0;
        foreach (var player in GameObject.FindGameObjectsWithTag("NetPlayer")) {
            if (player.name.Contains("Moth")) {
                totalMoths++;
            }
        }
        if (GameObject.FindGameObjectWithTag("Player").name.Contains("Moth")) {
            totalMoths++;
        }
        return totalMoths;
    }
}
