using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveManager
{
    public static List<PlayerObjectSerializable> backupDataList = new List<PlayerObjectSerializable>();
    public static GameplayDataSerializable gameplayData = new GameplayDataSerializable();

    public static void BackUpData()
    {
        backupDataList.Clear();
        foreach(PlayerObject plO in PlayerManager.Get.players)
            backupDataList.Add(NewPlayer(plO));

        var playerData = JsonConvert.SerializeObject(backupDataList);

        File.WriteAllText(Application.persistentDataPath + "\\BackUpData.txt", playerData);

        GameplayDataSerializable gpd = new GameplayDataSerializable()
        {
            nextQuestionNumber = QuestionManager.nextQuestionIndex,
            currentRound = GameplayManager.Get.currentRound,
            roundsPlayed = GameplayManager.Get.roundsPlayed
        };

        var gameStateData = JsonConvert.SerializeObject(gpd);

        File.WriteAllText(Application.persistentDataPath + "\\GameplayData.txt", gameStateData);
    }
    public static PlayerObjectSerializable NewPlayer(PlayerObject playerObject)
    {
        PlayerObjectSerializable pl = new PlayerObjectSerializable();
        pl.playerName = playerObject.playerName;
        pl.playerClientID = playerObject.playerClientID;
        pl.twitchName = playerObject.twitchName;
        pl.inHotseat = playerObject.inHotseat;
        pl.points = playerObject.points;
        pl.lastFive = playerObject.lastFive;
        return pl;
    }

    public static void RestoreData()
    {
        backupDataList.Clear();

        if(File.Exists(Application.persistentDataPath + "\\BackUpData.txt"))
            backupDataList = JsonConvert.DeserializeObject<List<PlayerObjectSerializable>>(File.ReadAllText(Application.persistentDataPath + "\\BackUpData.txt"));
        if (File.Exists(Application.persistentDataPath + "\\GameplayData.txt"))
            gameplayData = JsonConvert.DeserializeObject<GameplayDataSerializable>(File.ReadAllText(Application.persistentDataPath + "\\GameplayData.txt"));
    }

    public static void RestorePlayer(PlayerObject po)
    {
        PlayerObjectSerializable rc = backupDataList.FirstOrDefault(x => x.playerClientID.ToLowerInvariant() == po.playerClientID.ToLowerInvariant());

        if(rc != null)
        {
            po.playerName = rc.playerName;
            po.twitchName = rc.twitchName;
            po.inHotseat = rc.inHotseat;

            po.points = rc.points;
            po.lastFive = rc.lastFive;
        }
    }

    public static void UpdateAllPlayerPodia()
    {
        foreach(PlayerObject obj in PlayerManager.Get.players)
        {
            
        }
        BackUpData();
    }

    public static void RestoreGameplayState()
    {
        if(gameplayData != null)
        {
            QuestionManager.nextQuestionIndex = gameplayData.nextQuestionNumber;
            GameplayManager.Get.currentRound = gameplayData.currentRound;
            GameplayManager.Get.roundsPlayed = gameplayData.roundsPlayed;
        }
        Operator.Get.recoveryMode = false;
        HostManager.Get.host.ReloadHost = false;
    }
}
