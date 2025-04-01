using System.Collections;
using UnityEngine;

namespace Game.UISystem
{
    public class AnimationUI : MonoBehaviour
    {
        const string ANIMATION_SHOW_NAME = "Show";
        const string ANIMATION_HIDE_NAME = "Hide";

        Animator mAnimator;
        float mEnterTime;
        float mLeaveTime;
        bool mShow;

        public IEnumerator Show(bool pPlayAnimation)
        {
            if (mShow)
            {
                pPlayAnimation = false; // 防止重复播放动画
            }

            mShow = true;
            gameObject.SetActive(true);

            OnBeginShow(pPlayAnimation);
            if (pPlayAnimation)
            {
                yield return PlayEnterAnimation();
            }
            else if (HaveAnimationClipper(true))
            {
                mAnimator.Play(ANIMATION_SHOW_NAME, -1, 1);
            }
            OnShowed();
        }

        public IEnumerator Hide(bool pPlayAnimation)
        {
            if (!mShow)
            {
                pPlayAnimation = false; // 防止重复播放动画
            }

            mShow = false;

            OnBeginHide(pPlayAnimation);
            if (pPlayAnimation)
            {
                yield return PlayLeaveAnimation();
            }
            OnHided();
        }

        protected virtual void OnBeginShow(bool pPlayAnimation) { }

        protected virtual void OnShowed() { }

        protected virtual void OnBeginHide(bool pPlayAnimation) { }

        protected virtual void OnHided() { }

        protected virtual IEnumerator PlayEnterAnimation()
        {
            if (!HaveAnimationClipper(true))
            {
                yield break;
            }

            mAnimator.Play(ANIMATION_SHOW_NAME);
            yield return new WaitForSeconds(mEnterTime);
        }

        protected virtual IEnumerator PlayLeaveAnimation()
        {
            if (!HaveAnimationClipper(false))
            {
                yield break;
            }

            mAnimator.Play(ANIMATION_HIDE_NAME);
            yield return new WaitForSeconds(mLeaveTime);
        }

        bool HaveAnimationClipper(bool pEnter)
        {
            if (mAnimator == null)
            {
                Transform tPanel = transform.Find("Panel");
                if (tPanel == null)
                {
                    return false;
                }

                mAnimator = tPanel.GetComponent<Animator>();
                if (mAnimator == null)
                {
                    return false;
                }
            }

            AnimationClip[] tAnimationClips = GetAnimationClip(mAnimator);
            if (tAnimationClips == null || tAnimationClips.Length == 0)
            {
               // LogManager.Log(name + " animator controller doesn't contain animation clips.");
                return false;
            }

            for (int i = 0; i < tAnimationClips.Length; i++)
            {
                if (pEnter && tAnimationClips[i].name.Equals(ANIMATION_SHOW_NAME))
                {
                    mEnterTime = tAnimationClips[i].length;
                    return mAnimator != null;
                }
                else if (!pEnter && tAnimationClips[i].name.Equals(ANIMATION_HIDE_NAME))
                {
                    mLeaveTime = tAnimationClips[i].length;
                    return mAnimator != null;
                }
            }

            return false;
        }

        public float GetAnimationTime(bool pEnter)
        {
            if (!HaveAnimationClipper(pEnter))
            {
                return 0;
            }

            return pEnter ? mEnterTime : mLeaveTime;
        }

        AnimationClip[] GetAnimationClip(Animator pAnimator)
        {
            if (pAnimator == null)
            {
                return null;
            }

            return pAnimator.runtimeAnimatorController.animationClips;
        }
    }
}