using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleThingsInRound : MonoBehaviour {

    public Thing[] Things;
    public bool EnableSingleOnly = true;
    public bool DisableAllOnRoundEnd = false;

    private bool disableAllNextPress;

    public enum ToggleType {
        Key,
        Input
    }
    public ToggleType ChosenToggleType;

    private int currentEnabled;
    [HideInInspector]
    public string _ToggleInput;
    [HideInInspector]
    public string _ToggleKey;

    public enum ThingType {
        GameObject,
        Component
    }

    [System.Serializable]
    public struct Thing {
        public Component    ComponentThing;
        public GameObject   GameObjectThing;
        public ThingType    Type;
    }

	// Use this for initialization
	void Start () {
        currentEnabled = 0;
        disableAllNextPress = false;
	}
	
	// Update is called once per frame
	void Update () {
        var doToggle = false;
		if(ChosenToggleType == ToggleType.Key && Input.GetKeyDown(_ToggleKey)) {
            doToggle = true;
        } else if (ChosenToggleType == ToggleType.Input && Input.GetButtonDown(_ToggleInput)){
            doToggle = true;
        }
        if(doToggle) {
            if(disableAllNextPress) {
                disableAll();
                disableAllNextPress = false;
                return;
            }
            if(EnableSingleOnly) {
                disableAll();
            }
            var tempThingType = Things[currentEnabled].Type;
            if(tempThingType == ThingType.Component) {
                (Things[currentEnabled].ComponentThing as Behaviour).enabled = true;
            } else {
                Things[currentEnabled].GameObjectThing.gameObject.SetActive(true);
            }
            currentEnabled++;
            if(currentEnabled >= Things.Length) {
                currentEnabled = 0;
                if (DisableAllOnRoundEnd) {
                    disableAllNextPress = true;
                }
            }
        }
	}

    private void disableAll() {
        foreach (var thing in Things) {
            if (thing.Type == ThingType.Component) {
                (thing.ComponentThing as Behaviour).enabled = false;
            }
            else {
                thing.GameObjectThing.gameObject.SetActive(false);
            }
        }
    }
}
