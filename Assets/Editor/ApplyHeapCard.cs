/**
 *Copyright(C) 2017 by antiphon   
 *All rights reserved.             
 *Author:       Alan           
 *Version:      1.0          
 *UnityVersion：5.5.0f3     
 *Date:         2017-08-17 11:47:55             
 *Description:  运行时编辑场景中的牌会将数据保存到xml中，这儿是将xml的数据赋值到场景中（停止后）                   
 *History:    v1.0
*/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;
using System;
using UnityEditor.SceneManagement;

public class ApplyHeapCard : Editor
{
    
    #region 变量
    static Transform downArea;
    static Transform dCard;
    static Transform dCardGetCard;
    static Transform dCardSendCard;
    static Transform dPengGangCard;
    static Transform dPengGangCard1;
    static Transform dPengGangCard2;
    static Transform dPutCard;
    static Transform dPutCard1;
    static Transform dPutCard2;
    static Transform dPutCard3;
    static Transform dHeapCard;
    static Transform dHeapCard1;
    static Transform dHeapCard2;
    static Transform dHeapCard3;

    static Transform rightArea;
    static Transform rCard;
    static Transform rCardGetCard;
    static Transform rCardSendCard;
    static Transform rPengGangCard;
    static Transform rPengGangCard1;
    static Transform rPengGangCard2;
    static Transform rPutCard;
    static Transform rPutCard1;
    static Transform rPutCard2;
    static Transform rPutCard3;
    static Transform rHeapCard;
    static Transform rHeapCard1;
    static Transform rHeapCard2;
    static Transform rHeapCard3;

    static Transform upArea;
    static Transform upCard;
    static Transform upCardGetCard;
    static Transform upCardSendCard;
    static Transform upPengGangCard;
    static Transform upPengGangCard1;
    static Transform upPengGangCard2;
    static Transform upPutCard;
    static Transform upPutCard1;
    static Transform upPutCard2;
    static Transform upPutCard3;
    static Transform upHeapCard;
    static Transform upHeapCard1;
    static Transform upHeapCard2;
    static Transform upHeapCard3;

