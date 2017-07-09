using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class June_Rewrite : MonoBehaviour {

    [Header("Origin Management")]
    public bool root = false;
    private Vector3 originPoint;
    private Dictionary<int, Vector3> folderPositionDictionary;
    public Dictionary<int, GameObject> folderOriginGoDictionary;
    private OriginManager originManagerReference;

    [Header("Interpolation")]
    public int contentsSize = 7;
    public int numFoldersInContents = 1;
    private GameObject[] contents;
    //private int[] indexPositionsOfSplayingSubFolders;
    private Vector3[] interpolationVectorsArray;
    private GameObject originGO;
    private Vector3 sphereCenter;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    private float folderSphereColliderRadius = 0;
    private bool popped = false;

    [Header("File Options")]
    public float minimumPullDistance = 0.1f; // instantiates and destroy at this distance
    public float interpMultiplier = 10f;
    public float fileScaleMultiplier = 8f;
    public float fileMaxSize = 2f;
    public float fileThickness = 0.3f;
    public float folderScaler = 0.5f;
    public float tweeningTime = 0.1f;
    public bool LookAtHMD = false;
    private float newZ, newY, newX;
    private GameObject kid;

    [Header("Prefabs")]
    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject originPrefab;

    private SteamVR_Camera HMD;

    private void Awake() {
        HMD = SteamVR_Render.Top();

        kid = new GameObject(); //for use in scaling and transforming

        contents = new GameObject[contentsSize];
        interpolationVectorsArray = new Vector3[contentsSize];
        folderPositionDictionary = new Dictionary<int, Vector3>();
        folderOriginGoDictionary = new Dictionary<int, GameObject>();
    }

    void Start () {

        //if not the root, reference OriginManager for origin as set by parent
        if (!root) {
            originManagerReference = gameObject.GetComponent<OriginManager>();
            originPoint = originManagerReference.originPoint;
        } else {
            originPoint = transform.position;
        }
        originGO = Instantiate(originPrefab, originPoint, Quaternion.identity);
        //originGO.transform.parent = gameObject.transform;
        originGO.tag = "Startpoint";
    }
	
	void Update () {
        sphereCenter = transform.position;
        if (!root) {
            originPoint = originManagerReference.originPoint;
            //print("my originPoint = " + originPoint);
        }


        Interpolate();
        DecideSplay();
	}

    void InstantiateContents() {
        // First instantiate all as files
        for (int i = 0; i < contentsSize; i++) {
            GameObject tempKid = Instantiate(filePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, gameObject.transform) as GameObject;
            contents[i] = tempKid;
        }
        // then overwrite with randomly-placed folders
        int[] randFileLocations = new int[numFoldersInContents];
        for (int j = 0; j < numFoldersInContents; j++) {
            int randIndex = Random.Range(1, contentsSize);
            Vector3 folderPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            //Transform objectNew = (Transform)Instantiate(folderPrefab, folderPosition, Quaternion.identity, gameObject.transform);

            contents[randIndex] = Instantiate(folderPrefab, folderPosition, Quaternion.identity, gameObject.transform) as GameObject;
            //contents[randIndex].GetComponent<OriginManager>().originPoint = folderPosition;


            //tell child its index position
            contents[randIndex].GetComponent<ReportEntry_2>().myIndexPositionInParent = randIndex;
            //ASSIGN ORIGIN.TRANSFORM TO CHILD
            contents[randIndex].GetComponent<June_Rewrite_Child>().originPoint = folderPosition;

            //add to folder list
            folderPositionDictionary.Add(randIndex, folderPosition);
        }
    }

    public void EntryReportedByChild(int indexPositionInContents) {
        //if it already exists, ignore the report
        if (folderOriginGoDictionary.ContainsKey(indexPositionInContents)) {
            return;
        }
        //if it is a new addition, add it to the tracking list
        else {
            GameObject childOrigin = Instantiate(originPrefab, folderPositionDictionary[indexPositionInContents], Quaternion.identity);
            childOrigin.transform.parent = this.gameObject.transform;
            folderOriginGoDictionary.Add(indexPositionInContents, childOrigin);
            June_Rewrite_Child jrc = contents[indexPositionInContents].GetComponent<June_Rewrite_Child>();
            jrc.originPoint = childOrigin.transform.position;
            jrc.myIndexInContents = indexPositionInContents;
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

    void Interpolate() {
        distanceVector = sphereCenter - originPoint;
        segmentVector = distanceVector / (contentsSize + 2); //+2 to shorten the segments

        for (int i = 0; i < contentsSize; i++) {
            interpolationVectorsArray[i].z = segmentVector.z * (i + 2) * -1 * interpMultiplier; //-1 to reverse direction away from user
            interpolationVectorsArray[i].y = segmentVector.y * (i + 2) * -1 * interpMultiplier; // i + 2 to shift all items one notch away from sphereCenter
            interpolationVectorsArray[i].x = segmentVector.x * (i + 2) * -1 * interpMultiplier;
        }
    }




    void Splay() {

        float kidScale = segmentVector.magnitude * fileScaleMultiplier; // *scaleMultiplier probably   //to account for min popping distance
        Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale / 20, 0f, fileThickness));
        Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize));






        //SPLAY ORIGINS OF POPPED FOLDERS
        List<int> keys = new List<int>(folderOriginGoDictionary.Keys);
        foreach (int key in keys) {

            GameObject kidOriginGO = folderOriginGoDictionary[key];

            Vector3 temp = interpolationVectorsArray[key];
            iTween.MoveUpdate(kidOriginGO, iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", sphereCenter));
            folderOriginGoDictionary[key] = kidOriginGO; //reassign

            print("origin pos = " + kidOriginGO.transform.position);
            //print("origin localPos = " + kidOriginGO.transform.localPosition);

            //update child's origin
            contents[key].GetComponent<June_Rewrite_Child>().originPoint = kidOriginGO.transform.position;
        }

        




        //SPLAY REMAINING CONTENTS
        for (int i = 0; i < contentsSize; i++) {

            //skip splaying kids
            if (folderOriginGoDictionary.Keys.Contains<int>(i)) continue;

            kid = contents[i];
            Vector3 temp = interpolationVectorsArray[i];

            if (kid.tag == "File") {
                kid.transform.localScale = fileScaleVector;
            } else if (kid.tag == "Folder") {
                kid.transform.localScale = folderScaleVector;
                kid.GetComponent<SphereCollider>().radius = folderSphereColliderRadius; // scale collider to prevent rigidbody overlap
                //kid.GetComponent<June_Rewrite>().originPoint = new Vector3(temp.x, temp.y, temp.z); //untested 6/8
            }

            if (!LookAtHMD) {
                iTween.MoveUpdate(kid, iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", sphereCenter));
            } else {
                iTween.MoveUpdate(kid, iTween.Hash("z", temp.z, "y", temp.y, "x", temp.x, "islocal", true, "time", tweeningTime, "looktarget", HMD.transform.position));
            }
        }

        
    }

    //public void UnSplay() {
    //    iTween.ScaleTo(kid, iTween.Hash("z", 0, "y", 0, "x", 0, "time", tweeningTime)); //scale down
    //    iTween.MoveUpdate(kid, iTween.Hash("z", originPoint.z, "y", originPoint.y, "x", originPoint.x, "islocal", true, "time", tweeningTime, "looktarget", sphereCenter)); //move back to origin
    //    if (distanceVector.magnitude < 0.1) {
    //        DestroyContents();
    //    }
    //}

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