using UnityEngine;

public abstract class WindowPresenter : PanelPresenter
{
    public WindowTypes WindowType;
    public WindowPresenter Parent;
    public bool IsDynamic;

    public bool IsRootWindow
    {
        get { return Parent == null; }
    }

    public void Activate(bool value)
    {
        gameObject.SetActive(value);
    }


    public virtual void OnOrientationChanged(ScreenOrientation orientation) { }

    protected UnityEngine.UI.Button UnderlayButton
    {
        get
        {
            return Underlay.GetComponent<UnityEngine.UI.Button>();
        }
    }

    protected GameObject Underlay
    {
        get
        {
            return transform.Find("Underlay").gameObject;
        }
    }

    protected GameObject Content
    {
        get
        {
            return transform.Find("Content").gameObject;
        }
    }

    public SafeAreaOffset SafeAreaOffset { get; set; }

}

public enum WindowTypes
{
    Fullscreen,
    Popup
}