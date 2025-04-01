using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewJam
{
    public class BoardView : MonoBehaviour
    {
        public int boardInfoIndex { get; set; }
        public ArraySegment<ScrewData> screwSlice { get; private set; }
        public int startIndex { get; private set; }

        public int endIndex
        {
            get { return startIndex + length - 1; }
        }

        public int length { get; private set; }
        public int layerIndex { get; private set; }

        public Color Color { get; private set; }

        private List<GameObject> holes;

        void Start()
        {
            if (screwSlice != null && screwSlice.Count > 0)
                CreateHole();
        }

        public void SetData(ArraySegment<ScrewData> slice, int layerIndex, int startIndex, int length)
        {
            this.screwSlice = slice;
            this.layerIndex = layerIndex;
            this.startIndex = startIndex;
            this.length = length;
        }

        public void SetColor(Color color)
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.color = new Color(color.r, color.g, color.b, color.a);
            this.Color = renderer.color;
            renderer.material.SetTexture("_MainTex", renderer.sprite.texture);
            renderer.material.SetInt("_StencilRef", layerIndex + 1);
            //计算归一化uv，保证效果正常
            //var UVRect = UnityEngine.Sprites.DataUtility.GetOuterUV(renderer.sprite);
            //var originRect = renderer.sprite.rect;
            //var textureRect = renderer.sprite.textureRect;
            //var scaleX = textureRect.width / originRect.width;
            //var scaleY = textureRect.height / originRect.height;
            var scaleX = renderer.sprite.texture.width / renderer.sprite.rect.width;
            var scaleY = renderer.sprite.texture.height / renderer.sprite.rect.height;
            renderer.material.SetVector("_UVScale", new Vector4(scaleX, scaleY, 0, 0));

            //传递sprite在图集中的uv信息,(左下x,左下y,右上x,右上y)
            Vector4 atlasRect = UnityEngine.Sprites.DataUtility.GetOuterUV(renderer.sprite);
            renderer.material.SetVector("_AtlasRect", atlasRect);
        }

        public void CreateHole()
        {
            var templete = ResourcesManager.LoadAsset<GameObject>(Constant.HOLE_PATH);
            holes = new List<GameObject>();
            foreach (var item in screwSlice)
            {
                var holeObj = Instantiate(templete, transform);
                holeObj.transform.localPosition = item.position;
                var holeRenderer = holeObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
                holeRenderer.material.SetInt("_StencilRef", layerIndex + 1);
                holes.Add(holeObj);
            }
        }

        public void HideHole()
        {
            foreach (var hole in holes)
            {
                hole.SetActive(false);
            }
        }

        private void Update()
        {
            if (transform.position.y < -10)
                GameObject.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Debug.Log(name + " is out of range");
        }
    }
}