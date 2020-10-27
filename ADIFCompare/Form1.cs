using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADIFCompare
{
    public partial class Form1 : Form
    {
        Dictionary<String, int> list1 = new Dictionary<string, int>();
        Dictionary<String, int> list2 = new Dictionary<string, int>();
        Dictionary<String, int> listchk = new Dictionary<string, int>();
        int listchkTotal = 0;
        String filename1 = "Unknown";
        String filename2 = "Unknown";
        String callsignFilter = "";
        public Form1()
        {
            InitializeComponent();
            /*
            listchk.Add("20150209/22:?? 10M JT65 KE7LHT",0);
            listchk.Add("20141113/13:?? 40M JT65 N2VWP", 0);
            listchk.Add("20140810/12:?? 40M JT65 N2VQ", 0);
            listchk.Add("20140304/15:?? 10M JT65 PD9HJ", 0);
            listchk.Add("20140125/14:?? 40M JT65 VE3LFS", 0);
            listchk.Add("20140118/21:?? 17M JT65 W2KWO", 0);
            listchk.Add("20140112/13:?? 15M JT65 NR9Q", 0);
            listchk.Add("20140108/15:?? 15M JT65 KS2F", 0);
            listchk.Add("20140101/18:?? 17M JT65 DE9ZZZ", 0);
            listchk.Add("20131122/22:?? 15M JT65 JA1KXQ", 0);
            listchk.Add("20131122/11:?? 80M JT65 W9ATB", 0);
            listchk.Add("20131117/13:?? 40M JT65 W9ATB", 0);
            listchk.Add("20131115/14:?? 40M JT65 N9OJC", 0);
            listchk.Add("20131109/14:?? 20M JT65 KJ4RKB", 0);
            listchk.Add("20160119/14:?? 40M JT65 KM5LY", 0);
            listchk.Add("20151226/15:?? 20M JT65 KK4UQ", 0);
            listchk.Add("20150922/16:?? 15M JT9 F1IWH", 0);
            listchk.Add("20150724/04:?? 40M JT65 AF6PM", 0);
            listchk.Add("20150601/21:?? 20M JT9 UT7IS", 0);
            listchk.Add("20150529/20:?? 17M JT9 F5RRS", 0);
            listchk.Add("20150403/04:?? 20M JT65 KD7VDB", 0);
            listchk.Add("20150111/13:?? 40M JT9 K9HIO", 0);
            listchk.Add("20140701/16:?? 15M JT65 AE7JP", 0);
            listchk.Add("20140306/15:?? 10M JT65 HA5WA", 0);
            listchk.Add("20140101/13:?? 17M JT65 KN3A", 0);
            listchk.Add("20131222/19:?? 10M JT65 KG7AV", 0);
            listchk.Add("20131028/14:?? 20M JT65 WA7AG", 0);
            listchk.Add("20131019/15:?? 20M JT65 NA6L", 0);
            listchk.Add("20131215/20:?? 17M JT65 VA3WLD", 0);
            */
            /*
            20140118 / 21:?? 17M  JT65 W2KWO  Duplicate? Why is this a dup?
            */

        }


        int Digits(String s)
        {
            int n = 0;
            char[] array = s.ToCharArray();
            for (n = 0; n < array.Count(); ++n)
            {
                char c = array[n];
                if (c >= '0' && c <= '9') ++n;
            }
            return n;
        }

        void AddToList(String filename, Dictionary<String, int> list, Label label)
        {
            String line = "";
            String tmpline = "";
            String call = "";
            String qsodate = "";
            String timeon = "";
            String band = "";
            String mode = "";
            String submode = "";
            String stationCallSign = "";
            bool eqsl_qsl_rcvd = false;
            bool lotw_qsl_rcvd = false;
            bool qsl_rcvd = false;
            Char[] delims = { '>', ':' };
            int nlines = 0;
            int ndups = 0;
            StreamWriter writer = new StreamWriter(filename + ".txt");
            Stopwatch timer1 = new Stopwatch();
            bool async = false;
            //using (var reader = File.OpenText(filename))
            StreamReader reader;
            if (async)
            {
                FileStream freader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true);
                reader = new StreamReader(freader);
            }
            else
            {
                reader = File.OpenText(filename);
            }

            using (reader)
            {
                timer1.Start();
                while ((tmpline = reader.ReadLine()) != null)
                {
                    tmpline = tmpline.ToUpper();
                    if (reader.EndOfStream && tmpline.Length == 0)
                    {
                        continue;
                    }
                    if (tmpline.Length < 3) continue;
                    if (tmpline.Substring(0, 1) == "#") continue;
                    if (tmpline.Contains("ADIF_VERS:")) continue;
                    line += tmpline;
                    if (!line.Contains("<EOR")) continue;
                    //richTextBox1.AppendText(line);
                    if (line.Contains("<CALL:"))
                    {
                        //continue;
                        //Regex r = new Regex(@"", RegexOptions.None, TimeSpan.FromMilliseconds(150));
                        //if (line.Contains("NY1S"))
                        //    richTextBox1.AppendText("NY1S");
                        int nn = 0;
                        if (line.Contains("XE2/W7ZV"))
                        {
                            ++nn;
                        }
                        String[] tokens = line.Split('<');
                        int n = 0;

                        foreach (String s in tokens)
                        {
                            String[] tokens3 = s.Split(':');
                            String keyword = "";
                            if (tokens3.Length > 0) keyword = tokens3[0];
                            switch (keyword)
                            {

                                case "CALL":
                                    ++n;
                                    String[] tokens4 = s.Split(delims);
                                    call = tokens4[2].TrimStart().TrimEnd();
                                    ++nlines;
                                    if ((nlines % 100) == 0) {
                                        label.Text = nlines.ToString();
                                        Application.DoEvents();
                                    }
                                    break;
                                case "STATION_CALLSIGN":
                                    tokens4 = s.Split(delims);
                                    stationCallSign = tokens4[2].TrimStart().TrimEnd();
                                    break;
                                case "TIME_ON":
                                    ++n;
                                    String[] tokens5 = s.Split(delims);
                                    timeon = tokens5[2].TrimStart().Substring(0, 4);
                                    if (checkBoxTimeTrim.Checked) timeon = timeon.Substring(0, 3);
                                    //int hour = int.Parse(timeon.Substring(0,2));
                                    //int min  = int.Parse(timeon.Substring(2,2));
                                    //int whichMinute = 0;
                                    //if (min >= 30) whichMinute = 1;
                                    //else whichMinute = 0;
                                    //timeon = (hour*100 + whichMinute).ToString();
                                    break;
                                case "BAND":
                                    ++n;
                                    String[] tokens6 = s.Split(delims);
                                    band = tokens6[2].TrimStart().TrimEnd();
                                    break;
                                case "MODE":
                                    ++n;
                                    String[] tokens7 = s.Split(delims);
                                    mode = tokens7[2].TrimStart().TrimEnd();
                                    if (mode.Contains("JT65")) mode = "JT65";
                                    if (mode.Contains("PSK")) mode = "PSK";
                                    if (checkBoxIgnoreMode.Checked) mode = "ANY";
                                    break;
                                case "SUBMODE":
                                    String[] tokens8 = s.Split(delims);
                                    submode = tokens8[2].TrimStart().TrimEnd();
                                    break;
                                case "QSO_DATE":
                                    ++n;
                                    String[] tokens9 = s.Split(delims);
                                    int ntoken = 2;
                                    if (tokens9[ntoken].Equals("D"))
                                    {
                                        ++ntoken;
                                    }
                                    qsodate = tokens9[ntoken].TrimStart().TrimEnd();
                                    break;
                                default:
                                    break;
                                    /*
                                    if (keyword.Contains("QSL_RCVD"))
                                    {
                                        String[] tokens9 = s.Split(delims);
                                         String who = tokens9[0];
                                        if (who.Contains("LOTW"))
                                        {
                                            lotw_qsl_rcvd = tokens9[2].Trim().Equals("Y") || tokens9[2].Trim().Equals("V");

                                        }
                                        else if (who.Contains("EQSL"))
                                        {
                                            eqsl_qsl_rcvd = tokens9[2].Trim().Equals("Y");

                                        }
                                        else
                                        {
                                            qsl_rcvd = tokens9[2].Trim().Equals("Y");
                                        }
                                    }
                                    */
                            }
                            if (mode.Equals("MFSK") && submode.Equals("FT4")) 
                                mode = "FT4";
                            bool for_us = true;
                            if (callsignFilter.Length > 0 && !stationCallSign.Equals(callsignFilter)) for_us = false;
                            if (for_us && n >= 5)
                            {
                                //String key = qsodate + "/" + timeon + "?\t" + band + "\t" +mode + "\t" + call + "\tlotw/eqsl="+lotw_qsl_rcvd+"/"+eqsl_qsl_rcvd;
                                String key = qsodate + "/" + timeon + "?\t" + band + "\t" + mode + "\t" + call;
                                //richTextBox1.AppendText(key + "\n");
                                //break;
                                try
                                {
                                    writer.WriteLine(key);
                                    if (!list.ContainsKey(key))
                                    {
                                        list.Add(key, 0);
                                    }
                                    else if (!checkBoxIgnoreDups.Checked)
                                    {
                                        richTextBox1.AppendText(key + " Duplicate?\n");
                                        ++ndups;
                                    }
                                }
                                catch
                                {
                                    listchkTotal++;
                                }
                                n = 0;
                            }
                        }
                    }
                    if (line.Contains("<EOR>") || line.Contains("<eor>"))
                    {
                        line = "";
                    }
                }
            }
            writer.Close();
            timer1.Stop();
            richTextBox1.AppendText("Elapsed " + timer1.ElapsedMilliseconds / 1000.0 + " seconds\n");
            label.Text = nlines.ToString();
            if (ndups == 0) richTextBox1.AppendText("No duplicates within " + filename);
            else richTextBox1.AppendText(ndups + " duplicates to check\n");
            richTextBox1.AppendText("QSOs accepted: " + nlines + "\n");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            list1.Clear();
            list2.Clear();
            run.Text = "Running";
            run.Enabled = false;
            //String file1 = "/Users/Mike/Dropbox/Ham/LogOM_W9MDB20161226.adi";
            //String file2 = "/Users/Mike/Downloads/w9mdb.5824.20161226211542.adi";
            String file1 = textBox1.Text;
            String file2 = textBox2.Text;
            richTextBox1.AppendText("Processing " + file1 + "\n");
            Application.DoEvents();
            Char[] delimiters = { '.', '_', ' ' };
            filename1 = file1.Split('\\').Last();
            filename1 = filename1.Split(delimiters)[0];
            filename2 = file2.Split('\\').Last();
            filename2 = filename2.Split(delimiters)[0];
            callsignFilter = textBox3.Text;

            AddToList(file1, list1, label1);
            richTextBox1.AppendText("===============================\n");
            richTextBox1.AppendText("Processing " + file2 + "\n");
            Application.DoEvents();
            AddToList(file2, list2, label2);
            richTextBox1.AppendText("===============================\n");
            richTextBox1.AppendText("Extra QSOs in " + file1 + " that are not in "+file2+"\n");
            int n1 = 0;
            foreach (KeyValuePair<String, int> pair in list1)
            {
                Application.DoEvents();
                if (!list2.ContainsKey(pair.Key))
                {
                    Application.DoEvents();
                    ++n1;
                    richTextBox1.AppendText(filename1 + " has\t" + pair.Key + "\n");
                }
            }
            richTextBox1.AppendText(n1 + " entries need checking\n");
            richTextBox1.AppendText("===============================\n");
            richTextBox1.AppendText("Extra QSOs in " + file2 + " that are not in "+file1+"\n");
            int n2 = 0;
            foreach (KeyValuePair<String, int> pair in list2)
            {
                //if (!list1.ContainsKey(pair.Key) && !pair.Key.Contains("SWL") && digits(pair.Key) < 3)
                if (!list1.ContainsKey(pair.Key))
                {
                    Application.DoEvents();
                    richTextBox1.AppendText(filename2 + " has\t" + pair.Key + "\n");
                    ++n2;
                }
            }
            richTextBox1.AppendText(n2 + " entries need checking\n");
            richTextBox1.AppendText("===============================\n");
            //richTextBox1.AppendText(file1 + " has " + n1 + " potential errors");
            //richTextBox1.AppendText(file2 + " has " + n2 + " potential errors");
            //richTextBox1.AppendText(listchk.Count + " entries excluded from check\n");
            richTextBox1.AppendText(file1 + " has " + list1.Count + " uniqie entries\n");
            richTextBox1.AppendText(file2 + " has " + list2.Count + " unique entries\n");
            //richTextBox1.AppendText(file1 + " total entries should be " + (list1.Count + listchkTotal)+"\n");
            //richTextBox1.AppendText(file2 + " total entries should be " + (list2.Count + listchkTotal)+"\n");
            run.Text = "Run";
            run.Enabled = true;
        }

        private String GetFileName()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "ADIF Files (*.adi)|*.adi|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return "";
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            String filename = GetFileName();
            if (filename.Length > 0)
            {
                textBox1.Text = filename;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            String filename = GetFileName();
            if (filename.Length > 0)
            {
                textBox2.Text = filename;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
