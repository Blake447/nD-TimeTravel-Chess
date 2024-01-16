namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// ListView for the DataTypeGameMode.
	/// </summary>
	public partial class ListViewDataTypeGameMode : UIWidgets.ListViewCustom<ListViewComponentDataTypeGameMode, DataTypeGameMode>
	{
		ComparersFieldsDataTypeGameMode currentSortField = ComparersFieldsDataTypeGameMode.None;

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(ComparersFieldsDataTypeGameMode field)
		{
			if (field == currentSortField)
			{
				DataSource.Reverse();
			}
			else if (ComparersDataTypeGameMode.Comparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(ComparersDataTypeGameMode.Comparers[(int)field]);
			}
		}

		#region used in Button.OnClick()

		/// <summary>
		/// Sort by GameType.
		/// </summary>
		public void SortByGameType()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.GameType);
		}

		/// <summary>
		/// Sort by type.
		/// </summary>
		public void SortBytype()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.type);
		}

		/// <summary>
		/// Sort by multiverse.
		/// </summary>
		public void SortBymultiverse()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.multiverse);
		}

		/// <summary>
		/// Sort by dimensions.
		/// </summary>
		public void SortBydimensions()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.dimensions);
		}

		/// <summary>
		/// Sort by players.
		/// </summary>
		public void SortByplayers()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.players);
		}

		/// <summary>
		/// Sort by scene_index.
		/// </summary>
		public void SortByscene_index()
		{
			ToggleSort(ComparersFieldsDataTypeGameMode.scene_index);
		}
		#endregion
	}
}