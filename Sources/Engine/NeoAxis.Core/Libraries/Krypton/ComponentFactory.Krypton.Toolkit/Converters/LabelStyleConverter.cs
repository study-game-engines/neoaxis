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
using System.ComponentModel;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// Custom type converter so that LabelStyle values appear as neat text at design time.
    /// </summary>
    internal class LabelStyleConverter : StringLookupConverter
    {
        #region Static Fields
        private Pair[] _pairs = new Pair[] { new Pair(LabelStyle.NormalControl,     "Normal (Control)"),
                                             new Pair(LabelStyle.BoldControl,       "Bold (Control)"),
                                             new Pair(LabelStyle.ItalicControl,     "Italic (Control)"),
                                             new Pair(LabelStyle.TitleControl,      "Title (Control)"),
                                             new Pair(LabelStyle.NormalPanel,       "Normal (Panel)"),
                                             new Pair(LabelStyle.BoldPanel,         "Bold (Panel)"),
                                             new Pair(LabelStyle.ItalicPanel,       "Italic (Panel)"),
                                             new Pair(LabelStyle.TitlePanel,        "Title (Panel)"),
                                             new Pair(LabelStyle.GroupBoxCaption,   "Caption (Panel)"),
                                             new Pair(LabelStyle.ToolTip,           "ToolTip"), 
                                             new Pair(LabelStyle.SuperTip,          "SuperTip"), 
                                             new Pair(LabelStyle.KeyTip,            "KeyTip"), 
                                             new Pair(LabelStyle.Custom1,           "Custom1"), 
                                             new Pair(LabelStyle.Custom2,           "Custom2"), 
                                             new Pair(LabelStyle.Custom3,           "Custom3") };
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the LabelStyleConverter clas.
        /// </summary>
        public LabelStyleConverter()
            : base(typeof(LabelStyle))
        {
        }
        #endregion

        #region Protected
        /// <summary>
        /// Gets an array of lookup pairs.
        /// </summary>
        protected override Pair[] Pairs 
        {
            get { return _pairs; }
        }
        #endregion
    }
}

#endif