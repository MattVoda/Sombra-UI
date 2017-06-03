using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class June_Rewrite : MonoBehaviour {


    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject originPrefab;

    public int contentSize;
    public int numFolders;



    private Vector3 originPoint;
    private Vector3 sphereCenter;
    private Vector3 distanceVector;
    private Vector3 segmentVector;


    public bool LookAt = false;
    private SteamVR_Camera HMD;


    // Use this for initialization
    void Start () {

        //set HMD for lookAt() vector
        //TODO: default HMD position to some generic Vector3 for tile LookAt()
        HMD = SteamVR_Render.Top();

        // SCALE BETTER

        /// Build Contents array programatically.
        /// Prefabs: file, folder
        /// Place folders randomly, with N folders        
        /// each new folder opened will have a random order :)



        /// objects in objects... rb's in rb's

        /// only start tracking when sphere is >radius distance from its origin
        /// interpolation line should shift things towards origin, to avoid overlap with sphere
        /// interpolation only creates points when a minimum distance is satisfied
        /// instantiate files/folders along this line as points pop into existence




    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
