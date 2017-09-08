#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Diagnostics;
public class TexturePackBuilder : Editor
{
    [MenuItem("SpritesPacker/打包TexturePack")]
    public static void BuildTexturePacker()
    {
        //选择并设置TP命令行的参数和参数值
        string commandText = " --sheet {0}.png --data {1}.xml --format sparrow --trim-mode Trim --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT  --disable-rotation --scale 1 {2}";
        StringBuilder sb = new StringBuilder("");
        UnityEngine.Object[] paths = Selection.objects;
        if (paths.Length == 0)
        {
            return;
        }
        string folderName = "";
        for (int j = 0; j < paths.Length; j++)
        {
            string filePath = AssetDatabase.GetAssetPath(paths[j]);
            filePath = Application.dataPath.Replace("Assets", "") + filePath;
            string extenstion = Path.GetExtension(filePath);
            folderName = Path.GetDirectoryName(filePath);
            if (extenstion == ".png" || extenstion == ".jpg")
            {
                sb.Append(filePath);
                sb.Append("  ");
            }
        }
        string [] pathItemArr = folderName.Split('/');
        folderName += "/" + pathItemArr[pathItemArr.Length - 1] + "_Altas";
        //执行命令行
        processCommand("C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe", string.Format(commandText, folderName, folderName, sb.ToString()));
        AssetDatabase.Refresh();
    }

    private static void processCommand(string command, string argument)
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = false;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        Process p = Process.Start(start);
        if (!start.UseShellExecute)
        {
            UnityEngine.Debug.Log(p.StandardOutput.ReadToEnd());
            UnityEngine.Debug.Log(p.StandardError.ReadToEnd());
        }

        p.WaitForExit();
        p.Close();
    }

    [MenuItem("SpritesPacker/拆分TexturePacker")]
    public static void SplitTexturePacker()
    {
        string sheetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        Dictionary<string, Vector4> tIpterMap = new Dictionary<string, Vector4>();
        AssetDatabase.Refresh();
        FileStream fs = new FileStream(sheetPath.Replace(".png",".xml"), FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string jText = sr.ReadToEnd();
        fs.Close();
        sr.Close();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(jText);
        XmlNodeList elemList = xml.GetElementsByTagName("SubTexture");
        WriteMeta(elemList, sheetPath, tIpterMap);
        AssetDatabase.Refresh();
    }

    [MenuItem("SpritesPacker/拆分_Jpg_TexturePacker")]
    public static void SplitJpgTexturePacker()
    {
        string sheetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        Dictionary<string, Vector4> tIpterMap = new Dictionary<string, Vector4>();
        AssetDatabase.Refresh();
        FileStream fs = new FileStream(sheetPath.Replace("_RGB.jpg", ".xml"), FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string jText = sr.ReadToEnd();
        fs.Close();
        sr.Close();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(jText);
        XmlNodeList elemList = xml.GetElementsByTagName("SubTexture");
        WriteMeta(elemList, sheetPath, tIpterMap);
        AssetDatabase.Refresh();
    }

    [MenuItem("SpritesPacker/拆分_Alpha_TexturePacker")]
    public static void SplitAlphaTexturePacker()
    {
        string sheetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        Dictionary<string, Vector4> tIpterMap = new Dictionary<string, Vector4>();
        AssetDatabase.Refresh();
        FileStream fs = new FileStream(sheetPath.Replace("_Alpha.jpg", ".xml"), FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string jText = sr.ReadToEnd();
        fs.Close();
        sr.Close();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(jText);
        XmlNodeList elemList = xml.GetElementsByTagName("SubTexture");
        WriteMeta(elemList, sheetPath, tIpterMap);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 写信息到SpritesSheet里
    /// </summary>
    /// <param name="elemList"></param>
    /// <param name="sheetPath"></param>
    /// <param name="borders"></param>
    static void WriteMeta(XmlNodeList elemList, string sheetPath, Dictionary<string, Vector4> borders)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(sheetPath);
        string impPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter asetImp = TextureImporter.GetAtPath(impPath) as TextureImporter;
        for (int i = 0; i < asetImp.spritesheet.Length; i++)
        {
            //如果这张图集已经拉好了9宫格，需要先保存起来
            if (asetImp.spritesheet[i].border.x != 0 || asetImp.spritesheet[i].border.y != 0 || asetImp.spritesheet[i].border.z != 0 || asetImp.spritesheet[i].border.w != 0)
            {
                borders.Add(asetImp.spritesheet[i].name, asetImp.spritesheet[i].border);
            }
        }
        SpriteMetaData[] metaData = new SpriteMetaData[elemList.Count];
        for (int i = 0, size = elemList.Count; i < size; i++)
        {
            XmlElement node = (XmlElement)elemList.Item(i);
            Rect rect = new Rect();
            rect.x = int.Parse(node.GetAttribute("x"));
            rect.y = texture.height - int.Parse(node.GetAttribute("y")) - int.Parse(node.GetAttribute("height"));
            rect.width = int.Parse(node.GetAttribute("width"));
            rect.height = int.Parse(node.GetAttribute("height"));
            metaData[i].rect = rect;
            metaData[i].pivot = new Vector2(0.5f, 0.5f);
            metaData[i].name = node.GetAttribute("name");
            if (borders.ContainsKey(metaData[i].name))
            {
                metaData[i].border = borders[metaData[i].name];
            }
        }
        asetImp.spritesheet = metaData;
        asetImp.textureType = TextureImporterType.Sprite;
        asetImp.spriteImportMode = SpriteImportMode.Multiple;
        asetImp.mipmapEnabled = false;
        asetImp.SaveAndReimport();
    }

    static string GetAssetPath(string path)
    {
        string[] seperator = { "Assets" };
        string p = "Assets" + path.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[1];
        return p;
    }

}

internal class TextureIpter
{
    public string spriteName = "";
    public Vector4 border = new Vector4();
    public TextureIpter() { }
    public TextureIpter(string spriteName, Vector4 border)
    {
        this.spriteName = spriteName;
        this.border = border;
    }
}
#endif