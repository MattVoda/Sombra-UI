using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class June_Rewrite : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject originPrefab;

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

    [Space(10)]
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

    [Header("NO-EDIT: for telling kids their origin")]
    public bool originSetByParent = false;
    public Vector3 originPoint;
    private SteamVR_Camera HMD;


    void Start () {
        HMD = SteamVR_Render.Top();

        kid = new GameObject(); //for use in scaling and transforming

        contents = new GameObject[contentsSize];
        interpolationVectorsArray = new Vector3[contentsSize];

        //create startpoint in worldspace as empty GO. leave unparented
        originGO = Instantiate(originPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        originGO.tag = "Startpoint";
        originPoint = transform.position;
    }
	
	void Update () {
        sphereCenter = transform.position;

        Interpolate();
        DecideSplay();
	}

    void InstantiateContents() {
        // First instantiate all as files
        for (int i = 0; i < contentsSize; i++) {
            GameObject tempKid = Instantiate(filePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, gameObject.transform);
            contents[i] = tempKid;
        }
        // then overwrite with randomly-placed folders
        int[] randFileLocations = new int[numFoldersInContents];
        for (int j = 0; j < numFoldersInContents; j++) {
            int randIndex = Random.Range(1, contentsSize + 1);
            contents[randIndex] = Instantiate(folderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, gameObject.transform);
            //contents[randIndex].GetComponent<June_Rewrite>().originSetByParent = true; //mark this folder as a child
        }
    }

    void ActivateFolderSphereColliders() {
        GameObject[] subfolders;
        subfolders = GameObject.FindGameObjectsWithTag("Folder");
        foreach (GameObject subfolder in subfolders) {
            subfolder.GetComponent<SphereCollider>().enabled = true;
        }
    }

    void DecideSplay() {
        if (distanceVector.magnitude > minimumPullDistance) {
            if (popped) {
                Splay();
            } else { // a first pop!
                popped = true;
                InstantiateContents();
                ActivateFolderSphereColliders();
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
        segmentVector = distanceVector / (contentsSize + 1);

        for (int i = 0; i < contentsSize; i++) {
            interpolationVectorsArray[i].z = segmentVector.z * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            interpolationVectorsArray[i].y = segmentVector.y * (i + 1) * -1 * interpMultiplier;
            interpolationVectorsArray[i].x = segmentVector.x * (i + 1) * -1 * interpMultiplier;
        }
    }

    void Splay() {
        float kidScale = segmentVector.magnitude * fileScaleMultiplier; // *scaleMultiplier probably   //to account for min popping distance
        Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale, 0f, fileMaxSize), Mathf.Clamp(kidScale / 20, 0f, fileThickness));
        Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize), Mathf.Clamp(kidScale * folderScaler, 0f, fileMaxSize));

        for (int i = 0; i < contentsSize; i++) {
            kid = contents[i];
            Vector3 temp = interpolationVectorsArray[i];

            if (kid.tag == "File") {
                kid.transform.localScale = fileScaleVector;
            } else {
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