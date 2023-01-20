using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace monitoradorbitcoin;

public class Program{
    static async Task Main(string[] args)
    {
        string data = PegarInfo("https://br.investing.com/crypto/bitcoin/btc-brl");

        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        var valor_inicial = Convert.ToDouble(data);
        var valor_antigo = valor_inicial;
        
        // * Cronometro para verificar o valor do bitcoin a cada 1 minuto.
        while (await timer.WaitForNextTickAsync())
        {
            string data1 = PegarInfo("https://br.investing.com/crypto/bitcoin/btc-brl");
            var valor_atual = Convert.ToDouble(data1);
                
            if (valor_antigo == valor_atual){
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"O valor não mudou, o valor do bitcoin agora é R${valor_atual}");
            }
            else if (valor_antigo > valor_atual){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"O bitcoin caiu, o valor antigo era R${valor_antigo}, valor atual é R${valor_atual}, o valor caiu {Math.Round((valor_antigo-valor_atual)/valor_antigo*100,2)}% (R${valor_antigo-valor_atual}) comparado ao valor antigo");
                valor_antigo = valor_atual;
            }
            else if (valor_atual > valor_antigo){
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"O bitcoin aumentou, o valor antigo era R${valor_antigo}, valor atual é R${valor_atual}, o valor aumentou {Math.Round(((valor_antigo-valor_atual)/valor_antigo*100)*-1,2)}% (R${(valor_antigo-valor_atual)*-1}) comparado ao valor antigo");
                valor_antigo = valor_atual;
            }
        }
    }

    public static string PegarInfo(string url)
    {
        //Usando o driver do edge e prevenir de apresentar logs
        EdgeDriverService service = EdgeDriverService.CreateDefaultService();
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        var options = new EdgeOptions();
        options.PageLoadStrategy = PageLoadStrategy.Normal;
        options.AddArguments("headless");
        options.AddArgument("--disable-logging");
        options.AddArgument("--log-level=3");
        options.AddArgument("--blink-settings=imagesEnabled=false");
        var driver = new EdgeDriver(service, options);
        
        try
        {
            // * Entra no site
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(500);

            // * Pegar as informações
            var price = driver.FindElement(By.XPath("/html/body/div/div/div/div/div[2]/main/div/div[1]/div[2]/div[1]/span")).Text;
            driver.Close();
            if (price is not null)
            {
                return price;
            }
            else
            {
                return "ue?";
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            driver.Close();
            return "erro ao obter o valor"; //! Vai alertar que não encontrou nada
        }
    }
}