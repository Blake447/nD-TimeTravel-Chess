namespace UIWidgets.Custom.DataTypeGameModeNS
{
	/// <summary>
	/// ListView component for the DataTypeGameMode.
	/// </summary>
	public partial class ListViewComponentDataTypeGameMode : UIWidgets.ListViewItem, UIWidgets.IViewData<DataTypeGameMode>
	{
		/// <inheritdoc/>
		protected override void GraphicsBackgroundInit()
		{
			base.GraphicsBackgroundInit();

			if (CellsBackgroundVersion == 0)
			{
				var result = new System.Collections.Generic.List<UnityEngine.UI.Graphic>();

				foreach (UnityEngine.Transform child in transform)
				{
					var graphic = child.GetComponent<UnityEngine.UI.Graphic>();
					if (graphic != null)
					{
						result.Add(graphic);
					}
				}

				if (result.Count > 0)
				{
					cellsGraphicsBackground = result.ToArray();

					CellsBackgroundVersion = 1;
				}
			}
		}

		/// <inheritdoc/>
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

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public DataTypeGameMode Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Tooltip.
		/// </summary>
		[UnityEngine.SerializeField]
		protected TooltipDataTypeGameMode tooltip;

		/// <summary>
		/// Tooltip.
		/// </summary>
		public TooltipDataTypeGameMode Tooltip
		{
			get
			{
				return tooltip;
			}

			set
			{
				if (tooltip != null)
				{
					Tooltip.Unregister(gameObject);
				}

				tooltip = value;

				if ((tooltip != null) && (Item != null))
				{
					Tooltip.Register(gameObject, Item, TooltipSettings);
				}
			}
		}

		/// <summary>
		/// Tooltip settings.
		/// </summary>
		[UnityEngine.SerializeField]
		protected UIWidgets.TooltipSettings TooltipSettings = new UIWidgets.TooltipSettings(UIWidgets.TooltipPosition.TopCenter);

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(DataTypeGameMode item)
		{
			Item = item;

			#if UNITY_EDITOR
			name = Item == null ? "DefaultItem " + Index.ToString() : Item.GameType;
			#endif

			if ((Tooltip != null) && (Item != null))
			{
				Tooltip.Register(gameObject, Item, TooltipSettings);
			}

			UpdateView();
		}

		/// <inheritdoc/>
		public override void LocaleChanged()
		{
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected void UpdateView()
		{
			if (Item == null)
			{
				return;
			}

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

		/// <inheritdoc/>
		public override void MovedToCache()
		{
			if (Tooltip != null)
			{
				Tooltip.Unregister(gameObject);
			}
		}
	}
}