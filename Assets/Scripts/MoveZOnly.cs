using UnityEngine;
using System.Collections;
using Valve.VR;

public class MoveZOnly : MonoBehaviour
{
    public bool LookAt = false;
    //interpolating / MoveZ
    [Header("Interpolate")]
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 distanceVector;
    private Vector3 segmentVector;
    public float interpMultiplier = 10f;
    public float scaleMult = 3;
    private SteamVR_Camera HMD;

    //popping
    [Header("Pop")]
    public float popDistance = 0.5f;
    private float popDistancePlusStartPos;
    public int scalingStart = 3;

    //instantiating kids
    [Header("Kid Instantiation")]
    public int childCount = 5;
    public GameObject prefabToInstantiate; //added in the editor
    private GameObject endpointToInstantiate; //instantiated in Start()
    private GameObject[] childGoArray;
    private GameObject endPointGeo;
    private GameObject kid;
    public float kidThickness = 0.1f;
    public float kidHeightWidth = 1.6f;


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

        //instantiate kids
        childGoArray = new GameObject[childCount];
        for (int i = 0; i < childCount; i++) {
            GameObject go = Instantiate(prefabToInstantiate, endPoint, Quaternion.identity) as GameObject;
            go.transform.parent = gameObject.transform; //parent new instance
            childGoArray[i] = go;
            Debug.Log("instantiated one");
        }
        //declare kid for scaling loop
        kid = new GameObject();

    }



    void Update() {
        //update position of dragged folder
        startPoint = transform.position;
        /*
        if (startPoint.z > popDistancePlusStartPos) {
            //SplayArc();
        }
        else if (startPoint.z < popDistancePlusStartPos) {
            //UnsplayArc();
        }
        */
        MoveZ(startPoint);
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

            if (!LookAt) {
                iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7));
            }
            else {
                iTween.MoveUpdate(kid, iTween.Hash("z", newZ, "y", newY, "x", newX, "islocal", true, "time", 0.7, "looktarget", HMD.transform.position));
            }
        }
    }
}
