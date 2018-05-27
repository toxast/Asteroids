using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "Data", menuName = "HandleTypeData", order = 1)]
public class HandleTypeData : ScriptableObject{
	public string naming = "vertex";
	public Color gizmoColor = Color.green;
	public float gizmoSize  = 0.1f; 
	public bool drawDirection = false;

	static Regex regex = new Regex( @"(\w+)\s+(\w+)\s+\(\w+\)", RegexOptions.IgnoreCase);
	public static Regex GetRegexForNaming{get{ return regex;}}

	public string GetHandleName(int index){
		return string.Format ("{0} {1}", naming, index);
	}
} 
