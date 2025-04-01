using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening.RickExtension
{
    public static class DOTweenExteion
    {
        public static Tweener DOFadeImage(this Image image, float endValue, float duration)
        {
            return DOTween.To(image.AlphaGetter, image.AlphaSetter, endValue, duration);
        }
        private static float AlphaGetter(this Image image)
        {
            return image.color.a;
        }

        private static void AlphaSetter(this Image image, float alpha)
        {
            Color oldColor = image.color;
            oldColor.a = alpha;
            image.color = oldColor;
        }
    }
}