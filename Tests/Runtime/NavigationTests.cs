using NUnit.Framework;
using UnityEngine;
using UIX.Navigation;

namespace UIX.Tests.Runtime
{
    public class NavigationTests
    {
        [Test]
        public void TransitionType_HasExpectedValues()
        {
            Assert.AreEqual(0, (int)TransitionType.None);
            Assert.AreEqual(1, (int)TransitionType.Fade);
            Assert.AreEqual(2, (int)TransitionType.SlideLeft);
            Assert.AreEqual(3, (int)TransitionType.SlideRight);
        }

        [Test]
        public void UIXNavigator_DefaultTransition_IsFade()
        {
            var previous = UIXNavigator.DefaultTransition;
            try
            {
                UIXNavigator.DefaultTransition = TransitionType.Fade;
                Assert.AreEqual(TransitionType.Fade, UIXNavigator.DefaultTransition);
            }
            finally
            {
                UIXNavigator.DefaultTransition = previous;
            }
        }

        [Test]
        public void UIXNavigator_DefaultTransitionDuration_IsPositive()
        {
            var previous = UIXNavigator.DefaultTransitionDuration;
            try
            {
                UIXNavigator.DefaultTransitionDuration = 0.3f;
                Assert.Greater(UIXNavigator.DefaultTransitionDuration, 0);
            }
            finally
            {
                UIXNavigator.DefaultTransitionDuration = previous;
            }
        }
    }
}
