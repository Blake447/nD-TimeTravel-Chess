namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// Autocomplete for the DataTypeGameMode.
	/// </summary>
	public partial class AutocompleteDataTypeGameMode : UIWidgets.AutocompleteCustom<DataTypeGameMode, ListViewComponentDataTypeGameMode, ListViewDataTypeGameMode>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(DataTypeGameMode value)
		{
			return UIWidgets.UtilitiesCompare.StartsWith(value.GameType, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(DataTypeGameMode value)
		{
			return UIWidgets.UtilitiesCompare.Contains(value.GameType, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(DataTypeGameMode value)
		{
			return value.GameType;
		}
	}
}