    static Transform leftArea;
    static Transform lCard;
    static Transform lCardGetCard;
    static Transform lCardSendCard;
    static Transform lPengGangCard;
    static Transform lPengGangCard1;
    static Transform lPengGangCard2;
    static Transform lPutCard;
    static Transform lPutCard1;
    static Transform lPutCard2;
    static Transform lPutCard3;
    static Transform lHeapCard;
    static Transform lHeapCard1;
    static Transform lHeapCard2;
    static Transform lHeapCard3;
    static List<Transform> dlist = new List<Transform>();
    static List<Transform> rlist = new List<Transform>();
    static List<Transform> ulist = new List<Transform>();
    static List<Transform> llist = new List<Transform>();

#endregion
    static void findObj()
    {
        dlist.Clear();
        rlist.Clear();
        ulist.Clear();
        llist.Clear();

        #region 查找场景中相关的transform

        downArea = GameObject.Find("BattleMgr/DownArea").transform;
        dCard = GameObject.Find("BattleMgr/DownArea/Card").transform;
        dCardGetCard = GameObject.Find("BattleMgr/DownArea/Card/GetCard").transform;
        dCardSendCard = GameObject.Find("BattleMgr/DownArea/Card/SendCard").transform;
        dPengGangCard = GameObject.Find("BattleMgr/DownArea/PengGangCard").transform;
        dPengGangCard1 = GameObject.Find("BattleMgr/DownArea/PengGangCard/PengGangCard1").transform;
        dPengGangCard2 = GameObject.Find("BattleMgr/DownArea/PengGangCard/PengGangCard2").transform;
        dPutCard = GameObject.Find("BattleMgr/DownArea/PutCard").transform;
        dPutCard1 = GameObject.Find("BattleMgr/DownArea/PutCard/PutCard1").transform;
        dPutCard2 = GameObject.Find("BattleMgr/DownArea/PutCard/PutCard2").transform;
        dPutCard3 = GameObject.Find("BattleMgr/DownArea/PutCard/PutCard3").transform;
        dHeapCard = GameObject.Find("BattleMgr/DownArea/HeapCard").transform;
        dHeapCard1 = GameObject.Find("BattleMgr/DownArea/HeapCard/HeapCard1").transform;
        dHeapCard2 = GameObject.Find("BattleMgr/DownArea/HeapCard/HeapCard2").transform;
        dHeapCard3 = GameObject.Find("BattleMgr/DownArea/HeapCard/HeapCard3").transform;
        dlist.Add(downArea);
        dlist.Add(dCard);
        dlist.Add(dCardGetCard);
        dlist.Add(dCardSendCard);
        dlist.Add(dPengGangCard);
        dlist.Add(dPengGangCard1);
        dlist.Add(dPengGangCard2);
        dlist.Add(dPutCard);
        dlist.Add(dPutCard1);
        dlist.Add(dPutCard2);
        dlist.Add(dPutCard3);
        dlist.Add(dHeapCard);
        dlist.Add(dHeapCard1);
        dlist.Add(dHeapCard2);
        dlist.Add(dHeapCard3);

        rightArea = GameObject.Find("BattleMgr/RightArea").transform;
        rCard = GameObject.Find("BattleMgr/RightArea/Card").transform;
        rCardGetCard = GameObject.Find("BattleMgr/RightArea/Card/GetCard").transform;
        rCardSendCard = GameObject.Find("BattleMgr/RightArea/Card/SendCard").transform;
        rPengGangCard = GameObject.Find("BattleMgr/RightArea/PengGangCard").transform;
        rPengGangCard1 = GameObject.Find("BattleMgr/RightArea/PengGangCard/PengGangCard1").transform;
        rPengGangCard2 = GameObject.Find("BattleMgr/RightArea/PengGangCard/PengGangCard2").transform;
        rPutCard = GameObject.Find("BattleMgr/RightArea/PutCard").transform;
        rPutCard1 = GameObject.Find("BattleMgr/RightArea/PutCard/PutCard1").transform;
        rPutCard2 = GameObject.Find("BattleMgr/RightArea/PutCard/PutCard2").transform;
        rPutCard3 = GameObject.Find("BattleMgr/RightArea/PutCard/PutCard3").transform;
        rHeapCard = GameObject.Find("BattleMgr/RightArea/HeapCard").transform;
        rHeapCard1 = GameObject.Find("BattleMgr/RightArea/HeapCard/HeapCard1").transform;
        rHeapCard2 = GameObject.Find("BattleMgr/RightArea/HeapCard/HeapCard2").transform;
        rHeapCard3 = GameObject.Find("BattleMgr/RightArea/HeapCard/HeapCard3").transform;
        rlist.Add(rightArea);
        rlist.Add(rCard);
        rlist.Add(rCardGetCard);
        rlist.Add(rCardSendCard);
        rlist.Add(rPengGangCard);
        rlist.Add(rPengGangCard1);
        rlist.Add(rPengGangCard2);
        rlist.Add(rPutCard);
        rlist.Add(rPutCard1);
        rlist.Add(rPutCard2);
        rlist.Add(rPutCard3);
        rlist.Add(rHeapCard);
        rlist.Add(rHeapCard1);
        rlist.Add(rHeapCard2);
        rlist.Add(rHeapCard3);

        upArea = GameObject.Find("BattleMgr/UpArea").transform;
        upCard = GameObject.Find("BattleMgr/UpArea/Card").transform;
        upCardGetCard = GameObject.Find("BattleMgr/UpArea/Card/GetCard").transform;
        upCardSendCard = GameObject.Find("BattleMgr/UpArea/Card/SendCard").transform;
        upPengGangCard = GameObject.Find("BattleMgr/UpArea/PengGangCard").transform;
        upPengGangCard1 = GameObject.Find("BattleMgr/UpArea/PengGangCard/PengGangCard1").transform;
        upPengGangCard2 = GameObject.Find("BattleMgr/UpArea/PengGangCard/PengGangCard2").transform;
        upPutCard = GameObject.Find("BattleMgr/UpArea/PutCard").transform;
        upPutCard1 = GameObject.Find("BattleMgr/UpArea/PutCard/PutCard1").transform;
        upPutCard2 = GameObject.Find("BattleMgr/UpArea/PutCard/PutCard2").transform;
        upPutCard3 = GameObject.Find("BattleMgr/UpArea/PutCard/PutCard3").transform;
        upHeapCard = GameObject.Find("BattleMgr/UpArea/HeapCard").transform;
        upHeapCard1 = GameObject.Find("BattleMgr/UpArea/HeapCard/HeapCard1").transform;
        upHeapCard2 = GameObject.Find("BattleMgr/UpArea/HeapCard/HeapCard2").transform;
        upHeapCard3 = GameObject.Find("BattleMgr/UpArea/HeapCard/HeapCard3").transform;
        ulist.Add(upArea);
        ulist.Add(upCard);
        ulist.Add(upCardGetCard);
        ulist.Add(upCardSendCard);
        ulist.Add(upPengGangCard);
        ulist.Add(upPengGangCard1);
        ulist.Add(upPengGangCard2);
        ulist.Add(upPutCard);
        ulist.Add(upPutCard1);
        ulist.Add(upPutCard2);
        ulist.Add(upPutCard3);
        ulist.Add(upHeapCard);
        ulist.Add(upHeapCard1);
        ulist.Add(upHeapCard2);
        ulist.Add(upHeapCard3);

        leftArea = GameObject.Find("BattleMgr/LeftArea").transform;
        lCard = GameObject.Find("BattleMgr/LeftArea/Card").transform;
        lCardGetCard = GameObject.Find("BattleMgr/LeftArea/Card/GetCard").transform;
        lCardSendCard = GameObject.Find("BattleMgr/LeftArea/Card/SendCard").transform;
        lPengGangCard = GameObject.Find("BattleMgr/LeftArea/PengGangCard").transform;
        lPengGangCard1 = GameObject.Find("BattleMgr/LeftArea/PengGangCard/PengGangCard1").transform;
        lPengGangCard2 = GameObject.Find("BattleMgr/LeftArea/PengGangCard/PengGangCard2").transform;
        lPutCard = GameObject.Find("BattleMgr/LeftArea/PutCard").transform;
        lPutCard1 = GameObject.Find("BattleMgr/LeftArea/PutCard/PutCard1").transform;
        lPutCard2 = GameObject.Find("BattleMgr/LeftArea/PutCard/PutCard2").transform;
        lPutCard3 = GameObject.Find("BattleMgr/LeftArea/PutCard/PutCard3").transform;
        lHeapCard = GameObject.Find("BattleMgr/LeftArea/HeapCard").transform;
        lHeapCard1 = GameObject.Find("BattleMgr/LeftArea/HeapCard/HeapCard1").transform;
        lHeapCard2 = GameObject.Find("BattleMgr/LeftArea/HeapCard/HeapCard2").transform;
        lHeapCard3 = GameObject.Find("BattleMgr/LeftArea/HeapCard/HeapCard3").transform;
        llist.Add(leftArea);
        llist.Add(lCard);
        llist.Add(lCardGetCard);
        llist.Add(lCardSendCard);
        llist.Add(lPengGangCard);
        llist.Add(lPengGangCard1);
        llist.Add(lPengGangCard2);
        llist.Add(lPutCard);
        llist.Add(lPutCard1);
        llist.Add(lPutCard2);
        llist.Add(lPutCard3);
        llist.Add(lHeapCard);
        llist.Add(lHeapCard1);
        llist.Add(lHeapCard2);
        llist.Add(lHeapCard3);
        #endregion

        showXml();
    }

