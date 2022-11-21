using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class scr_PaintbrushDraw : MonoBehaviour
{
    public InputActionProperty finishDrawing;
    [Header("Point Settings")]
    [Tooltip("From where the tracking will spawn")]
    public Transform movementSource;
    [Tooltip("How far apart should the tracking points appear")]
    public float newPositionThresholdDistance = 0.02f;

    [Header("Line Renderer Settings")]
    public float lineWidth = 0.1f;
    public Material defaultLineMaterial;

    [Tooltip("At what certainty to trigger event")]
    public float recognitionThreshold = 0.9f;
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string,List<LineRenderer>> { }
    public UnityStringEvent OnRecognized;

    [Space]
    [Header("Creation mode")]
    public bool creationMode = false;
    public string newGestureName;
    public GameObject debugPrefab;


    //Line Renderer privates
    private LineRenderer currentLineRenderer;
    private List<LineRenderer> lineList = new List<LineRenderer>();
    private int positionCount;

    //Point tracking and gesture privates
    private List<Gesture> trainingSet = new List<Gesture> ();
    private bool isMoving = false;
    private List<Vector3> positionList = new List<Vector3>();
    private List<List<Vector3>> strokeList = new List<List<Vector3>>();


    Vector3 planePosition = Vector3.zero;
    Vector3 planeNormal = Vector3.zero;
    List<Vector3> vectorPoints = new List<Vector3>();

    private void Awake()
    {
        finishDrawing.action.performed += FinishMovement;
    }

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabable = GetComponent<XRGrabInteractable>();
        grabable.activated.AddListener(StartDrawing);
        grabable.deactivated.AddListener(StopDrawing);

        //Gather gestures/strokes that are already generated
        string[] gestureFile = Directory.GetFiles(Application.dataPath + "/Strokes", "*.xml");
        foreach (var item in gestureFile)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }
    private void StartDrawing(ActivateEventArgs arg0)
    {
        isMoving = true;
        //Debug.Log("Hello Im Im entering");
        isMoving = true;
        positionList.Clear();
        positionList.Add(movementSource.position);

        AddNewLineRenderer();
        if (debugPrefab)
            Destroy(Instantiate(debugPrefab, movementSource.position, Quaternion.identity), 3);
    }
    private void StopDrawing(DeactivateEventArgs arg0)
    {
        isMoving = false;
        //Debug.Log("Hello Im leaving");

        isMoving = false;

        /*Debug.Log(" " + String.Join("",
            new List<Vector3>(positionList)
            .ConvertAll(i => i.ToString())
            .ToArray()));*/

        strokeList.Add(new List<Vector3>(positionList));

    }


    // Update is called once per frame
    void Update()
    {

        if(isMoving)
        {
            UpdateDrawing();
        }
    }
    void UpdateDrawing()
    {
        Vector3 lastPosition = positionList[positionList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionList.Add(movementSource.position);
            UpdateLine(movementSource.position);
            if (debugPrefab)
                Destroy(Instantiate(debugPrefab, movementSource.position, Quaternion.identity), 3);
        }
    }

    void FinishMovement(InputAction.CallbackContext context)
    {
        if (strokeList.Count == 0) return;
        if (isMoving) return;


        Point[] pointArray = new Point[GetMultidimensionalCount(strokeList)];


        int count = 0;
        for (int i = 0; i < strokeList.Count; i++)
        {
            for (int j = 0; j < strokeList[i].Count; j++)
            {
                vectorPoints.Add(strokeList[i][j]);
                count++;
            }
        }

        //Calculate Plane from points
        Fit.Plane(vectorPoints, out planePosition, out planeNormal, 100, false);

        //Calculate each points position on the plane
        count = 0;
        for (int i = 0; i < strokeList.Count; i++)
        {
            //Create Gesture From Postion list
            for (int j = 0; j < strokeList[i].Count; j++)
            {
                //Vector2 positionInPlane = Camera.main.WorldToScreenPoint(strokeList[i][j]);
                Vector2 positionInPlane = GetLocalXAndYRelativeToPlane(planePosition, planeNormal, Vector3.up, strokeList[i][j]);
                pointArray[count] = new Point(positionInPlane.x, positionInPlane.y, i);
                count++;
            }
        }

        /*Debug.Log(" " + String.Join("",
            new List<Vector2>(pp)
            .ConvertAll(i => i.ToString())
            .ToArray()));*/

        Gesture newGesture = new Gesture(pointArray);

        
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.dataPath + "/Strokes/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log(result.GestureClass + result.Score);
            if (result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass,lineList);
            }
            else
            {
                foreach (LineRenderer line in lineList)
                {
                    StartCoroutine(LineDisapear(line, 3f));
                }
            }
        }
        vectorPoints.Clear();
        strokeList.Clear();
        lineList.Clear();
        currentLineRenderer = null;
    }

    private IEnumerator LineDisapear(LineRenderer line, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float width = Mathf.Lerp(line.startWidth, 0, time / duration);
            line.startWidth = width;
            line.endWidth = width;
            yield return null;
        }
        Destroy(line.gameObject);
    }

    void AddNewLineRenderer()
    {
        positionCount = 0;

        GameObject go = new GameObject($"LineRenderer");
        go.transform.parent = movementSource;
        go.transform.position = movementSource.position;

        LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = defaultLineMaterial;
        lineRenderer.positionCount = 1;
        lineRenderer.numCapVertices = 5;

        currentLineRenderer = lineRenderer;

        lineList.Add(lineRenderer);
    }
    void UpdateLine(Vector3 position)
    {
        currentLineRenderer.SetPosition(positionCount, position);
        positionCount++;
        currentLineRenderer.positionCount = positionCount + 1;
        currentLineRenderer.SetPosition(positionCount, position);
    }

    int GetMultidimensionalCount(List<List<Vector3>> list)
    {
        int sum = 0;
        for (int x = 0; x < list.Count; x++)
        {
            for (int y = 0; y < list[x].Count; y++)
            {
                if (list[x][y] != Vector3.zero)
                {
                    sum++;
                }
            }
        }
        return sum;
    }

    void OnDrawGizmos()
    {
        if(vectorPoints.Count > 0)
            Fit.Plane(vectorPoints, out planePosition, out planeNormal, 100, true);
    }

    private static Transform referenceObject;
    public static Vector2 GetLocalXAndYRelativeToPlane(Vector3 planeCenter, Vector3 forward, Vector3 up, Vector3 worldPoint)
    {
        if (referenceObject == null) referenceObject = new GameObject("Reference").transform;

        referenceObject.position = planeCenter;
        referenceObject.rotation = Quaternion.LookRotation(forward, up);
        Vector3 rtn = referenceObject.InverseTransformPoint(worldPoint);
        return rtn;
    }
}
