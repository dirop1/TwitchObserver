using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchObserver
{
    public partial class Chat : Form
    {
        public string channelName;
        public Chat()
        {
            InitializeComponent();

            webBrowserChat.Url = new Uri("http://www.twitch.tv/" + channelName + "/chat");
        }

        internal void setHtml()
        {
            //webBrowserChat.DocumentText = "<iframe frameborder=\"0\" scrolling=\"no\" id=\"chat_embed\" src=\"http://www.twitch.tv/"+channelName+"/chat\" height=\"600\" width=\"350\"></iframe>";
        }
    }
}
