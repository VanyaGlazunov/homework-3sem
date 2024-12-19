#pragma warning disable SA1200 // Using directives should be placed correctly
using Chat;
#pragma warning restore SA1200 // Using directives should be placed correctly

if (args.Length == 0)
{
    Console.WriteLine("Specify port to become server or port and IP address of a server to become client");
}
else if (args.Length == 1)
{
    var port = int.Parse(args[0]);
    var chatServer = new ChatServer(port);
    chatServer.Start();
}
else if (args.Length == 2)
{
    var port = int.Parse(args[0]);
    var address = int.Parse(args[1]);
    var chatClient = new ChatClient(new (address, port));
    chatClient.Connect(Console.In, Console.Out);
}