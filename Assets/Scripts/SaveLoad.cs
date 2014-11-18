using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {
    private const string saveFile = "/savefile.save";

    public delegate void FirstLaunch();
    public static event FirstLaunch InitGameEarly;
    public static event FirstLaunch InitGame;

    public delegate void SaveEvent(State gamestate);
    public static event SaveEvent SaveState;

    public delegate void LoadEvent(State gamestate);
    public static event LoadEvent LoadStateEarly;
    public static event LoadEvent LoadState;

    void Start() {
        Load();
    }

    void Update() {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Backbutton pressed, Quitting...");
            Application.Quit();
        }
#endif
    }

    void OnApplicationQuit() {
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
        State gameState = new State();
        if (SaveState != null) { SaveState(gameState); }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + saveFile);
        bf.Serialize(file, gameState);
        file.Close();
    }
    //TODO delete save nappi testitarkoituksiin
    private void Load() {
        if (!File.Exists(Application.persistentDataPath + saveFile)) {
            if (InitGameEarly != null) { InitGameEarly(); }
            if (InitGame != null) { InitGame(); }
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + saveFile, FileMode.Open);
        State gameState = (State)bf.Deserialize(file);
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

    private bool VerifySave(State gamestate) {
        if (gamestate.buildingManagerState == null) { return false; }
        if (gamestate.resourceManagerState == null) { return false; }
        if (gamestate.gridState == null) { return false; }
        return true;
    }

    [System.Serializable]
    public class State {
        public BuildingManager.BMState buildingManagerState;
        public ResourceManager.RMState resourceManagerState;
        public Grid.GridState gridState;
    }
}
