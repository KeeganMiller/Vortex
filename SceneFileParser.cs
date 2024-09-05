using System.Collections.Generic;
using System.Numerics;
using System.IO;
using Raylib_cs;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Reflection;
using Microsoft.VisualBasic;

namespace Vortex;

public enum EDataType
{
    SCENE_PROP_Asset,
    SCENE_PROP_AssetValue,
    SCENE_PROP_Element,
    SCENE_PROP_Component,
    SCENE_PROP_ComponentValue,
    SCENE_RPOP_Error,
}

public static class SceneFileParser
{
    /// <summary>
    /// Handles reading in the scene file and is base for
    /// Creating elements/components based on the input
    /// </summary>
    /// <param name="scenePath"></param>
    /// <param name="requested"></param>
    public static void ParseFile(string scenePath, ResourceManager requested)
    {
        if(!string.IsNullOrEmpty(scenePath) && File.Exists(scenePath))
        {
            var lines = File.ReadAllLines(scenePath);                   // Get the file as an array by lines
            bool isReadingAssets = false;                   // check if we are reading assets

            Element currentElement = null;                  // Pre define current element

            for(var i = 0; i < lines.Length; ++i)
            {
                List<string> relatedLines = new List<string>();                 // Pre-define related lines list
                var dataType = GetDataType(lines[i]);                   // determine what data type we are creating
                List<SceneFileDataContainer> properties = new List<SceneFileDataContainer>();                   // Define a list of properties
                relatedLines = GetRelatedLines(lines, i, dataType);                     // Get all the lines related to our current line
                var instance = ParseData(relatedLines, dataType);                   // Create the instance of the class

                if(instance is AssetData instanceAsset)
                    SceneManager.GlobalResources.AddLoadedAsset(instanceAsset);
                
                // If the instance is an element than set the current element
                if(instance is Element element)
                {
                    currentElement = element;
                    requested.AddElement(currentElement);
                }

                // Add any components that are required
                if(instance is Component compInstance && currentElement != null)
                {
                    if(compInstance is TransformComponent compTrans)
                    {
                        currentElement.SetTransform(compTrans);
                    } else 
                    {
                        currentElement.AddComponent(compInstance);
                    }
                }

                i += relatedLines.Count - 1;
            }
        }
    }

    public static List<string> GetRelatedLines(string[] lines, int currentIndex, EDataType type)
    {
        List<string> relatedLines = new List<string>();
        for(var i = currentIndex; i < lines.Length; ++i)
        {
            if(lines[i][0] != GetCurrentDataChar(type) || lines[i][0] != GetNextDataChar(type))
                break;
        }

        return relatedLines;
    }

    public static object ParseData(List<string> lines, EDataType dataType)
    {
        List<SceneFileDataContainer> properties = new List<SceneFileDataContainer>();                   // List of all the properties for this file
        object instance = null;                      // Reference to the instance we are creating
        string identifier = "";                          // Reference to the identifier string
        foreach(var line in lines)
        {
            // Generate the data identifiers
            var currentDataLine = GetCurrentDataChar(dataType);
            var nextDataLine = GetNextDataChar(dataType);


            if(line[0] == currentDataLine)
            {
                identifier = line;   
                continue;
            } else 
            {
                line.Replace(nextDataLine.ToString(), "");              // Remove the identifier
                var splitData = line.Split(":");                        // Split the name and value apart


                // Get the type of value this is
                Type type = splitData[1][0] switch
                {
                    'I' => typeof(int),
                    'V' => typeof(Vector2),
                    'S' => typeof(string),
                    'F' => typeof(float),
                    'B' => typeof(bool)
                };

                var value = ReplaceStrValueWithType(splitData[1]);                 // Remove the value type to get the raw value

                // Create the property
                properties.Add(new SceneFileDataContainer
                {
                    Name = line.Replace($"{GetDataType(line)}#", ""),
                    type = type,
                    Value = value
                });
            }
        }

        if(!string.IsNullOrEmpty(identifier))
            identifier.Replace($"{GetCurrentDataChar(dataType)}#", "");

        switch(dataType)
        {
            case EDataType.SCENE_PROP_Asset:
                return CreateInstance(dataType, null, properties);
            case EDataType.SCENE_PROP_Element:
                return CreateInstance(dataType, null, properties, identifier);
            case EDataType.SCENE_PROP_Component:
                return CreateInstance(dataType, identifier, properties);
            default:
                Debug.Print("SceneFileParser::ParseData -> Error parses file data", EPrintMessageType.PRINT_Error);
                break;
        }

        return null;
    }


