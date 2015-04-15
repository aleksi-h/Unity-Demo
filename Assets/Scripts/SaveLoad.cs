using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {
    private const string saveFile = "/savefile.save";

    public delegate void InitEvent();
    public static event InitEvent InitGameEarly;
    public static event InitEvent InitGame;

    public delegate void SaveEvent(GameState gamestate);
    public static event SaveEvent SaveState;

    public delegate void LoadEvent(GameState gamestate);
    public static event LoadEvent LoadStateEarly;
    public static event LoadEvent LoadState;

    public void Start() {
        Debug.Log(Application.persistentDataPath);
        Load();
    }

    public void Update() {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
#endif
    }

    public void OnApplicationQuit() {
        if (purge) { return; }
        Save();
    }

    private static bool purge;
    public static void PurgeSaveAndQuit() {
        if (File.Exists(Application.persistentDataPath + saveFile)) {
            File.Delete(Application.persistentDataPath + saveFile);
        }
        purge = true;
        Application.Quit();
    }

    private void Save() {
        GameState gameState = new GameState();
        if (SaveState != null) { SaveState(gameState); }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + saveFile);
        bf.Serialize(file, gameState);
        file.Close();
    }

    private void Load() {
        if (!File.Exists(Application.persistentDataPath + saveFile)) {
            AudioManager.Instance.PlayOnce(AudioManager.Instance.buildingUpgraded);
            if (InitGameEarly != null) { InitGameEarly(); }
            if (InitGame != null) { InitGame(); }
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + saveFile, FileMode.Open);
        GameState gameState = (GameState)bf.Deserialize(file);
        file.Close();

        if (VerifySave(gameState)) {
            if (LoadStateEarly != null) { LoadStateEarly(gameState); }
            if (LoadState != null) { LoadState(gameState); }
        }
        else {
            if (InitGameEarly != null) { InitGameEarly(); }
            if (InitGame != null) { InitGame(); }
        }
    }

    private bool VerifySave(GameState gamestate) {
        if (gamestate.buildingManagerState == null) { return false; }
        if (gamestate.resourceManagerState == null) { return false; }
        if (gamestate.gridState == null) { return false; }
        return true;
    }

    [System.Serializable]
    public class GameState {
        public BuildingManager.BMState buildingManagerState;
        public ResourceManager.RMState resourceManagerState;
        public Grid.GridState gridState;
    }
}
