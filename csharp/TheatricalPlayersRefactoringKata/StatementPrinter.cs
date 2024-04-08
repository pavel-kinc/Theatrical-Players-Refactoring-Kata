using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TheatricalPlayersRefactoringKata
{

    public class StatementPrinter
    {
        public class PerformanceResultModel
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }

            public int PlayAmount { get; set; }
            public int Audience { get; set; }
            public int VolumeCredits { get; set; }
        }

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var list = new List<PerformanceResultModel>();
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = CalculateAmount(perf, play);

                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                list.Add(new PerformanceResultModel
                {
                    Name = play.Name,
                    Amount = Convert.ToDecimal(thisAmount / 100),
                    PlayAmount = thisAmount,
                    Audience = perf.Audience,
                    VolumeCredits = CalculateVolumeCredits(perf, play)
                });
            }
            var volumeCredits = list.Sum(i => i.VolumeCredits);
            var totalAmount = list.Sum(i => i.PlayAmount);
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += String.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        private static int CalculateVolumeCredits(Performance perf, Play play)
        {
            // add volume credits
            var volumeCredits2 = Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type) volumeCredits2 += (int)Math.Floor((decimal)perf.Audience / 5);
            return volumeCredits2;
        }

        private int CalculateAmount(Performance perf, Play play)
        {
            int thisAmount;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = CalculateTragedyAmount(perf);
                    break;
                case "comedy":
                    thisAmount = CalculateComedyAmount(perf);
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return thisAmount;
        }

        public int CalculateTragedyAmount(Performance perf)
        {
            var amount = 40000;
            if (perf.Audience > 30)
            {
                amount += 1000 * (perf.Audience - 30);
            }
            return amount;
        }

        public int CalculateComedyAmount(Performance perf)
        {
            var amount = 30000;
            if (perf.Audience > 20)
            {
                amount += 10000 + 500 * (perf.Audience - 20);
            }
            amount += 300 * perf.Audience;
            return amount;
        }
    }
}
