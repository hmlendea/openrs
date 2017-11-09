﻿using System.Drawing;

namespace RuneScapeSolo.Settings
{
    public class GraphicsSettings
    {
        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        public Size Resolution { get; set; }

        /// <summary>
        /// Gets or sets the fullscreen mode toggle.
        /// </summary>
        /// <value>The fullscreen mode.</value>
        public bool Fullscreen { get; set; }

        /// <summary>
        /// Gets or sets the interlacing mode toggle.
        /// </summary>
        /// <value>The interlacing mode. </value>
        public bool Interlacing { get; set; }

        public GraphicsSettings()
        {
            Resolution = new Size(1280, 720);
        }
    }
}