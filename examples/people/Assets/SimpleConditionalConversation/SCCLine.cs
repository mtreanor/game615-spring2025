using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCCLine 
{
	public string questState;
	public string character;
	public string condition1Left;
	public string condition1Comp;
	public object condition1Right;
	public string condition2Left;
	public string condition2Comp;
	public object condition2Right;
	public string effectLeft;
	public string effectOp;
	public object effectRight;
	public string line1;
	public string line2;
	public string line3;

	public int numTimesSelected = 0;

	public SCCLine() 
	{
		this.line1 = "DEFAULT EMPTY LINE";
	}

	public string renderLine()
	{
		List<string> lines = new List<string>();
		if (line1 != "") lines.Add(line1);
		if (line2 != "") lines.Add(line2);
		if (line3 != "") lines.Add(line3);

		int index = this.numTimesSelected % lines.Count;
		this.numTimesSelected++;

		return lines[index];
	}
}
