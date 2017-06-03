using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tree_1 : MonoBehaviour {

    public GameObject[] contents;
    private List<int> skipContentsIndices;
    private int contentsCount;
    private ReportEntry reportEntryRef;

    private bool distanceHit = false;

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

    public GameObject SplayingFolder;
    private bool splaySwappedIn = false;

    public GameObject tubePrefab;
    public bool toggleTube;
    private GameObject tube;
    private Transform tubeContainer;
    public float ispScaleCompensation = 20f;

    public bool LookAt = false;
    private SteamVR_Camera HMD;


    void Start () {

        //play in scene view
//#if UNITY_EDITOR
//        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
//#endif

        //set HMD for lookAt() vector
        //TODO: default HMD position to some generic Vector3 for tile LookAt()
        HMD = SteamVR_Render.Top();

        kid = new GameObject(); //for use in scaling and transforming

        contentsCount = contents.Length;
        skipContentsIndices = new List<int>(); //set size to contents size

        //create startpoint in worldspace as empty GO. leave unparented
        instantiatedStartpoint = Instantiate(startpointPrefabToInstantiate, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        instantiatedStartpoint.tag = "Startpoint";

        //SET START POINT
        //SET UPDATE POINT = STARTPOINT -- THIS IS WHAT I'M UPDATING EVERY FRAME
        startPoint = transform.position;
        updatePoint = startPoint;


        if(toggleTube) {
            //INSTANTIATE TUBE
            //rotation should face forward with long axis
            tube = Instantiate(tubePrefab, instantiatedStartpoint.transform);
            //parent to ISP
            tube.transform.parent = instantiatedStartpoint.transform;
            tube.transform.localPosition = new Vector3(0f, 0f, 0f);
            //rotate tube 90 around X to point, then update ISP lookat each frame
            tube.transform.localRotation = Quaternion.LookRotation(Vector3.up);

        }



        //INSTANTIATE CHILDREN AND PARENT THEM TO THIS
        //LOOP THROUGH CONTENTS[]
        //contents = new GameObject[contentsCount];  //kosher?
        for (int i = 0; i < contentsCount; i++) {
            //GameObject go = Instantiate(contents[i], startPoint, Quaternion.identity) as GameObject;
            GameObject go = Instantiate(contents[i], startPoint, Quaternion.identity);
            go.transform.parent = gameObject.transform; //parent new instance
            contents[i] = go;
            //Debug.Log("instantiated one");


        //COMMENTED TO STOP ERROR AFTER PORTING NEW STUFF TO TREE_2
            //ReportEntry re = go.AddComponent<ReportEntry>();
            //re.tree = this;
            ////tell kid their index #
            //go.GetComponent<ReportEntry>().myIndex = i;
        }

        //disable any rigidbodies on child folders
        for (int i = 0; i < contentsCount; i++) {
            GameObject tempKid = contents[i];
            if(tempKid.tag == "Folder") {
                tempKid.GetComponent<Rigidbody>().detectCollisions = false;
                tempKid.GetComponent<Rigidbody>().isKinematic = true;
            }
        }



    }
	
	
	void Update () {
        
        
        //UPDATE UPDATE POINT
        updatePoint = transform.position;
        //SPLAY(UPDATEPOINT)
        Splay(updatePoint);
        ToggleChildFolderRBs();

        CheckDistanceFlag(); //used to turn on interactivity with children

        if (toggleTube) { 
            ScaleTube();
        }


        //if ((splaySwappedIn == false) && ( distanceVector.x > 1 || distanceVector.y > 1 || distanceVector.z > 1 )) {
        //    SwapInSplayFolders();
        //} else if (splaySwappedIn && (distanceVector.x < 1 && distanceVector.y < 1 && distanceVector.z < 1)) {
        //    SwapOutSplayFolders();
        //}
        
    }

    /*
    void SwapInSplayFolders() {

        //find children with tag folder
        for (int i = 0; i < contents.Length; i++) {
            kid = contents[i];
            if (kid.tag == "Folder") {

                Debug.Log("swapping in Splaying Folder");

                Transform transferTransform = kid.transform;
                contents[i] = Instantiate(SplayingFolder, transferTransform.position, Quaternion.identity);

            }
        }

            //reassign index position?
            //with tree

            splaySwappedIn = true;
        
    }


    void SwapOutSplayFolders() {


        splaySwappedIn = false;
    }
    */


    void CheckDistanceFlag() {
        if ((!distanceHit) && (distanceVector.x > distanceWhenFullyScaled || distanceVector.y > distanceWhenFullyScaled || distanceVector.z > distanceWhenFullyScaled)) {
            distanceHit = true;
        }
        if ((distanceHit) && (distanceVector.x < distanceWhenFullyScaled && distanceVector.y < distanceWhenFullyScaled && distanceVector.z < distanceWhenFullyScaled)) {
            distanceHit = false;
        }
    }

    void ScaleTube() {
        instantiatedStartpoint.transform.LookAt(updatePoint);
        Vector3 tubeMidpoint = (updatePoint - startPoint) / 2;
        Vector3 tubeMidpointCompensated = new Vector3(tubeMidpoint.x, tubeMidpoint.y, tubeMidpoint.z * ispScaleCompensation);
        tube.transform.localPosition = tubeMidpointCompensated; //maybe local rotation

        Vector3 tubeScale = new Vector3(tube.transform.localScale.x, distanceVector.z * ispScaleCompensation, tube.transform.localScale.z);
        tube.transform.localScale = tubeScale;
    }

    void BooleanStartpointSetting() {
        if(this.tag == "Folder") {
            //don't set SP?
            //wait until trigger enters tube?
        } else if (this.tag == "TopFolder") {

        }
    }


    void ToggleChildFolderRBs() {
        if (Vector3.Distance(updatePoint, startPoint) > toggleRBdistance) {
            //enable any rigidbodies on child folders
            for (int i = 0; i < contentsCount; i++) {
                GameObject tempKid = contents[i];
                if (tempKid.tag == "Folder") {
                    tempKid.GetComponent<Rigidbody>().detectCollisions = true;
                    tempKid.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }

    public void ReportEntry (int index) {
        //add to reported list
        //if list doesn't contain this index
        if (!skipContentsIndices.Contains(index)) {
            //add it to the list
            skipContentsIndices.Add(index);
        }
        //Debug.Log("enter: " + index);

    }

    public void ReportExit (int index) {
        //remove from reported list
        skipContentsIndices.Remove(index);
        //will splay pull it back?
        //Debug.Log("exit: " + index);
    }

    void Splay(Vector3 updatePoint) {
        //this doesn't feel right... vector3-vector3?
        distanceVector = updatePoint - startPoint;
        //Debug.Log("distance = " + distanceVector);

        segmentVector = distanceVector / (contentsCount + 1);  //+1?

        //i was using startPoint.z as distance mark -- n00b :)
        float kidScale = Vector3.Distance(updatePoint, startPoint) * scaleMult;
        //this may not apply well to both files and subfolders...
        //should i cache these? creating them every frame now
        Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale / 20, 0f, kidThickness));
        Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth));

        //SCALE AND INTERPOLATE CHILDREN
        for (int i = 0; i < contents.Length; i++) {
            
            //only update non-skipped items
            if (!skipContentsIndices.Contains(i)) {

                kid = contents[i];

                //Debug.Log(kid);

                if (kid.tag == "File") {
                    kid.transform.localScale = fileScaleVector;
                }
                else {
                    kid.transform.localScale = folderScaleVector;
                }

                float newZ = segmentVector.z * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
                float newY = segmentVector.y * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
                float newX = segmentVector.x * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user

                //update child positions
                if (!LookAt) {
                    iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7));
                } else {
                    iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", updatePoint));
                    //iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", HMD.transform.position));
                }
            }
        }
    }

    public void UnSplay() {

        //scale down
        iTween.ScaleTo(kid, iTween.Hash("z", 0, "y", 0, "x", 0));

        //move back to startpoint
        iTween.MoveUpdate(kid, iTween.Hash("z", startPoint.z, "y", startPoint.y, "x", startPoint.x, "islocal", true, "time", 0.7, "looktarget", updatePoint));

        
    }

}
