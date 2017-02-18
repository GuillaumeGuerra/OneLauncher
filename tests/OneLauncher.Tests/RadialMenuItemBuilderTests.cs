using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infragistics.Controls.Menus;
using NUnit.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.Tests
{
    [TestFixture]
    public class RadialMenuItemBuilderTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldCreateEmptyListOfItemsWhenBoundToEmptyListOfLaunchers(bool nullLaunchers)
        {
            var actual = new RadialMenuItemBuilder().BuildMenuItems(nullLaunchers ? null : new LaunchersNode());
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(0));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ShouldBuildRecursiveMenuItemsWhenParsingLaunchers()
        {
            var launchers = new LaunchersNode()
            {
                SubGroups = new List<LaunchersNode>()
                {
                    new LaunchersNode()
                    {
                        Header = "Item1",
                        Launchers = new List<LauncherLink>()
                        {
                            new LauncherLink()
                            {
                                Header = "Button1"
                            }
                        },
                        SubGroups = new List<LaunchersNode>()
                        {
                            new LaunchersNode()
                            {
                                Header = "Item1-1",
                                Launchers = new List<LauncherLink>()
                                {
                                    new LauncherLink()
                                    {
                                        Header = "Button1-1"
                                    }
                                }
                            }
                        }
                    },
                    new LaunchersNode()
                    {
                        Header = "Item2",
                        Launchers = new List<LauncherLink>()
                        {
                            new LauncherLink()
                            {
                                Header = "Button2"
                            }
                        }
                    }
                }
            };
            var actual = new RadialMenuItemBuilder().BuildMenuItems(launchers).ToList();

            Assert.That(actual, Has.Count.EqualTo(2));

            var first = actual[0];
            Assert.That(first.Header, Is.EqualTo("Item1"));
            Assert.That(first.Items, Has.Count.EqualTo(2));

            var firstChild = first.Items[0] as RadialMenuItem;
            Assert.That(firstChild, Is.Not.Null);
            Assert.That(firstChild.Header, Is.EqualTo("Item1-1"));
            Assert.That(firstChild.Items, Has.Count.EqualTo(1));
            Assert.That((firstChild.Items[0] as RadialMenuItem).Header, Is.EqualTo("Button1-1"));

            var firstSubChild = first.Items[1] as RadialMenuItem;
            Assert.That(firstSubChild, Is.Not.Null);
            Assert.That(firstSubChild.Header, Is.EqualTo("Button1"));

            var second = actual[1];
            Assert.That(second.Header, Is.EqualTo("Item2"));
            Assert.That(second.Items, Has.Count.EqualTo(1));

            var secondChild = second.Items[0] as RadialMenuItem;
            Assert.That(secondChild, Is.Not.Null);
            Assert.That(secondChild.Header, Is.EqualTo("Button2"));
        }
    }
}
