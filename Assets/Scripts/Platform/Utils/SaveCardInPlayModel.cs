/**
 *Copyright(C) 2017 by antiphon   
 *All rights reserved.             
 *Author:       Alan           
 *Version:      1.0          
 *UnityVersion：5.5.0f3     
 *Date:         2017-08-16 16:13:09             
 *Description:  保存运行时调整牌堆的数据在xml里面，停止运行后可在菜单tool->setHeapCard恢复成运行时的数据                  
 *History:    v1.0
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class SaveCardInPlayModel : MonoBehaviour
{
#if UNITY_EDITOR

    #region 场景物体变量

    private Transform downArea;
    private Transform dCard;
    private Transform dCardGetCard;
    private Transform dCardSendCard;
    private Transform dPengGangCard;
    private Transform dPengGangCard1;
    private Transform dPengGangCard2;
    private Transform dPutCard;
    private Transform dPutCard1;
    private Transform dPutCard2;
    private Transform dPutCard3;
    private Transform dHeapCard;
    private Transform dHeapCard1;
    private Transform dHeapCard2;
    private Transform dHeapCard3;

    private Transform rightArea;
    private Transform rCard;
    private Transform rCardGetCard;
    private Transform rCardSendCard;
    private Transform rPengGangCard;
    private Transform rPengGangCard1;
    private Transform rPengGangCard2;
    private Transform rPutCard;
    private Transform rPutCard1;
    private Transform rPutCard2;
    private Transform rPutCard3;
    private Transform rHeapCard;
    private Transform rHeapCard1;
    private Transform rHeapCard2;
    private Transform rHeapCard3;

    private Transform upArea;
    private Transform upCard;
    private Transform upCardGetCard;
    private Transform upCardSendCard;
    private Transform upPengGangCard;
    private Transform upPengGangCard1;
    private Transform upPengGangCard2;
    private Transform upPutCard;
    private Transform upPutCard1;
    private Transform upPutCard2;
    private Transform upPutCard3;
    private Transform upHeapCard;
    private Transform upHeapCard1;
    private Transform upHeapCard2;
    private Transform upHeapCard3;

    private Transform leftArea;
    private Transform lCard;
    private Transform lCardGetCard;
    private Transform lCardSendCard;
    private Transform lPengGangCard;
    private Transform lPengGangCard1;
    private Transform lPengGangCard2;
    private Transform lPutCard;
    private Transform lPutCard1;
    private Transform lPutCard2;
    private Transform lPutCard3;
    private Transform lHeapCard;
    private Transform lHeapCard1;
    private Transform lHeapCard2;
    private Transform lHeapCard3;

#endregion

    bool openTest;
    string tips;
    GUIStyle style;
    List<Transform> dlist = new List<Transform>();
    List<Transform> rlist = new List<Transform>();
    List<Transform> ulist = new List<Transform>();
    List<Transform> llist = new List<Transform>();
    string path;
    void Start()
    {

        path = Application.dataPath + "/Card.xml";
        style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.yellow;

        FindTransformInBattleDemo();
        tips = "按T调试，可保存牌数据...";
    }

    void Update()
    {

        if (openTest)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                openTest = false;
                tips = "按T进入调试模式，可保存牌的数据...";
            }
            if (Input.GetKeyDown(KeyCode.S))
            { 
                
                SaveDataInXML();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                openTest = true;
                tips = "调试模式已打开，按S场景的数据将被保存";
            }
        }
    }
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2.0f-150, 30, 400, 200), tips,style);
    }

    void InitXML()
    {
        if (!File.Exists(path))
        {
            CreateXML();
        }
        else
        {
            File.Delete(path);
            CreateXML();
        }
    }

    void CreateXML()
    {
        string area = "DownArea";
        string childArea = "d";
        Debug.Log(path);
        //创建最上一层的节点。
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("Transform"); ;
        for (int i = 0; i < 4; i++)
        {

            if (i == 0) { area = "DownArea"; childArea = "d"; }
            if (i == 1) { area = "RightArea"; childArea = "r"; }
            if (i == 2) { area = "UpArea"; childArea = "u"; }
            if (i == 3) { area = "LeftArea"; childArea = "l"; }
            List<XmlElement> arealist = new List<XmlElement>();

            XmlElement areaItem = xml.CreateElement(area); arealist.Add(areaItem);
            XmlElement card = xml.CreateElement(childArea+"Card"); arealist.Add(card);
            XmlElement getCard = xml.CreateElement(childArea + "Card_GetCard"); arealist.Add(getCard);
            XmlElement sendCard = xml.CreateElement(childArea + "Card_SendCard"); arealist.Add(sendCard);

            XmlElement pengGangCard = xml.CreateElement(childArea + "PengGangCard"); arealist.Add(pengGangCard);
            XmlElement pengGangCard1 = xml.CreateElement(childArea + "PengGangCard_PengGangCard1"); arealist.Add(pengGangCard1);
            XmlElement pengGangCard2 = xml.CreateElement(childArea + "PengGangCard_PengGangCard2"); arealist.Add(pengGangCard2);

            XmlElement putCard = xml.CreateElement(childArea + "PutCard"); arealist.Add(putCard);
            XmlElement putCard1 = xml.CreateElement(childArea + "PutCard_PutCard1"); arealist.Add(putCard1);
            XmlElement putCard2 = xml.CreateElement(childArea + "PutCard_PutCard2"); arealist.Add(putCard2);
            XmlElement putCard3 = xml.CreateElement(childArea + "PutCard_PutCard3"); arealist.Add(putCard3);

            XmlElement heapCard = xml.CreateElement(childArea + "HeapCard"); arealist.Add(heapCard);
            XmlElement heapCard1 = xml.CreateElement(childArea + "HeapCard_HeapCard1"); arealist.Add(heapCard1);
            XmlElement heapCard2 = xml.CreateElement(childArea + "HeapCard_HeapCard2"); arealist.Add(heapCard2);
            XmlElement heapCard3 = xml.CreateElement(childArea + "HeapCard_HeapCard3"); arealist.Add(heapCard3);

            List<XmlElement> plist = new List<XmlElement>();
            List<XmlElement> rlist = new List<XmlElement>();
            List<XmlElement> slist = new List<XmlElement>();

            for (int j = 0; j < 15; j++)
            {
                XmlElement position = xml.CreateElement("position");
                XmlElement rotation = xml.CreateElement("rotation");
                XmlElement scale = xml.CreateElement("scale");
                plist.Add(position);
                rlist.Add(rotation);
                slist.Add(scale);
            }
            for (int k = 0; k < 15; k++)
            {
                if (i==0)
                {
                    plist[k].InnerText = dlist[k].localPosition.x+","+ dlist[k].localPosition.y+","+ dlist[k].localPosition.z;
                    rlist[k].InnerText = (dlist[k].localRotation.eulerAngles.x)+","+
                        (dlist[k].localRotation.eulerAngles.y)+","+
                        (dlist[k].localRotation.eulerAngles.z);
                    slist[k].InnerText = dlist[k].localScale.x+","+ dlist[k].localScale.y + "," + dlist[k].localScale.z; 
                }else if (i==1)
                {
                    plist[k].InnerText = this.rlist[k].localPosition.x + "," + this.rlist[k].localPosition.y + ","+ this.rlist[k].localPosition.z;
                    rlist[k].InnerText = (this.rlist[k].localRotation.eulerAngles.x) + "," +
                        (this.rlist[k].localRotation.eulerAngles.y) + "," + 
                        (this.rlist[k].localRotation.eulerAngles.z);
                    slist[k].InnerText = this.rlist[k].localScale.x + "," + this.rlist[k].localScale.y + "," + this.rlist[k].localScale.z;
                }
                else if (i==2)
                {
                    plist[k].InnerText = ulist[k].localPosition.x + "," + ulist[k].localPosition.y + "," + ulist[k].localPosition.z;
                    rlist[k].InnerText = (ulist[k].localRotation.eulerAngles.x) + "," + 
                        (ulist[k].localRotation.eulerAngles.y) + "," + 
                        (ulist[k].localRotation.eulerAngles.z);
                    slist[k].InnerText = ulist[k].localScale.x + "," + ulist[k].localScale.y + "," + ulist[k].localScale.z;
                }
                else
                {
                    plist[k].InnerText = llist[k].localPosition.x + "," + llist[k].localPosition.y + "," + llist[k].localPosition.z;
                    rlist[k].InnerText = (llist[k].localRotation.eulerAngles.x )+ "," + 
                        (llist[k].localRotation.eulerAngles.y) + "," + 
                        (llist[k].localRotation.eulerAngles.z);
                    slist[k].InnerText = llist[k].localScale.x + "," + llist[k].localScale.y + "," + llist[k].localScale.z;
                }
            }


            for (int f = 0; f < 15; f++)
            {
                arealist[f].AppendChild(plist[f]);//添加position
                arealist[f].AppendChild(rlist[f]);//添加rotation
                arealist[f].AppendChild(slist[f]);//添加scale
            }
            
            //把节点一层一层的添加至xml中，注意他们之间的先后顺序，这是生成XML文件的顺序
            for (int m = 0; m < arealist.Count; m++)
            {
                root.AppendChild(arealist[m]);//添加所有保存对象的gameobject transform
            }
            
            xml.AppendChild(root);
        }
        //最后保存文件
        xml.Save(path);
        Debug.Log("保存xml成功！");

    }

    void SaveDataInXML()
    {
        InitXML();
    }
    void FindTransformInBattleDemo()
    {
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
        //Debug.Log("dlist[1].localRotation = " + dlist[1].localRotation);

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

    }

#endif
}