using System.Net.Http.Json;

var rand = new Random();
var client = new HttpClient();

var result = await client.PostAsJsonAsync("http://localhost:5037/api/order", new
{
    orderTime = "2022-06-09T16:40:47.3258091+02:00", 
    orderStatus = 0,
    orderItem = rand.Next(1, 5), 
    orderOrigin = rand.Next(1, 3)
});

Console.WriteLine(await result.Content.ReadAsStringAsync());
