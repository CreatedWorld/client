using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;

public class MaterialTextureForETC1{
    public static Dictionary<string, bool> texturesAlphaDic = new Dictionary<string, bool>();

    [MenuItem("EffortForETC1/拆分全部图集")]
    public static void SeperateAllTextures()
    {
        string[] paths = Directory.GetFiles(Application.dataPath, "*Atlas.png", SearchOption.AllDirectories);

        for (int i = 0; i < paths.Length; i++)
        {
            SeperateRGBAandlphaChannel(paths[i],100);
            Debug.Log(string.Format("图集生成{0}/{1}",i,paths.Length));
        }
        Debug.Log("图集生成完成!");
    }

    [MenuItem("EffortForETC1/拆分选定图集")]
    public static void SeperateSelectedTextures()
    {
        Object[] paths = Selection.objects;

        for (int i = 0; i < paths.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(paths[i]);
            SeperateRGBAandlphaChannel(path,100);
            Debug.Log(string.Format("图集生成{0}/{1}", i, paths.Length));
        }
        Debug.Log("图集生成完成!");
        Debug.Log("Finish!");
    }

    [MenuItem("EffortForETC1/拆分选定PNG")]
    public static void SeperateSelectedSprite()
    {
        Object[] paths = Selection.objects;

        for (int i = 0; i < paths.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(paths[i]);
            SeperateRGBAandlphaChannel(path, 70);
            Debug.Log(string.Format("拆分选定{0}/{1}", i, paths.Length));
        }
        Debug.Log("拆分选定完成!");
        Debug.Log("Finish!");
    }

    #region inspect material

    static string[] GetMaterialTexturesHavingAlphaChannel(Material _mat)
    {
        List<string> alphatexpaths = new List<string>();
        string[] texpaths = GetMaterialTexturePaths(_mat);
        foreach (string texpath in texpaths)
        {
            if (texturesAlphaDic[texpath])
            {
                alphatexpaths.Add(texpath);
            }
        }

        return alphatexpaths.ToArray();
    }

    static string[] GetMaterialTexturePaths(Material _mat)
    {
        List<string> results = new List<string>();
        Object[] roots = new Object[] { _mat };
        Object[] dependObjs = EditorUtility.CollectDependencies(roots);
        foreach (Object dependObj in dependObjs)
        {
            if (dependObj.GetType() == typeof(Texture2D))
            {
                string texpath = AssetDatabase.GetAssetPath(dependObj.GetInstanceID());
                results.Add(texpath);
            }
        }
        return results.ToArray();
    }

    #endregion

    static void CalculateTexturesAlphaChannelDic()
    {
        string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path)) //full name
            {
                string assetRelativePath = GetRelativeAssetPath(path);
                SetTextureReadable(assetRelativePath);
                Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;
                if (!sourcetex) //make sure the file is really Texture2D which can be loaded as Texture2D.
                {
                    continue;
                }
                if (HasAlphaChannel(sourcetex))
                {
                    AddValueToDic(assetRelativePath, true);
                }
                else
                {
                    AddValueToDic(assetRelativePath, false);
                }
            }
        }
    }

    static void AddValueToDic(string _key, bool _val)
    {
        if (texturesAlphaDic.ContainsKey(_key))
        {
            texturesAlphaDic[_key] = _val;
        }
        else
        {
            texturesAlphaDic.Add(_key, _val);
        }
    }

    #region process texture
    static void SeperateRGBAandlphaChannel(string _texPath,int quality)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadable(assetRelativePath);
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D; //not just the textures under Resources file
        Debug.logger.Log(assetRelativePath);
        if (!sourcetex)
        {
            Debug.Log("Load Texture Failed : " + assetRelativePath);
            return;
        }
        if (!HasAlphaChannel(sourcetex))
        {
            Debug.Log("Texture does not have Alpha channel : " + assetRelativePath);
            //return;
        }
        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);
        Texture2D alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);
        for (int i = 0; i < sourcetex.width; ++i)
        {
            for (int j = 0; j < sourcetex.height; ++j)
            {
                Color color = sourcetex.GetPixel(i, j);
                Color rgbColor = new Color(color.r,color.g,color.b,color.a);
                Color alphaColor = new Color(color.r, color.g, color.b, color.a);
                alphaColor.r = color.a;
                alphaColor.g = color.a;
                alphaColor.b = color.a;
                if (color.a <= 0)
                {
                    rgbColor.r = 0;
                    rgbColor.g = 0;
                    rgbColor.b = 0;
                }
                rgbTex.SetPixel(i, j, rgbColor);
                alphaTex.SetPixel(i, j, alphaColor);
            }
        }
        rgbTex.Apply();
        alphaTex.Apply();
        byte[] bytes = rgbTex.EncodeToJPG(quality);
        File.WriteAllBytes(GetRGBTexPath(_texPath), bytes);
        bytes = alphaTex.EncodeToJPG(quality);
        File.WriteAllBytes(GetAlphaTexPath(_texPath), bytes);
        Debug.Log("Succeed to seperate RGB and Alpha channel for texture : " + assetRelativePath);
    }

    static bool HasAlphaChannel(Texture2D _tex)
    {
        for (int i = 0; i < _tex.width; ++i)
        {
            for (int j = 0; j < _tex.height; ++j)
            {
                Color color = _tex.GetPixel(i, j);
                float alpha = color.a;
                if (alpha < 1.0f - 0.001f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    static void SetTextureReadable(string _relativeAssetPath)
    {
        string postfix = GetFilePostfix(_relativeAssetPath);
        if (postfix == ".dds") // no need to set .dds file. Using TextureImporter to .dds file would get casting type error.
        {
            return;
        }

        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        ti.isReadable = true;
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    #endregion

    #region string or path helper

    static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".dds") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static string GetRGBTexPath(string _texPath)
    {
        _texPath = _texPath.Replace("_RGB","");
        return GetTexPath(_texPath, "_RGB.jpg");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        _texPath = _texPath.Replace("_RGB", "");
        return GetTexPath(_texPath, "_Alpha.jpg");
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string result = _texPath.Replace(".png", _texRole);
        return result;
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    static string GetFilePostfix(string _filepath) //including '.' eg ".tga", ".dds"
    {
        string postfix = "";
        int idx = _filepath.LastIndexOf('.');
        if (idx > 0 && idx < _filepath.Length)
            postfix = _filepath.Substring(idx, _filepath.Length - idx);
        return postfix;
    }

    #endregion
}