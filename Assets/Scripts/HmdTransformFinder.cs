using UnityEngine;
using System.Collections;

public class HmdTransformFinder : MonoBehaviour {

    private SteamVR_Camera HMD;
    //public GameObject HmdPosition;

    // Use this for initialization
    void Start () {

        HMD = SteamVR_Render.Top();

        //GameObject HMDposition = new GameObject();

    }
	
	// Update is called once per frame
	void Update () {

        transform.position = HMD.transform.position;

	}
}
