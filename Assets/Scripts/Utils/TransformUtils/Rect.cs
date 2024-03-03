using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformUtils
{
    public struct Rect
    {
        public float top;
        public float bottom;
        public float left;
        public float right;

        public Rect(Vector2 bottomLeftCorner, Vector2 topRightCorner)
        {
            top = topRightCorner.y;
            bottom = bottomLeftCorner.y;
            left = bottomLeftCorner.x;
            right = topRightCorner.x;
        }

        public Vector2 Size()
        {
            return new Vector2(right - left, top - bottom);
        }
    }
}
