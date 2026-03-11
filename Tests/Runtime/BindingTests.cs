using NUnit.Framework;
using UIX.Binding;

namespace UIX.Tests.Runtime
{
    public class BindingTests
    {
        [Test]
        public void ReactiveProperty_NotifiesOnChange()
        {
            var prop = new ReactiveProperty<int>(0);
            var notified = false;
            prop.OnChanged += _ => notified = true;
            prop.Value = 1;
            Assert.IsTrue(notified);
            Assert.AreEqual(1, prop.Value);
        }

        [Test]
        public void ReactiveCollection_Add()
        {
            var col = new ReactiveCollection<int>();
            var count = 0;
            col.OnItemAdded += (_, _) => count++;
            col.Add(1);
            Assert.AreEqual(1, count);
            Assert.AreEqual(1, col[0]);
        }
    }
}
