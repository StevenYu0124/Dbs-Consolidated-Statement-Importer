

namespace DbsConsolidatedStatementImporter.Services;

public static class PdfServices
{
    // pdf before 2021 Jan
    public static List<Transaction> RetrieveTransactionsFromPdf1(string path)
    {
        using PdfDocument pdfDocument = PdfDocument.Open(path);

        // config
        int queueCapacity = 3;
        int roundDigits = 1;
        double lineSpacing = 12.0;
        double yearPositionLeftLimit1 = 280;
        double yearPositionLeftLimit2 = 287;
        double dayPositionLeft = 48.2;
        double monthPositionLeft = 60.7;
        double positoveAmountPositionRight = 470.2;
        double negativeAmountPositionRight = 371.5;
        double descriptionPositionLeft = 114.4;
        string[] startKeywords = new string[]{ "Balance", "Brought", "Forward" };
        string[] endKeywords = new string[] { "Balance", "Carried", "Forward" };

        List<Transaction> transactions = new();
        int year = -1;

        foreach (Page page in pdfDocument.GetPages())
        {
            // variable
            bool start = false;
            Queue<string?> previousTexts = new(queueCapacity);
            StringBuilder stringBuilder = new();
            string? shortMonth = null;
            int day = -1;
            decimal amount = 0;
            double dateBottom = -1;
            double previousBottom = -1;

            foreach (Word word in page.GetWords())
            {
                // filter only horizontal orientation text
                if (word.TextOrientation != TextOrientation.Horizontal)
                {
                    continue;
                }

                // keep previous word for checking
                if (previousTexts.Count == queueCapacity)
                {
                    previousTexts.Dequeue();
                }
                previousTexts.Enqueue(word.Text);

                // get position
                double left = Math.Round(word.BoundingBox.Left, roundDigits);
                double right = Math.Round(word.BoundingBox.Right, roundDigits);
                double bottom = Math.Round(word.BoundingBox.Bottom, roundDigits);

                // get year
                if (year == -1 && !start && yearPositionLeftLimit1 < left && left < yearPositionLeftLimit2)
                {
                    year = int.Parse(word.Text);
                }

                // if hit the target keyword then start
                if (!start)
                {
                    if (word.Text == startKeywords[2]
                        && previousTexts.ElementAtOrDefault(queueCapacity - 3) == startKeywords[0]
                        && previousTexts.ElementAtOrDefault(queueCapacity - 2) == startKeywords[1])
                    {
                        start = true;
                    }
                    continue;
                }

                // if hit the target keyword then end
                if (start
                    && word.Text == endKeywords[2]
                    && previousTexts.ElementAtOrDefault(queueCapacity - 3) == endKeywords[0]
                    && previousTexts.ElementAtOrDefault(queueCapacity - 2) == endKeywords[1])
                {
                    AddTransaction();
                    break;
                }

                // day
                if (left == dayPositionLeft && int.TryParse(word.Text, out int newDay))
                {
                    AddTransaction();
                    day = newDay;
                    dateBottom = bottom;
                    previousBottom = bottom;
                }
                // month
                else if (left == monthPositionLeft)
                {
                    shortMonth = word.Text;
                }
                // positive amount
                else if (right == positoveAmountPositionRight && amount == 0)
                {
                    amount = decimal.Parse(word.Text);
                }
                // negative amount
                else if (right == negativeAmountPositionRight && amount == 0)
                {
                    amount = -decimal.Parse(word.Text);
                }
                // description
                else if ((descriptionPositionLeft <= left && left <= negativeAmountPositionRight)
                        && (bottom == dateBottom || bottom > previousBottom - lineSpacing))
                {
                    stringBuilder.Append(word.Text);
                    stringBuilder.Append(' ');
                    previousBottom = bottom;
                }

                // local function
                void AddTransaction()
                {
                    if (day != -1 && shortMonth != null)
                    {
                        stringBuilder.Remove(stringBuilder.Length - 1, 1);
                        Transaction? transaction = new()
                        {
                            Date = new DateOnly(year, DateTime.ParseExact(shortMonth, "MMM", CultureInfo.CurrentCulture).Month, day),
                            Description = stringBuilder.ToString(),
                            Amount = amount,
                        };
                        transactions.Add(transaction);
                    }

                    // reset
                    day = -1;
                    shortMonth = null;
                    stringBuilder = new();
                    amount = 0;
                }
            }
        }

        return transactions;
    }

