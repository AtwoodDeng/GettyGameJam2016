using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLHelper 
{
    private string fileName = null; 
    // private IWorkbook workbook = null;
    // private FileStream fs = null;
    // private bool disposed;

    public XMLHelper(string fileName)
    {
        this.fileName = fileName;
        // disposed = false;
    }

    public DataTable ReadSheet(string sheetName = "" )
    {
        // Debug.Log("Begin to read " + sheetName + " form " + fileName);

        TextAsset xmlData = new TextAsset();
        xmlData = (TextAsset)Resources.Load(fileName, typeof(TextAsset));

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData.text);

        DataTable res = new DataTable();

        //Get sheet by sheetName
        XmlNodeList sheets = xmlDoc.GetElementsByTagName("Worksheet");
        XmlNode sheet = null;

        foreach (XmlNode s in sheets)
        {
            if (s.Attributes["ss:Name"].Value == sheetName)
            {
                sheet = s;
            }
        }

        if (sheet == null )
        {
            Debug.Log("Cannot find sheet " + sheetName);
            return res;
        }

        //Analyze the sheet
        // xmlDoc.LoadXml(sheet.InnerXml);

        XmlNode sheetContent = sheet.ChildNodes[0];

        for(int i = sheetContent.ChildNodes.Count - 1 ; i >= 0 ; --i)
        {
            // Debug.Log(sheetContent.ChildNodes[i].LocalName);
            if ( sheetContent.ChildNodes[i].LocalName != "Row" )
                sheetContent.RemoveChild(sheetContent.ChildNodes[i]);
        }

        // XmlNodeList rows = xmlDoc.GetElementsByTagName("Row");

        XmlNodeList rows = sheetContent.ChildNodes;

        var firstLineDic = new Dictionary<string,string>();

        // xmlDoc.LoadXml(rows[0].InnerXml);
        XmlNodeList firstLine = rows[0].ChildNodes;

        for(int i = 0 ; i < firstLine.Count ; ++ i )
        {
            firstLineDic.Add((i+1).ToString(), firstLine[i].InnerText);
            res.AddFirst(firstLine[i].InnerText);
            // Debug.Log("First Line " +  firstLine[i].InnerText);
        }

        for(int i = 1 ; i < rows.Count ; ++i )
        // for( int i = 1 ; i < 3 ; ++ i )
        {
            // xmlDoc.LoadXml(rows[i].InnerXml);
            // XmlNodeList cells = xmlDoc.GetElementsByTagName("Cell",rows[i].NamespaceURI);

            XmlNodeList cells = rows[i].ChildNodes;

            DataRow r = new DataRow();

            int indexNow = 1;

            for(int j = 0 ; j < cells.Count ; ++ j )
            // for( int j = 0  ; j < 3 ; ++ j)
            {

                if (cells[j].Attributes["ss:Index"] != null)
                {
                    indexNow = int.Parse( cells[j].Attributes["ss:Index"].Value);
                }

                if (cells[j].InnerText != "" )
                    r.Add(firstLineDic[indexNow.ToString()],cells[j].InnerText);
                indexNow ++;
            }

            string key = "";
            if (!r.isBlank(firstLineDic["1"]))
                key = r.Select(firstLineDic["1"]);

            res.AddRow( key, r);

        }

        // Debug.Log("rows " + rows.Count.ToString());


        return res;
    }

};
