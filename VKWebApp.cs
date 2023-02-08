using System.Net.Http.Json;
using Newtonsoft.Json;

namespace WebPracticeVK;

public class VKWebApp
{
    private readonly HttpClient client = new();
    public async void FetchUserInfo(string ACCESS_TOKEN, string user)
    {
        var responseMessage = client.GetAsync($"https://api.vk.com/method/users.get?user_ids={user}&fields=bdate&access_token={ACCESS_TOKEN}&v=5.131").Result;
        var text = await responseMessage.Content.ReadAsStringAsync();
        dynamic jsonDe = JsonConvert.DeserializeObject(text);
        string bday = jsonDe.response[0].bdate;
        string firstName = jsonDe.response[0].first_name;
        string lastName = jsonDe.response[0].last_name;

        // Console.WriteLine(text);
        Console.WriteLine($"Имя и Фамилия: {firstName} {lastName}");
        Console.WriteLine($"Дата рождения: {bday}");
        client.Dispose();
    }

    public void StartConsoleApp()
    {
        Console.WriteLine("Введите сервисный ключ:");
        string ACCESS_TOKEN = Console.ReadLine();
        // string ACCESS_TOKEN = "bce93f7cbce93f7cbce93f7c21bffb42c5bbce9bce93f7cdf09201372391e47c1d5f5ad";
        Console.WriteLine("Введите id пользователя:");
        string user = Console.ReadLine();
        // string user = "ivanpozdin";
        
        FetchUserInfo(ACCESS_TOKEN, user);
    }
}