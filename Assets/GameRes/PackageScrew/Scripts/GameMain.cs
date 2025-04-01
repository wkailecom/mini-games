using System.Collections;
using UnityEngine.SceneManagement;

namespace ScrewJam
{
    public class GameMain : MonoSingleton<GameMain>
    {
        public int level;

        void Start()
        {
            NextLevel();
        }

        public void NextLevel()
        {
            StartCoroutine(DelayNextLevel());
        }

        private IEnumerator DelayNextLevel()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
            var handle = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            yield return handle;
            var model = FindObjectOfType<GameModel>();
            model.Init(level);
            model.GetComponent<GameView>().Init(model.levelData, model.holeSlotInfos);
            level += 1;
        }
    }
}

