using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NewEditorTest {

    [Test]
    public void EditorTest()
    {
        Debug.Log(1 << 8);
        Debug.Log(0x8);
        Debug.Log(~(1<<8));
        Debug.Log((1<<8));
        Debug.Log(LayerMask.GetMask("H"));
    }
}
