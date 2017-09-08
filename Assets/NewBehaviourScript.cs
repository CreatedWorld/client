/**
 *Copyright(C) 2017 by Alan   
 *All rights reserved.             
 *Author:       PC-20170617HLDC           
 *Version:      1.0          
 *UnityVersion：5.5.0f3     
 *Date:         2017-09-07 11:23:49             
 *Description:                     
 *History:    
*/
using System.Collections;                   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {

    SpriteRenderer c;
	void Start () {
        c = transform.GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        c.color = new Color(1,1,1,Mathf.PingPong(Time.time, 1));
    } 
}