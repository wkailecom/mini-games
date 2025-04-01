using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public Button undo;

    public Button replace;

    public Button replay;

    public Button shuffle;
    public Button back;
    // Start is called before the first frame update
    void Start()
    {
        undo.onClick.AddListener(delegate
        {
            JamManager.GetSingleton().Undo();
        });
        replace.onClick.AddListener(delegate
        {
            JamManager.GetSingleton().Replace();
        });
        replay.onClick.AddListener(delegate
        {
            JamManager.GetSingleton().Replay();
        });
        shuffle.onClick.AddListener(delegate
        {
            JamManager.GetSingleton().Shuffle();
        });
        back.onClick.AddListener(delegate
        {
            JamManager.GetSingleton().UnloadScene();
            //ActivitySceneManager.Instance.UnloadSceneAsync(ActivitySceneName.Jam3d);
        });
    }
    
}
