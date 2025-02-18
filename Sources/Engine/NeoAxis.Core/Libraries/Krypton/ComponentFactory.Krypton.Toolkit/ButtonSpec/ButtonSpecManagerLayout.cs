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
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// Manage a collection of button specs for placing within a collection of ViewLayoutDocker instances.
    /// </summary>
    public class ButtonSpecManagerLayout : ButtonSpecManagerBase
    {
        #region Instance Fields
        private ViewLayoutDocker[] _viewDockers;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ButtonSpecManagerLayout class.
        /// </summary>
        /// <param name="control">Control that owns the button manager.</param>
        /// <param name="redirector">Palette redirector.</param>
        /// <param name="variableSpecs">Variable set of button specifications.</param>
        /// <param name="fixedSpecs">Fixed set of button specifications.</param>
        /// <param name="viewDockers">Array of target view dockers.</param>
        /// <param name="viewMetrics">Array of target metric providers.</param>
        /// <param name="viewMetricInt">Array of target metrics for outside/inside spacer size.</param>
        /// <param name="viewMetricPaddings">Array of target metrics for button padding.</param>
        /// <param name="getRenderer">Delegate for returning a tool strip renderer.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public ButtonSpecManagerLayout(Control control,
                                       PaletteRedirect redirector,
                                       ButtonSpecCollectionBase variableSpecs,
                                       ButtonSpecCollectionBase fixedSpecs,
                                       ViewLayoutDocker[] viewDockers,
                                       IPaletteMetric[] viewMetrics,
                                       PaletteMetricInt[] viewMetricInt,
                                       PaletteMetricPadding[] viewMetricPaddings,
                                       GetToolStripRenderer getRenderer,
                                       NeedPaintHandler needPaint)
            : this(control, redirector, variableSpecs, fixedSpecs, 
                   viewDockers, viewMetrics, viewMetricInt, viewMetricInt,
                   viewMetricPaddings, getRenderer, needPaint)
        {
        }
            
        /// <summary>
        /// Initialize a new instance of the ButtonSpecManagerLayout class.
        /// </summary>
        /// <param name="control">Control that owns the button manager.</param>
        /// <param name="redirector">Palette redirector.</param>
        /// <param name="variableSpecs">Variable set of button specifications.</param>
        /// <param name="fixedSpecs">Fixed set of button specifications.</param>
        /// <param name="viewDockers">Array of target view dockers.</param>
        /// <param name="viewMetrics">Array of target metric providers.</param>
        /// <param name="viewMetricIntOutside">Array of target metrics for outside spacer size.</param>
        /// <param name="viewMetricIntInside">Array of target metrics for inside spacer size.</param>
        /// <param name="viewMetricPaddings">Array of target metrics for button padding.</param>
        /// <param name="getRenderer">Delegate for returning a tool strip renderer.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public ButtonSpecManagerLayout(Control control,
                                       PaletteRedirect redirector,
                                       ButtonSpecCollectionBase variableSpecs,
                                       ButtonSpecCollectionBase fixedSpecs,
                                       ViewLayoutDocker[] viewDockers,
                                       IPaletteMetric[] viewMetrics,
                                       PaletteMetricInt[] viewMetricIntOutside,
                                       PaletteMetricInt[] viewMetricIntInside,
                                       PaletteMetricPadding[] viewMetricPaddings,
                                       GetToolStripRenderer getRenderer,
                                       NeedPaintHandler needPaint)
            : base(control, redirector, variableSpecs, fixedSpecs, 
                   viewMetrics, viewMetricIntOutside, viewMetricIntInside,
                   viewMetricPaddings, getRenderer, needPaint)
        {
            // Remember references
            _viewDockers = viewDockers;

            Construct();
        }
        #endregion

        #region Protected Overrides
        /// <summary>
        /// Gets the number of dockers.
        /// </summary>
        /// <returns>Number of dockers.</returns>
        protected override int DockerCount
        {
            get { return _viewDockers.Length; }
        }

        /// <summary>
        /// Gets the index of the provided docker.
        /// </summary>
        /// <param name="viewDocker">View docker reference.</param>
        /// <returns>Index of docker; otherwise -1.</returns>
        protected override int DockerIndex(ViewBase viewDocker)
        {
            for (int i = 0; i < _viewDockers.Length; i++)
                if (_viewDockers[i] == viewDocker)
                    return i;

            return -1;
        }

        /// <summary>
        /// Gets the docker at the specified index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>View docker reference; otherwise null.</returns>
        protected override ViewBase IndexDocker(int i)
        {
            if (_viewDockers.Length > i)
                return _viewDockers[i];
            else
                return null;
        }

        /// <summary>
        /// Gets the orientation of the docker at the specified index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>VisualOrientation value.</returns>
        protected override VisualOrientation DockerOrientation(int i)
        {
            if (_viewDockers.Length > i)
                return _viewDockers[i].Orientation;
            else
                return VisualOrientation.Top;
        }

        /// <summary>
        /// Gets the element that represents the foreground color.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>View content instance.</returns>
        protected override ViewDrawContent GetDockerForeground(int i)
        {
            return null;
        }

        /// <summary>
        /// Add a view element to a docker.
        /// </summary>
        /// <param name="i">Index of view docker.</param>
        /// <param name="dockStyle">Dock style for placement.</param>
        /// <param name="view">Actual view to add.</param>
        /// <param name="usingSpacers">Are view spacers being used.</param>
        protected override void AddViewToDocker(int i,
                                                ViewDockStyle dockStyle,
                                                ViewBase view,
                                                bool usingSpacers)
        {
            // Get the indexed docker
            ViewLayoutDocker viewDocker = _viewDockers[i];

            // By default add to the end of the children
            int insertIndex = viewDocker.Count;

            // If using spacers, then insert before the first spacer
            if (usingSpacers)
            {
                for (int j = 0; j < insertIndex; j++)
                    if (viewDocker[j] is ViewLayoutMetricSpacer)
                    {
                        insertIndex = j;
                        break;
                    }
            }

            viewDocker.Insert(insertIndex, view);
            viewDocker.SetDock(view, dockStyle);
        }

        /// <summary>
        /// Add the spacing views into the indexed docker.
        /// </summary>
        /// <param name="i">Index of docker.</param>
        /// <param name="spacerL">Spacer for the left side.</param>
        /// <param name="spacerR">Spacer for the right side.</param>
        protected override void AddSpacersToDocker(int i,
                                                   ViewLayoutMetricSpacer spacerL,
                                                   ViewLayoutMetricSpacer spacerR)
        {
            // Get the indexed instance
            ViewLayoutDocker viewDocker = _viewDockers[i];

            // Add them into the view docker instance
            viewDocker.Add(spacerL, ViewDockStyle.Left);
            viewDocker.Add(spacerR, ViewDockStyle.Right);
        }
        #endregion
    }
}

#endif