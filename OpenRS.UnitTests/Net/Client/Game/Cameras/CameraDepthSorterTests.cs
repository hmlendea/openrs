using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraDepthSorterTests
    {
        private CameraDepthSorter depthSorter = null!;

        [SetUp]
        public void SetUp()
        {
            depthSorter = new CameraDepthSorter();
        }

        [Test]
        public void GivenUnsortedModels_WhenSortingByDepth_ThenTheirScalesAreDescending()
        {
            CameraModel[] models = BuildModels(4, -8, 42, 0, 16, int.MaxValue, int.MinValue);

            depthSorter.SortByDepth(models, 0, models.Length - 1);

            Assert.That(
                GetScales(models),
                Is.EqualTo(new[] { int.MaxValue, 42, 16, 4, 0, -8, int.MinValue }));
        }

        [Test]
        public void GivenDuplicateScales_WhenSortingByDepth_ThenEveryModelIsRetained()
        {
            CameraModel[] models = BuildModels(42, 4, 42, 8, 4, 42);

            depthSorter.SortByDepth(models, 0, models.Length - 1);

            Assert.That(GetScales(models), Is.EqualTo(new[] { 42, 42, 42, 8, 4, 4 }));
        }

        [Test]
        public void GivenAPartialRange_WhenSortingByDepth_ThenModelsOutsideTheRangeRemainInPlace()
        {
            CameraModel firstModel = BuildModel(4);
            CameraModel lastModel = BuildModel(8);
            CameraModel[] models = [firstModel, .. BuildModels(16, 42, 32), lastModel];

            depthSorter.SortByDepth(models, 1, 3);

            Assert.That(models[0], Is.SameAs(firstModel));
            Assert.That(models[^1], Is.SameAs(lastModel));
            Assert.That(GetScales(models), Is.EqualTo(new[] { 4, 42, 32, 16, 8 }));
        }

        [Test]
        public void GivenAnEmptyRange_WhenSortingByDepth_ThenNoArrayAccessOccurs()
            => Assert.That(
                () => depthSorter.SortByDepth([], 0, -1),
                Throws.Nothing);

        [Test]
        public void GivenASingleModel_WhenSortingByDepth_ThenItsIdentityIsPreserved()
        {
            CameraModel expectedModel = BuildModel(42);
            CameraModel[] models = [expectedModel];

            depthSorter.SortByDepth(models, 0, 0);

            Assert.That(models[0], Is.SameAs(expectedModel));
        }

        [Test]
        public void GivenNoModels_WhenResolvingRenderOrder_ThenNoArrayAccessOccurs()
            => Assert.That(
                () => depthSorter.ResolveRenderOrder(4, [], 0),
                Throws.Nothing);

        [Test]
        public void GivenAnExactlyFullDisjointModelArray_WhenResolvingRenderOrder_ThenEveryModelIsSorted()
        {
            CameraModel[] models =
            [
                BuildDisjointModel(0),
                BuildDisjointModel(42),
                BuildDisjointModel(96),
            ];

            depthSorter.ResolveRenderOrder(4, models, models.Length);

            Assert.That(models, Has.All.Property(nameof(CameraModel.IsSorted)).True);
            Assert.That(models.Select(model => model.SortIndex), Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(models.Select(model => model.DependencyIndex), Has.All.EqualTo(-1));
        }

        [Test]
        public void GivenZeroLookAhead_WhenResolvingRenderOrder_ThenEveryModelIsInitialisedAndSorted()
        {
            CameraModel[] models =
            [
                BuildDisjointModel(0),
                BuildDisjointModel(42),
                BuildDisjointModel(96),
            ];
            Array.ForEach(models, model => model.DependencyIndex = 42);

            depthSorter.ResolveRenderOrder(0, models, models.Length);

            Assert.That(models, Has.All.Property(nameof(CameraModel.IsSorted)).True);
            Assert.That(models.Select(model => model.SortIndex), Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(models.Select(model => model.DependencyIndex), Has.All.EqualTo(-1));
        }

        [Test]
        public void GivenAPrefixModelCount_WhenResolvingRenderOrder_ThenTheSuffixStateIsUnchanged()
        {
            CameraModel suffixModel = BuildDisjointModel(96);
            suffixModel.IsSorted = false;
            suffixModel.SortIndex = 42;
            suffixModel.DependencyIndex = 64;
            CameraModel[] models =
            [
                BuildDisjointModel(0),
                BuildDisjointModel(42),
                suffixModel,
            ];

            depthSorter.ResolveRenderOrder(8, models, 2);

            Assert.That(models.Take(2), Has.All.Property(nameof(CameraModel.IsSorted)).True);
            Assert.That(suffixModel.IsSorted, Is.False);
            Assert.That(suffixModel.SortIndex, Is.EqualTo(42));
            Assert.That(suffixModel.DependencyIndex, Is.EqualTo(64));
        }

        [Test]
        public void GivenANullArrayAndNoModels_WhenResolvingRenderOrder_ThenNoArrayAccessOccurs()
            => Assert.That(
                () => depthSorter.ResolveRenderOrder(8, null!, 0),
                Throws.Nothing);

        [Test]
        public void GivenANullArrayWithAnActiveRange_WhenSortingByDepth_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => depthSorter.SortByDepth(null!, 0, 1),
                Throws.TypeOf<NullReferenceException>());

        private static CameraModel BuildDisjointModel(int minimumPositionX) => new()
        {
            BoundsMinX = minimumPositionX,
            BoundsMaxX = minimumPositionX + 8,
            BoundsMinY = 0,
            BoundsMaxY = 8,
        };

        private static CameraModel BuildModel(int scale) => new()
        {
            Scale = scale,
        };

        private static CameraModel[] BuildModels(params int[] scales)
            => scales.Select(BuildModel).ToArray();

        private static int[] GetScales(CameraModel[] models)
            => models.Select(model => model.Scale).ToArray();
    }
}