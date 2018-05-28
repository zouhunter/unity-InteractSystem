using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using WorldActionSystem;
using System.Linq;
using System;
using System.Collections.Generic;

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
    public bool TestExecute()
    {
        Debug.Log("TestExecute");
        return true;
    } 
	[Test]
    public void LaterSelect()
    {
        var names = new List<string> { "Nino", "Alberto", "Juan", "Mike", "Phil" };

        var namesWithJ = from n in names
                         where n.StartsWith("J") && TestExecute()
                         orderby n
                         select n;
        var namesWithJforWhere = names.Where<string>(n => n.StartsWith("J"));
        var namesWithJforFindAll = names.FindAll(n => n.StartsWith("J"));
        Debug.Log("First iteration by Linq:");
        foreach (string name in namesWithJ)
        {
            Debug.Log(name);
        }

        Debug.Log("First iteration by names.Where():");
        foreach (var name in namesWithJforWhere)
        {
            Debug.Log(name);
        }
        Debug.Log("First iteration by names.FindAll():");
        foreach (var name in namesWithJforFindAll)
        {
            Debug.Log(name);
        }


        Debug.Log("\n");

        names.Add("John");
        names.Add("Jim");
        names.Add("Jack");
        names.Add("Denny");
        Debug.Log("Second iteration by Linq:");
        foreach (string name in namesWithJ)
        {
            Debug.Log(name);
        }
        Debug.Log("Second iteration by names.Where():");
        foreach (var name in namesWithJforWhere)
        {
            Debug.Log(name);
        }
        Debug.Log("Second iteration by names.FindAll():");
        foreach (var name in namesWithJforFindAll)
        {
            Debug.Log(name);
        }
    }
}
