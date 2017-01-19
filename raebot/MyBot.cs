using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace raebot
{
    class MyBot
    {
        DiscordClient discord;
        WebRequest rae_site;
        bool muted;
        
        public MyBot()
        {
            muted = false;
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
            });

            var commands = discord.GetService<CommandService>();

            commands.CreateCommand("rae-probar")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("RAE Bot está en línea y funcionando.");
                });

            commands.CreateCommand("rae-silenciar")
                .Do(async (e) =>
                {
                    if (e.User.ServerPermissions.Administrator)
                    {
                        this.muted = true;
                        await e.Channel.SendMessage("RAE Bot ha sido silenciado.");
                    }
                });

            commands.CreateCommand("rae-iniciar")
                .Do(async (e) =>
                {
                    if (e.User.ServerPermissions.Administrator)
                    {
                        this.muted = false;
                        await e.Channel.SendMessage("RAE Bot ya no está silenciado.");
                    }
                });

            commands.CreateCommand("rae")
                .Parameter("palabra", ParameterType.Required)
                .Do(async (e) =>
                {
                    
                    try
                    {
                        if (!muted)
                        {
                            //get search results json object from RAE site
                            rae_site = WebRequest.Create(String.Format("https://dle.rae.es/data/search?w={0}", e.GetArg("palabra")));
                            rae_site.Headers.Add("Authorization", "Basic cDY4MkpnaFMzOmFHZlVkQ2lFNDM0");
                            rae_site.Credentials = CredentialCache.DefaultCredentials;
                            WebResponse rae_repuesta = rae_site.GetResponse();
                            Stream dataStream = rae_repuesta.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();
                            List<string> ids = new List<string>(); //each result has an id, store here

                            //deserialize JSON
                            SearchResult sr = JsonConvert.DeserializeObject<SearchResult>(responseFromServer);

                            //sort by if result contains search query, probably a much better way but EH
                            var sr_sort = sr.res.OrderByDescending(resu => resu["header"].Replace(".", "").Contains(e.GetArg("palabra")));

                            //fetch response is html, store it all here
                            string all_html = String.Empty;

                            foreach (Dictionary<string, string> result in sr_sort)
                            {
                                //fetch definitions for each id
                                string currentid = result["id"];
                                ids.Add(currentid);
                                rae_site = WebRequest.Create(String.Format("https://dle.rae.es/data/fetch?id={0}", currentid));
                                rae_site.Headers.Add("Authorization", "Basic cDY4MkpnaFMzOmFHZlVkQ2lFNDM0");
                                rae_site.Credentials = CredentialCache.DefaultCredentials;
                                rae_repuesta = rae_site.GetResponse();
                                dataStream = rae_repuesta.GetResponseStream();
                                reader = new StreamReader(dataStream);
                                responseFromServer = reader.ReadToEnd();
                                all_html += responseFromServer;
                                //final_output += responseFromServer;
                            }

                            if (sr.res.Count == 0)
                            {
                                await e.Channel.SendMessage(String.Format("No existe la palabra \"{0}\"...", e.GetArg("palabra")));
                            }
                            else
                            {
                                //Format html response for discord
                                all_html = all_html.Replace("*", "").Replace("_", "");
                                var final_output = MyBot.FormatDefinitionShort(all_html, ids);
                                List<string> Messages = new List<string>();

                                int curr_message = 0;
                                int curr_size = 0;
                                Messages.Add(String.Empty);

                                //break up message if exceeds 2000 characters
                                foreach (var line in final_output)
                                {
                                    if (curr_size + line.Length > 2000)
                                    {
                                        curr_message += 1;
                                        curr_size = 0;
                                        Messages.Add(String.Empty);
                                    }

                                    curr_size += line.Length;
                                    Messages[curr_message] += line;
                                }



                                foreach (var mess in Messages)
                                {
                                    await e.Channel.SendMessage(mess);
                                }
                            }
                        }
                    }
                    catch (Exception exc) //shitty error checking
                    {
                        Console.WriteLine(exc.StackTrace);
                        Console.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToString("d MMM h:mm:ss tt"), exc.Message));
                        Console.WriteLine(String.Format("\tCommand issued: !rae {0}", e.Args));
                    }
                });

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("PLACEHOLDER_TOKEN", TokenType.Bot);
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static List<string> FormatDefinition(string def_html)
        {
            List<string> final = new List<string>(); //lines of text, gets returned

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(def_html);
            var root = doc.DocumentNode;
            
            var articles = root
                .Descendants("article").ToList();

            //each result is in an <article></article>
            foreach (var article in articles)
            {
                //get header
                var header = article
                    .Descendants("header").ToList();
                
                if (header.Count == 1)
                {
                    final.Add(String.Format("__**{0}**__\n\n", header[0].InnerText.Trim().ToUpper()).Replace(".", ""));
                }
                

                //etymology
                var ety1 = article
                    .Descendants("p")
                    .Where(x =>
                    x.Attributes.Contains("class")
                        &&
                    x.Attributes["class"].Value.Contains("n2")
                ).ToList();

                if (ety1.Count == 1)
                {
                    var ety = ety1[0];
                    List<string> etys = new List<string>();
                    foreach (var node in ety.Descendants().Where(x => x.ParentNode == ety && !x.InnerText.Trim().Equals(String.Empty)))
                    {
                        //italicize
                        if (new List<string> { "i", "em" }.Contains(node.Name))
                        {
                            etys.Add(String.Format("*{0}*", node.InnerText.Trim()));
                        }
                        else
                        {
                            etys.Add(node.InnerText.Trim());
                        }
                    }
                    final.Add(String.Join(" ", etys) + "\n\n");
                }

                

                //definitions 
                var defs = article
                    .Descendants("p")
                    .Where(x =>
                    x.Attributes.Contains("class")
                        &&
                    x.Attributes["class"].Value.Contains("j")
                ).ToList();

                foreach (var def in defs)
                {
                    List<string> def_part = new List<string>();
                    foreach (var node in def.Descendants().Where(x => x.ParentNode == def && !x.InnerText.Trim().Equals(String.Empty)))
                    {
                        //bold and underline abbreviations
                        if (new List<string> { "abbr" }.Contains(node.Name))
                        {
                            def_part.Add(String.Format("__**{0}**__", node.InnerText.Trim()));
                        }
                        else if (new List<string> { "span" }.Contains(node.Name) && (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("h")))
                        {
                            def_part.Add(String.Format("*{0}*", node.InnerText.Trim()));
                        }
                        else
                        {
                            def_part.Add(node.InnerText.Trim());
                        }
                    }
                    final.Add(String.Join(" ", def_part) + "\n\n");
                }
            }

            return final;
        }

        private static List<string> FormatDefinitionShort(string def_html, List<string> ids)
        {
            List<string> final = new List<string>();
            bool terminator = false;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(def_html);
            var root = doc.DocumentNode;

            var articles = root
                .Descendants("article").ToList();
            int def_count = 0;

            foreach (var article in articles)
            {
                //get header
                var header = article
                    .Descendants("header").ToList();

                if (header.Count == 1 && def_count < 5)
                {
                    final.Add(String.Format("__**{0}**__\n\n", header[0].InnerText.Trim().ToUpper()).Replace(".", ""));
                }


                //etymology
                var ety1 = article
                    .Descendants("p")
                    .Where(x =>
                    x.Attributes.Contains("class")
                        &&
                    x.Attributes["class"].Value.Contains("n2")
                ).ToList();

                if (ety1.Count == 1 && def_count < 5)
                {
                    var ety = ety1[0];
                    List<string> etys = new List<string>();
                    foreach (var node in ety.Descendants().Where(x => x.ParentNode == ety && !x.InnerText.Trim().Equals(String.Empty)))
                    {
                        if (new List<string> { "i", "em" }.Contains(node.Name))
                        {
                            etys.Add(String.Format("*{0}*", node.InnerText.Trim()));
                        }
                        else
                        {
                            etys.Add(node.InnerText.Trim());
                        }
                    }
                    final.Add(String.Join(" ", etys) + "\n\n");
                }


                //definitions 
                var defs = article
                    .Descendants("p")
                    .Where(x =>
                    x.Attributes.Contains("class")
                        &&
                    x.Attributes["class"].Value.Contains("j")
                ).ToList();

                foreach (var def in defs)
                {
                    List<string> def_part = new List<string>();
                    foreach (var node in def.Descendants().Where(x => x.ParentNode == def && !x.InnerText.Trim().Equals(String.Empty)))
                    {
                        if (new List<string> { "abbr" }.Contains(node.Name))
                        {
                            def_part.Add(String.Format("__**{0}**__", node.InnerText.Trim()));
                        }
                        else if (new List<string> { "span" }.Contains(node.Name) && (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("h")))
                        {
                            def_part.Add(String.Format("*{0}*", node.InnerText.Trim()));
                        }
                        else
                        {
                            def_part.Add(node.InnerText.Trim());
                        }
                    }

                    //limit definitions to 5
                    if (def_count < 5)
                    {
                        final.Add(String.Join(" ", def_part) + "\n\n");
                        def_count++;
                    }
                    else
                    {
                        if (!terminator)
                        {
                            final.Add(String.Format("Para ver más: http://dle.rae.es/?id={0}", String.Join("|", ids)));
                            terminator = true;
                        }
                    }
                }
            }

            return final;
        }
    }
}
