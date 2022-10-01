using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class XMLGenerator : MonoBehaviour
{
    public static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }
    public static byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }
    public static string SerializeObject(object pObject, Type classType)
    {
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(classType);
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        string XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }
    public static object DeserializeObject(string pXmlizedString, Type classType)
    {
        XmlSerializer xs = new XmlSerializer(classType);
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }
    public static void CreateXML(string rawData, string fileName, string fileLocation = "")
    {
        StreamWriter writer;
        if (fileLocation == "")
        {
            fileLocation = Application.dataPath + "/XMLs/";
        }
        FileInfo t = new FileInfo(fileLocation + "\\" + fileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(rawData);
        writer.Close();
    }
    public static string LoadXML(string fileName, string fileLocation = "")
    {
        if (fileLocation == "")
        {
            fileLocation = Application.dataPath + "/XMLs/";
        }
        if (!File.Exists(fileLocation + "\\" + fileName))
        {
            return "";
        }
        StreamReader r = File.OpenText(fileLocation + "\\" + fileName);
        string _info = r.ReadToEnd();
        r.Close();
        return _info;
    }
    public void GenerateXMLs()
    {
        BaseUtils baseUtils = FindObjectOfType<BaseUtils>();
        CreatureValues[] creatureValues = new CreatureValues[baseUtils.creatures.Length];
        for (int i = 0; i < baseUtils.creatures.Length; i++)
        {
            creatureValues[i].creatureType = baseUtils.creatures[i].creatureType;
            creatureValues[i].evolutionType = baseUtils.creatures[i].evolutionType;
            creatureValues[i].damageType = baseUtils.creatures[i].damageType;
            creatureValues[i].bodyType = baseUtils.creatures[i].bodyType;
            creatureValues[i].levelToEvolve = baseUtils.creatures[i].levelToEvolve;
            creatureValues[i].evolution = baseUtils.creatures[i].evolution;
            creatureValues[i].damage = baseUtils.creatures[i].damage;
            creatureValues[i].speed = baseUtils.creatures[i].speed;
            creatureValues[i].defense = baseUtils.creatures[i].defense;
            creatureValues[i].magic = baseUtils.creatures[i].magic;
        }
        string rawUnitData = SerializeObject(creatureValues, typeof(CreatureValues[]));
        CreateXML(rawUnitData, "Creatures.xml");
    }
}
public struct CreatureValues
{
    public CreatureType creatureType;
    public CreatureType evolutionType;
    public ElementType damageType;
    public ElementType bodyType;
    public int levelToEvolve;
    public int evolution;
    public int damage;
    public int speed;
    public int defense;
    public int magic;
}
#if UNITY_EDITOR
[CustomEditor(typeof(XMLGenerator))]
public class XMLGeneratorUI : Editor
{
    public override void OnInspectorGUI()
    {
        XMLGenerator generator = (XMLGenerator)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate XMLs"))
        {
            EditorUtility.SetDirty(generator);
            generator.GenerateXMLs();
            EditorUtility.ClearDirty(generator);
        }
    }
}
#endif
