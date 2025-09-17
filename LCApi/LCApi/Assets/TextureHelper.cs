using System.IO;
using UnityEngine;

namespace LCApi.Assets
{
    public static class TextureHelper
    {
        public static Texture2D GenSolidTex()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            return texture;
        }

        public static Texture2D LoadBlockTexture(string pathToTexture)
        {
            byte[] texData = File.ReadAllBytes(pathToTexture);
            Texture2D texture = new Texture2D(64, 48, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.anisoLevel = 1;
            texture.ignoreMipmapLimit = false;
            texture.LoadImage(texData, false);
            texture.Apply();

            // TODO better fix by patching MeshData.GenerateBlockMesh
            // Current game renderer creates incorrect UV map,
            // this monstrousity looks for transparent pixels and fills them
            // with neighbor pixels to avoid black edges on the blocks,
            // unfortunately TextureWrapMode.Repeat does not solve this for custom textures, Idk why

            int width = texture.width;
            int height = texture.height;

            Vector2Int[] dirs = new Vector2Int[4];
            dirs[0] = Vector2Int.left;
            dirs[1] = Vector2Int.right;
            dirs[2] = Vector2Int.up;
            dirs[3] = Vector2Int.down;

            Color32[] colors = texture.GetPixels32();
            Color32[] colors2 = texture.GetPixels32();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        Color32 col = colors[y * width + x];
                        if (col.a <= 0)
                        {
                            foreach (Vector2Int dir in dirs)
                            {
                                if (x + dir.x >= 0 && x + dir.x < width &&
                                    y + dir.y >= 0 && y + dir.y < height)
                                {
                                    Color32 neighborColor = colors[(y + dir.y) * width + (x + dir.x)];
                                    if (neighborColor.a != 0)
                                    {
                                        colors2[y * width + x] = neighborColor;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            texture.SetPixels32(colors2);
            texture.Apply();
            return texture;
        }
    }
}
