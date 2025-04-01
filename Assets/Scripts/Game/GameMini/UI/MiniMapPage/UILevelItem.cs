using System.Collections; 
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class UILevelItem : MonoBehaviour
    {
        public Image imgUnlock;
        public Image imgLock;
        public Animator unlockAnim;
        public Text txtLevel;
        public Button button;
        public Sprite[] images;

        [HideInInspector] public int levelIndex;
        [HideInInspector] public int sectionIndex;
        [HideInInspector] public ItemState itemState;
        [HideInInspector] public ItemType itemType;

        public void SetData(int pSectionIndex, int pLevelIndex, int pLevel, ItemState pState, ItemType pType)
        {
            levelIndex = pLevelIndex;
            sectionIndex = pSectionIndex;
            txtLevel.text = pLevel.ToString();
            SetState(pState);
            SetType(pType);
        }

        public void SetState(ItemState pState)
        {
            itemState = pState;
            unlockAnim.enabled = false;
            imgUnlock.transform.localScale = Vector3.one;
            imgLock.transform.localScale = Vector3.one;
            if (pState == ItemState.Unlock)
            {
                imgUnlock.gameObject.SetActive(true);
                imgLock.gameObject.SetActive(false);
            }
            else
            {
                imgLock.color = Color.white;
                imgUnlock.gameObject.SetActive(false);
                imgLock.gameObject.SetActive(true);
            }
        }

        public void SetType(ItemType pType)
        {
            itemType = pType;
            if (pType == ItemType.Normal)
            {
                imgUnlock.sprite = images[0];
                imgLock.sprite = images[1];
            }
            else if (pType == ItemType.Hard)
            {
                imgUnlock.sprite = images[2];
                imgLock.sprite = images[3];
            }
        }

        public IEnumerator PlayPassAnim(float pTime)
        {
            imgUnlock.gameObject.SetActive(true);
            unlockAnim.enabled = true;
            yield return new WaitForSeconds(pTime);
        }

        public enum ItemType
        {
            Normal,
            Hard,
            Check,
        }

        public enum ItemState
        {
            Lock,
            Unlock,
        }
    }
}