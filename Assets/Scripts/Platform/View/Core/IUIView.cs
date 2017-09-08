public interface IUIView
{
    void OnInit();
    void OnShow();
    void OnHide();
    void OnDestroy();
    void LoadUI();
    void OnRemove();
    void Update();
    void FixedUpdate();
}
