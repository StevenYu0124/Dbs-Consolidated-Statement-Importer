// Arragne
using DbsConsolidatedStatementImporter.Services;

decimal expectedBalance = 9258.92m;
decimal actualBalance = 0;

// Act
for (int year = 2019; year <= 2020; year++)
{
    for (int month = 1; month <= 12; month++)
    {
        if (year == 2019 && month == 1)
        {
            continue;
        }
        if (year == 2020 && month == 9)
        {
            break;
        }

        string? monthString = null;
        if (month >= 10)
        {
            monthString = month.ToString();
        }
        else
        {
            monthString = "0" + month.ToString();
        }
        var transactions = PdfServices.RetrieveTransactionsFromPdf(@$"Assets\{year}{monthString}.pdf");
        actualBalance += transactions.Sum(x => x.Amount);
    }
}