    // pdf after 2021 Jan
    public static List<Transaction> RetrieveTransactionsFromPdf2(string path)
    {
        using PdfDocument pdfDocument = PdfDocument.Open(path);

        // config
        int queueCapacity = 3;
        int roundDigits = 1;
        double lineSpacing = 10.5;
        double datePositionLeft = 45.4;
        double positoveAmountPositionRight = 474;
        double negativeAmountPositionRight = 394.9;
        double descriptionPositionLeft = 113.1;
        string[] startKeywords = new string[] { "Balance", "Brought", "Forward" };
        string[] endKeywords = new string[] { "Balance", "Carried", "Forward" };

        List<Transaction> transactions = new();

        foreach (Page page in pdfDocument.GetPages())
        {
            // variable
            bool start = false;
            Queue<string?> previousTexts = new(queueCapacity);
            StringBuilder stringBuilder = new();
            string? dateString = null;
            decimal amount = 0;
            double dateBottom = -1;
            double previousBottom = -1;

            foreach (Word word in page.GetWords())
            {
                // filter only horizontal orientation text
                if (word.TextOrientation != TextOrientation.Horizontal)
                {
                    continue;
                }

                // keep previous word for checking
                if (previousTexts.Count == queueCapacity)
                {
                    previousTexts.Dequeue();
                }
                previousTexts.Enqueue(word.Text);

                // get position
                double left = Math.Round(word.BoundingBox.Left, roundDigits);
                double right = Math.Round(word.BoundingBox.Right, roundDigits);
                double bottom = Math.Round(word.BoundingBox.Bottom, roundDigits);

                // if hit the target keyword then start
                if (!start)
                {
                    if (word.Text == startKeywords[2]
                        && previousTexts.ElementAtOrDefault(queueCapacity - 3) == startKeywords[0]
                        && previousTexts.ElementAtOrDefault(queueCapacity - 2) == startKeywords[1])
                    {
                        start = true;
                    }
                    continue;
                }

                // if hit the target keyword then end
                if (start
                    && word.Text == endKeywords[2]
                    && previousTexts.ElementAtOrDefault(queueCapacity - 3) == endKeywords[0]
                    && previousTexts.ElementAtOrDefault(queueCapacity - 2) == endKeywords[1])
                {
                    AddTransaction();
                    break;
                }

                // date
                if (left == datePositionLeft)
                {
                    if (dateString != null)
                    {
                        AddTransaction();
                    }
                    dateString = word.Text;
                    dateBottom = bottom;
                    previousBottom = bottom;
                }
                // for case amount come first
                else if (right == positoveAmountPositionRight && amount != 0)
                {
                    if (amount != 0)
                    {
                        AddTransaction();
                    }
                    amount = decimal.Parse(word.Text);
                    dateBottom = bottom;
                    previousBottom = bottom;
                }
                // positive amount
                else if (right == positoveAmountPositionRight && amount == 0)
                {
                    amount = decimal.Parse(word.Text);
                }
                // negative amount
                else if (right == negativeAmountPositionRight && amount == 0)
                {
                    amount = -decimal.Parse(word.Text);
                }
                // description
                else if ((descriptionPositionLeft <= left && left <= negativeAmountPositionRight)
                        && (bottom == dateBottom || bottom > previousBottom - lineSpacing))
                {
                    stringBuilder.Append(word.Text);
                    stringBuilder.Append(' ');
                    previousBottom = bottom;
                }

                // local function
                void AddTransaction()
                {
                    if (dateString == null)
                    {
                        return;
                    }

                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    Transaction? transaction = new()
                    {
                        Date = DateOnly.ParseExact(dateString, "dd/MM/yyyy"),
                        Description = stringBuilder.ToString(),
                        Amount = amount,
                    };
                    transactions.Add(transaction);

                    // reset
                    dateString = null;
                    stringBuilder = new();
                    amount = 0;
                }
            }
        }

        return transactions;
    }
}
