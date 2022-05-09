using DbsConsolidatedStatementImporter.Services;

var transactions = PdfServices.RetrieveTransactionsFromPdf2(@"Assets\202104.pdf");
transactions.ForEach(transaction => global::System.Console.WriteLine(transaction));