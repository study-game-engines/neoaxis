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
using System.IO;
using System.Xml;
using System.Drawing;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Navigator;

namespace Internal.ComponentFactory.Krypton.Docking
{
	/// <summary>
    /// Event data for loading docking page configuration.
	/// </summary>
    public class DockPageLoadingEventArgs : DockGlobalLoadingEventArgs
	{
		#region Instance Fields
        private KryptonPage _page;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the DockPageLoadingEventArgs class.
		/// </summary>
        /// <param name="manager">Reference to owning docking manager instance.</param>
        /// <param name="xmlReading">Xml reader for persisting custom data.</param>
        /// <param name="page">Reference to page being loaded.</param>
        public DockPageLoadingEventArgs(KryptonDockingManager manager,
                                        XmlReader xmlReading,
                                        KryptonPage page)
            : base(manager, xmlReading)
		{
            _page = page;
		}
		#endregion

		#region Public
		/// <summary>
        /// Gets the loading page reference.
		/// </summary>
        public KryptonPage Page
		{
            get { return _page; }
            set { _page = value; }
		}
        #endregion
	}
}

#endif