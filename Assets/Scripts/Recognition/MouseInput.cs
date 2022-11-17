using System.Collections.Generic;
//using GestureControl;
using UnityEngine;

namespace Recognition
{
    public class MouseInput : MonoBehaviour
    {
        MouseGesture mouseGesture;

        [Header("Gesture Properties")]
        [Tooltip("The distance of the rayCast to see if a object is Interactive")]
        [SerializeField] private float distanceRayCast = 2.5f;
        [SerializeField] private GestureClass gesture = new GestureClass();
        [SerializeField] private float score;
        [SerializeField] private float correctRate;
        [SerializeField] private int minPoints = 10;

        private RaycastHit hit;

        public GestureClass Gesture => gesture;

        [Header("Line Properties")]
        [SerializeField] private LineRenderer lineRenderer;
        [Range(0.001f, 0.1f)][SerializeField] private float lineWidth = 0.2f;
        [Tooltip("The distance that the lineRenderer is being create from the camera")]
        [SerializeField] private float zline = 0.5f;
        [Tooltip("Material used in the lineRenderer")]
        [SerializeField] private Material lineMaterial;
        [Tooltip("The width of the line that is created when you do the gesture, We recommend one unit more than the brush used in the creation of the texture")]
        [SerializeField] private int widthTextLine = 3;

        private int index;

        private void Start()
        {
            mouseGesture = FindObjectOfType<MouseGesture>();

            SetupLine();
            lineRenderer.positionCount = 0;

            gesture.SetTextWidht(widthTextLine);

            //mouseGesture.DisplayPattern();
        }

        private void SetupLine()
        {
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = lineWidth;

            if (lineMaterial == null)
            {
                Debug.LogError("<b>Mouse Gesture Interpretation:</b> Line Material need a material to display colors in the drawning line");
            }
        }

        private void Update()
        {
            SetupLine();

            DetectGesture();
        }

        private void FixedUpdate()
        {
            HandleArea();
        }

        #region Gesture

        private void DetectGesture()
        {
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            index = 0;
            gesture.SetIsGesturing(b: false);
        }

        public void CompareGestureToModel()
        {
            if (gesture.mouseData.Count <= 0) return;

            index = 0;
            gesture.SetIsGesturing(b: false);

            /// Probleme conversion en 32x32 (compression) -> CHANGER
            Texture2D texture2D = gesture.MapPattern();

            if ((bool)texture2D)
            {
                mouseGesture.TextureDrawing = texture2D;

                /// Essayer de vérifier 1 fois + vérifier avec pixels blancs et noirs inversés -> TOLERANCE
                //score = gesture.TestPattern(texture2D, mouseGesture.TexturePattern);
                score = gesture.CompareDrawingWithPattern(texture2D, mouseGesture.TexturePattern);

                if (score >= correctRate)
                {
                    mouseGesture.OnGestureCorrect();
                }
                else
                {
                    mouseGesture.OnGestureWrong();
                }

                mouseGesture.Score.text = $"{score * 100:0.00}%";
            }

            gesture.mouseData = new List<Vector3>();
        }

        #endregion

        #region Line Drawing

        private void HandleArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out hit, distanceRayCast))
            {
                return;
            }

            HandleInteractiveArea();
        }

        private void HandleInteractiveArea()
        {
            if (hit.collider.gameObject.tag == "Interactive")
            {
                DrawLine();
            }
        }

        private void DrawLine()
        {
            if (Input.GetMouseButton(0) && !gesture.mouseData.Contains(Input.mousePosition))
            {
                mouseGesture.Score.text = "Score ?";

                gesture.SetIsGesturing(b: true);
                gesture.mouseData.Add(Input.mousePosition);

                lineRenderer.positionCount = index + 1;

                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zline);

                lineRenderer.SetPosition(index, Camera.main.ScreenToWorldPoint(position));
                index++;
            }
        }

        #endregion
    }
}