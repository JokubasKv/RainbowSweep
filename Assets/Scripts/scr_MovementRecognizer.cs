using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;
using System;

public class scr_MovementRecognizer : MonoBehaviour
{
    [Header("Control references")]
    public XRNode inputSource;
    public InputHelpers.Button drawButton;
    public InputHelpers.Button finishButton;
    public float inputThreshold = 0.1f;
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
    public class UnityStringEvent : UnityEvent<string> { }
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



    // Start is called before the first frame update
    void Start()
    {
        //Gather gestures/strokes that are already generated
        string[] gestureFile = Directory.GetFiles(Application.dataPath + "/Strokes", "*.xml");
        foreach (var item in gestureFile)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Track Buttons
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), drawButton, out bool drawIsPressed, inputThreshold);
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), finishButton, out bool finishIsPressed, inputThreshold);

        //Check The Lines
        if(!isMoving && finishIsPressed && strokeList.Count != 0)
        {
            FinishMovement();
        }
        //Start The movement
        else if (!isMoving && drawIsPressed)
        {
            StartMovement();
        }
        //Ending The movement
        else if(isMoving && !drawIsPressed)
        {
            EndMovement();
        }
        //Updating movement
        else if(isMoving && drawIsPressed)
        {
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionList.Clear();
        positionList.Add(movementSource.position);

        AddNewLineRenderer();
        if (debugPrefab)
            Destroy(Instantiate(debugPrefab, movementSource.position, Quaternion.identity),3);
    }
    void EndMovement()
    {
        isMoving = false;

        /*Debug.Log(" " + String.Join("",
            new List<Vector3>(positionList)
            .ConvertAll(i => i.ToString())
            .ToArray()));*/

        strokeList.Add(new List<Vector3>(positionList));

    }
    void UpdateMovement()
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

    void FinishMovement()
    {

        Point[] pointArray = new Point[GetMultidimensionalCount(strokeList)];

        int count = 0;
        for (int i = 0; i < strokeList.Count; i++)
        {
            //Create Gesture From Postion list
            for (int j = 0; j < strokeList[i].Count; j++)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(strokeList[i][j]);
                pointArray[count] = new Point(screenPoint.x, screenPoint.y, i);
                count++;
            }
        }


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
                OnRecognized.Invoke(result.GestureClass);
            }
        }

        strokeList.Clear();
        foreach(LineRenderer line in lineList)
        {
            Destroy(line.gameObject);
        }
        lineList.Clear();
        currentLineRenderer = null;
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
}
