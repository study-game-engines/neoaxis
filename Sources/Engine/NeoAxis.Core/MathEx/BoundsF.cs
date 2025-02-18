// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<BoundsF> ) )]
	/// <summary>
	/// Represents a single precision axis-aligned bounding box in three dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct BoundsF
	{
		public Vector3F Minimum;
		public Vector3F Maximum;

		public static readonly BoundsF Zero = new BoundsF( Vector3F.Zero, Vector3F.Zero );
		public static readonly BoundsF Cleared = new BoundsF(
			new Vector3F( float.MaxValue, float.MaxValue, float.MaxValue ),
			new Vector3F( float.MinValue, float.MinValue, float.MinValue ) );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF( BoundsF source )
		{
			Minimum = source.Minimum;
			Maximum = source.Maximum;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF( Vector3F minimum, Vector3F maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF( float minimumX, float minimumY, float minimumZ, float maximumX, float maximumY, float maximumZ )
		{
			this.Minimum = new Vector3F( minimumX, minimumY, minimumZ );
			this.Maximum = new Vector3F( maximumX, maximumY, maximumZ );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF( Vector3F v )
		{
			this.Minimum = v;
			this.Maximum = v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is BoundsF && this == (BoundsF)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( BoundsF v1, BoundsF v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( BoundsF v1, BoundsF v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe Vector3F this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index == 0 )
					return Minimum;
				else if( index == 1 )
					return Maximum;
				else
					throw new ArgumentOutOfRangeException( "index" );

				//if( index < 0 || index > 1 )
				//	throw new ArgumentOutOfRangeException( "index" );
				//fixed( Vector3F* v = &this.Minimum )
				//{
				//	return v[ index ];
				//}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index == 0 )
					Minimum = value;
				else if( index == 1 )
					Maximum = value;
				else
					throw new ArgumentOutOfRangeException( "index" );

				//if( index < 0 || index > 1 )
				//	throw new ArgumentOutOfRangeException( "index" );
				//fixed( Vector3F* v = &this.Minimum )
				//{
				//	v[ index ] = value;
				//}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( BoundsF v, float epsilon )
		{
			if( !Minimum.Equals( ref v.Minimum, epsilon ) )
				return false;
			if( !Maximum.Equals( ref v.Maximum, epsilon ) )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsCleared()
		{
			return Minimum.X > Maximum.X;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetCenter()
		{
			Vector3F result;
			result.X = ( Minimum.X + Maximum.X ) * .5f;
			result.Y = ( Minimum.Y + Maximum.Y ) * .5f;
			result.Z = ( Minimum.Z + Maximum.Z ) * .5f;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetCenter( out Vector3F result )
		{
			result.X = ( Minimum.X + Maximum.X ) * .5f;
			result.Y = ( Minimum.Y + Maximum.Y ) * .5f;
			result.Z = ( Minimum.Z + Maximum.Z ) * .5f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetRadius()
		{
			float total = 0.0f;
			for( int i = 0; i < 3; i++ )
			{
				float b0 = Math.Abs( Minimum[ i ] );
				float b1 = Math.Abs( Maximum[ i ] );
				if( b0 > b1 )
					total += b0 * b0;
				else
					total += b1 * b1;
			}
			return MathEx.Sqrt( total );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetRadius( Vector3F center )
		{
			float total = 0.0f;
			for( int i = 0; i < 3; i++ )
			{
				float b0 = Math.Abs( center[ i ] - Minimum[ i ] );
				float b1 = Math.Abs( Maximum[ i ] - center[ i ] );
				if( b0 > b1 )
					total += b0 * b0;
				else
					total += b1 * b1;
			}
			return MathEx.Sqrt( total );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphereF GetBoundingSphere()
		{
			//!!!!slowly

			var c = GetCenter();
			return new SphereF( c, GetRadius( c ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetVolume()
		{
			Vector3F s;
			Vector3F.Subtract( ref Maximum, ref Minimum, out s );
			return s.X * s.Y * s.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( Vector3F v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( ref Vector3F v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( BoundsF v )
		{
			if( v.Minimum.X < Minimum.X )
				Minimum.X = v.Minimum.X;
			if( v.Minimum.Y < Minimum.Y )
				Minimum.Y = v.Minimum.Y;
			if( v.Minimum.Z < Minimum.Z )
				Minimum.Z = v.Minimum.Z;
			if( v.Maximum.X > Maximum.X )
				Maximum.X = v.Maximum.X;
			if( v.Maximum.Y > Maximum.Y )
				Maximum.Y = v.Maximum.Y;
			if( v.Maximum.Z > Maximum.Z )
				Maximum.Z = v.Maximum.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( ref BoundsF v )
		{
			if( v.Minimum.X < Minimum.X )
				Minimum.X = v.Minimum.X;
			if( v.Minimum.Y < Minimum.Y )
				Minimum.Y = v.Minimum.Y;
			if( v.Minimum.Z < Minimum.Z )
				Minimum.Z = v.Minimum.Z;
			if( v.Maximum.X > Maximum.X )
				Maximum.X = v.Maximum.X;
			if( v.Maximum.Y > Maximum.Y )
				Maximum.Y = v.Maximum.Y;
			if( v.Maximum.Z > Maximum.Z )
				Maximum.Z = v.Maximum.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( ICollection<Vector3F> collection )
		{
			foreach( var v in collection )
				Add( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( Vector3F[] collection )
		{
			for( int i = 0; i < collection.Length; i++ )
				Add( ref collection[ i ] );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF Intersection( BoundsF v )
		{
			BoundsF result;
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Intersection( ref BoundsF v, out BoundsF result )
		{
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Expand( float d )
		{
			Minimum.X -= d;
			Minimum.Y -= d;
			Minimum.Z -= d;
			Maximum.X += d;
			Maximum.Y += d;
			Maximum.Z += d;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Expand( Vector3F d )
		{
			Minimum.X -= d.X;
			Minimum.Y -= d.Y;
			Minimum.Z -= d.Z;
			Maximum.X += d.X;
			Maximum.Y += d.Y;
			Maximum.Z += d.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Vector3F p )
		{
			if( p.X < Minimum.X || p.Y < Minimum.Y || p.Z < Minimum.Z || p.X > Maximum.X || p.Y > Maximum.Y || p.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Vector3F p )
		{
			if( p.X < Minimum.X || p.Y < Minimum.Y || p.Z < Minimum.Z || p.X > Maximum.X || p.Y > Maximum.Y || p.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( BoundsF v )
		{
			if( v.Minimum.X < Minimum.X || v.Minimum.Y < Minimum.Y || v.Minimum.Z < Minimum.Z
				|| v.Maximum.X > Maximum.X || v.Maximum.Y > Maximum.Y || v.Maximum.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref BoundsF v )
		{
			if( v.Minimum.X < Minimum.X || v.Minimum.Y < Minimum.Y || v.Minimum.Z < Minimum.Z
				|| v.Maximum.X > Maximum.X || v.Maximum.Y > Maximum.Y || v.Maximum.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( BoundsF v )
		{
			if( v.Maximum.X < Minimum.X || v.Maximum.Y < Minimum.Y || v.Maximum.Z < Minimum.Z
				|| v.Minimum.X > Maximum.X || v.Minimum.Y > Maximum.Y || v.Minimum.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref BoundsF v )
		{
			if( v.Maximum.X < Minimum.X || v.Maximum.Y < Minimum.Y || v.Maximum.Z < Minimum.Z
				|| v.Minimum.X > Maximum.X || v.Minimum.Y > Maximum.Y || v.Minimum.Z > Maximum.Z )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( SphereF v )
		{
			return v.Intersects( ref this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref SphereF v )
		{
			return v.Intersects( ref this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetSize()
		{
			Vector3F result;
			Vector3F.Subtract( ref Maximum, ref Minimum, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetSize( out Vector3F result )
		{
			Vector3F.Subtract( ref Maximum, ref Minimum, out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F[] ToPoints()
		{
			Vector3F[] r = null;
			ToPoints( ref r );
			return r;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToPoints( ref Vector3F[] points )
		{
			if( points == null || points.Length < 8 )
				points = new Vector3F[ 8 ];
			for( int i = 0; i < 8; i++ )
			{
				//slowly
				points[ i ].X = this[ ( i ^ ( i >> 1 ) ) & 1 ].X;
				points[ i ].Y = this[ ( i >> 1 ) & 1 ].Y;
				points[ i ].Z = this[ ( i >> 2 ) & 1 ].Z;
			}
		}

		[AutoConvertType]
		public static BoundsF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 6 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 6 parts separated by spaces in the form (x y z x y z).", text ) );

			try
			{
				return new BoundsF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ),
					float.Parse( vals[ 4 ] ),
					float.Parse( vals[ 5 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", Minimum, Maximum );
			//return string.Format( "{0} {1} {2} {3} {4} {5}",
			//	Minimum.X, Minimum.Y, Minimum.Z,
			//	Maximum.X, Maximum.Y, Maximum.Z );
		}

		public string ToString( int precision )
		{
			return string.Format( "{0} {1}", Minimum.ToString( precision ), Maximum.ToString( precision ) );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Intersects( ref RayF ray, out float scale )
		{
			int i, ax0, ax1, ax2, side, inside;
			float f;
			Vector3F hit = Vector3F.Zero;

			scale = 0.0f;

			ax0 = -1;
			inside = 0;
			for( i = 0; i < 3; i++ )
			{
				if( ray.Origin[ i ] < Minimum[ i ] )
					side = 0;
				else if( ray.Origin[ i ] > Maximum[ i ] )
					side = 1;
				else
				{
					inside++;
					continue;
				}
				if( ray.Direction[ i ] == 0.0f )
					continue;
				f = ( ray.Origin[ i ] - this[ side ][ i ] );
				if( ax0 < 0 || Math.Abs( f ) > Math.Abs( scale * ray.Direction[ i ] ) )
				{
					scale = -( f / ray.Direction[ i ] );
					ax0 = i;
				}
			}

			if( scale < 0 || scale > 1 )
				return false;

			if( ax0 < 0 )
			{
				scale = 0.0f;
				return ( inside == 3 );
			}

			ax1 = ( ax0 + 1 ) % 3;
			ax2 = ( ax0 + 2 ) % 3;
			hit[ ax1 ] = ray.Origin[ ax1 ] + scale * ray.Direction[ ax1 ];
			hit[ ax2 ] = ray.Origin[ ax2 ] + scale * ray.Direction[ ax2 ];

			return ( hit[ ax1 ] >= Minimum[ ax1 ] && hit[ ax1 ] <= Maximum[ ax1 ] &&
						hit[ ax2 ] >= Minimum[ ax2 ] && hit[ ax2 ] <= Maximum[ ax2 ] );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( RayF ray, out float scale )
		{
			return Intersects( ref ray, out scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref RayF ray )
		{
			float s;
			return Intersects( ref ray, out s );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( RayF ray )
		{
			float s;
			return Intersects( ref ray, out s );
		}

		//public static Bounds FromTransformedBounds( Bounds bounds, Vec3 origin, Mat3 axis )
		//{
		//   Vec3 center = ( bounds.minimum + bounds.maximum ) * 0.5f;
		//   Vec3 extents = bounds.maximum - center;

		//   Vec3 rotatedExtents = new Vec3(
		//      Math.Abs( extents.x * axis.mat0.x ) +
		//      Math.Abs( extents.y * axis.mat1.x ) +
		//      Math.Abs( extents.z * axis.mat2.x ),

		//      Math.Abs( extents.x * axis.mat0.y ) +
		//      Math.Abs( extents.y * axis.mat1.y ) +
		//      Math.Abs( extents.z * axis.mat2.y ),

		//      Math.Abs( extents.x * axis.mat0.z ) +
		//      Math.Abs( extents.y * axis.mat1.z ) +
		//      Math.Abs( extents.z * axis.mat2.z ) );

		//   //for( int i = 0; i < 3; i++ )
		//   //{
		//   //   rotatedExtents[ i ] =
		//   //      Math.Abs( extents.x * axis.mat0[ i ] ) +
		//   //      Math.Abs( extents.y * axis.mat1[ i ] ) +
		//   //      Math.Abs( extents.z * axis.mat2[ i ] );
		//   //}

		//   center = origin + center * axis;

		//   return new Bounds( center - rotatedExtents, center + rotatedExtents );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF operator +( BoundsF b, Vector3F v )
		{
			BoundsF result;
			Vector3F.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Add( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF operator +( Vector3F v, BoundsF b )
		{
			BoundsF result;
			Vector3F.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Add( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF operator -( BoundsF b, Vector3F v )
		{
			BoundsF result;
			Vector3F.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Subtract( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF operator -( Vector3F v, BoundsF b )
		{
			BoundsF result;
			Vector3F.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Subtract( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref BoundsF b, ref Vector3F v, out BoundsF result )
		{
			Vector3F.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Add( ref b.Maximum, ref v, out result.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Vector3F v, ref BoundsF b, out BoundsF result )
		{
			Vector3F.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Add( ref v, ref b.Maximum, out result.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref BoundsF b, ref Vector3F v, out BoundsF result )
		{
			Vector3F.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Subtract( ref b.Maximum, ref v, out result.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Vector3F v, ref BoundsF b, out BoundsF result )
		{
			Vector3F.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Subtract( ref v, ref b.Maximum, out result.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF Add( ref BoundsF b, ref Vector3F v )
		{
			BoundsF result;
			Vector3F.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Add( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF Add( ref Vector3F v, ref BoundsF b )
		{
			BoundsF result;
			Vector3F.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Add( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF Subtract( ref BoundsF b, ref Vector3F v )
		{
			BoundsF result;
			Vector3F.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3F.Subtract( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF Subtract( ref Vector3F v, ref BoundsF b )
		{
			BoundsF result;
			Vector3F.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3F.Subtract( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF.Side GetPlaneSide( ref PlaneF plane )
		{
			Vector3F center;
			GetCenter( out center );

			float d1 = plane.GetDistance( ref center );
			float d2 = Math.Abs( ( Maximum.X - center.X ) * plane.A ) +
				Math.Abs( ( Maximum.Y - center.Y ) * plane.B ) +
				Math.Abs( ( Maximum.Z - center.Z ) * plane.C );

			if( d1 - d2 > 0 )
				return PlaneF.Side.Positive;
			if( d1 + d2 < 0 )
				return PlaneF.Side.Negative;
			return PlaneF.Side.No;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF.Side GetPlaneSide( PlaneF plane )
		{
			return GetPlaneSide( ref plane );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetPlaneDistance( ref PlaneF plane )
		{
			Vector3F center;
			GetCenter( out center );

			float d1 = plane.GetDistance( ref center );
			float d2 = Math.Abs( ( Maximum.X - center.X ) * plane.Normal.X ) +
				Math.Abs( ( Maximum.Y - center.Y ) * plane.Normal.Y ) +
				Math.Abs( ( Maximum.Z - center.Z ) * plane.Normal.Z );

			if( d1 - d2 > 0.0f )
				return d1 - d2;
			if( d1 + d2 < 0.0f )
				return d1 + d2;
			return 0.0f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetPlaneDistance( PlaneF plane )
		{
			return GetPlaneDistance( ref plane );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		internal bool LineIntersection( ref Vector3F start, ref Vector3F end )
		{
			//unsafe
			{
				Vector3F center;
				GetCenter( out center );

				Vector3F extents;
				Vector3F.Subtract( ref Maximum, ref center, out extents );
				//Vec3 extents = Maximum - center;

				Vector3F lineDir;
				lineDir.X = 0.5f * ( end.X - start.X );
				lineDir.Y = 0.5f * ( end.Y - start.Y );
				lineDir.Z = 0.5f * ( end.Z - start.Z );
				//Vec3 lineDir = 0.5f * ( end - start );

				Vector3F lineCenter;
				Vector3F.Add( ref start, ref lineDir, out lineCenter );
				//Vec3 lineCenter = start + lineDir;

				Vector3F dir;
				Vector3F.Subtract( ref lineCenter, ref center, out dir );
				//Vec3 dir = lineCenter - center;

				float ld0, ld1, ld2;

				ld0 = Math.Abs( lineDir.X );
				if( Math.Abs( dir.X ) > extents.X + ld0 )
					return false;
				ld1 = Math.Abs( lineDir.Y );
				if( Math.Abs( dir.Y ) > extents.Y + ld1 )
					return false;
				ld2 = Math.Abs( lineDir.Z );
				if( Math.Abs( dir.Z ) > extents.Z + ld2 )
					return false;

				Vector3F cross;
				Vector3F.Cross( ref lineDir, ref dir, out cross );

				if( Math.Abs( cross.X ) > extents.Y * ld2 + extents.Z * ld1 )
					return false;
				if( Math.Abs( cross.Y ) > extents.X * ld2 + extents.Z * ld0 )
					return false;
				if( Math.Abs( cross.Z ) > extents.X * ld1 + extents.Y * ld0 )
					return false;
				return true;
			}
		}

		//!!!!
		//public RectF ToRect()
		//{
		//	RectF result;
		//	result.left = minimum.X;
		//	result.top = minimum.Y;
		//	result.right = maximum.X;
		//	result.bottom = maximum.Y;
		//	return result;
		//}

		//!!!!
		//public void ToRect( out RectF result )
		//{
		//	result.left = minimum.X;
		//	result.top = minimum.Y;
		//	result.right = maximum.X;
		//	result.bottom = maximum.Y;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetPointDistanceSquared( ref Vector3F point )
		{
			float x;
			if( point.X < Minimum.X )
				x = Minimum.X - point.X;
			else if( point.X > Maximum.X )
				x = point.X - Maximum.X;
			else
				x = 0;

			float y;
			if( point.Y < Minimum.Y )
				y = Minimum.Y - point.Y;
			else if( point.Y > Maximum.Y )
				y = point.Y - Maximum.Y;
			else
				y = 0;

			float z;
			if( point.Z < Minimum.Z )
				z = Minimum.Z - point.Z;
			else if( point.Z > Maximum.Z )
				z = point.Z - Maximum.Z;
			else
				z = 0;

			float sqr = x * x + y * y + z * z;
			return sqr;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetPointDistanceSquared( Vector3F point )
		{
			return GetPointDistanceSquared( ref point );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetPointDistance( Vector3F point )
		{
			float sqr = GetPointDistanceSquared( point );
			if( sqr == 0 )
				return 0;
			return MathEx.Sqrt( sqr );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Bounds ToBounds()
		{
			Bounds result;
			result.Minimum.X = Minimum.X;
			result.Minimum.Y = Minimum.Y;
			result.Minimum.Z = Minimum.Z;
			result.Maximum.X = Maximum.X;
			result.Maximum.Y = Maximum.Y;
			result.Maximum.Z = Maximum.Z;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public BoundsI ToBoundsI()
		{
			BoundsI result;
			result.Minimum.X = (int)Minimum.X;
			result.Minimum.Y = (int)Minimum.Y;
			result.Minimum.Z = (int)Minimum.Z;
			result.Maximum.X = (int)Maximum.X;
			result.Maximum.Y = (int)Maximum.Y;
			result.Maximum.Z = (int)Maximum.Z;
			return result;
		}

		//!!!!!����� �����
		//public static implicit operator BoundsD( Bounds b )
		//{
		//	return b.ToBoundsD();
		//}

#if !DISABLE_IMPLICIT
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Bounds( BoundsF v )
		{
			return new Bounds( v );
		}
#endif

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static BoundsF Merge( BoundsF a, BoundsF b )
		{
			var v = a;
			v.Add( b );
			return v;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Intersects( ref TriangleF triangle )
		{
			//https://github.com/juj/MathGeoLib/blob/master/src/Geometry/Triangle.cpp#L697

			var a = triangle.A;
			var b = triangle.B;
			var c = triangle.C;

			var triangleBounds = new BoundsF( a );
			triangleBounds.Add( ref b );
			triangleBounds.Add( ref c );
			var tMin = triangleBounds.Minimum;
			var tMax = triangleBounds.Maximum;
			//var tMin = a.Min( b.Min( c ) );
			//var tMax = a.Max( b.Max( c ) );

			if( tMin.X >= Maximum.X || tMax.X <= Minimum.X || tMin.Y >= Maximum.Y || tMax.Y <= Minimum.Y || tMin.Z >= Maximum.Z || tMax.Z <= Minimum.Z )
				return false;

			var center = ( Minimum + Maximum ) * 0.5f;
			var h = Maximum - center;

			var t0 = b - a;
			var t1 = c - a;
			var t2 = c - b;

			var ac = a - center;

			var n = Vector3F.Cross( t0, t1 );
			var s = n.Dot( ac );
			var r = Math.Abs( h.Dot( Vector3F.Abs( n ) ) );
			if( Math.Abs( s ) >= r )
				return false;

			var at0 = Vector3F.Abs( t0 );
			var at1 = Vector3F.Abs( t1 );
			var at2 = Vector3F.Abs( t2 );

			var bc = b - center;
			var cc = c - center;

			// eX <cross> t[0]
			var d1 = t0.Y * ac.Z - t0.Z * ac.Y;
			var d2 = t0.Y * cc.Z - t0.Z * cc.Y;
			var tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at0.Z + h.Z * at0.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eX <cross> t[1]
			d1 = t1.Y * ac.Z - t1.Z * ac.Y;
			d2 = t1.Y * bc.Z - t1.Z * bc.Y;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at1.Z + h.Z * at1.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eX <cross> t[2]
			d1 = t2.Y * ac.Z - t2.Z * ac.Y;
			d2 = t2.Y * bc.Z - t2.Z * bc.Y;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at2.Z + h.Z * at2.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eY <cross> t[0]
			d1 = t0.Z * ac.X - t0.X * ac.Z;
			d2 = t0.Z * cc.X - t0.X * cc.Z;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.X * at0.Z + h.Z * at0.X );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eY <cross> t[1]
			d1 = t1.Z * ac.X - t1.X * ac.Z;
			d2 = t1.Z * bc.X - t1.X * bc.Z;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.X * at1.Z + h.Z * at1.X );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eY <cross> t[2]
			d1 = t2.Z * ac.X - t2.X * ac.Z;
			d2 = t2.Z * bc.X - t2.X * bc.Z;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.X * at2.Z + h.Z * at2.X );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eZ <cross> t[0]
			d1 = t0.X * ac.Y - t0.Y * ac.X;
			d2 = t0.X * cc.Y - t0.Y * cc.X;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at0.X + h.X * at0.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eZ <cross> t[1]
			d1 = t1.X * ac.Y - t1.Y * ac.X;
			d2 = t1.X * bc.Y - t1.Y * bc.X;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at1.X + h.X * at1.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// eZ <cross> t[2]
			d1 = t2.X * ac.Y - t2.Y * ac.X;
			d2 = t2.X * bc.Y - t2.Y * bc.X;
			tc = ( d1 + d2 ) * 0.5f;
			r = Math.Abs( h.Y * at2.X + h.X * at2.Y );
			if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
				return false;

			// No separating axis exists, the AABB and triangle intersect.
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( TriangleF triangle )
		{
			return Intersects( ref triangle );
		}

	}
}
