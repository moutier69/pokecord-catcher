using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokecordCatcherBot
{
    public class ResponseGrabber
    {
        private readonly DiscordSocketClient client;

        public ResponseGrabber(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task<SocketMessage> GrabResponse(Func<SocketMessage, bool> predicate, double timeout)
        {
            if (predicate == null)
                throw new ArgumentException("Predicate cannot be null.");

            var completion = new TaskCompletionSource<SocketMessage>();
            var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            cancellation.Token.Register(() => completion.TrySetResult(null));

            try
            {
                client.MessageReceived += messageChecker;
                return await completion.Task;
            }
            finally
            {
                client.MessageReceived -= messageChecker;
            }

            async Task messageChecker(SocketMessage msg)
            {
                if (predicate(msg))
                    completion.TrySetResult(msg);
            }
        }
    }
}
