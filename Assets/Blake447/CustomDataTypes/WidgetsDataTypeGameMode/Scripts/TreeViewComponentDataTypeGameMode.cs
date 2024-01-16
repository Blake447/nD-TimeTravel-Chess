namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// TreeView component for the DataTypeGameMode.
	/// </summary>
	public partial class TreeViewComponentDataTypeGameMode : UIWidgets.TreeViewComponentBase<DataTypeGameMode>
	{
		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new UnityEngine.UI.Graphic[] { UIWidgets.UtilitiesUI.GetGraphic(GameType), UIWidgets.UtilitiesUI.GetGraphic(type), UIWidgets.UtilitiesUI.GetGraphic(multiverse), UIWidgets.UtilitiesUI.GetGraphic(dimensions), UIWidgets.UtilitiesUI.GetGraphic(players), UIWidgets.UtilitiesUI.GetGraphic(scene_index),  };
				if (!UIWidgets.UtilitiesCollections.AllNull(Foreground))
				{
					GraphicsForegroundVersion = 1;
				}
			}
		}

		/// <summary>
		/// The GameType.
		/// </summary>
		public UIWidgets.TextAdapter GameType;

		/// <summary>
		/// The type.
		/// </summary>
		public UIWidgets.TextAdapter type;

		/// <summary>
		/// The multiverse.
		/// </summary>
		public UIWidgets.TextAdapter multiverse;

		/// <summary>
		/// The dimensions.
		/// </summary>
		public UIWidgets.TextAdapter dimensions;

		/// <summary>
		/// The players.
		/// </summary>
		public UIWidgets.TextAdapter players;

		/// <summary>
		/// The scene_index.
		/// </summary>
		public UIWidgets.TextAdapter scene_index;

		DataTypeGameMode item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public DataTypeGameMode Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;

				UpdateView();
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(UIWidgets.TreeNode<DataTypeGameMode> node, int depth)
		{
			Node = node;

			base.SetData(Node, depth);

			Item = (Node == null) ? null : Node.Item;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (GameType != null)
			{
				GameType.text = Item.GameType;
			}

			if (type != null)
			{
				type.text = Item.type;
			}

			if (multiverse != null)
			{
				multiverse.text = Item.multiverse;
			}

			if (dimensions != null)
			{
				dimensions.text = Item.dimensions;
			}

			if (players != null)
			{
				players.text = Item.players.ToString();
			}

			if (scene_index != null)
			{
				scene_index.text = Item.scene_index.ToString();
			}

		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
		}
	}
}