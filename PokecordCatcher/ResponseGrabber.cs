using Discord;
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

        public Task<SocketMessage> SendMessageAndGrabResponse(ITextChannel channel, string msg, Func<SocketMessage, bool> predicate, double timeout) =>
            GrabResponse(async () => await channel.SendMessageAsync(msg), predicate, timeout);

        public async Task<SocketMessage> GrabResponse(Func<Task> action, Func<SocketMessage, bool> predicate, double timeout)
        {

            if (action == null)
                throw new ArgumentException("Action cannot be null.");

            if (predicate == null)
                throw new ArgumentException("Predicate cannot be null.");

            var completion = new TaskCompletionSource<SocketMessage>();
            var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            cancellation.Token.Register(() => completion.TrySetResult(null));

            try
            {
                client.MessageReceived += messageChecker;
                await action();
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
