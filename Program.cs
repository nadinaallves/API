using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// -------------------------------------------------------------------------
//  CLASSE DE MODELO (DTO)
//  Define a estrutura do objeto que receberá os dados do JSON.
//  É definida fora da lógica principal.
// -------------------------------------------------------------------------
public class Endereco
{
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Localidade { get; set; }
    public string Uf { get; set; }
    public string Ibge { get; set; }
    public string Gia { get; set; }
    public string Ddd { get; set; }
    public string Siafi { get; set; }
}

// -------------------------------------------------------------------------
//  LÓGICA PRINCIPAL DA APLICAÇÃO
// -------------------------------------------------------------------------
class Program
{
    // O ponto de entrada da aplicação agora é o 'Main' e é assíncrono
    static async Task Main(string[] args)
    {
        // 1. O HttpClient agora é criado dentro do Main
        using HttpClient client = new HttpClient();

        Console.WriteLine("---------------------------------------------");
        Console.WriteLine("    CONSUMINDO API VIA CEP EM C# (.NET)");
        Console.WriteLine("---------------------------------------------");

        // 2. Coleta de Entrada do Usuário
        Console.Write("Digite o CEP (apenas números): ");
        string cep = Console.ReadLine();

        // Verifica se o CEP foi digitado
        if (string.IsNullOrWhiteSpace(cep))
        {
            Console.WriteLine("\nCEP não pode ser vazio.");
            return;
        }

        // 3. Montagem da URL da Requisição
        string url = $"https://viacep.com.br/ws/{cep}/json/";

        Console.WriteLine($"\nBuscando dados na API: {url}...");

        // 4. Execução da Requisição HTTP e Tratamento de Erros
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            // 5. Verificação do Status Code
            if (response.IsSuccessStatusCode)
            {
                // 6. Leitura da Resposta (JSON)
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Verifica se a API retornou um erro (como CEP não encontrado)
                if (jsonResponse.Contains("\"erro\": true"))
                {
                    Console.WriteLine($"\n[ERRO] CEP '{cep}' não encontrado.");
                    return;
                }

                // 7. Configuração da Deserialização
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // 8. Deserialização (Conversão de JSON para Objeto C#)
                Endereco endereco = JsonSerializer.Deserialize<Endereco>(jsonResponse, options);

                // 9. Apresentação dos Dados
                Console.WriteLine("\n--- Endereço Encontrado ---");
                Console.WriteLine($"CEP: {endereco.Cep}");
                Console.WriteLine($"Rua: {endereco.Logradouro}");
                Console.WriteLine($"Bairro: {endereco.Bairro}");
                Console.WriteLine($"Cidade/UF: {endereco.Localidade} - {endereco.Uf}");
            }
            else
            {
                Console.WriteLine($"\n[ERRO HTTP] Falha na comunicação com a API. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nOcorreu um erro inesperado: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}