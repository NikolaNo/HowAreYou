using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorLinks.Data
{
    public class SentimentAI
    {
        public Guid Id { get; set; }

        public string UserID { get; set; }

        public string Sentiment { get; set; }

        public float SentimentScore { get; set; }

        public DateTime Date { get; set; }

    }
}
