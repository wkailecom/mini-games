using UnityEngine;

namespace GameLogic
{
    public class TwinsBlocker : Blocker
    {
        public TwinsDirection direction;

        public Transform left;
        public Transform right;

        private int affectIndex;

        private SpawnItem _spawnItem;

        public int spawnOrder = -1;
        
        public override void ShowBlockerEffect()
        {
            if (spawnOrder == -1)
            {
                switch (direction)
                {
                    case TwinsDirection.Horizontal:
                        affectIndex = index + 1;
                        transform.localRotation = Quaternion.identity;
                        break;
                    case TwinsDirection.Vertical:
                        transform.localRotation = Quaternion.Euler(0,90,0);
                        affectIndex = index + JamManager.GetSingleton().JamGridMono.gridSize;
                        break;
                }
            }
            else
            {
                affectIndex = index;
            }

            transform.Find("Adhesions").gameObject.SetActive(true);
        }

        public void ClickOther()
        {
            (affectIndex, index) = (index, affectIndex);
        }

        public override bool CanMove()
        {
            return true;
        }

        public override bool ClearBlockerEffect()
        {
            if (blood == 0)
            {
                return true;
            }

            if (spawnOrder != -1)
            {
                if (_spawnItem.CurrentIndex != spawnOrder + 1)
                {
                    return true;
                }
            }

            blood = 0;
            effect.transform.position =
                left.transform.position + (right.transform.position - left.transform.position) * 0.5f;
            effect.Play();
            transform.Find("Adhesions").gameObject.SetActive(false);
            return false;
        }

        public void OtherDoPath()
        {
            if (spawnOrder != -1)
            {
                _spawnItem.Next.DoPath();
            }
            else
            {
                var affect = JamManager.GetSingleton().GetTerrainTile(affectIndex);
                var sourceItem = JamManager.GetSingleton().JamGridMono.GetTileItem(affect.tileItem);
                if(sourceItem)
                    sourceItem.DoPath();
            }
        }

        public void Update()
        {
            if (!JamManager.GetSingleton().InGame()) return;
            if (blood <= 0) return;

            if (spawnOrder != -1)
            {
                if (_spawnItem.CurrentIndex != spawnOrder) return;
                left.transform.position = _spawnItem.Current.TwinsBone.position;
                right.transform.position = _spawnItem.Next.TwinsBone.position;
            }
            else
            {
                var sourceTile = JamManager.GetSingleton().GetTerrainTile(index);
                var affectTile = JamManager.GetSingleton().GetTerrainTile(affectIndex);
                if (sourceTile.tileItem != 0)
                {
                    var sourceItem = JamManager.GetSingleton().JamGridMono.GetTileItem(sourceTile.tileItem);

                    if (sourceItem != null && sourceItem is VirusItem item)
                    {
                        left.transform.position = item.TwinsBone.position;
                    }
                }
                if (affectTile.tileItem != 0)
                {
                    var affectItem = JamManager.GetSingleton().JamGridMono.GetTileItem(affectTile.tileItem);
                    if (affectItem != null && affectItem is VirusItem item)
                    {
                        right.transform.position = item.TwinsBone.position;
                    }
                }
            }
        }
    }
}