using Assets.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemAccess
{
    private GameData items;
    public TestItemAccess(string name) {
        var fileStream = File.OpenRead("Assets\\Data\\GameData.gdjs"); // or .json
        items = new GameData(fileStream, new Formatters.GameDataLoadOptions { Format = Formatters.GameDataFormat.Json });
        fileStream.Dispose();
    }

    public GameData GetGameData() { return items; }
}
