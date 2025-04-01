// 一个Animation中包括UI的进入、循环、退出动画
// 在Prefab中填写enterFinishFrame，leaveStartFrame,即进入动画的结束帧和退出动画的开始帧，2个参数之间的帧数为循环动画帧
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationClipper : MonoBehaviour
{
    public int enterFinishFrame;
    public int leaveStartFrame;

    bool mInited;
    Animation mAnimation;
    AnimationState mAnimationState;
    AnimPlayState mPlayState;

    float mEnterFinishTime;
    float mLeaveStartTime;

    void Init()
    {
        mAnimation = GetComponent<Animation>();
        if (mAnimation.clip == null)
        {
           // LogManager.LogError("No animation clip in : " + gameObject.name);
            return;
        }
        mAnimationState = mAnimation[mAnimation.clip.name];

        mAnimation.Stop();
        mPlayState = AnimPlayState.Pause;
        mEnterFinishTime = enterFinishFrame / mAnimation.clip.frameRate;
        mLeaveStartTime = leaveStartFrame / mAnimation.clip.frameRate;

        mInited = true;
    }

    bool CheckInit()
    {
        if (!mInited)
        {
            Init();
        }

        return mInited;
    }

    public void PlayEnterAnimation()
    {
        if (!CheckInit())
        {
            return;
        }

        PauseAnimationAtTime(0);
        mAnimationState.speed = 1;
        mPlayState = AnimPlayState.PlayingEnterAnim;
    }

    public void PlayLeaveAnimation()
    {
        if (!CheckInit())
        {
            return;
        }

        PauseAnimationAtTime(mLeaveStartTime);
        mAnimationState.speed = 1;
        mPlayState = AnimPlayState.PlayingLeaveAnim;
    }

    void PauseAnimationAtTime(float pPauseTime)
    {
        mAnimation.Play();
        mAnimationState.speed = 0;
        mAnimationState.time = pPauseTime;
        mPlayState = AnimPlayState.Pause;
    }

    public void SetAsStartState()
    {
        if (!CheckInit())
        {
            return;
        }

        PauseAnimationAtTime(0);
    }

    public void SetAsPauseState()
    {
        if (!CheckInit())
        {
            return;
        }

        PauseAnimationAtTime(mEnterFinishTime);
    }

    public void SetAsEndState()
    {
        if (!CheckInit())
        {
            return;
        }

        PauseAnimationAtTime(mAnimation.clip.length);
    }

    void Update()
    {
        if (!mInited)
        {
            return;
        }

        if (mPlayState == AnimPlayState.PlayingEnterAnim)
        {
            if (mAnimationState.time + Time.deltaTime >= mEnterFinishTime)
            {
                if (enterFinishFrame < leaveStartFrame)
                {
                    mPlayState = AnimPlayState.PlayingLoopAnim;
                }
                else
                {
                    SetAsPauseState();
                }
            }
        }
        else if (mPlayState == AnimPlayState.PlayingLoopAnim)
        {
            if (mAnimationState.time + Time.deltaTime >= mLeaveStartTime)
            {
                mAnimationState.time = mEnterFinishTime;
            }
        }
    }

    public float EnterTime
    {
        get
        {
            CheckInit();
            return Mathf.Max(0, mEnterFinishTime);
        }
    }

    public float LeaveTime
    {
        get
        {
            CheckInit();
            return Mathf.Max(0, mAnimation.clip.length - mLeaveStartTime);
        }
    }

    enum AnimPlayState
    {
        Pause,
        PlayingEnterAnim,
        PlayingLeaveAnim,
        PlayingLoopAnim,
    }
}