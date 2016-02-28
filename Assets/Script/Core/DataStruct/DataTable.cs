using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DataTable {

	public struct Pair{
		public string key;
		public DataRow row;
		public Pair(string k , DataRow r ){key=k;row=r;}
	}
	public List<Pair> rows = new List<Pair>();
	public List<string> firstList = new List<string>();

	public void AddFirst( string name )
	{
		firstList.Add(name);
	}

	public DataRow Select(string name)
	{
		DataRow res = null;
		foreach(Pair p in rows)
		{
			if (p.key == name)
				res = p.row;
		}

		return res;
	}

	public bool Contains(string name )
	{
		// foreach(Pair p in rows)
		// {
		// 	if (p.key == name)
		// 		return true;
		// }
		// return false;

		return firstList.Contains(name);
	}

	public void AddRow( string key , DataRow row)
	{

		if ( row != null && !row.isAllBlank() && key != null && key != "" )
		{
			// Debug.Log("Add row " + key + row.Select("State"));
			Pair p = new Pair(key,row);
			rows.Add(p);
		}
	}
}

public class DataRow{
	Dictionary<string,string> elements = new Dictionary<string, string>();

	public string Select(string name)
	{
		string res = "";
		elements.TryGetValue(name, out res);
		if (res == null )
			res = "";
		return res;
	}

	public bool isBlank(string name)
	{
		string res;
		elements.TryGetValue(name, out res);
		if(res == null || res == "" )
			return true;

		return false;
	}

	public bool isFull(string name )
	{
		return !isBlank(name);
	}

	public bool isAllBlank()
	{
		foreach(string key in elements.Keys)
		{
			if (!isBlank(key))
				return false;
		}
		return true;
	}

	public bool Add(string key, string ele)
	{
		if (elements.ContainsKey(key))
		{
			Debug.Log("Mutiple keys " + key);
			return false;
		}

		// Debug.Log("Add Element " + key + " " + ele);
		elements.Add(key, ele);
		return true;
	}

}