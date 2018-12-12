using MyPureMVC;
using UnityEngine;

public class PureMVCTest : MonoBehaviour {

	void Awake()
    {
        MVCFacad facade = new MVCFacad();
        facade.StartUp();
    }
}
