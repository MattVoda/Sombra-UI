using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class June_Rewrite : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject originPrefab;

    [Header("Interpolation")]
    private GameObject[] contents;
    public int contentsSize = 7;
    public int numFoldersInContents = 1;

    private Vector3[] interpolationVectorsArray;
    public Vector3 originPoint;
    private GameObject originGO;
    private Vector3 sphereCenter;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    private bool popped = false;

    [Header("Child Options")]
    private GameObject kid;
    public float minimumPullDistance = 0.2f; // 2x sphere size
    public float interpMultiplier = 10f;
    public float kidHeightWidth = 1f;
    public float kidThickness = 0.3f;
    public float folderScaler = 0.5f;
    private float newZ, newY, newX;

    [Header("NO-EDIT: for telling kids their origin")]
    public bool originSetByParent = false;

    public bool LookAt = false;
    private SteamVR_Camera HMD;


    // Use this for initialization
    void Start () {
        //set HMD for lookAt() vector  //TODO: default HMD position to some generic Vector3 for tile LookAt()
        HMD = SteamVR_Render.Top();

        kid = new GameObject(); //for use in scaling and transforming

        contents = new GameObject[contentsSize];
        interpolationVectorsArray = new Vector3[contentsSize];

        //create startpoint in worldspace as empty GO. leave unparented
        originGO = Instantiate(originPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        originGO.tag = "Startpoint";
        originPoint = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //update SphereCenter position
        sphereCenter = transform.position;
        Interpolate();
	}

    void InstantiateContents() {
        // First instantiate all as files
        for (int i = 0; i < contentsSize; i++) {
            contents[i] = Instantiate(filePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            contents[i].transform.parent = gameObject.transform; //parent to this GO to keep neat
        }
        // then overwrite with randomly-placed folders
        int[] randFileLocations = new int[numFoldersInContents];
        for (int j = 0; j < numFoldersInContents; j++) {
            int randIndex = Random.Range(1, contentsSize + 1);
            contents[randIndex] = Instantiate(folderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            contents[randIndex].GetComponent<June_Rewrite>().originSetByParent = true; //mark this folder as a child
        }
    }

    public void UnSplay() {
        iTween.ScaleTo(kid, iTween.Hash("z", 0, "y", 0, "x", 0)); //scale down
        iTween.MoveUpdate(kid, iTween.Hash("z", originPoint.z, "y", originPoint.y, "x", originPoint.x, "islocal", true, "time", 0.7, "looktarget", sphereCenter)); //move back to origin
        if(distanceVector.magnitude < 0.15) {
            DestroyContents();
        }
    }
    void DestroyContents() {
        foreach (GameObject go in contents) {
            Destroy(go);
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

        if (distanceVector.magnitude > minimumPullDistance) {
            if (popped) {
                Splay();
            } else { // a first pop!
                InstantiateContents();
                popped = true;
            }
        } else {
            if (popped) {
                UnSplay();
                popped = false;
            }
        }

    }

    void Splay() {
        float kidScale = segmentVector.magnitude - minimumPullDistance; // *scaleMultiplier probably   //to account for min popping distance
        Vector3 fileScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale / 20, 0f, kidThickness));
        Vector3 folderScaleVector = new Vector3(Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth), Mathf.Clamp(kidScale * folderScaler, 0f, kidHeightWidth));

        for (int i = 0; i < contentsSize; i++) {
            kid = contents[i];
            if (kid.tag == "File") {
                kid.transform.localScale = fileScaleVector;
            } else {
                kid.transform.localScale = folderScaleVector;
            }

            if (!LookAt) {
                iTween.MoveUpdate(kid, iTween.Hash("z", interpolationVectorsArray[i].z, "y", interpolationVectorsArray[i].y, "x", interpolationVectorsArray[i].x, "islocal", true, "time", 0.7));
                kid.GetComponent<June_Rewrite>().originPoint = new Vector3(interpolationVectorsArray[i].x, interpolationVectorsArray[i].y, interpolationVectorsArray[i].z);
            } else {
                iTween.MoveUpdate(kid, iTween.Hash("z", interpolationVectorsArray[i].z, "y", interpolationVectorsArray[i].y, "x", interpolationVectorsArray[i].x, "islocal", true, "time", 0.7, "looktarget", sphereCenter));
                //iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", HMD.transform.position));
                kid.GetComponent<June_Rewrite>().originPoint = new Vector3(interpolationVectorsArray[i].x, interpolationVectorsArray[i].y, interpolationVectorsArray[i].z);
            }
        }
    }
}
