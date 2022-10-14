using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System;
using System.IO;
using UnityEditor.U2D;

public class Recognizer : MonoBehaviour
{
    public Transform gestureOnScreenPrefab;

    [SerializeField] private List<Gesture> trainingSet = new List<Gesture>();

    [SerializeField] private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private Rect drawingArea;

    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    [SerializeField] private Sprite sprite;

    [SerializeField] private float scoreMin = 0.7f;

    [SerializeField] private LineRenderer shapeDrawingLineRenderer;

    //GUI
    private string message;
    private bool recognized;
    private string newGestureName = "";

    void Start()
    {
        drawingArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);
        LoadGestures();
    }

    private void LoadGestures()
    {
        //Load pre-made gestures
        /*TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));*/

        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Resources/Recognizer/", "*.xml");
        foreach (string filePath in filePaths)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
            Debug.Log($"{GestureIO.ReadGestureFromFile(filePath).Name} loaded");
        }
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Input.GetMouseButton(0))
        {
            virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (drawingArea.Contains(virtualKeyPosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (recognized)
                {
                    recognized = false;
                    strokeId = -1;

                    points.Clear();

                    foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                    {
                        lineRenderer.positionCount = 0;
                        Destroy(lineRenderer.gameObject);
                    }

                    gestureLinesRenderer.Clear();
                }

                ++strokeId;
                Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                gestureLinesRenderer.Add(currentGestureLineRenderer);

                vertexCount = 0;
            }

            if (Input.GetMouseButton(0))
            {
                points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                currentGestureLineRenderer.positionCount = ++vertexCount;
                currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
            }
        }
    }

    void OnGUI()
    {
        GUI.Box(drawingArea, "Draw Area");

        GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

        // Recognize
        if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
        {
            recognized = true;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            if (gestureResult.Score >= scoreMin)
            {
                message = gestureResult.GestureClass + " " + gestureResult.Score;
            }
            else
            {
                message = "Retry !";
            }
        }

        // Add new as
        GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
        newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

        if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
        {
            CreateShapeModelFile(points);
        }

        // Add sprite
        if (GUI.Button(new Rect(Screen.width - 150, 90, 150, 30), "Add sprite model"))
        {
            if (recognized) strokeId = -1;

            ++strokeId;

            List<Point> spritePoints = new List<Point>();

            // PROBLEM HERE WITH INFORMATIONS SAVED (UV)

            for (int i = 0; i < sprite.uv.Length; i++)
            {
                Debug.Log(sprite.uv[i]);
                newGestureName = sprite.name;

                spritePoints.Add(new Point(sprite.uv[i].x, -sprite.uv[i].y, strokeId));
            }

            CreateShapeModelFile(spritePoints);
        }

        // Charge Model
        if (GUI.Button(new Rect(Screen.width - 150, 200, 150, 30), "Charge model"))
        {
            for (int i = 0; i < trainingSet.Count; i++)
            {
                if (trainingSet[i].Name == sprite.name)
                {
                    Debug.Log($"set : {trainingSet[i].Name}");
                    shapeDrawingLineRenderer.positionCount = trainingSet[i].Points.Length;
                    for (int p = 0; p < trainingSet[i].Points.Length; p++)
                    {
                        Vector3 position = new Vector3(trainingSet[i].Points[p].X, trainingSet[i].Points[p].Y);
                        Debug.Log($"-> position '{p}' : {position}");
                        shapeDrawingLineRenderer.SetPosition(p, position);
                    }
                    return;
                }
            }
        }
    }

    private void CreateShapeModelFile(List<Point> pointsList)
    {
        string fileName = String.Format("{0}/{1}-{2}.xml", Application.dataPath + "/Resources/Recognizer/", newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
        GestureIO.WriteGesture(pointsList.ToArray(), newGestureName, fileName);
#endif

        trainingSet.Add(new Gesture(pointsList.ToArray(), newGestureName));
        
        Debug.Log($"New model created : {newGestureName}");

        newGestureName = "";
    }
}