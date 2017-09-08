
using System;
/**
*Copyright(C) 2017 by antiphon   
*All rights reserved.             
*Author:       Alan           
*Version:      1.0          
*UnityVersion：5.5.0f3     
*Date:         2017-08-17 13:52:19             
*Description:      没用到               
*History:    
*/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : Editor {

    // Simple editor Script that lets you save a scene while in play mode.
    //简单的编辑器脚本, 可以使你在播放模式下保存场景
    // WARNING: All Undo posibilities are lost after saving the scene.
    //警告:保存场景后,所有操作是不可逆的
[MenuItem("Example/Save Scene while on play mode")]
static void EditorPlaying()
    {

        if (EditorApplication.isPlaying)
        {
            var sceneName = EditorApplication.currentScene;
            var path = sceneName.Split(char.Parse("/"));
            path[path.Length - 1] = "Temp_" + path[path.Length - 1];
            var tempScene = String.Join("/", path);

            //EditorApplication.SaveScene(tempScene);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "F:/project/91chengshi/client/Assets/Scene",true);
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = false;

            FileUtil.DeleteFileOrDirectory(EditorApplication.currentScene);
            FileUtil.MoveFileOrDirectory(tempScene, sceneName);
            FileUtil.DeleteFileOrDirectory(tempScene);

            EditorApplication.OpenScene(sceneName);
        }
    }
}