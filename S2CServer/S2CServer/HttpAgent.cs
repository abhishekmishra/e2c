using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace S2CCore
{
    public class HttpAgent : ICleaningAgent
    {
        private int _AgentId;
        private Coords _SpaceSize;
        private Dictionary<string, string> _Args;
        private string _AgentUrl;
        private static readonly HttpClient client = new HttpClient();

        public HttpAgent(Dictionary<string, string> args)
        {
            _Args = args;
            _AgentUrl = _Args["url"];
        }

        public int AgentId
        {
            get => _AgentId;
            set
            {
                _AgentId = value;
                var task = Task.Run(async () => await client.GetAsync(_AgentUrl + "/" + _AgentId + "/init"));
                var res = task.Result;
            }
        }

        public Coords SpaceSize
        {
            get => _SpaceSize;
            set
            {
                _SpaceSize = value;
                var task = Task.Run(
                    async () => await client.GetAsync(
                        _AgentUrl + "/" + _AgentId + "/spacesize?rows="
                        + _SpaceSize.Row + "&cols=" + _SpaceSize.Column));
                var res = task.Result;
            }
        }

        public void CommandResult(bool success, string failureReason, SimulationErrorCode errorCode, Coords location)
        {
            var task = Task.Run(
                async () => await client.GetAsync(
                    _AgentUrl + "/" + _AgentId + "/commandresult?row="
                    + location.Row + "&col=" + location.Column
                    + "&errorCode=" + errorCode.ToString()
                    + "&failureReason=" + failureReason
                    + "&success=" + (success ? "1" : "0")));
            var res = task.Result;
        }

        public IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            var task = Task.Run(
                async () => await client.GetAsync(
                     _AgentUrl + "/" + _AgentId + "/nextcommand?row="
                    + location.Row + "&col=" + location.Column
                    + "&isdirty=" + (isDirty ? "1" : "0")));
            var res = task.Result;
            res.EnsureSuccessStatusCode();
            var strTask = Task.Run(
                async () => await res.Content.ReadAsStringAsync());
            string responseBody = strTask.Result;
            JsonDocument doc = JsonDocument.Parse(responseBody);

            int count = 0;
            string cmdName = null;
            int agentId = 0;
            int row = 0;
            int col = 0;
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                switch (count)
                {
                    case 0:
                        cmdName = element.ToString();
                        break;
                    case 1:
                        agentId = Int32.Parse(element.ToString());
                        break;
                    case 2:
                        row = Int32.Parse(element.ToString());
                        break;
                    case 3:
                        col = Int32.Parse(element.ToString());
                        break;
                };
                count += 1;
            }

            if(cmdName.Equals("moveto"))
            {
                return new MoveToCommand(agentId, row, col);
            }
            if (cmdName.Equals("clean"))
            {
                return new CleanCommand(agentId, row, col);
            }
            return null;
        }
    }
}
