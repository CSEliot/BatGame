using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCamNPlayer : Photon.MonoBehaviour {

    public GameObject[] SpawnLocations;

    // Use this for initialization
    void Start () {
        //Debug.LogWarning("whoa!!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnJoinedRoom()
    {
        CreatePlayerObject();
    }
    void CreatePlayerObject()
    {
        Vector3 position = new Vector3(33.5f, 1.5f, 20.5f);

        GameObject newPlayerObject = PhotonNetwork.Instantiate("Moth", SpawnLocations[0].transform.position, SpawnLocations[0].transform.rotation, 0);
    }
}
