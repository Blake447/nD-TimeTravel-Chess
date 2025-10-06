using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
	public byte Id { get; set; }

	public bool isTimeTravel;

	const int LENGTH = 1 * 5;
	public static readonly byte[] memSettings = new byte[LENGTH];
	public static short SerializeSettings(StreamBuffer outStream, object settingsObj)
	{
		GameSettings settings = (GameSettings)settingsObj;
		lock (memSettings)
		{
			int timeTravelInt = settings.isTimeTravel ? 1 : 0;
			byte[] bytes = memSettings;
			int index = 0;
			Protocol.Serialize(timeTravelInt, bytes, ref index);
			outStream.Write(bytes, 0, LENGTH);
		}
		return LENGTH;
	}
	public static object DeserializeSettings(StreamBuffer inStream, short length)
	{
		GameSettings settings = new GameSettings();
		int timeTravelInt = 0;
		lock(memSettings)
		{
			inStream.Read(memSettings, 0, LENGTH);
			int index = 0;
			Protocol.Deserialize(out timeTravelInt, memSettings, ref index);
		}
		settings.isTimeTravel = timeTravelInt == 1;
		return settings;
	}


}
