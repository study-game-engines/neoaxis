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
    /// Event arguments for events that need to provide a unique name.
	/// </summary>
	public class UniqueNameEventArgs : EventArgs
	{
		#region Instance Fields
        private string _uniqueName;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the UniqueNameEventArgs class.
		/// </summary>
        /// <param name="uniqueName">Unique name of page.</param>
        public UniqueNameEventArgs(string uniqueName)
		{
            _uniqueName = uniqueName;
		}
        #endregion

		#region Public
        /// <summary>
        /// Gets the unique name of a page.
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
        }
        #endregion
	}
}

#endif