public class ModuleBase
{
    bool mInited = false;

    public void Init()
    {
        if (mInited)
        {
            return;
        }
        mInited = true;

        OnInit();
    }

    public void Uninit()
    {
        if (!mInited)
        {
            return;
        }
        mInited = false;

        OnUninit();
    }

    protected virtual void OnInit() { }
    protected virtual void OnUninit() { }
}