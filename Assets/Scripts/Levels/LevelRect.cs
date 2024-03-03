using Camera;
using Connection;
using Events;
using System.Collections;
using System.Collections.Generic;
using TransformUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace Levels
{
    public class LevelRect : MonoBehaviour
    {
        public Vector2 center { get; private set; }

        public TransformUtils.Rect rect { get; private set; }

        public void Awake()
        {
            CameraController.Instance.SetRect(this);
            center = transform.position;

            var scale = new Vector2(transform.localScale.x, transform.localScale.y);
            var bottomLeftCorner = center - scale / 2f;
            var topRightCorner = center + scale / 2f;

            rect = new TransformUtils.Rect(bottomLeftCorner, topRightCorner);

        }
    }
}
