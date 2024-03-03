using Camera;
using Levels;
using Player;
using Player.ActionHandlers;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Scenes;
using Utils.Singleton;


namespace Camera
{
    public class CameraController : DontDestroyMonoBehaviourSingleton<CameraController>
    {
        [Header("Settings")]
        [SerializeField] private float decelerationSpeed = 5;
        [SerializeField] private float sensitivity = 1;

        // Camera
        private UnityEngine.Camera _activeCamera;
        private LevelRect _currentLevelRect;
        private Vector3 _sourcePosition;
        private TransformUtils.Rect _cameraViewportWorldRect;

        // Drag
        private Vector2 _startDragScreenPoint;
        private float _dragDistance;
        private Vector2 _dragDirection;
        private float _dragSpeed;
        private bool _dragging;

        private void Start()
        {
            ScenesChanger.SceneLoadedEvent += ResetPosition;
            _activeCamera = CameraHolder.Instance.MainCamera;
            _sourcePosition = _activeCamera.transform.position;
        }

        private void Update()
        {
            CalculateCameraRect();

            if (Input.GetMouseButtonDown(0))
                OnDragStart();
            else if (Input.GetMouseButtonUp(0))
                OnDragEnd();
        }

        public void SetRect(LevelRect levelRect)
        {
            _currentLevelRect = levelRect;
            ResetPosition();
        }

        private void ResetPosition()
        {
            if (_currentLevelRect)
                _activeCamera.transform.position = new Vector3(_currentLevelRect.center.x, _currentLevelRect.center.y, _sourcePosition.z);
            else
                _activeCamera.transform.position = _sourcePosition;
        }

        private void CalculateCameraRect()
        {
            _cameraViewportWorldRect = new TransformUtils.Rect(
                _activeCamera.ScreenToWorldPoint(Vector2.zero),
                _activeCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)));
        }

        private void OnDragStart()
        {
            _startDragScreenPoint = Input.mousePosition;
            _dragging = true;
        }

        private void OnDragEnd()
        {
            _startDragScreenPoint = Input.mousePosition;
            _dragging = false;
        }

        private void FixedUpdate()
        {
            if (_currentLevelRect && _dragging && PlayerController.PlayerState != PlayerState.Connecting)
                UpdateParameters(Input.mousePosition);

            if (_dragSpeed > 0)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            _dragSpeed -= Mathf.Min(Time.deltaTime * _dragSpeed * decelerationSpeed, _dragSpeed);

            var speedDir = _dragSpeed * _dragDirection * Time.deltaTime;

            var cameraViewportWorldRectSize = _cameraViewportWorldRect.Size();

            var signedDir = new Vector2(Mathf.Sign(_dragDirection.x), Mathf.Sign(_dragDirection.y)) * 0.1f;

            Vector3 cameraNewPosition = _activeCamera.transform.position + (Vector3)speedDir;

            if (_cameraViewportWorldRect.left + signedDir.x <= _currentLevelRect.rect.left)
                cameraNewPosition.x = _currentLevelRect.rect.left + cameraViewportWorldRectSize.x / 2f;
            else if (_cameraViewportWorldRect.right + signedDir.x >= _currentLevelRect.rect.right)
                cameraNewPosition.x = _currentLevelRect.rect.right - cameraViewportWorldRectSize.x / 2f;

            if (_cameraViewportWorldRect.bottom + signedDir.y <= _currentLevelRect.rect.bottom)
                cameraNewPosition.y = _currentLevelRect.rect.bottom + cameraViewportWorldRectSize.y / 2f;
            else if (_cameraViewportWorldRect.top + signedDir.y >= _currentLevelRect.rect.top)
                cameraNewPosition.y = _currentLevelRect.rect.top - cameraViewportWorldRectSize.y / 2f;

            _activeCamera.transform.position = new Vector3(cameraNewPosition.x, cameraNewPosition.y, _sourcePosition.z);
        }

        private void UpdateParameters(Vector2 secondDragScreenPoint)
        {
            _dragDistance = Vector2.Distance(secondDragScreenPoint, _startDragScreenPoint);
            _dragDirection = -(secondDragScreenPoint - _startDragScreenPoint).normalized;
            _dragSpeed = _dragDistance / 50f * sensitivity;
        }
    }
}
