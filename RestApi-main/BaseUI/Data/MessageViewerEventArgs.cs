using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseUI.Data
{
    public class MessageViewerEventArgs : EventArgs
    {
        public MessageViewerEventArgs() : base()
        { }

        public object commandArguments { get; set; }

        public string commandName { get; set; }
    }
}
