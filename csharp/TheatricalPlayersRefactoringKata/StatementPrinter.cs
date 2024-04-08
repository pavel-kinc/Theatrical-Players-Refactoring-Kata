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
            public decimal PrintAmount => Convert.ToDecimal(PlayAmount / 100);
            public int PlayAmount { get; set; }
            public int Audience { get; set; }
            public int VolumeCredits { get; set; }
        }

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var report = GetPerformaceModels(invoice, plays);
            var volumeCredits = report.Sum(i => i.VolumeCredits);
            var totalAmount = Convert.ToDecimal(report.Sum(i => i.PlayAmount) / 100);

            CultureInfo cultureInfo = new("en-US");
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            foreach (var item in report)
            {
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", item.Name, item.PrintAmount, item.Audience);
            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", totalAmount);
            result += String.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        private List<PerformanceResultModel> GetPerformaceModels(Invoice invoice, Dictionary<string, Play> plays)
        {
            var list = new List<PerformanceResultModel>();
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = CalculateAmount(perf, play);
                list.Add(new PerformanceResultModel
                {
                    Name = play.Name,
                    PlayAmount = thisAmount,
                    Audience = perf.Audience,
                    VolumeCredits = CalculateVolumeCredits(perf, play)
                });
            }

            return list;
        }

        private static int CalculateVolumeCredits(Performance perf, Play play)
        {
            var volumeCredits2 = Math.Max(perf.Audience - 30, 0);
            if ("comedy" == play.Type)
            {
                volumeCredits2 += (int)Math.Floor((decimal)perf.Audience / 5);
            }
            return volumeCredits2;
        }

        private int CalculateAmount(Performance perf, Play play)
        {
            var amount = play.Type switch
            {
                "tragedy" => CalculateTragedyAmount(perf),
                "comedy" => CalculateComedyAmount(perf),
                _ => throw new Exception("unknown type: " + play.Type),
            };
            return amount;
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
