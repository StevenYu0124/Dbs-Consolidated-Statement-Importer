namespace DbsConsolidatedStatementImporterTestCase;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
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
        const int count = 14;
        // 2. first transaction
        Transaction firstTransaction = new()
        {
            Date = new DateOnly(2019, 2, 2),
            Description = "Cash Accepting Machine Deposit 34002305,TAMPINES MRT 3",
            Amount = 50
        };
        // 3. last transaction
        Transaction lastTransaction = new()
        {
            Date = new DateOnly(2019, 2, 28),
            Description = "Interest Earned",
            Amount = 0.16m
        };
        // 4. 4 lines transaction
        Transaction fourLinesTransaction = new()
        {
            Date = new DateOnly(2019, 2, 13),
            Description = "Advice MEPS Receipt 90138IO00276 0016II9081530 Value Date : 13 Feb",
            Amount = 9980
        };
        // 5. balance
        const decimal balance = 9789.67m;


        // Act
        var transactions = PdfServices.RetrieveTransactionsFromPdf(@"Assets\201902.pdf");

        // Assert
        // 1. count
        Assert.Equal(count, count);
        // 2. first transaction
        Assert.Equal(firstTransaction, transactions.First());
        // 3. last transaction
        Assert.Equal(lastTransaction, transactions.Last());
        // 4. 4 lines transaction
        Assert.Equal(fourLinesTransaction, transactions[5]);
        // 5. balance
        Assert.Equal(balance, transactions.Sum(x => x.Amount));
    }

    // check balance 
    [Fact]
    public void Test2()
    {
        // Arragne
        Dictionary<string, decimal> expectedBalances = new();
        expectedBalances["202001"] = 7895.57m;
        expectedBalances["202010"] = 3046.69m;
        expectedBalances["202008"] = 9258.92m;
        expectedBalances["201912"] = 11498.17m;
        expectedBalances["201903"] = 25012.14m;
        // missing consolidated statement of 2020 Sep
        decimal MissingDifference = 1962.67m;

        Dictionary<string, decimal> actualBalances = new();
        decimal actualBalance = 0;

        // Act
        for (int year = 2019; year <= 2021; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                if (year == 2019 && month == 1)
                {
                    continue;
                }
                if (year == 2020 && month == 9)
                {
                    actualBalance += MissingDifference;
                    continue;
                }
                if (year == 2021 && month == 2)
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
