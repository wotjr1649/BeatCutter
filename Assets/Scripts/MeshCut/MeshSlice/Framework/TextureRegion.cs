using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice
{
    public struct TextureRegion
    {
        private readonly float pos_start_x;
        private readonly float pos_start_y;
        private readonly float pos_end_x;
        private readonly float pos_end_y;

        public TextureRegion(float startX, float startY, float endX, float endY)
        {
            this.pos_start_x = startX;
            this.pos_start_y = startY;
            this.pos_end_x = endX;
            this.pos_end_y = endY;
        }

        public float startX
        {
            get { return this.pos_start_x; }
        }

        public float startY
        {
            get { return this.pos_start_y; }
        }

        public float endX
        {
            get { return this.pos_end_x; }
        }

        public float endY
        {
            get { return this.pos_end_y; }
        }

        public Vector2 start
        {
            get { return new Vector2(startX, startY); }
        }

        public Vector2 end
        {
            get { return new Vector2(endX, endY); }
        }

        public Vector2 Map(Vector2 uv)
        {
            return Map(uv.x, uv.y);
        }

        public Vector2 Map(float x, float y)
        {
            float mappedX = MAP(x, 0.0f, 1.0f, pos_start_x, pos_end_x);
            float mappedY = MAP(y, 0.0f, 1.0f, pos_start_y, pos_end_y);

            return new Vector2(mappedX, mappedY);
        }

        private static float MAP(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }

    public static class TextureRegionExtension
    {
        public static TextureRegion GetTextureRegion(this Material mat,
            int pixX,
            int pixY,
            int pixWidth,
            int pixHeight)
        {
            return mat.mainTexture.GetTextureRegion(pixX, pixY, pixWidth, pixHeight);
        }

        public static TextureRegion GetTextureRegion(this Texture tex,
            int pixX,
            int pixY,
            int pixWidth,
            int pixHeight)
        {
            int textureWidth = tex.width;
            int textureHeight = tex.height;

            int calcWidth = Mathf.Min(textureWidth, pixWidth);
            int calcHeight = Mathf.Min(textureHeight, pixHeight);
            int calcX = Mathf.Min(Mathf.Abs(pixX), textureWidth);
            int calcY = Mathf.Min(Mathf.Abs(pixY), textureHeight);

            float startX = calcX / (float) textureWidth;
            float startY = calcY / (float) textureHeight;
            float endX = (calcX + calcWidth) / (float) textureWidth;
            float endY = (calcY + calcHeight) / (float) textureHeight;

            return new TextureRegion(startX, startY, endX, endY);
        }
    }
}