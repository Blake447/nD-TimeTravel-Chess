namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// AutoCombobox for the DataTypeGameMode.
	/// </summary>
	public partial class AutoComboboxDataTypeGameMode : UIWidgets.AutoCombobox<DataTypeGameMode, ListViewDataTypeGameMode, ListViewComponentDataTypeGameMode, AutocompleteDataTypeGameMode, ComboboxDataTypeGameMode>
	{
		/// <inheritdoc/>
		protected override string GetStringValue(DataTypeGameMode item)
		{
			return item.GameType;
		}
	}
}