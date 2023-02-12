using System.Diagnostics;
using Newtonsoft.Json;

namespace WebPracticeVK;

public class VkWebApp
{
    private readonly HttpClient _client = new();

    private async void FetchUserInfo(string accessToken, string user)
    {
        var responseMessage = _client
            .GetAsync(
                $"https://api.vk.com/method/users.get?user_ids={user}&fields=bdate&access_token={accessToken}&v=5.131")
            .Result;
        var text = await responseMessage.Content.ReadAsStringAsync();
        dynamic jsonDe = JsonConvert.DeserializeObject(text) ?? throw new InvalidOperationException();
        string birthday = jsonDe.response[0].bdate ?? "no data";
        string firstName = jsonDe.response[0].first_name ?? "no data";
        string lastName = jsonDe.response[0].last_name ?? "no data";
        Console.WriteLine($"имя и фамилия, дата рождения: {firstName} {lastName}, {birthday}");
    }

    public void StartConsoleApp()
    {
        Console.WriteLine(
            "Всего есть 3 сценария работы:\n1. Получение открытой информации о пользователе.\n2. Список контактов онлайн.\n3. Поменять аватарку.");
        var workScenario = " ";
        while (workScenario != "1" && workScenario != "2" && workScenario != "3")
        {
            Console.WriteLine("Введите сценарий работы одной цифрой: ");
            workScenario = Console.ReadLine();
        }

        switch (workScenario)
        {
            case "1":
                DoScenario1();
                break;
            case "2":
                DoScenario2();
                break;
            case "3":
                DoScenario3();
                break;
        }
    }

    private void DoScenario1()
    {
        Console.WriteLine("Введите сервисный ключ:");
        var accessToken = Console.ReadLine() ?? throw new ArgumentNullException();
        Console.WriteLine("Введите id пользователя:");
        var user = Console.ReadLine();
        if (user != null) FetchUserInfo(accessToken, user);
    }

    private void DoScenario2()
    {
        var accessToken = GetAccessToken(true);
        Console.WriteLine("Введите сервисный ключ");
        var serviceKey = Console.ReadLine();
        PrintFriendsOnline(accessToken ?? throw new InvalidOperationException(),
            serviceKey ?? throw new InvalidOperationException());
    }

    private void DoScenario3()
    {
        var accessToken = GetAccessToken(photos: true);
        var serverAddress = GetServerAddress(accessToken ?? throw new InvalidOperationException()).Result;
        Console.WriteLine("Введите путь к файлу фотографии");
        var photoPath = Console.ReadLine() ?? throw new InvalidOperationException();
        Console.WriteLine("Введите название файла фотографии(включая разрешение файла)");
        var photoName = Console.ReadLine() ?? throw new InvalidOperationException();
        ChangeAvatar(accessToken ?? throw new InvalidOperationException(), serverAddress, photoPath, photoName);
        Console.WriteLine("Аватарка изменена.");
    }

    private async void PrintFriendsOnline(string accessToken, string serviceKey)
    {
        var responseMessage = _client
            .GetAsync($"https://api.vk.com/method/friends.getOnline?access_token={accessToken}&v=5.131").Result;
        var text = await responseMessage.Content.ReadAsStringAsync();
        Console.WriteLine(text);
        dynamic jsonDe = JsonConvert.DeserializeObject(text) ?? throw new InvalidOperationException();
        foreach (var id in jsonDe.response) FetchUserInfo(serviceKey, "id" + id);
    }

    private string? GetAccessToken(bool friends = false, bool photos = false)
    {
        var scope = "";
        if (photos) scope = "photos";
        if (friends) scope = "friends";

        var authString =
            $"https://oauth.vk.com/authorize?client_id=51543481&display=page&redirect_uri=https://oauth.vk.com/blank.html/&scope={scope}&response_type=token&v=5.131&state=123456&revoke=1";
        authString = authString.Replace("&", "^&");
        Process.Start(new ProcessStartInfo(
                "cmd",
                $"/c start {authString}")
            { CreateNoWindow = true });
        Console.WriteLine(
            "Скопируйте и вставьте access token из адресной строки браузера после access_token= и до &expires_in=");
        var accessToken = Console.ReadLine();
        return accessToken;
    }

    private async Task<string> GetServerAddress(string accessToken)
    {
        var serverAddressResponse = _client
            .GetAsync($"https://api.vk.com/method/photos.getOwnerPhotoUploadServer?access_token={accessToken}&v=5.131")
            .Result;
        var responseMessage = await serverAddressResponse.Content.ReadAsStringAsync();
        dynamic serverAddressJsonDe =
            JsonConvert.DeserializeObject(responseMessage) ?? throw new InvalidOperationException();
        string serverAddress = serverAddressJsonDe.response.upload_url;
        return serverAddress;
    }

    private async void ChangeAvatar(string accessToken, string serverAddress, string photoPath, string photoName)
    {
        var requestContent = new MultipartFormDataContent
        {
            { new ByteArrayContent(File.ReadAllBytes(photoPath)), "photo", photoName }
        };
        var uploadResponse = _client.PostAsync(serverAddress, requestContent).Result;
        var uploadResponseMessage = await uploadResponse.Content.ReadAsStringAsync();
        dynamic uploadJsonDe = JsonConvert.DeserializeObject(uploadResponseMessage) ??
                               throw new InvalidOperationException();
        string server = uploadJsonDe.server;
        string photo = uploadJsonDe.photo;
        string hash = uploadJsonDe.hash;

        var saveResponse = _client
            .GetAsync(
                $"https://api.vk.com/method/photos.saveOwnerPhoto?access_token={accessToken}&server={server}&hash={hash}&photo={photo}&v=5.131")
            .Result;
        var saveResponseMessage = await saveResponse.Content.ReadAsStringAsync();
    }
}