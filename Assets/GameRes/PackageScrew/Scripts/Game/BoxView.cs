using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScrewJam
{
    public class BoxView : MonoBehaviour
    {
        public SpriteRenderer boxSprite;

        public SpriteRenderer boxCoverSprite;

        public Transform holeRoot;

        public List<Transform> holes = new List<Transform>();

        private Color boxColor;

        private List<GameObject> screws = new List<GameObject>();

        void Start()
        {

        }

        public void SetBoxWidth(int holeCount)
        {
            var boxSize = boxSprite.size;
            var coverSize = boxCoverSprite.size;
            var holePosition = holeRoot.position;
            if (holeCount == 3)
            {
                boxSprite.size = new Vector2(3.5f, boxSize.y);
                boxCoverSprite.size = new Vector2(3.5f, coverSize.y);
                holeRoot.position = new Vector3(0, holePosition.y, holePosition.z);
            }
            else if (holeCount == 2)
            {
                boxSprite.size = new Vector2(2.5f, boxSize.y);
                boxCoverSprite.size = new Vector2(2.5f, coverSize.y);
                holeRoot.position = new Vector3(0.25f, holePosition.y, holePosition.z);
            }
            else if (holeCount == 4)
            {
                boxSprite.size = new Vector2(4.5f, boxSize.y);
                boxCoverSprite.size = new Vector2(4.5f, coverSize.y);
                holeRoot.position = new Vector3(-0.25f, holePosition.y, holePosition.z);
            }
            for (int i = 0; i < holeRoot.childCount; i++)
            {
                holeRoot.GetChild(i).gameObject.SetActive(i < holeCount);
            }
        }

        public void SetBoxColor(Color color)
        {
            boxColor = color;
            boxSprite.color = color;
            boxCoverSprite.color = color;
        }

        public Vector3 GetEmptyHolePos(int holeIndex)
        {
            return holes[holeIndex].position + new Vector3(0, 0, -0.1f);
        }

        public void AddScrew()
        {
            var tScrew = ResourcesManager.LoadAsset<GameObject>(Constant.SCREW_PATH);
            var obj = Instantiate(tScrew, holeRoot);
            obj.GetComponent<ScrewView>().SetColor(boxColor);
            obj.transform.position = holes[screws.Count].position + new Vector3(0, 0, -0.1f);
            screws.Add(obj);
        }

        public void PlayAnimation()
        {
            GetComponent<Animation>().Play("Anim");
            EventManager.Instance.triggerEvents.PlaySound?.Invoke("Module_Close");
        }
    }
}
