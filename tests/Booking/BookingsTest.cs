// 1- Bibliotecas 
using Models;
using RestSharp;
using Newtonsoft.Json; // dependencia para o JsonConvert

// 2 - NameSpace
namespace Booker;

// 3 - Classe
public class BookingsTest
{
    // 3.1 - Atributos
    // Endereço da API
    private const String BASE_URL = "https://restful-booker.herokuapp.com/";
    private string bookingId;
    public String token;

    // 3.2 - Funções e Métodos
    // Função de leitura de dados a partir de um arquivo csv

public static IEnumerable<TestCaseData> getTestData()
{
    String caminhoMassa = @"C:\Iterasys\nunit-booker\fixtures\reserva.csv";

    using var reader = new StreamReader(caminhoMassa);

    // Pula a primeira linha com os cabeçalhos
    reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        var values = line.Split(",");

        // Converta os valores corretamente para os tipos esperados
        yield return new TestCaseData(
            values[0],            // string
            values[1],            // string
            int.Parse(values[2]), // int
            bool.Parse(values[3]),// bool
            DateTime.Parse(values[4]), // DateTime
            DateTime.Parse(values[5]), // DateTime
            values[6]             // string
        );
    }
}

    [Test, Order(1)]
    public void PostCreateTokenTest()
    {
        // Ler os dados do arquivo JSON
        string jsonContent = File.ReadAllText(@"C:\Iterasys\nunit-booker\fixtures\data.json");

        // Converter o JSON para um objeto dinâmico
        dynamic json = JsonConvert.DeserializeObject(jsonContent);

        // Extrair as credenciais do objeto dinâmico
        string username = json.username;
        string password = json.password;

        // Configurar o corpo da solicitação
        var requestBody = new { username, password };

        // Configurar a solicitação
        var client = new RestClient(BASE_URL);
        var request = new RestRequest("auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(requestBody);

        // Executar a solicitação
        var response = client.Execute(request);

        // Extrair o token da resposta
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        // Validar o status code esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        // Salvar o token em uma variável
        token = responseBody.token;
        Console.WriteLine($"Token = {token}");
        Environment.SetEnvironmentVariable("token", token);
    }

    [Test, Order(2)]
    public void PostBookingTest()
    {
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest("booking", Method.Post);

        // Ler os dados do arquivo JSON
        string jsonBody = File.ReadAllText(@"C:\Iterasys\nunit-booker\fixtures\reserva1.json");

        // Adiciona na requisição o corpo do JSON diretamente
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(jsonBody);

        // Executa
        var response = client.Execute(request);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        // Exibe o responseBody no console
        Console.WriteLine(response.Content);

        // Valide que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        // Valida o firstName da resposta
        var firstName = responseBody.booking.firstname.ToString();
        Assert.That(firstName, Is.EqualTo("Samuca"));

        // Valida o lastname da resposta
        var lastName = responseBody.booking.lastname.ToString();
        Assert.That(lastName, Is.EqualTo("Braco"));
            
        // Valida o totalPrice da resposta
        var totalPrice = Convert.ToInt32(responseBody.booking.totalprice);
        Assert.That(totalPrice, Is.EqualTo(2222));

        // Salva o bookingId para uso posterior
        bookingId = responseBody.bookingid.ToString();
        
    }   

    [Test, Order(3)]
    public void GetBookingsIdsTest()
    {
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest("booking", Method.Get);

        // Executa
        var response = client.Execute(request);

        // Desserializa a resposta do GET para uma lista de objetos 
        var responseBody = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        
        //Imprime o bookingId
        Console.WriteLine($"BookingId: {bookingId}");

        // Valida se o bookingId do PostBookingTest está na lista de bookingIds do GetBookingsIdsTest
        Assert.That(responseBody.Any(x => x.bookingid.ToString() == bookingId), Is.True);

        // Valide que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

    }

    [Test, Order(4)]
    public void GetBookingsTest()
    {
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"booking/{bookingId}", Method.Get);
        request.AddHeader("Accept", "application/json");

        // Executa
        var response = client.Execute(request);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        // Valida
        //Valida que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        // Valida o totalPrice da resposta
        var totalPrice = Convert.ToInt32(responseBody.totalprice);
        Assert.That(totalPrice, Is.EqualTo(2222));

        // Valida o additionalneeds é iqual a Breakfast
        Assert.That(responseBody.additionalneeds.ToString(), Is.EqualTo("Breakfast"));     
    }

    [Test, Order(5)]
    public void UpdateBookingTest()
    {   
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"booking/{bookingId}", Method.Put);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}"); // Usando o token salvo anteriormente
        string jsonBody = File.ReadAllText(@"C:\Iterasys\nunit-booker\fixtures\updated_reserva.json");
        request.AddBody(jsonBody);

        //Executa
        var response = client.Execute(request);
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        // Valida que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        
        //Valida que o lastname é o do arquivo updated
        Assert.That(responseBody.lastname.ToString(), Is.EqualTo("Update"));
        
        // Valida que additionalneeds é o do arquivo updated
        Assert.That(responseBody.additionalneeds.ToString(), Is.EqualTo("NotBreakfast"));
    }

    [Test, Order(6)]
    public void PartialUpdateBookingTest()
    {
        // Configura
        string firstName = "James";
        string lastName = "Brown";

        // Monta o corpo da requisição 
        var partialUpdateRequestBody = new
        {
            firstname = firstName,
            lastname = lastName
        };

        // Configuração da solicitação de atualização parcial
        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"booking/{bookingId}", Method.Patch); // Método Patch para atualização parcial
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}"); // Usando o token salvo anteriormente
        request.AddJsonBody(partialUpdateRequestBody); // Adiciona o corpo do JSON à solicitação

        // Executa
        var response = client.Execute(request);
        Console.WriteLine(response.Content);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        // Validação do status code esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        
        //valida o lastname é Brown
        Assert.That(responseBody.lastname.ToString(), Is.EqualTo("Brown"));
    }

    [Test, Order(7)]
    public void DeleteBookingTest()
    {   
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"booking/{bookingId}", Method.Delete); // Método Delete para exclusão
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Cookie", $"token={token}"); // Usando o token salvo anteriormente

        // Execução 
        var response = client.Execute(request);
        Console.WriteLine(response.Content);

        // Validação do status code esperado (201)
        Assert.That((int)response.StatusCode, Is.EqualTo(201));

        // Valida que o response retorna a palavra Created
        Assert.That(response.Content.ToString(), Is.EqualTo("Created"));
    }

    [Test,Order (8)]
    public void HealthCheckTest()
    {   
        // Configura
        var client = new RestClient(BASE_URL);
        var request = new RestRequest("ping", Method.Get);

        // Execução 
        var response = client.Execute(request);
        Console.WriteLine(response.Content);

        // Validação do status code esperado (201)
        Assert.That((int)response.StatusCode, Is.EqualTo(201));

        // Valida que o response retorna a palavra Created
        Assert.That(response.Content.ToString(), Is.EqualTo("Created"));
    }

    [TestCaseSource("getTestData", new object[]{}), Order(9)]
    public void PostBookingDDTest(String firstName,
                                String lastName,
                                int totalPrice,
                                Boolean depositPaid,
                                DateTime  checkin,
                                DateTime  checkout,
                                String additionalNeeds                                
                                )
    {
        // Configura
        BookingsModel bookingsModel = new BookingsModel();
        bookingsModel.firstname = firstName;
        bookingsModel.lastname = lastName;
        bookingsModel.totalprice = totalPrice;
        bookingsModel.depositpaid = depositPaid;
        bookingsModel.bookingdates = new BookingDates (checkin, checkout);
        bookingsModel.additionalneeds = additionalNeeds;

        // A estrutura de dados está pronta, agora vamos serializar
        String jsonBody = JsonConvert.SerializeObject(bookingsModel, Formatting.Indented);
        Console.WriteLine(jsonBody);

        var client = new RestClient(BASE_URL);
        var request = new RestRequest("booking", Method.Post);
        
        // Adiciona na requisição o corpo do JSON diretamente
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(jsonBody);

        // Executa
        var response = client.Execute(request);

        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Console.WriteLine(response.Content);

        // Valide que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

       // Valida o firstName da resposta
        Assert.That((String)responseBody.booking.firstname, Is.EqualTo(firstName));

        // Valida o lastName da resposta
        Assert.That((String)responseBody.booking.lastname, Is.EqualTo(lastName));
        
        // Valida o totalPrice da resposta
        Assert.That((int)responseBody.booking.totalprice, Is.EqualTo(totalPrice));

        // Valida o depositPaid da resposta
        Assert.That((bool)responseBody.booking.depositpaid, Is.EqualTo(depositPaid));

       // Valida o checkin da resposta
        Assert.That((DateTime)responseBody.booking.bookingdates.checkin, Is.EqualTo(checkin));
    
       // Valida o checkout da resposta
        Assert.That((DateTime)responseBody.booking.bookingdates.checkout, Is.EqualTo(checkout));

      // Valida o additionalNeeds da resposta
        Assert.That((String)responseBody.booking.additionalneeds, Is.EqualTo(additionalNeeds));

    }
}    

