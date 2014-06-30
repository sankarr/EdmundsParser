using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using CsvHelper;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WindowsFormsApplication3
{
    public partial class Edmunds : Form
    {
        private  int maxnumposts;
        class data
        {
            public string un { set; get; }
            public string date { set; get; }
            public string message { set; get; }
        }

        public Edmunds()
        {
            InitializeComponent();
        }

        class Data
        {
            string un { set; get; }
            string date { set; get; }
            string message { set; get; }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            getData();
            

            //Console.WriteLine("{0} {1} {2}", users.Count, dates.Count, comments.Count);

            
        }

        void findUniqueWords()//string [] messages)
        {

            string[] files = File.ReadAllLines("edmunds_extraction.csv");

            var stop = new string[] { "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself",
                "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them",
                "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", 
                "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", 
                "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about", "against", "between", "into", "through",
                "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", 
                "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", 
                "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", 
                "just", "don", "should", "now" };

            HashSet<string> stopwords = new HashSet<string>(stop);
            /*
            foreach (var item in files)
            {
                Console.WriteLine(item.Split(','));
            }*/


            string[] messages = GetPosts();

            

            //var uniqueWords = new HashSet<string>();
            // this will contain each pair of words
            Dictionary<Tuple<string, string>, int> twowords = new Dictionary<Tuple<string, string>, int>();
            
            // This will containt each word
            Dictionary<string, int> onewords = new Dictionary<string,int>();
            
            //list of all unique words
            HashSet <string> listofwords = new HashSet<string>();
            
            //each message hashed
            var posts = new List<HashSet<string>>();

            Stopwatch watch = new Stopwatch();
            watch.Start();
                    
            //process every message
            foreach (var sentence in messages)
            {
                HashSet<string> listofwords_in_message = new HashSet<string>();
                String noPunc = Regex.Replace(sentence, "[.,!?]", " ");
                String stripped = Regex.Replace(noPunc, "\\p{P}", "");

                string[] splitWords = stripped.Split(' ');

                //for each of the split words
                foreach (var item in splitWords)
                {

                    if (!stopwords.Contains(item.ToLower()) && item != "")
                    {
                        //if (item.ToLower() == "fast330i")
                        //    throw new Exception("Stop here");


                        if (listofwords_in_message.Add(item.ToLower()))
                        {
                            if (!onewords.ContainsKey(item.ToLower()))
                            {
                                onewords.Add(item.ToLower(), 1);
                            }
                            else
                            {
                                onewords[item.ToLower()]++;
                            }
                        }
                        
                    }
                }

                //for handling pair combinations
                var listofwords_in_message_array = listofwords_in_message.ToArray();
                int count = 0;
                for (int i = 0; i < listofwords_in_message_array.Length; i++)
                {
                    for (int j = 0; j < listofwords_in_message_array.Length; j++)
                    {
                        if (listofwords_in_message_array[i] != listofwords_in_message_array[j])
                        {
                            count++;
                            //see if words exists forward or backward since BMW pretty is same as pretty BMW
                            bool forward = twowords.ContainsKey(Tuple.Create(listofwords_in_message_array[i],listofwords_in_message_array[j]));
                            bool reverse = twowords.ContainsKey(Tuple.Create(listofwords_in_message_array[j],listofwords_in_message_array[i]));


                            if (!forward && !reverse)
                            {
                                twowords.Add(Tuple.Create(listofwords_in_message_array[i], listofwords_in_message_array[j]), 1);
                            }
                            else if (reverse)
                            {
                                twowords[Tuple.Create(listofwords_in_message_array[j], listofwords_in_message_array[i])]++;
                            }
                            else
                            {
                                twowords[Tuple.Create(listofwords_in_message_array[i], listofwords_in_message_array[j])]++;
                            }

                        }
                        
                    }
                    
                }

                //add hashed list of messages
                posts.Add(listofwords_in_message);
                //String stripped = input.replaceAll("\\p{Punct}+", "");
            }

            watch.Stop();
            Console.WriteLine("{0}", watch.ElapsedMilliseconds);
            /*
            foreach (var item in listofwords)
            {
                onewords.Add(item, 0);
            }

            foreach (var item in onewords.Keys.ToArray())
            {
                foreach (var item2 in posts)
                {
                    if (item2.Contains(item))
                    {
                        onewords[item]++;
                    }

                }

            }

            int j = 0;
            foreach (var item in listofwords)
            {
                foreach (var item2 in listofwords)
                {
                    if (item != item2 && !twowords.Keys.Contains(Tuple.Create(item2, item)))
                    {
                        j++;
                        twowords.Add(Tuple.Create(item, item2), 0);

                        foreach (var item3 in posts)
                        {
                            if (item3.Contains(item) && item3.Contains(item2))
                            {
                                twowords[Tuple.Create(item, item2)]++;
                            }
                        }
                    }
                }
            }*/
            /*
            using (StreamWriter outh = new StreamWriter("text.txt"))
            {
                foreach (var item in onewords.Keys)
                {
                    outh.WriteLine("{0},{1}", item, onewords[item]);
                }

                foreach (var item in twowords.Keys)
                {
                    outh.WriteLine("{0},{1}, {2}", item.Item1, item.Item2, twowords[item]);
                }
            }*/

            using (StreamWriter outh = new StreamWriter("word_post.csv"))
            {
                foreach (var item in onewords.OrderByDescending(k => k.Value))
                {
                    outh.WriteLine("{0},{1}", item.Key, item.Value);
                }
            }
            using (StreamWriter outh = new StreamWriter("word_pair_post.csv"))
            {
                foreach (var item in twowords.OrderByDescending(k=>k.Value))
                {
                    outh.WriteLine("{0},{1}, {2}", item.Key.Item1, item.Key.Item2, item.Value);
                }
            }

            MessageBox.Show("Wrote word_post.csv and word_pair_post.csv");
        }

        private string[] GetPosts()
        {

            CsvReader read = new CsvReader(new StreamReader("edmunds_extraction.csv"));
            read.Configuration.HasHeaderRecord = false;

            //var a = read.GetRecord<data>();


            var records = read.GetRecords<data>().ToArray();
            //var messages=new string[444];
            string[] messages = new string[records.Count()];

            for (int i = 0; i < records.Length; i++)
            {
                messages[i] = records[i].message;
            }
            return messages;
        }

        private void getData()
        {
            string value="";
            int numberofposts = 0;
            if (Input.InputBox("How Many Posts do you want", "How many posts do you want?", ref value) == DialogResult.OK)
            {

                try
                {
                    maxnumposts =  int.Parse(value);
                }
                catch (Exception err)
                {

                    MessageBox.Show(err.Message);
                    return;
                }
            }
            else
            {
                return;
            }
           
            bool stop = false;
            var url = toolStripTextBox1.Text;
            while (!stop)
            {

                
                var Webget = new HtmlWeb();
                var doc = Webget.Load(url);


                var users = doc.DocumentNode.SelectNodes("//a[@class='Username']");
                var dates = doc.DocumentNode.SelectNodes("//time");
                var comments = doc.DocumentNode.SelectNodes("//div[@class='Message']");
                var nextlink = doc.DocumentNode.SelectNodes("//a[@class='Next']");



                using (var outh = new CsvWriter(new StreamWriter("edmunds_extraction.csv", true)))
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        outh.WriteField(users[i].InnerText);
                        outh.WriteField(dates[i].GetAttributeValue("title", "none"));

                        outh.WriteField(comments[i].InnerText.TrimStart(new char[] { '\n', ' ' }));
                        outh.NextRecord();

                        //outh.WriteLine("\"{0}\",\"{1}\",\"{2}\"", users[i].InnerText, dates[i].GetAttributeValue("title", "none"),
                        //    System.Security.SecurityElement.Escape( System.Web.HttpUtility.HtmlDecode(comments[i].InnerHtml.TrimStart(new char[]{'\n',' '}))));
                    }
                }

                numberofposts += users.Count;

                url = nextlink[0].GetAttributeValue("href", "none");

                if (url == "none")
                    stop = true;

                if (numberofposts > maxnumposts)
                    stop = true;

                /*
                using (StreamWriter outh = new StreamWriter("out.csv"))
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        outh.WriteLine("\"{0}\",\"{1}\",\"\"{2}\"\"", users[i].InnerText, dates[i].GetAttributeValue("title", "none"),
                            comments[i].InnerText.TrimStart(new char[] { '\n', ' ' }));
                        //outh.WriteLine("\"{0}\",\"{1}\",\"{2}\"", users[i].InnerText, dates[i].GetAttributeValue("title", "none"),
                        //    System.Security.SecurityElement.Escape( System.Web.HttpUtility.HtmlDecode(comments[i].InnerHtml.TrimStart(new char[]{'\n',' '}))));
                    }

                }*/
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findUniqueWords();
            
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

            webBrowser1.Navigate(toolStripTextBox1.Text);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            getData();

        }

       // private List<string> termstosearch = null;
        Dictionary<string, string> termstosearchone = null;
        Dictionary<Tuple<string, string>,string> termstosearchtwo = null;

        private void tabPage3_Click(object sender, EventArgs e)
        {
            updateTab3();

        
        }

        private void updateTab3()
        {
            if (termstosearchone == null)
            {
                if (!File.Exists("word_post.csv"))
                {
                    MessageBox.Show("Run the Parse First");
                    return;
                }

                var lines = File.ReadAllLines("word_post.csv");
                termstosearchone = new Dictionary<string, string>();

                foreach (var item in lines)
                {
                    termstosearchone.Add(item.Split(',')[0], item.Split(',')[1]);
                }

                lines = File.ReadAllLines("word_pair_post.csv");
                termstosearchtwo = new Dictionary<Tuple<string,string>,string>();

                foreach (var item in lines)
                {
                    termstosearchtwo.Add(Tuple.Create(item.Split(',')[0], item.Split(',')[1]), item.Split(',')[2]);
                }


                var source = new AutoCompleteStringCollection();
                source.AddRange(termstosearchone.Keys.ToArray());

                textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox1.AutoCompleteCustomSource = source;

                textBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox2.AutoCompleteCustomSource = source;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {


            try
            {
               

                var var1 = termstosearchone[textBox1.Text];
                
                var var2 = termstosearchone[textBox2.Text];
                
                var var1var2 = termstosearchtwo[Tuple.Create(textBox1.Text, textBox2.Text)];

                if (var1var2 == null)
                {
                    var1var2 = termstosearchtwo[Tuple.Create(textBox2.Text, textBox1.Text)];
                }

                textBox3.Text = "";
                textBox3.Text += textBox1.Text + "," + var1 + Environment.NewLine;
                textBox3.Text += textBox2.Text + "," + var1 + Environment.NewLine;
                textBox3.Text += textBox1.Text + "," + textBox2.Text + "," + var1var2 + Environment.NewLine;


            }
            catch (Exception err)
            {

                textBox3.Text = err.Message;
            }
            
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateTab3();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] messages = GetPosts();
            List<string> results = new List<string>();
            List<string> short_results = new List<string>();
            string value="";
            int sentence_length=0;
            if (Input.InputBox("How many words do you want in sentence?", "How many letters do you want in sentence?", ref value)==System.Windows.Forms.DialogResult.OK)
            {

                try
                {
                    sentence_length = int.Parse(value);
                }
                catch (Exception err)
                {

                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                return;
            }

            foreach (var item in messages)
            {


                var lower_sentence = item.ToLower();
                if(lower_sentence.Contains(textBox4.Text.ToLower()) && lower_sentence.Contains(textBox5.Text.ToLower()))
                {
                    results.Add(item);
                }
                
            }


            // get only a certain number of words
            foreach (var item in results)
            {
                

                MatchCollection wordColl = Regex.Matches(item, @"[\w]+");
                
                //find the last word and copy from 0 to that point
                int last_word_index = 0;
                if (wordColl.Count > sentence_length)
                {
                    last_word_index = wordColl[sentence_length].Index;
                }

                short_results.Add(item.Substring(0, last_word_index));
                

                
                
            }

            using (StreamWriter outh = new StreamWriter("limit_post.csv"))
            {
                foreach (var item in short_results)
                {
                    outh.WriteLine(item);
                }
            }

            MessageBox.Show("wrote to limit_post.csv");
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                MessageBox.Show("Click Refresh First if you updated data");
            }

        }
    }
}
