using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
namespace InteractSystem.Actions
{
    public class TranformTest
    {

        [Test]
        public void Test0()
        {
            var transform = new GameObject("temp").GetComponent<Transform>();
            transform.position = new Vector3(10, 20, 30);
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            Debug.Log("当前旋转状态：" + transform.eulerAngles);

            var dir = transform.TransformVector(Vector3.forward);
            Debug.Log("正前方是：" + dir);

            dir = transform.TransformVector(Vector3.right);
            Debug.Log("右方向是：" + dir);

            dir = transform.InverseTransformVector(Vector3.forward);
            Debug.Log("反方向是：" + dir);
            Object.DestroyImmediate(transform.gameObject);

        }

    }
}