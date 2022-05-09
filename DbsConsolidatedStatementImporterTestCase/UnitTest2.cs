namespace DbsConsolidatedStatementImporterTestCase;

public class UnitTest2
{
    private readonly ITestOutputHelper _output;

    public UnitTest2(ITestOutputHelper output)
    {
        _output = output;
    }

    // check
    // 1. count
    // 2. first transaction
    // 3. last transaction
    // 4. 4 lines transaction
    // 5. balance
    [Fact]
    public void Test1()
    {
        // Arragne
        // 1. count
        const int count = 22;
        // 2. first transaction
        Transaction firstTransaction = new()
        {
            Date = new DateOnly(2021, 1, 31),
            Description = "Debit Card transaction SHENG SIONG@ELIASMALL SI NG 30JAN 5264-7110-3400-2305",
            Amount = -9
        };
        // 3. last transaction
        Transaction lastTransaction = new()
        {
            Date = new DateOnly(2021, 2, 28),
            Description = "Interest Earned",
            Amount = 0.07m
        };
        // 4. 4 lines transaction
        Transaction fourLinesTransaction = new()
        {
            Date = new DateOnly(2021, 2, 18),
            Description = "Advice FAST Collection DCOMUSOVJZBSDGSG IIRGPCSG180221A0011267 OTHER",
            Amount = -2000
        };
        // 5. balance
        const decimal balance = 341.55m;


        // Act
        var transactions = PdfServices.RetrieveTransactionsFromPdf2(@"Assets\202102.pdf");

        // Assert
        // 1. count
        Assert.Equal(count, count);
        // 2. first transaction
        Assert.Equal(firstTransaction, transactions.First());
        // 3. last transaction
        Assert.Equal(lastTransaction, transactions.Last());
        // 4. 4 lines transaction
        Assert.Equal(fourLinesTransaction, transactions[14]);
        // 5. balance
        Assert.Equal(balance, transactions.Sum(x => x.Amount));
    }

    // check balance 
    [Fact]
    public void Test2()
    {
        // Arragne
        Dictionary<string, decimal> expectedBalances = new();
        expectedBalances["202102"] = 1457.75m;
        expectedBalances["202103"] = 3145.43m;
        expectedBalances["202104"] = 4891.85m;
        expectedBalances["202105"] = 2135.07m;
        expectedBalances["202107"] = 1183.83m;
        expectedBalances["202112"] = 1307.99m;
        expectedBalances["202204"] = 6367.53m;
        // Balance Brought Forward from 2021 Jan
        decimal balanceBroughtForward = 1116.20m;

        Dictionary<string, decimal> actualBalances = new();
        decimal actualBalance = balanceBroughtForward;

        // Act
        for (int year = 2021; year <= 2022; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                if (year == 2021 && month == 1)
                {
                    continue;
                }
                if (year == 2022 && month == 5)
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
                var transactions = PdfServices.RetrieveTransactionsFromPdf2(@$"Assets\{year}{monthString}.pdf");
                var sum = transactions.Sum(x => x.Amount);
                actualBalance += sum;
                actualBalances[year + monthString] = actualBalance;
            }
        }

        // Assert
        foreach (var key in expectedBalances.Keys)
        {
            Assert.Equal(expectedBalances[key], actualBalances[key]);
        }
    }
}
