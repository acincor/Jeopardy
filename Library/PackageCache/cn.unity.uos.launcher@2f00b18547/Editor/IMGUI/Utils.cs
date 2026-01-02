using UnityEngine;
using UnityEditor;

namespace Unity.UOS.Launcher
{
    public class Utils
    {
        public static int Normalize(int num)
        {
            return (int)(num / EditorGUIUtility.pixelsPerPoint);
        }

        public static Texture2D CreateTexture(Color color, Color borderColor, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            for (int i = 0; i < width; i += 1)
            {
                for (int j = 0; j < height; j += 1)
                {
                    texture.SetPixel(i, j, color);
                    if (j == 0)
                    {
                        texture.SetPixel(i, j, borderColor);
                    }
                }
            }
            texture.Apply();
            return texture;
        }
    }
}