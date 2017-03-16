using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OneLauncher.Framework;
using OneLauncher.Tests.Annotations;

namespace OneLauncher.Tests.Framework
{
    [TestFixture]
    public class TrulyObservableCollectionTests
    {
        [Test]
        public void ShouldRaiseEventWhenItemIsAddedRemovedOrEdited()
        {
            var eventRaised = false;

            var collection = new TrulyObservableCollection<MockNotifiable>();
            collection.CollectionChanged += (s, e) => eventRaised = true;

            eventRaised = false;
            collection.Add(new MockNotifiable());
            Assert.That(eventRaised, Is.True);

            eventRaised = false;
            // The collection is supposed to raise the event if the property of one inner element is modified
            // This is the only added feature to the genuine ObservableCollection<>
            collection.First().Property = 100;
            Assert.That(eventRaised, Is.True);

            eventRaised = false;
            collection.RemoveAt(0);
            Assert.That(eventRaised, Is.True);
        }

        private class MockNotifiable : INotifyPropertyChanged
        {
            private int _property;
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public int Property
            {
                get { return _property; }
                set
                {
                    _property = value; 
                    OnPropertyChanged();
                }
            }
        }
    }
}