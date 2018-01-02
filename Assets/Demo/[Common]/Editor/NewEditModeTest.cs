using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using WorldActionSystem;

public class NewEditModeTest {

	[Test]
	public void NewEditModeTestSimplePasses() {
        var operatetype = typeof(OperateController);
        var types = operatetype.Assembly.GetTypes();
        foreach (var item in types)
        {
            Debug.Log(item.Name);
        }
    }
}
