using UnityEngine;
using System.Collections;
using Valve.VR;

public class SplayRefac2 : MonoBehaviour
{

    //interpolating / MoveZ
    [Header("Interpolate")] 
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    public float interpMultiplier = 1.5f;
    public float scaleMult = 1;
    private SteamVR_Camera HMD;

    //popping
    [Header("Pop")]
    public float popDistance = 2.5f;
    private float popDistancePlusStartPos;
    private bool splayed = false;
    public float splayDistance = 0.1f;
    public int scalingStart = 3;

    //instantiating kids
    [Header("Kid Instantiation")]
    public int childCount = 3;
    public GameObject prefabToInstantiate; //added in the editor
    private GameObject endpointToInstantiate; //instantiated in Start()
    private GameObject[] childGoArray;
    private GameObject endPointGeo;
    private GameObject kid;
    public float kidThickness = 0.1f;
    public float kidHeightWidth = 1f;
    private bool kidsInstantiated = false;
    public float instantiateDistance = 1f;

    //circle placement
    [Header("Arc")]
    public float radius = 1f;
    public int angleStep = 15;
    public int multiplier = 10;
    public float zeroToOne = 0.5f;
    private Vector3[] circlePositions;

    void Start() {
        //play in scene view
        /*
        #if UNITY_EDITOR
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        #endif
        */

        //set HMD for lookAt() vector
        //TODO: default HMD position to some generic Vector3 for tile LookAt()
        HMD = SteamVR_Render.Top();

        //create endpoint in worldspace as empty GO
        endpointToInstantiate = new GameObject("EndpointInstantiated");
        endpointToInstantiate.transform.position = gameObject.transform.position;

        //assign start and end positions
        //maybe use localPosition here? for endpoint?
        startPoint = transform.position;
        endPoint = endpointToInstantiate.transform.position;

        popDistancePlusStartPos = popDistance + startPoint.z;

        childGoArray = new GameObject[childCount];

        //instantiate kids -- moved to separate func
        /*
        childGoArray = new GameObject[childCount];
        for (int i = 0; i < childCount; i++) {
            GameObject go = Instantiate(prefabToInstantiate, endPoint, Quaternion.identity) as GameObject;
            go.transform.parent = gameObject.transform; //parent new instance
            childGoArray[i] = go;
            //Debug.Log("instantiated one");
        }
        */

        //declare kid for scaling loop
        kid = new GameObject();
        

        //instantiate circle pos array
        circlePositions = new Vector3[childCount];

    }



    void Update() {
        //update position of dragged folder
        startPoint = transform.position;
        

        if (kidsInstantiated == false && startPoint.z > instantiateDistance) {
            InstantiateKidsAtDistance();
        }

        if (startPoint.z > popDistancePlusStartPos) {
            //SplayArc();
        }
        else if (startPoint.z < popDistancePlusStartPos) {
            //UnsplayArc();
        }

        MoveZ(startPoint);
    }

    void InstantiateKidsAtDistance() {
        
        for (int i = 0; i < childCount; i++) {
            GameObject go = Instantiate(prefabToInstantiate, endPoint, Quaternion.identity) as GameObject;
            go.transform.parent = gameObject.transform; //parent new instance
            childGoArray[i] = go;
            Debug.Log("instantiated one");
        }
        kidsInstantiated = true;
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
        //float kidScale = startPoint.z - (popDistancePlusStartPos / scalingStart);
        float kidScale = startPoint.z * scaleMult;
        Vector3 kidScaleVector = new Vector3(Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale, 0f, kidHeightWidth), Mathf.Clamp(kidScale / 20, 0f, kidThickness));


        //scale and interpolate each kid
        for (int i = 0; i < childGoArray.Length; i++) {
            kid = childGoArray[i];
            kid.transform.localScale = kidScaleVector; //scale kid
            float newZ = segmentVector.z * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            float newY = segmentVector.y * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            float newX = segmentVector.x * (i + 1) * -1 * interpMultiplier; //-1 to reverse direction away from user
            iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", HMD.transform.position));
        }
    }
}
