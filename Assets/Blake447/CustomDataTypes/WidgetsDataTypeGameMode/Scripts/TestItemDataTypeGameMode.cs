namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// Test script for the DataTypeGameMode.
	/// </summary>
	public partial class TestDataTypeGameMode : UIWidgets.WidgetGeneration.TestBase<DataTypeGameMode>
	{
		/// <summary>
		/// Generate item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override DataTypeGameMode GenerateItem(int index)
		{
			return new DataTypeGameMode()
			{
				GameType = "GameType " + index.ToString("0000"),
				type = "type " + index.ToString("0000"),
				multiverse = "multiverse " + index.ToString("0000"),
				dimensions = "dimensions " + index.ToString("0000"),
				players = UnityEngine.Random.Range(0, 100000),
				scene_index = UnityEngine.Random.Range(0, 100000),
			};
		}

		/// <summary>
		/// Generate item with the specified name.
		/// </summary>
		/// <param name="name">Item name.</param>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override DataTypeGameMode GenerateItem(string name, int index)
		{
			var item = GenerateItem(index);

			item.GameType = name;

			return item;
		}
	}
}