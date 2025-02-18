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
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace Internal.ComponentFactory.Krypton.Ribbon
{
    /// <summary>
    /// Manage the items that can be added to a ribbon group button cluster container.
    /// </summary>
    public class KryptonRibbonGroupClusterCollection : TypedRestrictCollection<KryptonRibbonGroupItem>
    {
        #region Static Fields
        private static readonly Type[] _types = new Type[] { typeof(KryptonRibbonGroupClusterButton),
                                                             typeof(KryptonRibbonGroupClusterColorButton)};
        #endregion

        #region Restrict
        /// <summary>
        /// Gets an array of types that the collection is restricted to contain.
        /// </summary>
        public override Type[] RestrictTypes 
        {
            get { return _types; }
        }
        #endregion
    }
}

#endif