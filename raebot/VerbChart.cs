using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace raebot
{
    class VerbChart
    {
        public string infinitive { get; set; }
        public string gerund { get; set; }
        public string participle { get; set; }

        private Dictionary<string, Dictionary<string, string>> indicative;
        private Dictionary<string, Dictionary<string, string>> subjunctive;

        public Dictionary<string, string> imperative;
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> chart_dict;

        public VerbChart(string html)
        {
            //init dicts
            this.indicative = new Dictionary<string, Dictionary<string, string>>();
            indicative.Add("pres", new Dictionary<string, string>());
            indicative.Add("impe", new Dictionary<string, string>());
            indicative.Add("pret", new Dictionary<string, string>());
            indicative.Add("futu", new Dictionary<string, string>());
            indicative.Add("cond", new Dictionary<string, string>());

            this.subjunctive = new Dictionary<string, Dictionary<string, string>>();
            subjunctive.Add("pres", new Dictionary<string, string>());
            subjunctive.Add("futu", new Dictionary<string, string>());
            subjunctive.Add("impe", new Dictionary<string, string>());

            this.imperative = new Dictionary<string, string>();
            imperative.Add("2s", String.Empty);
            imperative.Add("2sf", String.Empty);
            imperative.Add("2p", String.Empty);
            imperative.Add("2pf", String.Empty);

            foreach (var pair in this.indicative)
            {
                indicative[pair.Key].Add("1s", String.Empty);
                indicative[pair.Key].Add("2s", String.Empty);
                indicative[pair.Key].Add("2sf", String.Empty);
                indicative[pair.Key].Add("3s", String.Empty);
                indicative[pair.Key].Add("1p", String.Empty);
                indicative[pair.Key].Add("2p", String.Empty);
                indicative[pair.Key].Add("2pf", String.Empty);
                indicative[pair.Key].Add("3p", String.Empty);
            }

            foreach (var pair in this.subjunctive)
            {
                subjunctive[pair.Key].Add("1s", String.Empty);
                subjunctive[pair.Key].Add("2s", String.Empty);
                subjunctive[pair.Key].Add("2sf", String.Empty);
                subjunctive[pair.Key].Add("3s", String.Empty);
                subjunctive[pair.Key].Add("1p", String.Empty);
                subjunctive[pair.Key].Add("2p", String.Empty);
                subjunctive[pair.Key].Add("2pf", String.Empty);
                subjunctive[pair.Key].Add("3p", String.Empty);
            }

            this.chart_dict = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            chart_dict.Add("indi", this.indicative);
            chart_dict.Add("subj", this.subjunctive);


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;

            this.infinitive = root.Descendants("tr").ToList()[2].Descendants("td").ToList()[3].InnerText;
            this.gerund = root.Descendants("tr").ToList()[2].Descendants("td").ToList()[4].InnerText;
            this.participle = root.Descendants("tr").ToList()[4].Descendants("td").ToList()[3].InnerText;

            //indicative present
            this.indicative["pres"]["1s"] = root.Descendants("tr").ToList()[7].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["2s"] = root.Descendants("tr").ToList()[8].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["2sf"] = root.Descendants("tr").ToList()[9].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["3s"] = root.Descendants("tr").ToList()[10].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["1p"] = root.Descendants("tr").ToList()[11].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["2p"] = root.Descendants("tr").ToList()[12].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["2pf"] = root.Descendants("tr").ToList()[13].Descendants("td").ToList()[3].InnerText;
            this.indicative["pres"]["3p"] = root.Descendants("tr").ToList()[14].Descendants("td").ToList()[3].InnerText;

            //indicative imperfect
            this.indicative["impe"]["1s"] = root.Descendants("tr").ToList()[7].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["2s"] = root.Descendants("tr").ToList()[8].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["2sf"] = root.Descendants("tr").ToList()[9].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["3s"] = root.Descendants("tr").ToList()[10].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["1p"] = root.Descendants("tr").ToList()[11].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["2p"] = root.Descendants("tr").ToList()[12].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["2pf"] = root.Descendants("tr").ToList()[13].Descendants("td").ToList()[4].InnerText;
            this.indicative["impe"]["3p"] = root.Descendants("tr").ToList()[14].Descendants("td").ToList()[4].InnerText;

            //indicative preterite
            this.indicative["pret"]["1s"] = root.Descendants("tr").ToList()[16].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["2s"] = root.Descendants("tr").ToList()[17].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["2sf"] = root.Descendants("tr").ToList()[18].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["3s"] = root.Descendants("tr").ToList()[19].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["1p"] = root.Descendants("tr").ToList()[20].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["2p"] = root.Descendants("tr").ToList()[21].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["2pf"] = root.Descendants("tr").ToList()[22].Descendants("td").ToList()[3].InnerText;
            this.indicative["pret"]["3p"] = root.Descendants("tr").ToList()[23].Descendants("td").ToList()[3].InnerText;

            //indicative future
            this.indicative["futu"]["1s"] = root.Descendants("tr").ToList()[16].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["2s"] = root.Descendants("tr").ToList()[17].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["2sf"] = root.Descendants("tr").ToList()[18].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["3s"] = root.Descendants("tr").ToList()[19].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["1p"] = root.Descendants("tr").ToList()[20].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["2p"] = root.Descendants("tr").ToList()[21].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["2pf"] = root.Descendants("tr").ToList()[22].Descendants("td").ToList()[4].InnerText;
            this.indicative["futu"]["3p"] = root.Descendants("tr").ToList()[23].Descendants("td").ToList()[4].InnerText;

            //indicative conditional
            this.indicative["cond"]["1s"] = root.Descendants("tr").ToList()[25].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["2s"] = root.Descendants("tr").ToList()[26].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["2sf"] = root.Descendants("tr").ToList()[27].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["3s"] = root.Descendants("tr").ToList()[28].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["1p"] = root.Descendants("tr").ToList()[29].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["2p"] = root.Descendants("tr").ToList()[30].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["2pf"] = root.Descendants("tr").ToList()[31].Descendants("td").ToList()[3].InnerText;
            this.indicative["cond"]["3p"] = root.Descendants("tr").ToList()[32].Descendants("td").ToList()[3].InnerText;

            //subjunctive present
            this.subjunctive["pres"]["1s"] = root.Descendants("tr").ToList()[35].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["2s"] = root.Descendants("tr").ToList()[36].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["2sf"] = root.Descendants("tr").ToList()[37].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["3s"] = root.Descendants("tr").ToList()[38].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["1p"] = root.Descendants("tr").ToList()[39].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["2p"] = root.Descendants("tr").ToList()[40].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["2pf"] = root.Descendants("tr").ToList()[41].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["pres"]["3p"] = root.Descendants("tr").ToList()[42].Descendants("td").ToList()[3].InnerText;

            //subjunctive future
            this.subjunctive["futu"]["1s"] = root.Descendants("tr").ToList()[35].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["2s"] = root.Descendants("tr").ToList()[36].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["2sf"] = root.Descendants("tr").ToList()[37].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["3s"] = root.Descendants("tr").ToList()[38].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["1p"] = root.Descendants("tr").ToList()[39].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["2p"] = root.Descendants("tr").ToList()[40].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["2pf"] = root.Descendants("tr").ToList()[41].Descendants("td").ToList()[4].InnerText;
            this.subjunctive["futu"]["3p"] = root.Descendants("tr").ToList()[42].Descendants("td").ToList()[4].InnerText;

            //subjunctive imperfect
            this.subjunctive["impe"]["1s"] = root.Descendants("tr").ToList()[44].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["2s"] = root.Descendants("tr").ToList()[45].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["2sf"] = root.Descendants("tr").ToList()[46].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["3s"] = root.Descendants("tr").ToList()[47].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["1p"] = root.Descendants("tr").ToList()[48].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["2p"] = root.Descendants("tr").ToList()[49].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["2pf"] = root.Descendants("tr").ToList()[50].Descendants("td").ToList()[3].InnerText;
            this.subjunctive["impe"]["3p"] = root.Descendants("tr").ToList()[51].Descendants("td").ToList()[3].InnerText;

            //imperative
            this.imperative["2s"] = root.Descendants("tr").ToList()[54].Descendants("td").ToList()[3].InnerText;
            this.imperative["2sf"] = root.Descendants("tr").ToList()[55].Descendants("td").ToList()[3].InnerText;
            this.imperative["2p"] = root.Descendants("tr").ToList()[56].Descendants("td").ToList()[3].InnerText;
            this.imperative["2pf"] = root.Descendants("tr").ToList()[57].Descendants("td").ToList()[3].InnerText;
        }
    }
}
