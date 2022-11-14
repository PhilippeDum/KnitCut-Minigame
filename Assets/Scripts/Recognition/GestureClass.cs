using System.Collections.Generic;
using UnityEngine;

public class GestureClass
{
    public List<Vector3> mouseData = new List<Vector3>();

    private bool gesturing;

    public int widthText = 3;

    public Texture2D MapPattern()
    {
        if (mouseData.Count < 10)
        {
            return null;
        }

        Bounds bounds = new Bounds(mouseData[0], Vector3.zero);
        for (int i = 1; i < mouseData.Count; i++)
        {
            bounds.min = Vector3.Min(bounds.min, mouseData[i]);
            bounds.max = Vector3.Max(bounds.max, mouseData[i]);
        }

        Texture2D texture2D = new Texture2D(32, 32);
        Color[] pixels = texture2D.GetPixels();
        for (int j = 0; j < pixels.Length; j++)
        {
            ref Color reference = ref pixels[j];
            reference = Color.white;
        }

        if (bounds.size.magnitude < 20f)
        {
            return null;
        }

        for (int k = 0; k < mouseData.Count - 1; k++)
        {
            int num = (int)Mathf.Clamp((mouseData[k].x - bounds.min.x) / bounds.size.x * 32f, 0f, 31f);
            int num2 = (int)Mathf.Clamp((mouseData[k].y - bounds.min.y) / bounds.size.y * 32f, 0f, 31f);
            int num3 = (int)Mathf.Clamp((mouseData[k + 1].x - bounds.min.x) / bounds.size.x * 32f, 0f, 31f);
            int num4 = (int)Mathf.Clamp((mouseData[k + 1].y - bounds.min.y) / bounds.size.y * 32f, 0f, 31f);
            float num5 = Mathf.Sqrt(Mathf.Pow(num3 - num, 2f) + Mathf.Pow(num4 - num2, 2f));
            for (int l = 0; l <= 20; l++)
            {
                float num6 = (float)l * 0.05f;
                int num7 = (int)((float)num + (float)(num3 - num) * num6);
                int num8 = (int)((float)num2 + (float)(num4 - num2) * num6);
                ref Color reference2 = ref pixels[num7 + num8 * 32];
                reference2 = Color.black;
                for (int m = 1; m < widthText; m++)
                {
                    int num9 = (int)((float)num7 + (float)((num4 - num2) * m) / num5);
                    int num10 = (int)((float)num8 - (float)((num3 - num) * m) / num5);
                    int num11 = (int)((float)num7 - (float)((num4 - num2) * m) / num5);
                    int num12 = (int)((float)num8 + (float)((num3 - num) * m) / num5);
                    if (num9 >= 0 && num9 < 32 && num10 >= 0 && num10 < 32)
                    {
                        ref Color reference3 = ref pixels[num9 + num10 * 32];
                        reference3 = Color.black;
                    }

                    if (num11 >= 0 && num11 < 32 && num12 >= 0 && num12 < 32)
                    {
                        ref Color reference4 = ref pixels[num11 + num12 * 32];
                        reference4 = Color.black;
                    }
                }
            }
        }

        texture2D.SetPixels(pixels);
        texture2D.Apply();
        return texture2D;
    }

    public float TestPattern(Texture2D fromTexture, Texture2D toTexture)
    {
        if (toTexture == null)
        {
            Debug.LogError("<b>Mouse Gesture Interpretation:</b> texture pattern for comparison is not set.");
            return 0f;
        }

        Color[] pixels = fromTexture.GetPixels();
        Color[] pixels2 = toTexture.GetPixels();
        float num = 0f;
        float num2 = 0f;
        float num3 = 0f;
        for (int i = 0; i < pixels2.Length; i++)
        {
            if (pixels2[i] == Color.black)
            {
                num += 1f;
            }
        }

        for (int j = 0; j < pixels.Length; j++)
        {
            if (pixels[j] == Color.black)
            {
                num2 += 1f;
                if (pixels2[j] == Color.black)
                {
                    num3 += 1f;
                }
            }
        }

        //float num4 = num2 - num3;
        float result = num3 / num;
        //Debug.Log($"num: {num} - num2: {num2} - num3: {num3} - num4: {num4} - result: {result}");
        /*if (num4 < num3)
        {
            return result;
        }*/

        //return 0f;
        return result;
    }

    public void SetIsGesturing(bool b)
    {
        gesturing = b;
    }

    public bool GetIsGesturing()
    {
        return gesturing;
    }

    public void SetTextWidht(int w)
    {
        widthText = w;
    }
}