    private static EDataType GetDataType(string line)
    {
        switch(line[0])
        {
            case 'E':
                return EDataType.SCENE_PROP_Element;
            case 'A':
                return EDataType.SCENE_PROP_Asset;
            case '&':
                return EDataType.SCENE_PROP_AssetValue;
            case '@':
                return EDataType.SCENE_PROP_Component;
            case '-':
                return EDataType.SCENE_PROP_ComponentValue;

        }

        return EDataType.SCENE_RPOP_Error;
    }

    private static char GetNextDataChar(EDataType dataType)
    {
        switch(dataType)
        {
            case EDataType.SCENE_PROP_Asset:
                return '&';
            case EDataType.SCENE_PROP_Element:
                return '@';
            case EDataType.SCENE_PROP_Component:
                return '-';
        }

        return '@';
    }

    private static char GetCurrentDataChar(EDataType dataType)
    {
        switch(dataType)
        {
            case EDataType.SCENE_PROP_Asset:
                return 'A';
            case EDataType.SCENE_PROP_AssetValue:
                return '&';
            case EDataType.SCENE_PROP_Element:
                return 'E';
            case EDataType.SCENE_PROP_Component:
                return '@';
            case EDataType.SCENE_PROP_ComponentValue:
                return '-';
            default:
                Debug.Print("SceneFileParser::GetCurrentDataChar -> Failed to get data type, please check the input", EPrintMessageType.PRINT_Error);
                return 'P';
            
        }
    }

    private static object CreateInstance(EDataType identifier, string className, List<SceneFileDataContainer> properties, string objectName = "VortexObject")
    {
        switch(identifier)
        {
            case EDataType.SCENE_PROP_Asset:
                var type = GetAssetType((string)properties[3].Value);
                switch(type)
                {
                    case EAssetType.ASSET_Sprite:
                        return new SpriteData((string)properties[0].Value, (int)properties[1].Value, (string)properties[2].Value, type);
                    case EAssetType.ASSET_Font:
                        return new FontAsset((string)properties[0].Value, (int)properties[1].Value, (string)properties[2].Value, type);
                    case EAssetType.ASSET_Shader:
                        return new ShaderAsset((string)properties[0].Value, (int)properties[1].Value, (string)properties[2].Value, type);
                }
                break;
            case EDataType.SCENE_PROP_Element:
                return new Element(objectName);
            case EDataType.SCENE_PROP_Component:
                Type classType = Type.GetType(className);
                if(classType == null)
                {
                    Debug.Print("SceneFileParser::CreateInstance -> Unable to create component instance", EPrintMessageType.PRINT_Error);
                    return null;
                }
                var componentInstance = Activator.CreateInstance(classType);
                if(componentInstance != null)
                    foreach(var prop in properties)
                        SetPropertyValue(componentInstance, prop.Name, prop.Value);
                return componentInstance;

        }

        Debug.Print($"SceneFileParser::CreateInstance -> Failed to create instance of class: {className}", EPrintMessageType.PRINT_Error);
        return null;
    }

    private static EAssetType GetAssetType(string value)
    {
        switch(value)
        {
            case "Texture":
                return EAssetType.ASSET_Sprite;
            case "Font":
                return EAssetType.ASSET_Font;
            case "Shader":
                return EAssetType.ASSET_Shader;
            default:
                return EAssetType.ASSET_Sprite;
        }
    }

    private static object ReplaceStrValueWithType(string value)
    {
        value.Replace(")", "");
        switch(value[0])
        {
            case 'I':
                value.Replace("I(", "");
                return GetIntValue(value);
            case 'S':
                value.Replace("S(", "");
                return value;
            case 'V':
                value.Replace("V(", "");
                return GetVectorValue(value);
            case 'F':
                value.Replace("F(", "");
                return GetFloatValue(value);
            case 'B':
                value.Replace("B(", "");
                return GetBoolValue(value);
        }

        return value;
    }

    private static int GetIntValue(string data)
    {
        return int.Parse(data);
    }

    private static Vector2 GetVectorValue(string data)
    {
        var split = data.Split(",");
        return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
    }

    private static float GetFloatValue(string data)
    {
        return float.Parse(data);
    }

    private static bool GetBoolValue(string data)
    {
        return data == "true" ? true : false;
    }

    private static void SetPropertyValue(object instance, string propName, object value)
    {
        if(instance == null)
            return;

        var type = instance.GetType();
        PropertyInfo propInfo = type.GetProperty(propName);

        if(propInfo != null && propInfo.CanWrite)
        {
            propInfo.SetValue(instance, Convert.ChangeType(value, propInfo.PropertyType));
        } else 
        {
            Debug.Print($"SceneFileParser::SetPropertyValue -> Property: {propName} not found or writable", EPrintMessageType.PRINT_Error);
        }
    }
}

public class SceneFileDataContainer
{
    public string Name;
    public Type type;
    public object Value;
}