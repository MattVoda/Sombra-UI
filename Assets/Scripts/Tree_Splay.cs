using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Splay : MonoBehaviour {

    public GameObject[] contents;
    private int contentsCount;

    public GameObject startpointPrefabToInstantiate;
    private GameObject instantiatedStartpoint;

    //interpolating / MoveZ
    [Header("Interpolate")]
    private Vector3 startPoint;
    private Vector3 updatePoint;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    private GameObject kid;

    [Header("Child Options")]
    public float interpMultiplier = 10f;
    public float scaleMult = 3f;
    public float folderScaler = 0.5f;
    public float toggleRBdistance = 1f;
    public float distanceWhenFullyScaled = 3.5f; //3.5 was prev value
    public float kidHeightWidth = 1f;
    public float kidThickness = 0.3f;

    public bool LookAt = false;
    private SteamVR_Camera HMD;


    void Start() {

        //play in scene view
        //#if UNITY_EDITOR
        //        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        //#endif

        //set HMD for lookAt() vector
        //TODO: default HMD position to some generic Vector3 for tile LookAt()
        HMD = SteamVR_Render.Top();

        kid = new GameObject(); //for use in scaling and transforming

        contentsCount = contents.Length;


        //BooleanStartpointSetting();

        

        //INSTANTIATE CHILDREN AND PARENT THEM TO THIS
        //LOOP THROUGH CONTENTS[]
        //contents = new GameObject[contentsCount];  //kosher?
        for (int i = 0; i < contentsCount; i++) {
            //GameObject go = Instantiate(contents[i], startPoint, Quaternion.identity) as GameObject;
            GameObject go = Instantiate(contents[i], startPoint, Quaternion.identity);
            go.transform.parent = gameObject.transform; //parent new instance
            contents[i] = go;
            //Debug.Log("instantiated one");
        }

        //disable any rigidbodies on child folders
        for (int i = 0; i < contentsCount; i++) {
            GameObject tempKid = contents[i];
            if (tempKid.tag == "Folder") {
                tempKid.GetComponent<Rigidbody>().detectCollisions = false;
                tempKid.GetComponent<Rigidbody>().isKinematic = true;
            }
        }



    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        //set SP values
        //instantiate sp
        //create startpoint in worldspace as empty GO
        //startpointToInstantiate = new GameObject("StartpointInstantiated");
        instantiatedStartpoint = Instantiate(startpointPrefabToInstantiate, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        instantiatedStartpoint.transform.parent = gameObject.transform; //parent it to this
        instantiatedStartpoint.transform.position = gameObject.transform.position;
        instantiatedStartpoint.tag = "Startpoint";

        //SET START POINT
        //SET UPDATE POINT = STARTPOINT -- THIS IS WHAT I'M UPDATING EVERY FRAME
        startPoint = transform.position;
        updatePoint = startPoint;


        //start updating

        //unparent from top folder so it can move
        gameObject.transform.parent = null;
    }

    private void OnCollisionStay(Collision collision) {
        updatePoint = transform.position;
        Splay(updatePoint);
    }

    void OnTriggerExit(Collider other) {
        //stop splaying
        //unsplay
        StartCoroutine("UnSplay");

        //destroy sp instantiation
        Destroy(instantiatedStartpoint);

        //reset sp values
        startPoint = new Vector3(0f, 0f, 0f);
        updatePoint = new Vector3(0f, 0f, 0f);
    }

    IEnumerator UnSplay() {

        for (int i = 0; i < contents.Length; i++) {
            kid = contents[i];
            float mag = kid.transform.localScale.magnitude;

            for (float f = mag; f >= 0; f -= mag/10) {

                //scale down
                //iTween.ScaleTo(kid, iTween.Hash("z", f, "y", f, "x", f));
                iTween.ScaleUpdate(kid, iTween.Hash("z", f, "y", f, "x", f, "islocal", true, "time", 0.7));

                //move back to sp
                iTween.MoveUpdate(kid, iTween.Hash("z", startPoint.z, "y", startPoint.y, "x", startPoint.x, "islocal", true, "time", 0.7, "looktarget", updatePoint));

                
            }

            yield return null; //returns after cycling once through all kids
        }
    }



    void Splay(Vector3 updatePoint) {
        //this doesn't feel right... vector3-vector3?
        distanceVector = updatePoint - startPoint;
        segmentVector = distanceVector / (contentsCount + 1);  //+1?

        //i was using startPoint.z as distance mark -- n00b :)
        float kidScale = Vector3.Distance(updatePoint, startPoint) * scaleMult;
        //this may not apply well to both files and subfolders...
        Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale / 20, 0f, kidThickness));
        Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth));

        //SCALE AND INTERPOLATE CHILDREN
        for (int i = 0; i < contents.Length; i++) {
            kid = contents[i];

            Debug.Log(kid);

            if (kid.tag == "File") {
                kid.transform.localScale = fileScaleVector;
            }
            else {
                kid.transform.localScale = folderScaleVector;
            }

            float newZ = segmentVector.z * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            float newY = segmentVector.y * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            float newX = segmentVector.x * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user

            //omitted the 'emptychildgoarray' code. didn't look useful

            //update child positions
            if (!LookAt) {
                iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7));
            }
            else {
                iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", updatePoint));
                //iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", HMD.transform.position));
            }
        }
    }

    

}
