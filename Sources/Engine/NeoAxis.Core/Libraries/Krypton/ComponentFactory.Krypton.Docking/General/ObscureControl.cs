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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace Internal.ComponentFactory.Krypton.Docking
{
    internal class ObscureControl : Control
    {
        #region Protected
        /// <summary>
        /// Raises the PaintBackground event.
        /// </summary>
        /// <param name="e">An PaintEventArgs containing the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // We do nothing, so the area underneath shows through
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">An PaintEventArgs containing the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // We do nothing, so the area underneath shows through
        }
        #endregion
    }
}

#endif