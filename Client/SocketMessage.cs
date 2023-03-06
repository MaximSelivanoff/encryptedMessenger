using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Client
{
    internal class SocketMessage
    {
        int messageCode;
        string JsonObject;

        public SocketMessage(int messageCode, string JsonObject) { 
            this.messageCode = messageCode;
            this.JsonObject = JsonObject;
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
