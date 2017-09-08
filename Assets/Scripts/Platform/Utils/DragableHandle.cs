using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DragableHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public Transform canvas;
    //private float left;
    //private float top;
    //private float right;
    //private float bottom;
    //private RectTransform buttonRect;
    //void Awake()
    //{
    //    this.left = this.transform.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
    //    this.right = this.canvas.GetComponent<RectTransform>().sizeDelta.x - left;
    //    this.top = this.transform.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
    //    this.bottom = this.canvas.GetComponent<RectTransform>().sizeDelta.y - top;
    //    this.buttonRect = this.transform.GetComponent<RectTransform>();
    //}
    private void SetDraggedPosition(PointerEventData eventData)
    {
        ///Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.canvas, this.transform);
        RectTransform rt = this.gameObject.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            //if (globalMousePos.x <= left)
            //{
            //    globalMousePos = new Vector3(left, globalMousePos.y, 0);
            //}
            //if (globalMousePos.x >= right)
            //{
            //    globalMousePos = new Vector3(right, globalMousePos.y, 0);
            //}
            //if (globalMousePos.y <= top)
            //{
            //    globalMousePos = new Vector3(globalMousePos.x, top, 0);
            //}
            //if (globalMousePos.y >= bottom)
            //{
            //    globalMousePos = new Vector3(globalMousePos.x, bottom, 0);
            //}
            rt.position = globalMousePos;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
}
