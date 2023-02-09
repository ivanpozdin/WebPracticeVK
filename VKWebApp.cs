using System.Diagnostics;
using Newtonsoft.Json;

namespace WebPracticeVK;

public class VKWebApp
{
    private readonly HttpClient client = new();

    public async void FetchUserInfo(string ACCESS_TOKEN, string user)
    {
        var responseMessage = client
            .GetAsync(
                $"https://api.vk.com/method/users.get?user_ids={user}&fields=bdate&access_token={ACCESS_TOKEN}&v=5.131")
            .Result;
        var text = await responseMessage.Content.ReadAsStringAsync();
        dynamic jsonDe = JsonConvert.DeserializeObject(text);
        // string bday = jsonDe.response[0].bdate;
        string firstName = jsonDe.response[0].first_name;
        string lastName = jsonDe.response[0].last_name;

        // Console.WriteLine(text);
        Console.WriteLine($"Имя и Фамилия: {firstName} {lastName}");
    }

    public void StartConsoleApp()
    {
        Console.WriteLine("Введите сервисный ключ:");
        var ACCESS_TOKEN = Console.ReadLine();
        // string ACCESS_TOKEN = "bce93f7cbce93f7cbce93f7c21bffb42c5bbce9bce93f7cdf09201372391e47c1d5f5ad";
        Console.WriteLine("Введите id пользователя:");
        var user = Console.ReadLine();
        // string user = "ivanpozdin";

        FetchUserInfo(ACCESS_TOKEN, user);
    }

    public async void PrintFriendsOnline()
    {
        /*var authString =
            "https://oauth.vk.com/authorize?client_id=51543481&display=page&redirect_uri=https://oauth.vk.com/blank.html/&scope=friends&response_type=token&v=5.131&state=123456&revoke=1";
        authString = authString.Replace("&", "^&");
        Process.Start(new ProcessStartInfo(
                "cmd",
                $"/c start {authString}")
            { CreateNoWindow = true });
        Console.WriteLine("Скопируйте и вставьте access token из адресной строки браузера после слова code= "); */
        // var accessToken = Console.ReadLine();
        // Console.WriteLine("Введите сервисный ключ");
        // var serviceKey = Console.ReadLine();
        var accessToken = "vk1.a.nzIlpBnGv7UPticuN6LlyImq7A_JWgUEr8Ak7t5ThbEj-bxKB826a9qBHe0wJB534TMVjf2Ds2bHQY7G4qEqQz_B86TCZpSE4IU2qLDaKyFW_i1b2a4IMk3tCBFOJoF_xzDpmEsDJupWltGup9i9-91bh8icfzmTHFLQPkcws-GWxhgXnhhoVoUW5FCeKDpy";
        var serviceKey = "bce93f7cbce93f7cbce93f7c21bffb42c5bbce9bce93f7cdf09201372391e47c1d5f5ad";
        var responseMessage = client
            .GetAsync($"https://api.vk.com/method/friends.getOnline?access_token={accessToken}&v=5.131").Result;
        var text = await responseMessage.Content.ReadAsStringAsync();
        Console.WriteLine(text);
        dynamic jsonDe = JsonConvert.DeserializeObject(text);
        foreach (var id in jsonDe.response) FetchUserInfo(serviceKey, "id"+id);
        
    }
}
// vk1.a.nzIlpBnGv7UPticuN6LlyImq7A_JWgUEr8Ak7t5ThbEj-bxKB826a9qBHe0wJB534TMVjf2Ds2bHQY7G4qEqQz_B86TCZpSE4IU2qLDaKyFW_i1b2a4IMk3tCBFOJoF_xzDpmEsDJupWltGup9i9-91bh8icfzmTHFLQPkcws-GWxhgXnhhoVoUW5FCeKDpy
// bce93f7cbce93f7cbce93f7c21bffb42c5bbce9bce93f7cdf09201372391e47c1d5f5ad