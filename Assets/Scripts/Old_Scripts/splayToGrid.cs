using UnityEngine;
using System.Collections;

public class splayToGrid : MonoBehaviour
{
    //grid org
    public int childCount = 8;
    public int numCols = 3;
    public int numRows = 3;
    public float spacingAmt = 1f;
    public float startPointOffset = 0.1f;

    private Vector3 [] gridPositions;

    //scale, interpolate, and pop
    public float popDistance = 0.5f;
    public int scalingStart = 4;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    private float popDistancePlusStartPos;
    private bool splayed = false;

    //instantiating kids
    public GameObject prefabToInstantiate; //added in the editor
    private GameObject[] childGoArray;
    public float kidThickness = 0.015f;
    public float kidHeightWidth = 0.2f;

    

    // Use this for initialization
    void Start() {

        //declare size of gridPositions
        gridPositions = new Vector3 [childCount];

        //play in scene view
        #if UNITY_EDITOR
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif

        //assign start and end positions
        foreach (Transform child in transform) {
            if (child.name == "Startpoint") {
                startPoint = child.position;
                popDistancePlusStartPos = popDistance + startPoint.z;
            }
            else if (child.name == "Endpoint") {
                endPoint = child.position;
            }
        }

        //instantiate kids
        childGoArray = new GameObject[childCount];
        for (int i = 0; i < childCount; i++) {
            GameObject go = Instantiate(prefabToInstantiate, endPoint, Quaternion.identity) as GameObject;
            go.transform.parent = gameObject.transform; //parent new instance
            childGoArray[i] = go;
        }

        MakeGridPoints();
    }

    // Update is called once per frame
    void Update() {
        //update position of dragged folder
        foreach (Transform child in transform) {
            if (child.name == "Startpoint") {
                startPoint = child.position;
            }
        }

        if (startPoint.z > popDistancePlusStartPos) {
            SplayGrid(startPoint);
        }
        else if (startPoint.z < popDistancePlusStartPos) {
            //UnsplayArc();
        }

        //if pulled far enough, and not splayed already, splay sideways
        //Splay1(startPoint);

        //(de)compress
        MoveZ(startPoint);
    }

    void SplayGrid(Vector3 startPoint) {

        //not adding Z offset to startpoint -- REVISE maybe
        Vector3 startPointPlusOffset = new Vector3(startPoint.x + startPointOffset, startPoint.y + startPointOffset, startPoint.z);

        for (int y = 0; y < numRows; y++) {
            for (int x = 0; x < numCols; x++) {
                int arrayIndex = y + x; //cumulative iterator

                //add row or col * spacing
                Vector3 newGridPos = new Vector3(startPointPlusOffset.x + (x*spacingAmt), startPointPlusOffset.y + (y * spacingAmt), startPointPlusOffset.z);
                childGoArray[arrayIndex].transform.position = newGridPos;
            }
        }
    }

    void MakeGridPoints() {

        
    }
    /*
    void SplayArc() {
        //calc circle positions of kids
        for (int c = 0; c < childGoArray.Length; c++) {
            GameObject kid = childGoArray[c];
            Vector3 pos = PositionOnCircle(startPoint, radius, angleStep * (c + 1)); //increment step with childCount
            iTween.MoveUpdate(kid, iTween.Hash("x", pos.x, "z", pos.z, "islocal", true, "time", 1.5, "looktarget", startPoint));
        }
    }
    */
    void UnsplayArc() {
        for (int x = 0; x < childGoArray.Length; x++) {
            GameObject kid = childGoArray[x];
            iTween.MoveUpdate(kid, iTween.Hash("x", 0f, "islocal", true, "time", 1.5, "looktarget", startPoint));
        }
    }


    void MoveZ(Vector3 startPoint) {
        distanceVector = startPoint - endPoint;
        segmentVector = distanceVector / (childCount + 1);

        //scale = 1 at popDistancePlusStartPos, and doesn't grow bigger (unless hover effected)
        float kidScale = startPoint.z - (popDistancePlusStartPos / scalingStart);

        Vector3 kidScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale / 20, 0f, kidThickness));

        for (int i = 0; i < childGoArray.Length; i++) {
            GameObject kid = childGoArray[i];
            kid.transform.localScale = kidScaleVector; //scale kid
            float newZ = segmentVector.z * (i + 1);
            iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "islocal", true, "time", 0.7));
        }
    }
}

