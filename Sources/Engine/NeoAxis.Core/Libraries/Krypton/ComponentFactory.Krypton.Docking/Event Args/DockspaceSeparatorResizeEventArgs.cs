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
using System.Drawing;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Workspace;

namespace Internal.ComponentFactory.Krypton.Docking
{
	/// <summary>
    /// Event arguments for a DockspaceSeparatorResize event.
	/// </summary>
    public class DockspaceSeparatorResizeEventArgs : DockspaceSeparatorEventArgs
	{
		#region Instance Fields
        private Rectangle _resizeRect;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the DockspaceSeparatorResizeEventArgs class.
		/// </summary>
        /// <param name="separator">Reference to separator control instance.</param>
        /// <param name="element">Reference to dockspace docking element that is managing the separator.</param>
        /// <param name="resizeRect">Initial resizing rectangle.</param>
        public DockspaceSeparatorResizeEventArgs(KryptonSeparator separator,
                                                 KryptonDockingDockspace element,
                                                 Rectangle resizeRect)
            : base(separator, element)
		{
            _resizeRect = resizeRect;
		}
		#endregion

		#region Public
        /// <summary>
        /// Gets and sets the rectangle that limits resizing of the dockspace using the separator.
        /// </summary>
        public Rectangle ResizeRect
        {
            get { return _resizeRect; }
            set { _resizeRect = value; }
        }
        #endregion
	}
}

#endif