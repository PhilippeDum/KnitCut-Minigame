using UnityEngine;
using UnityEngine.UI;

namespace Recognition
{
    public class MouseGesture : MonoBehaviour
    {
        [SerializeField] private Texture2D texturePattern;
        [SerializeField] private Texture2D textureDrawing;
        [SerializeField] private RawImage imageModel;
        [SerializeField] private Text score;

        public Texture2D TexturePattern => texturePattern;
        public Texture2D TextureDrawing
        {
            get => textureDrawing;
            set => textureDrawing = value;
        }
        public Text Score => score;

        private void Start()
        {
            DisplayPattern();
        }

        public void OnGestureCorrect()
        {
            Debug.Log($"Gesture correct !");
        }

        public void OnGestureWrong()
        {
            Debug.Log($"Gesture wrong !");
        }

        public void DisplayPattern()
        {
            imageModel.texture = texturePattern;
        }
    }
}