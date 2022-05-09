using DbsConsolidatedStatementImporter.Models;
using DbsConsolidatedStatementImporter.Services;
using Newtonsoft.Json;

string oldPath = "old";
string newPath = "new";
string outputPath = "output.json";
List<Transaction> output = new();

if (!Directory.Exists(oldPath))
{
    Console.WriteLine("old path not exist.");
    Directory.CreateDirectory(oldPath);
}
else
{
    string[] oldStatementsFiles = Directory.GetFiles(oldPath);
    foreach (string file in oldStatementsFiles)
    {
        Console.WriteLine(file);
        var transactions = PdfServices.RetrieveTransactionsFromPdf1(file);
        transactions.ForEach(transaction => {
            output.Add(transaction);
            global::System.Console.WriteLine(transaction);
        });
    }
}

if (!Directory.Exists(newPath))
{
    Console.WriteLine("new path not exist.");
    Directory.CreateDirectory(newPath);
}
else
{
    string[] newStatementsFiles = Directory.GetFiles(newPath);
    foreach (string file in newStatementsFiles)
    {
        Console.WriteLine(file);
        var transactions = PdfServices.RetrieveTransactionsFromPdf2(file);
        transactions.ForEach(transaction => {
            output.Add(transaction);
            global::System.Console.WriteLine(transaction);
        });
    }
}

if (output.Count > 0)
{
    

    Console.WriteLine($"output to {outputPath}...");
    string jsonString = JsonConvert.SerializeObject(output);
    File.WriteAllText(outputPath, jsonString);
    Console.WriteLine($"output to {outputPath}...ok");
}
else
{
    Console.WriteLine("no transaction found");
}

Console.WriteLine("finish");