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
    internal class PublishFileInfo
    {
        public string FilePath { get; set; }
        public byte[] Content { get; set; }
        public Action<string> Callback { get; set; }
    }
}
