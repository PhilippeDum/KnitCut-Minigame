using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System;
using System.IO;
using UnityEngine.UI;

namespace Minigame_Drawing_Recognier
{
    public class Recognizer : MonoBehaviour
    {
        [Header("Draw Parameters")]
        [SerializeField] private Transform gestureOnScreenPrefab;
        [SerializeField] private float scoreMin = 0.7f;

        private List<Gesture> trainingSet = new List<Gesture>();
        private List<Point> points = new List<Point>();
        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();

        private LineRenderer currentGestureLineRenderer;

        private Vector3 virtualKeyPosition = Vector2.zero;

        private int strokeId = -1;
        private int vertexCount = 0;

        [Header("UI")]
        [SerializeField] private Transform drawingArea;
        [SerializeField] private InputField result;
        [SerializeField] private InputField newModelName;

        [Header("Model(s)")]
        [SerializeField] private List<Sprite> modelsSprite;
        [SerializeField] private Image modelSurface;

        private string message;
        private string newGestureName = "";
        private bool recognized;

        void Start()
        {
            LoadGestures();

            LoadModel();
        }

        private void LoadGestures()
        {
            //Load pre-made gestures
            /*TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
            foreach (TextAsset gestureXml in gesturesXml)
                trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));*/

            //Load user custom gestures
            string[] filePaths = Directory.GetFiles(Application.dataPath + "/Resources/Recognizer/", "*.xml");

            // Choose one random model from list
            int randomFilePathIndex = UnityEngine.Random.Range(0, filePaths.Length);
            string randomFilePath = filePaths[randomFilePathIndex];
            Debug.Log($"Random model : {GestureIO.ReadGestureFromFile(randomFilePath).Name}");

            trainingSet.Add(GestureIO.ReadGestureFromFile(randomFilePath));
        }

        void Update()
        {
            DrawUI();
        }

        private void DrawUI()
        {
            // Get position of mouse button click
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }

            // If cursor is in drawing area
            if (DrawOverUI.InDrawingArea)
            {
                // If mouse button 0 is hold down
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

                // Stop drawing
                if (Input.GetMouseButton(0) && currentGestureLineRenderer != null)
                {
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
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

        #region Handle Options

        public void Recognize()
        {
            recognized = true;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            if (gestureResult.Score >= scoreMin)
            {
                message = $"{gestureResult.GestureClass} : {(gestureResult.Score * 100).ToString("0.00")}%";
            }
            else
            {
                message = "Retry !";
            }

            result.text = message;
        }

        public void AddModel()
        {
            if (newModelName.text == "") return;

            newGestureName = newModelName.text;

            CreateShapeModelFile(points);
        }

        private void LoadModel()
        {
            for (int i = 0; i < modelsSprite.Count; i++)
            {
                if (modelsSprite[i].name == trainingSet[0].Name)
                {
                    modelSurface.sprite =modelsSprite[i];
                }
            }
        }

        #endregion
    }
}