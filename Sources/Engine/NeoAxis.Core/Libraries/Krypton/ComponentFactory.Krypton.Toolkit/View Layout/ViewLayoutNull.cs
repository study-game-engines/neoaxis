#if !DEPLOY
// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 17/267 Nepean Hwy, 
//  Seaford, Vic 3198, Australia and are supplied subject to licence terms.
// 
//
// *****************************************************************************

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	/// View element that draws nothing and just takes up the provided size.
	/// </summary>
	public class ViewLayoutNull : ViewLeaf
	{
		#region Identity
		/// <summary>
        /// Initialize a new instance of the ViewLayoutNull class.
		/// </summary>
		public ViewLayoutNull()
		{
		}

		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <returns>User readable name of the instance.</returns>
		public override string ToString()
		{
			// Return the class name and instance identifier
            return "ViewLayoutNull:" + Id;
		}
		#endregion

		#region Layout
		/// <summary>
		/// Discover the preferred size of the element.
		/// </summary>
		/// <param name="context">Layout context.</param>
		public override Size GetPreferredSize(ViewLayoutContext context)
		{
			Debug.Assert(context != null);

			// We are a null leaf, so have no size
			return Size.Empty;
		}

		/// <summary>
		/// Perform a layout of the elements.
		/// </summary>
		/// <param name="context">Layout context.</param>
		public override void Layout(ViewLayoutContext context)
		{
			Debug.Assert(context != null);

            // Validate incoming reference
            if (context == null) throw new ArgumentNullException("context");

			// We take on all the available display area
			ClientRectangle = context.DisplayRectangle;
		}
		#endregion
	}
}

#endif