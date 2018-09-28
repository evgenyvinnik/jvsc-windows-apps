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
    /// <summary>
    /// 
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the referral_link.
        /// </summary>
        /// <value>The referral_link.</value>
        public string Referral_link { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the display_name.
        /// </summary>
        /// <value>The display_name.</value>
        public string Display_name { get; set; }

        /// <summary>
        /// Gets or sets the quota_ info.
        /// </summary>
        /// <value>The quota_ info.</value>
        public QuotaInfo Quota_Info { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        /// <value>The uid.</value>
        public int Uid { get; set; }
    }
}
