//   Copyright Nexus Kharsun Sergey
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Nexus.Core.Channels;

namespace Nexus.Messages
{
    public class Message : IMessage
    {
        private readonly IChannel _from;
        public int Id { get; }
        public object Data { get; set; }

        public Message(int id, bool isRequest, object data, IChannel from = null)
        {
            Id = id;
            _from = from;
            Data = data;
            IsRequest = isRequest;
        }

        public bool IsRequest { get; set; }

        public void Respounce<T>(T data)
        {
            _from?.Send(data);
        }
    }
}