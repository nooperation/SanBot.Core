using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanBot.Core
{
    interface IMessageHandler
    {
        public bool OnMessage(uint messageId, BinaryReader reader);
    }
}
