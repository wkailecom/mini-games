using System;

namespace Game
{
    public static class DelegateUtils
    {
        public static Action<int> OnChangeDay;
        public static Action OnChangeTokensRecord;
        public static Action OnChangeScoreRecord;
    }
}