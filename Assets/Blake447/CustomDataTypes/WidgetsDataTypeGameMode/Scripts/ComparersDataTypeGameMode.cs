namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// Sort fields for the for the DataTypeGameMode.
	/// </summary>
	public enum ComparersFieldsDataTypeGameMode
	{
		/// <summary>
		/// Not sorted list.
		/// </summary>
		None,

		/// <summary>
		/// GameType.
		/// </summary>
		GameType,

		/// <summary>
		/// type.
		/// </summary>
		type,

		/// <summary>
		/// multiverse.
		/// </summary>
		multiverse,

		/// <summary>
		/// dimensions.
		/// </summary>
		dimensions,

		/// <summary>
		/// players.
		/// </summary>
		players,

		/// <summary>
		/// scene_index.
		/// </summary>
		scene_index,

	}

	/// <summary>
	/// Comparer functions for the DataTypeGameMode.
	/// </summary>
	public static partial class ComparersDataTypeGameMode
	{
		/// <summary>
		/// Comparer.
		/// </summary>
		public static readonly System.Collections.Generic.Dictionary<int, System.Comparison<DataTypeGameMode>> Comparers = new System.Collections.Generic.Dictionary<int, System.Comparison<DataTypeGameMode>>()
			{
				{ (int)ComparersFieldsDataTypeGameMode.GameType, GameTypeComparer },
				{ (int)ComparersFieldsDataTypeGameMode.type, typeComparer },
				{ (int)ComparersFieldsDataTypeGameMode.multiverse, multiverseComparer },
				{ (int)ComparersFieldsDataTypeGameMode.dimensions, dimensionsComparer },
				{ (int)ComparersFieldsDataTypeGameMode.players, playersComparer },
				{ (int)ComparersFieldsDataTypeGameMode.scene_index, scene_indexComparer },
			};

		#region Items comparer

		/// <summary>
		/// GameType comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int GameTypeComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.GameType, y.GameType);
		}

		/// <summary>
		/// type comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int typeComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.type, y.type);
		}

		/// <summary>
		/// multiverse comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int multiverseComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.multiverse, y.multiverse);
		}

		/// <summary>
		/// dimensions comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int dimensionsComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.dimensions, y.dimensions);
		}

		/// <summary>
		/// players comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int playersComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.players, y.players);
		}

		/// <summary>
		/// scene_index comparer.
		/// </summary>
		/// <param name="x">First DataTypeGameMode.</param>
		/// <param name="y">Second DataTypeGameMode.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int scene_indexComparer(DataTypeGameMode x, DataTypeGameMode y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.scene_index, y.scene_index);
		}
		#endregion

		#region Items comparer
		#endregion
	}
}