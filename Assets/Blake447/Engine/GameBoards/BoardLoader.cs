using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BoardLoader
{
    static BoardState loaded_state = null;
    static public void ClearBoardState()
    {
        loaded_state = null;
    }

    static public void SaveGame(History history, string name)
    {
        string filePath = Application.persistentDataPath + "/game_state_" + name + ".data";
        if (File.Exists(filePath))
        {
            //Debug.LogError("save failed, " + filePath + " already exists");
            Messanger.DisplayMessage("Save failed, " + filePath + " already exists");
        }
        else
        {
            FileStream dataStream = new FileStream(filePath, FileMode.Create);
            BinaryFormatter converter = new BinaryFormatter();
            int[] serialized = (int[])history.Serialized().Clone();
            converter.Serialize(dataStream, serialized);
            dataStream.Close();
            //Debug.Log("BoardState " + name + " saved to file path " + filePath);
            Messanger.DisplayMessage("Saved to file path " + filePath);
        }
    }
    static public void SaveRecovery(int[] recovery)
    {
        string filePath = Application.persistentDataPath + "/game_state_" + "recovery" + ".data";
        FileStream dataStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, recovery);
        dataStream.Close();
    }
    static public void LoadGame(string name, Historian historian)
    {
        string filePath = Application.persistentDataPath + "/game_state_" + name + ".data";
        if (File.Exists(filePath))
        {
            Debug.Log("Streaming Assets path: " + filePath);
            FileStream dataStream = new FileStream(filePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            int[] serialized = converter.Deserialize(dataStream) as int[];
            dataStream.Close();

            History history = new History();
            history.SetHistory(serialized);
            bool pushToNetwork = true;
            historian.SetFromHistory(history, pushToNetwork);

            Messanger.DisplayMessage("BoardState " + name + " loaded from file path " + filePath);
            //return boardState;
        }
        else
        {
            Messanger.DisplayMessage("No history found with name " + name);
        }
    }


    static public void SaveCustomBoardState(BoardState boardState)
    {
#if UNITY_EDITOR
        string name = boardState.name;
        string filePath = Application.dataPath + "/StreamingAssets/saves/" + name + ".data";
        FileStream dataStream = new FileStream(filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, boardState);

        dataStream.Close();
        Messanger.DisplayMessage("BoardState " + name + " saved to file path " + filePath);
        UnityEditor.AssetDatabase.Refresh();
#else
        string name = boardState.name;
        string filePath = Application.persistentDataPath + "/" + name + ".data";
        FileStream dataStream = new FileStream(filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, boardState);

        dataStream.Close();
        Debug.Log("BoardState " + name + " saved to file path " + filePath);
        Messanger.DisplayMessage("Saved to file path " + filePath);

#endif
    }
    static public BoardState LoadCustomBoardState(string name)
    {
#if UNITY_ANDROID
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(name);
        TextAsset textAsset = Resources.Load(fileNameWithoutExtension) as TextAsset;
        Stream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter formatter = new BinaryFormatter();
        BoardState boardState = formatter.Deserialize(stream) as BoardState;
        return boardState;
#else
        string filePath = Application.dataPath + "/StreamingAssets/saves/" + name + ".data";
        if (File.Exists(filePath))
        {
            Debug.Log("Streaming Assets path: " + filePath);
            FileStream dataStream = new FileStream(filePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            BoardState boardState = converter.Deserialize(dataStream) as BoardState;

            dataStream.Close();

            loaded_state = boardState;
            Messanger.DisplayMessage("BoardState " + name + " loaded from file path " + filePath);
            return boardState;
        }
        else
        {
            Debug.Log("BoardState " + name + " failed to load from " + filePath);
            string filePathPersistant = Application.persistentDataPath + "/" + name + ".data";
            if (File.Exists(filePathPersistant))
            {
                Debug.Log("Streaming Assets path: " + filePathPersistant);
                FileStream dataStream = new FileStream(filePathPersistant, FileMode.Open);

                BinaryFormatter converter = new BinaryFormatter();
                BoardState boardState = converter.Deserialize(dataStream) as BoardState;

                dataStream.Close();

                loaded_state = boardState;
                Messanger.DisplayMessage("BoardState " + name + " loaded from file path " + filePathPersistant);
                return boardState;
            }
            else
            {
                Messanger.DisplayMessage("BoardState " + name + " failed to load from " + filePathPersistant);
                return null;
            }
        }
#endif
    }
}
