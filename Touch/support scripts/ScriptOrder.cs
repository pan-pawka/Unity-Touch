using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/* 
*** SCRIPT ORDER CLASS ***
This adds [ScriptOrder(int)] attribute.
Used to set script execution order for TouchHandler.cs
 */

public class ScriptOrder:Attribute {
	public int order;
	
	public ScriptOrder(int order) {
		this.order = order;
	}
}

#if UNITY_EDITOR
[InitializeOnLoad]
public class ScriptOrderManager {
	
	static ScriptOrderManager() {
		foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
			if (monoScript.GetClass() != null) {
				foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrder))) {
					var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
					var newOrder = ((ScriptOrder)a).order;
					if (currentOrder != newOrder)
						MonoImporter.SetExecutionOrder(monoScript, newOrder);
				}
			}
		}
	}
}
#endif