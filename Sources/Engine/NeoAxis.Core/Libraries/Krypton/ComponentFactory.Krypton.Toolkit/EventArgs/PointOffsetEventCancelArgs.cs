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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	/// Details for an cancellable event that provides a position, offset and control value.
	/// </summary>
    public class DragStartEventCancelArgs : PointEventCancelArgs
	{
		#region Instance Fields
        private Point _offset;
        private Control _c;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the DragStartEventCancelArgs class.
		/// </summary>
        /// <param name="point">Point associated with event.</param>
        /// <param name="offset">Offset associated with event.</param>
        /// <param name="c">Control that is generating the drag start.</param>
        public DragStartEventCancelArgs(Point point, Point offset, Control c)
            : base(point)
		{
            _offset = offset;
            _c = c;
		}
		#endregion

        #region Point
        /// <summary>
		/// Gets and sets the offset.
		/// </summary>
        public Point Offset
		{
            get { return _offset; }
            set { _offset = value; }
		}
		#endregion

        #region Point
        /// <summary>
        /// Gets the control starting the drag.
        /// </summary>
        public Control Control
        {
            get { return _c; }
        }
        #endregion
    }
}

#endif