    static void showXml()
    {
        string filepath = Application.dataPath + @"/Card.xml";
        if (File.Exists(filepath))
        {
            List<Transform> list = new List<Transform>();
            list.AddRange(dlist);
            list.InsertRange(15,rlist);
            list.InsertRange(30, ulist);
            list.InsertRange(45, llist);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("Transform").ChildNodes;
            //遍历每一个节点，拿节点的属性以及节点的内容
            for(int i= 0;i<nodeList.Count;i++)
            {
                for (int x1 =0; x1 < nodeList[i].ChildNodes.Count;x1++)
                {
                    if (x1 == 0)
                    {
                        list[i].localPosition = StringToVector3(nodeList[i].ChildNodes[x1].InnerText);
                    }
                    if (x1==1)
                    {
                        list[i].localRotation =Quaternion.Euler(StringToVector3(nodeList[i].ChildNodes[x1].InnerText));
                    }
                    if (x1==2)
                    {
                        list[i].localScale = StringToVector3(nodeList[i].ChildNodes[x1].InnerText);
                    }
                }
            }
            Debug.Log("牌面数据重置成功！");
        }
        else
        {
            Debug.LogError("Card.xml不存在！");
        }
    }

    [MenuItem("Tools/还原牌数据")]
    static void setData()
    {
        string filepath = Application.dataPath + @"/Card.xml";
        if (!File.Exists(filepath))
        {
            Debug.LogError("Card.xml不存在！");
            return;
        }
            Scene s;
        s = SceneManager.GetActiveScene();
        if (s.name != "BattleDemo")
        {
            //Debug.LogError("please open BattleDemo Scene!");
            EditorSceneManager.OpenScene("Assets/Scene/BattleDemo.unity", OpenSceneMode.Additive);
            GameObject go = GameObject.Find("BattleMgr");
            if (go.GetComponent<SaveCardInPlayModel>() == null)
            {
                go.AddComponent<SaveCardInPlayModel>();
                EditorSceneManager.SaveScene(s);
            }
            findObj();
        }
        else
        {
            GameObject go = GameObject.Find("BattleMgr");
            if (go.GetComponent<SaveCardInPlayModel>() == null)
            {
                go.AddComponent<SaveCardInPlayModel>();
                EditorSceneManager.SaveScene(s);
            }
            findObj();
        }
    }
    static Vector3 StringToVector3(string s)
    {
        
        string[] c = s.Split(',');
        if(c.Length!=3)
        {
            Debug.LogError("解析xml的InnerText时出错！");
            return new Vector3();
        }

        Vector3 vec;
        float x;float y;float z;
        x = Convert.ToSingle(c[0]);
        y = Convert.ToSingle(c[1]);
        z = Convert.ToSingle(c[2]);
        vec = new Vector3(x,y,z);
        return vec;
    }

}