using Config;

namespace Game
{
    public static class DataConvert
    {
        /// <summary>
        /// 道具广告获取来源
        /// </summary>
        public static ADShowReason GetADShowReason(PropID pPropID)
        {
            return pPropID switch
            {
                PropID.Energy => ADShowReason.Video_GetEnergy,
                PropID.Coin => ADShowReason.Video_GetCoin,
                PropID.ScrewExtraSlot => ADShowReason.Video_GetScrewExtraSlot,
                PropID.ScrewHammer => ADShowReason.Video_GetScrewHammer,
                PropID.ScrewExtraBox => ADShowReason.Video_GetScrewExtraBox,
                PropID.Jam3DReplace => ADShowReason.Video_GetJam3DReplace,
                PropID.Jam3DRevert => ADShowReason.Video_GetJam3DRevert,
                PropID.Jam3DShuffle => ADShowReason.Video_GetJam3DShuffle,
                PropID.TileRecall => ADShowReason.Video_GetTileRecall,
                PropID.TileMagnet => ADShowReason.Video_GetTileMagnet,
                PropID.TileShuffle => ADShowReason.Video_GetTileShuffle,
                _ => ADShowReason.Invalid,
            };
        }

        /// <summary>
        /// 广告对应道具获取
        /// </summary>
        public static PropID GetADPropID(ADShowReason pShowReason)
        {
            return pShowReason switch
            {
                ADShowReason.Video_GetEnergy => PropID.Energy,
                ADShowReason.Video_GetCoin => PropID.Coin,
                ADShowReason.Video_GetScrewExtraSlot => PropID.ScrewExtraSlot,
                ADShowReason.Video_GetScrewHammer => PropID.ScrewHammer,
                ADShowReason.Video_GetScrewExtraBox => PropID.ScrewExtraBox,
                ADShowReason.Video_GetJam3DReplace => PropID.Jam3DReplace,
                ADShowReason.Video_GetJam3DRevert => PropID.Jam3DRevert,
                ADShowReason.Video_GetJam3DShuffle => PropID.Jam3DShuffle,
                ADShowReason.Video_GetTileRecall => PropID.TileRecall,
                ADShowReason.Video_GetTileMagnet => PropID.TileMagnet,
                ADShowReason.Video_GetTileShuffle => PropID.TileShuffle,
                _ => PropID.Invalid,
            };
        }

    }

}