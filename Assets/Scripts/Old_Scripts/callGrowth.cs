using UnityEngine;
using System.Collections;

public class callGrowth : MonoBehaviour {

    public GameObject growthTileToInstantiate;
    private GameObject growthTile;
    private growth growthScript;

	// Use this for initialization
	void Start () {

        growthTile = GameObject.Instantiate(growthTileToInstantiate, transform.position, transform.rotation, gameObject.transform) as GameObject;
        //growthTile.transform.parent = gameObject.transform; //parent growthtile to this GO
        growthScript = growthTile.GetComponent<growth>(); //cache growth script
        growthScript.originalScaleVector = gameObject.transform.localScale; //tell script this obj's scale 
        growthScript.growNow = true; //tell script to grow
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
