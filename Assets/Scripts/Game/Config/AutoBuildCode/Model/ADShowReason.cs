namespace Config
{
	public enum ADShowReason
	{
		Invalid = -1,
		Video_GMCommand = 1,
		Interstitial_GMCommand = 2,
		Video_GetPropHealth = 1001,
		Video_GetPropHint = 1002,
		Video_GameRevive = 1003,
		Video_RewardDoubled = 1004,
		Video_GetScrewExtraSlot = 1111,
		Video_GetScrewHammer = 1112,
		Video_GetScrewExtraBox = 1113,
		Video_GetJam3DReplace = 1121,
		Video_GetJam3DRevert = 1122,
		Video_GetJam3DShuffle = 1123,
		Video_GetTileRecall = 1131,
		Video_GetTileMagnet = 1132,
		Video_GetTileShuffle = 1133,
		Interstitial_GameStart = 2001,
		Interstitial_GameOver = 2002,
		Interstitial_GameRetry = 2003,
		Interstitial_ReturnHome = 2004,
		Interstitial_CumulativeDuration = 2005,
		Interstitial_MiniGameStart = 2111,
		Interstitial_MiniGameOver = 2112,
		Interstitial_MiniGameRetry = 2113,
		Interstitial_MiniGameReturn = 2114,
	}
}