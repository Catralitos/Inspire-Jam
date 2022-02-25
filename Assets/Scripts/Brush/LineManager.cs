using System.Collections.Generic;
using Combat;
using PDollarGestureRecognizer;
using UnityEngine;

namespace Brush
{
    public class LineManager : MonoBehaviour
    {
        [HideInInspector] public static LineManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple line managers present in scene! Destroying...");
                Destroy(gameObject);
            }
        }

        private Vector3 loc;
        private BattleSystem battleSystem;
        public LayerMask layerMask;

        public Transform gestureOnScreenPrefab;

        private List<Gesture> _trainingSet = new List<Gesture>();

        private List<Point> _points = new List<Point>();
        private int _strokeId = -1;

        private Vector3 _virtualKeyPosition = Vector2.zero;
        private Rect _drawArea;

        private int _vertexCount = 0;

        private List<LineRenderer> _gestureLinesRenderer = new List<LineRenderer>();
        private LineRenderer _currentGestureLineRenderer;

        //GUI
        private string _message;
        private bool _recognized;
        private string _newGestureName = "";


        [Header("Em dúvida, 75%")] [Range(0f, 1f)]
        public float matchingPercentage = 0.75f;

        private void Start()
        {
            battleSystem = BattleSystem.Instance;
            //TODO possivelmente mudar isto para um RECT de UI
            _drawArea = new Rect(0, 0, Screen.width, Screen.height);

            TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Moves");
            foreach (TextAsset gestureXml in gesturesXml)
                _trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
        }

        private void Update()
        {
            if (battleSystem.state != BattleState.Turn) return;

            if (Input.GetMouseButton(0))
            {
                _virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (_drawArea.Contains(_virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_recognized)
                    {
                        _recognized = false;
                        _strokeId = -1;

                        _points.Clear();

                        foreach (LineRenderer lineRenderer in _gestureLinesRenderer)
                        {
                            lineRenderer.SetVertexCount(0);
                            Destroy(lineRenderer.gameObject);
                        }

                        _gestureLinesRenderer.Clear();
                    }

                    ++_strokeId;

                    Transform tmpGesture =
                        Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                    _currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();
                    //Selection.activeGameObject = tmpGesture.gameObject;

                    _gestureLinesRenderer.Add(_currentGestureLineRenderer);

                    _vertexCount = 0;
                }
            }

            if (Input.GetMouseButton(0))
            {
                _points.Add(new Point(_virtualKeyPosition.x, -_virtualKeyPosition.y, _strokeId));

                if (_currentGestureLineRenderer == null) return;

                _currentGestureLineRenderer.SetVertexCount(++_vertexCount);
                _currentGestureLineRenderer.SetPosition(_vertexCount - 1,
                    Camera.main.ScreenToWorldPoint(new Vector3(_virtualKeyPosition.x, _virtualKeyPosition.y, 10)));
            }
        }

        public string TryRecognize()
        {
            if (_points.Count <= 0)
                return "";

            if (_recognized)
                ClearLine();

            _recognized = true;

            Gesture candidate = new Gesture(_points.ToArray());

            Result gestureResult = PointCloudRecognizer.Classify(candidate, _trainingSet.ToArray());

            if (gestureResult.Score < matchingPercentage)
            {
                ClearLine();
                return "";
            }

            if (_recognized) ClearLine();
            return gestureResult.GestureClass;
        }


        public void ClearLine()
        {
            _recognized = false;
            _strokeId = -1;

            _points.Clear();

            foreach (LineRenderer lineRenderer in _gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            _gestureLinesRenderer.Clear();
        }
    }
}