using UnityEngine;
using System.Linq;

namespace ScrewJam
{
    public class HoleSlotView : MonoBehaviour
    {
        [HideInInspector]
        public Transform[] slots;

        private ScrewView[] screws;

        private void Awake()
        {
            slots = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                slots[i] = transform.GetChild(i);
            }

            var tScrew = ResourcesManager.LoadAsset<GameObject>(Constant.SCREW_PATH);
            screws = new ScrewView[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                var obj = Instantiate(tScrew, transform);
                obj.transform.position = slots[i].position + new Vector3(0, 0, -0.1f);
                screws[i] = obj.GetComponent<ScrewView>();
                obj.SetActive(false);
            }
        }

        void Start()
        {

        }

        public void Init(HoleInfo[] holeInfos)
        {
            for (int i = 0; i < holeInfos.Length; i++)
            {
                if (!holeInfos[i].isEnabled)
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }

        public void AddHoleSlot()
        {
            var result = slots.LastOrDefault(t => !t.gameObject.activeSelf);
            if (result != null)
            {
                result.gameObject.SetActive(true);
            }
        }

        public void AddScrew(int index, Color color)
        {
            screws[index].SetColor(color);
            screws[index].gameObject.SetActive(true);
        }

        public void RemoveScrew(int index)
        {
            screws[index].gameObject.SetActive(false);
        }

        public Vector3 GetSlotPos(int i)
        {
            return slots[i].position;
        }

        public Color GetSlotColor(int i)
        {
            return screws[i].GetComponent<ScrewView>().Color;
        }
    }
}

