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
        GameObject newPlayerObject = PhotonNetwork.Instantiate("Moth", SpawnLocations[0].transform.position, SpawnLocations[0].transform.rotation, 0);
        newPlayerObject.GetComponent<PlaneControls>().SpawnLocation = SpawnLocations[0].transform.position;
        newPlayerObject.GetComponent<PlaneControls>().SpawnRotation = SpawnLocations[0].transform.rotation;
        newPlayerObject.GetComponent<PlaneControls>().SpawnLocations = SpawnLocations;
    }
}
