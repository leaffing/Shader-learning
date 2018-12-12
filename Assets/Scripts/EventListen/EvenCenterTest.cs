using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenCenterTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventCenter.AddListener<string>(EventType.ShowText, ShowText);
        EventCenter.AddListener<string>(EventType.ShowText, s => print("Second envnts"));
	}

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
            EventCenter.Broadcast(EventType.ShowText, "Click left mouse!");
	}

    void OnDestory()
    {
        EventCenter.RemoveListener<string>(EventType.ShowText, ShowText);
        EventCenter.RemoveListener<string>(EventType.ShowText, s => print("Second envnts"));
    }

    private void ShowText(string str)
    {
        Debug.Log(str);
    }

}
