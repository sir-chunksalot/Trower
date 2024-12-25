using UnityEngine;
using System.Collections;


[System.Serializable]
public class ArrayLayout
{


	[System.Serializable]
	public struct rowData
	{
		public bool[] row;
	}


	public rowData[] rows = new rowData[7];
	public int xBounds = 5;
	public int yBounds = 6;
}
