// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A class for grouping components of the scene.
	/// </summary>
	public class Layer : Component, IVisibleInHierarchy, ICanBeSelectedInHierarchy
	{
		IVisibleInHierarchy parentIVisibleInHierarchy;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the object and its children are visible.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set
			{
				if( _visible.BeginSet( ref value ) )
				{
					try
					{
						VisibleChanged?.Invoke( this );

						//OnVisibleChanged();

						//_UpdateVisibleInHierarchy( false );

						//transform = new Reference<Transform>( new Transform( visible, rotation, scale ), transform.GetByReference );
						//VisibleChanged?.Invoke( this );
						//TransformChanged?.Invoke( this );
					}
					finally { _visible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Layer> VisibleChanged;
		ReferenceField<bool> _visible = true;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				if( !Visible )
					return false;

				if( parentIVisibleInHierarchy != null )
					return parentIVisibleInHierarchy.VisibleInHierarchy;
				else
					return true;

				//if( !Visible )
				//	return false;

				//var p = Parent as IVisibleInHierarchy;
				//if( p != null )
				//	return p.VisibleInHierarchy;
				//else
				//	return true;
			}
		}

		/// <summary>
		/// Whether the object and its children are selectable in the editor.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> CanBeSelected
		{
			get { if( _canBeSelected.BeginGet() ) CanBeSelected = _canBeSelected.Get( this ); return _canBeSelected.value; }
			set { if( _canBeSelected.BeginSet( ref value ) ) { try { CanBeSelectedChanged?.Invoke( this ); } finally { _canBeSelected.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanBeSelected"/> property value changes.</summary>
		public event Action<Layer> CanBeSelectedChanged;
		ReferenceField<bool> _canBeSelected = true;

		[Browsable( false )]
		public bool CanBeSelectedInHierarchy
		{
			get
			{
				if( !CanBeSelected )
					return false;

				var p = Parent as ICanBeSelectedInHierarchy;
				if( p != null )
					return p.CanBeSelectedInHierarchy;
				else
					return true;
			}
		}

		protected override void OnAddedToParent()
		{
			parentIVisibleInHierarchy = Parent as IVisibleInHierarchy;

			base.OnAddedToParent();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			parentIVisibleInHierarchy = null;

			base.OnRemovedFromParent( oldParent );
		}
	}
}
