using UnityEngine;
using System.Collections;

public class SplayOnArc : MonoBehaviour
{
    public int multiplier = 2;
    public float zeroToOne = 0.5f;
    //popping
    public float popDistance = 2.5f;
    private float popDistancePlusStartPos;
    private bool splayed = false;
    public float splayDistance = 0.1f;
    public int scalingStart = 3;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 distanceVector;
    private Vector3 segmentVector;

    //instantiating kids
    public int childCount = 3;
    public GameObject prefabToInstantiate; //added in the editor
    public GameObject sphereToInstantiate;
    private GameObject[] childGoArray;
    public float kidThickness = 0.1f;
    public float kidHeightWidth = 1f;

    //circle placement
    public float radius = 1f;
    public int angleStep = 15;
    private Vector3[] circlePositions;

    void Start() {
        //play in scene view
        /*
        #if UNITY_EDITOR
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif
        */

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

        //instantiate circle pos array
        circlePositions = new Vector3[childCount];

    }

    

    void Update() {
        //update position of dragged folder
        foreach (Transform child in transform) {
            if (child.name == "Startpoint") {
                startPoint = child.position;
            }
        }

        if (startPoint.z > popDistancePlusStartPos) {
            SplayArc();
        }   else if (startPoint.z < popDistancePlusStartPos) {
            UnsplayArc();
        }

            //if pulled far enough, and not splayed already, splay sideways
            //Splay1(startPoint);

            //(de)compress
            MoveZ(startPoint);
    }


    void SplayArc() {
        //calc circle positions of kids
        for (int c = 0; c < childGoArray.Length; c++) {
            GameObject kid = childGoArray[c];

            Vector3 pos = PositionOnCircle(startPoint, radius, angleStep * (c + 1)); //increment step with childCount

            iTween.MoveUpdate(kid, iTween.Hash("x", pos.x, "z", pos.z, "islocal", true, "time", 1.5, "looktarget", startPoint));
        }
    }
    void UnsplayArc() {
        for (int x = 0; x < childGoArray.Length; x++) {
            GameObject kid = childGoArray[x];
            iTween.MoveUpdate(kid, iTween.Hash("x", 0f, "islocal", true, "time", 1.5, "looktarget", startPoint));
        }
    }

    Vector3 PositionOnCircle(Vector3 center, float radius, int angle) {
        float ang = angle;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        //z is overlapping in later/closer files

        //Debug.Log("angle = " + angle / denom);
        //Debug.Log("pos.z = " + center.z);
        pos.z = center.z + ((angle * multiplier) * zeroToOne);// + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }


    void Splay1(Vector3 startPoint) {
        if (startPoint.z > popDistance) {
            for (int x = 0; x < childGoArray.Length; x++) {
                GameObject kid = childGoArray[x];
                float newX = splayDistance * (x + 1) * -1; //-1 to change direction
                iTween.MoveUpdate(kid, iTween.Hash("x", newX, "islocal", true, "time", 1.5, "looktarget", startPoint));
            }
        }
        else if (startPoint.z < popDistance) {
            for (int x = 0; x < childGoArray.Length; x++) {
                GameObject kid = childGoArray[x];
                iTween.MoveUpdate(kid, iTween.Hash("x", 0f, "islocal", true, "time", 1.5, "looktarget", startPoint));
            }
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