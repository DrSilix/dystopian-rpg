using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTests
{
    /*[Test]
    public void TestNodeGridEquals()
    {
        Vector2Int[] test = new Vector2Int[6];
        test[0] = new Vector2Int(0, 5);
        test[1] = new Vector2Int(3, 5);
        AStarNodeGrid aStarNodeGrid = new AStarNodeGrid(16, 9, test);
        Assert.AreEqual(aStarNodeGrid, aStarNodeGrid);
        test[2] = new Vector2Int(3, 7);
        AStarNodeGrid aStarNodeGrid2 = new AStarNodeGrid(16, 9, test);
        Assert.AreNotEqual(aStarNodeGrid, aStarNodeGrid2);
    }*/




    // A Test behaves as an ordinary method
    [Test]
    public void UnitTestsSimplePasses()
    {
        // Use the Assert class to test conditions
        Vector2Int test = new Vector2Int(1, 1);
        Assert.AreEqual(test, test);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnitTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
