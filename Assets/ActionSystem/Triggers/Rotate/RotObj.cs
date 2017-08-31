using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions.Comparers;
namespace WorldActionSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class RotObj : MonoBehaviour
    {
        public enum DirType
        {
            x, y, z
        }

        public string stapName;
        public Color color = Color.green;
        public float triggerRadius = 1;
        public float minAngle = 0;
        public float maxAngle = 30;
        public float triggerAngle = 28;
        public float circleDetail = 40;
        public float deviation = 1;
        public float rotSpeed = 40;
        public DirType dirType;
        public bool startActive;
        public bool endActive;
        public Vector3 Direction
        {
            get
            {
                switch (dirType)
                {
                    case DirType.x:
                        return -transform.right;
                    case DirType.y:
                        return -transform.up;
                    case DirType.z:
                        return -transform.forward;
                }
                return Vector3.zero;
            }
        }
        public bool RotateAble
        {
            get
            {
                return rotAble;
            }
        }

        private float currAngle;
        private List<Vector3> _lines = new List<Vector3>();
        private LineRenderer lineRender;
        private bool highLight;
        public int queueID;
        private bool rotAble;
        private Quaternion startRot;
        private FloatComparer comparer;


        private void Start()
        {
            startRot = transform.rotation;
            lineRender = gameObject.AddComponent<LineRenderer>();
            lineRender.material = new Material(Shader.Find("Sprites/Default"));
            lineRender.SetWidth(.1f, .01f);
            gameObject.layer = LayerMask.NameToLayer("rotateItem");
            comparer = new FloatComparer(deviation);
            gameObject.SetActive(startActive);
        }

        private void Update()
        {
            _lines.Clear();
            AddCircle(transform.position, Direction, triggerRadius, _lines);
            if (highLight)
            {
                DrawCircles(_lines, color);
            }
        }

        void AddCircle(Vector3 origin, Vector3 axisDirection, float size, List<Vector3> resultsBuffer)
        {
            Vector3 up = axisDirection.normalized * size;
            Vector3 forward = Vector3.Slerp(up, -up, .5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * size;
            Camera myCamera = Camera.main;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = right.x;
            matrix[1] = right.y;
            matrix[2] = right.z;

            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;

            matrix[8] = forward.x;
            matrix[9] = forward.y;
            matrix[10] = forward.z;

            Vector3 lastPoint = origin + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 nextPoint = Vector3.zero;
            float multiplier = 360f / circleDetail;

            Plane plane = new Plane((myCamera.transform.position - transform.position).normalized, transform.position);

            for (var i = 0; i < circleDetail + 1; i++)
            {
                nextPoint.x = Mathf.Cos((i * multiplier) * Mathf.Deg2Rad);
                nextPoint.z = Mathf.Sin((i * multiplier) * Mathf.Deg2Rad);
                nextPoint.y = 0;

                nextPoint = origin + matrix.MultiplyPoint3x4(nextPoint);

                resultsBuffer.Add(lastPoint);
                resultsBuffer.Add(nextPoint);

                lastPoint = nextPoint;
            }
        }

        internal void SetRotateStartState()
        {
            rotAble = false;
            transform.rotation = startRot;
            gameObject.SetActive(startActive);
        }
        internal void SetActiveStep()
        {
            rotAble = true;
            gameObject.SetActive(true);
        }
        internal void SetHighLight(bool on)
        {
            this.highLight = on;
        }

        void DrawCircles(List<Vector3> lines, Color color)
        {
            lineRender.SetColors(color, color);
            lineRender.SetVertexCount(lines.Count);
            lineRender.SetPositions(lines.ToArray());
        }

        internal bool TryMarchRot()
        {
            return comparer.Equals(currAngle, triggerAngle);
        }

        public IEnumerator Clamp()
        {
            if (currAngle > maxAngle || currAngle < minAngle)
            {
                currAngle = Mathf.Clamp(currAngle, minAngle, maxAngle);
                var target = Quaternion.Euler(Direction * currAngle) * startRot;
                var start = transform.rotation;
                for (float timer = 0; timer < 1f; timer += Time.deltaTime)
                {
                    yield return new WaitForEndOfFrame();
                    transform.rotation = Quaternion.Lerp(start, target, timer);
                }
            }
            Debug.Log(currAngle);
        }

        public void Rotate(float amount)
        {
            currAngle += amount;
            transform.Rotate(Direction, amount, Space.World);
        }

        public void SetRotateEndState()
        {
            gameObject.SetActive(endActive);
        }
    }
}

