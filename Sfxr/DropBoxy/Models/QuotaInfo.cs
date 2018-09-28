using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DropBoxy.Models
{
    public class QuotaInfo
    {
        /// <summary>
        /// Gets or sets the shared.
        /// </summary>
        /// <value>The shared.</value>
        public double Shared { get; set; }

        /// <summary>
        /// Gets or sets the quota.
        /// </summary>
        /// <value>The quota.</value>
        public double Quota { get; set; }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public double Normal { get; set; }
    }
}
