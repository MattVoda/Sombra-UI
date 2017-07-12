using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class July_Container : MonoBehaviour {


    [Header("Interpolation")]
    private GameObject[] contents;
    public int contentsSize = 7;
    public int numFoldersInContents = 1;
    public Dictionary<int, GameObject> folderOriginGoDictionary;
    private Dictionary<int, Vector3> folderPositionDictionary;

    private Vector3 distanceVector;
    private Vector3 segmentVector;
    private Vector3[] interpolationVectorsArray;

    private bool popped = false;
    private GameObject originGO;


    [Header("File Options")]
    private Vector3 startPosition;
    public float minimumPullDistance = 0.5f; // instantiates and destroy at this distance
    public float interpMultiplier = 1f;
    //public float fileMaxSize = 2f;
    //public float fileThickness = 0.3f;
    //public float folderScaler = 0.5f;
    public float tweeningTime = 0.1f;
    public bool LookAtHMD = false;
    private float newZ, newY, newX;
    //private GameObject kid;
    private GameObject RedBall;

    [Header("Prefabs")]
    public GameObject RedBallPrefab;
    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject originPrefab;

    private SteamVR_Camera HMD;


    private void Awake() {
        HMD = SteamVR_Render.Top();
        //kid = new GameObject(); //for use in scaling and transforming
        contents = new GameObject[contentsSize];
        interpolationVectorsArray = new Vector3[contentsSize];
        //folderPositionDictionary = new Dictionary<int, Vector3>();
        folderOriginGoDictionary = new Dictionary<int, GameObject>();
        folderPositionDictionary = new Dictionary<int, Vector3>();
        startPosition = transform.position;
        print("startPos: " + startPosition);
    }

    void Start () {
        RedBall = Instantiate(RedBallPrefab, startPosition, Quaternion.identity, gameObject.transform);
        originGO = Instantiate(originPrefab, startPosition, Quaternion.identity, gameObject.transform);
    }
	
	void Update () {
        Interpolate();
        DecideSplay();
    }

    void Interpolate() {
        distanceVector = RedBall.transform.position - startPosition;
        segmentVector = distanceVector / (contentsSize + 2); //+2 to shorten the segments

        for (int i = 0; i < contentsSize; i++) {
            interpolationVectorsArray[i].z = segmentVector.z * (i + 1) * interpMultiplier;
            interpolationVectorsArray[i].y = segmentVector.y * (i + 1) * interpMultiplier;
            interpolationVectorsArray[i].x = segmentVector.x * (i + 1) * interpMultiplier;
            //interpolationVectorsArray[i] += startPosition; //to move transformations to RedBall start, as opposed to 0,0,0 container pos
        }
    }

    void DecideSplay() {
        if (distanceVector.magnitude > minimumPullDistance) {
            if (popped) {
                Splay();
            } else { // a first pop!
                popped = true;
                InstantiateContents();
            }
        } else {
            if (popped) {
                popped = false;
                DestroyContents();
            }
        }
    }

    public void EntryReportedByChild(int indexPositionInContents) {
        if (folderOriginGoDictionary.ContainsKey(indexPositionInContents)) {
            return; //if it already exists, ignore the report
        } else { //if it is a new addition, add it to the tracking list
            GameObject childOrigin = Instantiate(originPrefab, folderPositionDictionary[indexPositionInContents], Quaternion.identity);
            childOrigin.transform.parent = this.gameObject.transform;
            folderOriginGoDictionary.Add(indexPositionInContents, childOrigin);
            //June_Rewrite_Child jrc = contents[indexPositionInContents].GetComponent<June_Rewrite_Child>();
            //jrc.originPoint = childOrigin.transform.position;
            //jrc.myIndexInContents = indexPositionInContents;
        }
    }


    void InstantiateContents() {
        Vector3 tempVec = RedBall.transform.position;
        // First instantiate all as files
        for (int i = 0; i < contentsSize; i++) {
            contents[i] = Instantiate(filePrefab, tempVec, Quaternion.identity, gameObject.transform) as GameObject;
        }
        // then overwrite with randomly-placed folders
        int[] randFileLocations = new int[numFoldersInContents];
        for (int j = 0; j < numFoldersInContents; j++) {
            int randIndex = Random.Range(1, contentsSize);
            Destroy(contents[randIndex]); //make space for folder
            contents[randIndex] = Instantiate(folderPrefab, tempVec, Quaternion.identity, gameObject.transform) as GameObject;
            //contents[randIndex].transform.parent = this.gameObject.transform;
            //contents[randIndex].GetComponent<ReportEntry_2>().myIndexPositionInParent = randIndex; //tell child its index position
            //folderPositionDictionary.Add(randIndex, tempVec); //add to folder list
        }
    }


    void Splay() {

        //float kidScale = segmentVector.magnitude * fileScaleMultiplier; // *scaleMultiplier probably   //to account for min popping distance
        //Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale / 20, 0f, fileThickness));
        //Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize));


        //SPLAY ORIGINS OF POPPED FOLDERS
        //List<int> keys = new List<int>(folderOriginGoDictionary.Keys);
        //foreach (int key in keys) {

        //    GameObject kidOriginGO = folderOriginGoDictionary[key];

        //    Vector3 temp = interpolationVectorsArray[key];
        //    iTween.MoveUpdate(kidOriginGO, iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", RedBall.transform.position));
        //    folderOriginGoDictionary[key] = kidOriginGO; //reassign

        //    //print("origin pos = " + kidOriginGO.transform.position);
        //    //print("origin localPos = " + kidOriginGO.transform.localPosition);

        //    //update child's origin
        //    //contents[key].GetComponent<June_Rewrite_Child>().originPoint = kidOriginGO.transform.position;
        //}


        //SPLAY REMAINING CONTENTS
        for (int i = 0; i < contentsSize; i++) {

            //skip splaying kids
            //if (folderOriginGoDictionary.Keys.Contains<int>(i)) continue;

            //kid = contents[i];
            Vector3 temp = interpolationVectorsArray[i];

            //if (kid.tag == "File") {
            //    kid.transform.localScale = fileScaleVector;
            //} else if (kid.tag == "Folder") {
            //    kid.transform.localScale = folderScaleVector;
            //    //kid.GetComponent<SphereCollider>().radius = folderSphereColliderRadius; // scale collider to prevent rigidbody overlap
            //    //kid.GetComponent<June_Rewrite>().originPoint = new Vector3(temp.x, temp.y, temp.z); //untested 6/8
            //}

            if (!LookAtHMD) {
                iTween.MoveUpdate(contents[i], iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", RedBall.transform.position));
            } else {
                iTween.MoveUpdate(contents[i], iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", HMD.transform.position));
            }
        }


    }










    










    void DestroyContents() {
        foreach (GameObject go in contents) {
            GameObject.Destroy(go);
        }
        int childs = transform.childCount;
        for (int i = childs - 1; i > 0; i--) {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }


}
