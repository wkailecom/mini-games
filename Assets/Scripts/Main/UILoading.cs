using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    private static UILoading instance;

    public static UILoading Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UILoading>();
            }
            return instance;
        }
    }

    public GameObject logo;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public Action OnLoadingComplete;

    float mCurrentProgress;
    float mTargetProgress;
    float mSpeed;

    void Awake()
    {
        instance = GetComponent<UILoading>();
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        gameObject.SetActive(true);

        SetCurrentProgress(0);
        SetTargetProgress(0);
    }


    public IEnumerator HideTask()
    {
        SetTargetProgress(1);
        while (mCurrentProgress < 1)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    void SetCurrentProgress(float pProgress)
    {
        mCurrentProgress = pProgress;
        if (mCurrentProgress > mTargetProgress)
        {
            mCurrentProgress = mTargetProgress;
        }

        if (progressBar)
        {
            progressBar.value = mCurrentProgress;
        }
        if (progressText)
        {
            progressText.gameObject.SetActive(mCurrentProgress >= 0.1f);
            progressText.text = $"{Math.Round(mCurrentProgress * 100)}%";
        }
    }

    public void SetTargetProgress(float pProgress)
    {
        mTargetProgress = pProgress;
        if (mTargetProgress > 1)
        {
            mTargetProgress = 1;
        }

        mSpeed = GetProgressSpeed();
    }

    public void AddTargetProgress(float pProgress)
    {
        SetTargetProgress(mTargetProgress + pProgress);
    }

    float GetProgressSpeed()
    {
        if (mTargetProgress == 1)
        {
            return 5f;
        }

        float tInterval = mTargetProgress - mCurrentProgress;
        if (tInterval >= 0.5f)
        {
            return 1f;
        }
        if (tInterval >= 0.3f)
        {
            return 0.7f;
        }
        if (tInterval >= 0.1f)
        {
            return 0.2f;
        }
        return 0.1f;
    }


    void Update()
    {
        if (mCurrentProgress < mTargetProgress)
        {
            SetCurrentProgress(mCurrentProgress + mSpeed * Time.deltaTime);
            if (mCurrentProgress >= 1)
            {
                SetCurrentProgress(1);
                OnLoadingComplete?.Invoke();
            }
        }
    }

}