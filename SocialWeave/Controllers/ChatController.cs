using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace SocialWeave.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult WebSocket()
        {
            return View();
        }

        public async Task WebSocketHandler(HttpContext context, WebSocket webSocket) 
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            }
            while(!result.CloseStatus.HasValue);
        }

    }
}
