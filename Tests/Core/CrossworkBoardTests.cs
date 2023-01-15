using Crosswork.Core;
using Crosswork.Tests.Core.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace Crosswork.Tests.Core
{
    public class CrossworkBoardTests
    {
        public static readonly Element AnyElement = null;

        public static void Assert_BoardModelContainsOnlyExpectedElements(IBoardModel model, params (int, int, Type)[] elements)
        {
            var allFoundElements = 0;
            var matchedPositions = 0;

            for (int y = 0; y < model.Height; ++y)
            {
                for (int x = 0; x < model.Width; ++x)
                {
                    if (model.TryGetCellModel(x, y, out var cellModel) && cellModel.Elements != null)
                    {
                        for (int j = 0; j < cellModel.Elements.Length; ++j)
                        {
                            allFoundElements++;

                            for (int i = 0; i < elements.Length; ++i)
                            {
                                var ex = elements[i].Item1;
                                var ey = elements[i].Item2;

                                if (ex == x && ey == y)
                                {
                                    matchedPositions++;

                                    if (elements[i].Item3 != null)
                                    {
                                        Assert.IsTrue(elements[i].Item3 == cellModel.Elements[j].GetType(), "Element model type is different from expected");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (allFoundElements != matchedPositions)
            {
                Assert.Fail("Board contains wrong set of elements");
            }
        }

        public static void AssertBoardContainsOnlyGivenElements(CrossworkBoard board, params (int, int, Type)[] elements)
        {
            var allFoundElements = 0;
            var matchedPositions = 0;

            for (int y = 0; y < board.Height; ++y)
            {
                for (int x = 0; x < board.Width; ++x)
                {
                    ref var bucket = ref board.GetBucketAt(x, y);

                    for (int k = 0; k < bucket.Count; ++k)
                    {
                        allFoundElements++;

                        for (int i = 0; i < elements.Length; ++i)
                        {
                            var ex = elements[i].Item1;
                            var ey = elements[i].Item2;

                            if (ex == x && ey == y)
                            {
                                matchedPositions++;

                                Assert.IsTrue(elements[i].Item3 == bucket.Elements[k].GetType(), "Element type is different from expected");
                            }
                        }
                    }
                }
            }

            if (allFoundElements != matchedPositions)
            {
                Assert.Fail("Board contains wrong set of elements");
            }
        }

        public static void AssertBoardContainsOnlyGivenElementInstances(CrossworkBoard board, params (int, int, Element)[] elements)
        {
            var allFoundElements = 0;
            var matchedPositions = 0;

            for (int y = 0; y < board.Height; ++y)
            {
                for (int x = 0; x < board.Width; ++x)
                {
                    ref var bucket = ref board.GetBucketAt(x, y);

                    for (int k = 0; k < bucket.Count; ++k)
                    {
                        allFoundElements++;

                        for (int i = 0; i < elements.Length; ++i)
                        {
                            var ex = elements[i].Item1;
                            var ey = elements[i].Item2;

                            if (ex == x && ey == y)
                            {
                                matchedPositions++;

                                if (elements[i].Item3 != null)
                                {
                                    Assert.IsTrue(elements[i].Item3 == bucket.Elements[k], "Element instance is different from expected");
                                }
                            }
                        }
                    }
                }
            }

            if (allFoundElements != matchedPositions)
            {
                Assert.Fail("Board contains wrong set of elements");
            }
        }

        public static void CreateBoard(int width, int height, out CrossworkBoard board, out BoardModel model)
        {
            board = new CrossworkBoard(new BoardView(), new BoardFactory());
            model = new BoardModel(width, height);
            model.FillCells();
            board.Load(model);
        }

        [Test]
        public void SavingAndLoading()
        {
            CreateBoard(9, 9, out var board, out _);

            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0));
            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 5, 5));

            var model = new BoardModel(0, 0);
            board.Save(model);

            Assert_BoardModelContainsOnlyExpectedElements(model,
                (0, 0, typeof(TestElementBigModel)),
                (5, 5, typeof(TestElementModel)));

            board.Unload();

            var newBoard = new CrossworkBoard(new BoardView(), new BoardFactory());
            newBoard.Load(model);

            AssertBoardContainsOnlyGivenElements(newBoard,
                (0, 0, typeof(TestElementBig)),
                (1, 0, typeof(SlaveElement)),
                (0, 1, typeof(SlaveElement)),
                (1, 1, typeof(SlaveElement)),
                (5, 5, typeof(TestElement)));
        }

        [Test]
        public void CreatingElements()
        {
            var board = new CrossworkBoard(new BoardView(), new BoardFactory());
            var model = new BoardModel(10, 10);
            model.FillCells();
            model.SetCell(2, 3, false);
            board.Load(model);

            Assert.IsFalse(board.TryCreateElement(new TestElementBigModel() { Width = 11, Height = 11 }, 0, 0));
            Assert.IsFalse(board.TryCreateElement(new TestElementBigModel() { Width = 5, Height = 5 }, 7, 7));
            Assert.IsFalse(board.TryCreateElement(new TestElementModel(), 2, 3));
            Assert.IsFalse(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 2, 2));
            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0, out var element));

            ref var bucket = ref board.GetBucketAt(1, 1);
            Assert.AreSame((bucket.Elements[0] as SlaveElement).Master, element);
        }

        [Test]
        public void DestroyingElements()
        {
            CreateBoard(6, 3, out var board, out _);

            var elementOutsideBoard = new TestElement(new TestElementModel());
            Assert.IsFalse(board.TryDestroyElement(elementOutsideBoard));

            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 0, 2, out var testElement1));
            Assert.IsTrue(board.TryDestroyElement(testElement1));

            AssertBoardContainsOnlyGivenElementInstances(board);

            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0, out var testElement2));

            ref var slaveBucket = ref board.GetBucketAt(1, 1);
            Assert.IsFalse(board.TryDestroyElement(slaveBucket.Elements[0]));
            Assert.IsTrue(board.TryDestroyElement(testElement2));
            Assert.AreEqual(slaveBucket.Count, 0);

            AssertBoardContainsOnlyGivenElementInstances(board);
        }

        [Test]
        public void LockingElements()
        {
            CreateBoard(10, 10, out var board, out _);

            // creating
            {
                var _lock = board.LockCell(1, 1);
                Assert.IsFalse(board.CanCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0));
                Assert.IsFalse(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0));
                board.UnlockCell(_lock);
                Assert.IsTrue(board.CanCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0));
                Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0));
            }

            // destroying
            {
                var elem = board.GetElementsAt(0, 0).First();
                var _lock = board.LockElement(elem);
                Assert.IsFalse(board.CanDestroyElement(elem));
                Assert.IsFalse(board.TryDestroyElement(elem));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.CanDestroyElement(elem));
                Assert.IsTrue(board.TryDestroyElement(elem));
            }

            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0, out TestElementBig element1));
            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 5, 5, out TestElementBig element2));

            // moving
            {
                // lock main element
                var _lock = board.LockElement(element1);
                Assert.IsFalse(board.TryMoveElement(element1, 1, 1));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.TryMoveElement(element1, 1, 1));

                // lock slave element
                var element1Slave = board.GetElementsAt(2, 2).First();
                Assert.IsNotNull(element1Slave);
                Assert.IsAssignableFrom(typeof(SlaveElement), element1Slave);
                _lock = board.LockElement(element1Slave);
                Assert.IsFalse(board.TryMoveElement(element1, 2, 2));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.TryMoveElement(element1, 2, 2));

                // lock target cell
                var _celllock = board.LockCell(1, 1);
                Assert.IsFalse(board.TryMoveElement(element1, 1, 1));
                board.UnlockCell(_celllock);
                Assert.IsTrue(board.TryMoveElement(element1, 1, 1));

                // lock source cell
                _celllock = board.LockCell(1, 1);
                Assert.IsTrue(board.TryMoveElement(element1, 2, 2));
                board.UnlockCell(_celllock);

                // lock source cell (intersecing pattern when moved)
                _celllock = board.LockCell(2, 2);
                Assert.IsFalse(board.TryMoveElement(element1, 1, 1));
                board.UnlockCell(_celllock);
                Assert.IsTrue(board.TryMoveElement(element1, 1, 1));

                // reset position back to default
                Assert.IsTrue(board.TryMoveElement(element1, 0, 0));
            }

            // swapping
            {
                // lock master element 1
                var _lock = board.LockElement(element1);
                Assert.IsFalse(board.TrySwapElements(element1, element2));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.TrySwapElements(element1, element2));
                Assert.IsTrue(board.TrySwapElements(element1, element2)); // reset

                // lock master element 2
                _lock = board.LockElement(element2);
                Assert.IsFalse(board.TrySwapElements(element1, element2));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.TrySwapElements(element1, element2));
                Assert.IsTrue(board.TrySwapElements(element1, element2)); // reset

                // lock slave element
                var element1Slave = board.GetElementsAt(1, 1).First();
                Assert.IsNotNull(element1Slave);
                Assert.IsAssignableFrom(typeof(SlaveElement), element1Slave);
                _lock = board.LockElement(element1Slave);
                Assert.IsFalse(board.TrySwapElements(element1, element2));
                board.UnlockElement(_lock);
                Assert.IsTrue(board.TrySwapElements(element1, element2));
                Assert.IsTrue(board.TrySwapElements(element1, element2)); // reset

                // lock element 1 cell
                var _celllock = board.LockCell(1, 0);
                Assert.IsFalse(board.TrySwapElements(element1, element2));
                board.UnlockCell(_celllock);
                Assert.IsTrue(board.TrySwapElements(element1, element2));
                Assert.IsTrue(board.TrySwapElements(element1, element2)); // reset

                // lock element 2 cell
                _celllock = board.LockCell(6, 5);
                Assert.IsFalse(board.TrySwapElements(element1, element2));
                board.UnlockCell(_celllock);
                Assert.IsTrue(board.TrySwapElements(element1, element2));
                Assert.IsTrue(board.TrySwapElements(element1, element2)); // reset
            }
        }

        [Test]
        public void SwappingElements()
        {
            CreateBoard(10, 10, out var board, out var model);

            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 0, 0, out var e1));
            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 1, 1, out var e2));

            Assert.IsFalse(board.TrySwapElements(e1, e1));
            Assert.IsFalse(board.TrySwapElements(e2, e2));
            Assert.IsTrue(board.TrySwapElements(e1, e2));

            AssertBoardContainsOnlyGivenElementInstances(board, (0, 0, e2), (1, 1, e1));

            Assert.IsTrue(board.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 2, 2, out var ebig1));
            var ebig1slave = board.GetElementsAt(3, 3).First();
            Assert.IsNotNull(ebig1slave);

            Assert.IsFalse(board.TrySwapElements(ebig1, e2));
            Assert.IsFalse(board.TrySwapElements(e1, ebig1slave));
            Assert.IsFalse(board.TrySwapElements(ebig1, e1));
            Assert.IsTrue(board.TryMoveElement(ebig1, 3, 3));
            Assert.IsTrue(board.TrySwapElements(ebig1, e1));
        }

        [Test]
        public void BasicElementsMoving()
        {
            var board = new CrossworkBoard(new BoardView(), new BoardFactory());
            var model = new BoardModel(3, 3);
            model.FillCells();
            model.SetCell(2, 2, false);
            board.Load(model);

            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 0, 1, out var element1));
            Assert.IsFalse(board.TryMoveElement(element1, 2, 2));
            Assert.IsTrue(board.TryMoveElement(element1, 1, 2));
            var emptyElement = board.GetElementsAt(0, 0).FirstOrDefault();
            Assert.IsTrue(emptyElement == null);
            var element1_2 = board.GetElementsAt(1, 2).First();
            Assert.AreSame(element1, element1_2);

            Assert.IsTrue(board.TryCreateElement(new TestElementModel(), 2, 0, out var element2));
            Assert.IsFalse(board.TryMoveElement(element2, 1, 2));
            Assert.IsFalse(board.TryMoveElement(element2, 3, 0));
            Assert.IsFalse(board.TryMoveElement(element2, 0, 3));
            Assert.IsFalse(board.TryMoveElement(element2, -1, 0));

            AssertBoardContainsOnlyGivenElementInstances(board, (2, 0, element2), (1, 2, element1));
        }

        [Test]
        public void BigElementsMoving()
        {
            var system = new CrossworkBoard(new BoardView(), new BoardFactory());
            var model = new BoardModel(5, 5);
            model.FillCells();
            model.SetCell(4, 4, false);
            system.Load(model);

            Assert.IsTrue(system.TryCreateElement(new TestElementBigModel() { Width = 2, Height = 2 }, 0, 0, out TestElementBig element1));

            AssertBoardContainsOnlyGivenElementInstances(system,
                (0, 0, element1),
                (1, 0, AnyElement),
                (0, 1, AnyElement),
                (1, 1, AnyElement));

            var element1Slave = system.GetElementsAt(1, 1).First();
            Assert.IsFalse(system.TryMoveElement(element1, 3, 3)); // moving where pattern contains empty cell
            Assert.IsFalse(system.TryMoveElement(element1, 4, 0)); // moving outside bounds
            Assert.IsFalse(system.TryMoveElement(element1, 0, 4)); // moving outside bounds
            Assert.IsFalse(system.TryMoveElement(element1Slave, 2, 2)); // moving slave
            Assert.IsTrue(system.TryMoveElement(element1, 1, 1));

            AssertBoardContainsOnlyGivenElementInstances(system,
                (1, 1, element1),
                (2, 1, AnyElement),
                (1, 2, AnyElement),
                (2, 2, AnyElement));

            Assert.IsTrue(system.TryCreateElement(new TestElementBigModel() { Width = 3, Height = 1 }, 0, 0, out TestElementBig element2));

            Assert.IsFalse(system.TryMoveElement(element2, 2, 4));
            Assert.IsFalse(system.TryMoveElement(element2, 0, 1));
            Assert.IsFalse(system.TryMoveElement(element2, 2, 1));
            Assert.IsFalse(system.TryMoveElement(element2, 0, 2));
            Assert.IsTrue(system.TryMoveElement(element2, 0, 3));

            AssertBoardContainsOnlyGivenElementInstances(system,
                (1, 1, element1),
                (2, 1, AnyElement),
                (1, 2, AnyElement),
                (2, 2, AnyElement),
                (0, 3, element2),
                (1, 3, AnyElement),
                (2, 3, AnyElement));
        }
    }
}
