using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISDBTphSimplified
{
    public partial class Form1 : Form
    {
        List<string[]> channelList = new List<string[]>();
        int currentChannel = 0;
        string aspectRatio = "16:9";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            axVLCPlugin21.Size = Screen.PrimaryScreen.Bounds.Size;
            try
            {
                string line = "";
                System.IO.StreamReader channelFile = new System.IO.StreamReader("ChannelsList.txt");
                System.IO.StreamReader currentConfig = new System.IO.StreamReader("CurrentConfig.txt");


                while ((line = channelFile.ReadLine()) != null)
                {
                    channelList.Add(line.Split('`'));
                }

                channelFile.Close();

                while ((line = currentConfig.ReadLine()) != null)
                {
                    currentChannel = Convert.ToInt32(line.Split('`')[0]);
                    aspectRatio = line.Split('`')[1];
                    axVLCPlugin21.video.aspectRatio = aspectRatio;
                    if (!Regex.Match(line, "[0-9]+[`][0-9]+[:][0-9]+").Success)
                    {
                        aspectRatio = "16:9";
                        axVLCPlugin21.video.aspectRatio = "16:9";
                        for (int i = 0; i < channelList.Count; i++)
                        {
                            if (channelList[i][2].Contains("TV5"))
                            {
                                currentChannel = i;
                            }

                        }
                    }
                }

                currentConfig.Close();

                File.WriteAllText(@"CurrentConfig.txt", currentChannel.ToString() + "`" + aspectRatio);
                string[] vOptions = { @":dvb-frequency=" + channelList[currentChannel][0], @":dvb-bandwidth=6", @":program=" + channelList[currentChannel][1] };
                axVLCPlugin21.playlist.stop();
                axVLCPlugin21.playlist.items.clear();
                axVLCPlugin21.playlist.add(@"dvb-t://", null, vOptions);
                axVLCPlugin21.playlist.play();
            }
            catch (Exception)
            {

            }

        }

        private void axVLCPlugin21_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int keyVal = e.KeyValue;

            if (keyVal == 27)
            {
                this.Close();
            }
            else if (keyVal > 47 && keyVal < 58)
            {
                if (!timer1.Enabled)
                {
                    timer1.Enabled = true;
                    timer1.Start();
                }
                label1.Text += (e.KeyValue - 48).ToString();
            }
            else if (keyVal == 13)
            {
                if(label1.Text != "" && Convert.ToInt32(label1.Text) != currentChannel && Convert.ToInt32(label1.Text) < channelList.Count)
                {
                    currentChannel = Convert.ToInt32(label1.Text);
                    string[] vOptions = { @":dvb-frequency="+channelList[currentChannel][0], @":dvb-bandwidth=6", @":program=" + channelList[currentChannel][1]};
                    axVLCPlugin21.playlist.stop();
                    axVLCPlugin21.playlist.items.clear();
                    axVLCPlugin21.playlist.add(@"dvb-t://", null, vOptions);
                    axVLCPlugin21.playlist.play();
                    timer1.Enabled = false;
                    timer1.Stop();
                }
                label1.Text = "";
            }
            else if(keyVal == 38 || keyVal == 40)
            {
                currentChannel = (keyVal == 40? (currentChannel + 1) % channelList.Count : currentChannel + (currentChannel == 0? channelList.Count - 1 : -1)) ;
                string[] vOptions = { @":dvb-frequency=" + channelList[currentChannel][0], @":dvb-bandwidth=6", @":program=" + channelList[currentChannel][1] };
                axVLCPlugin21.playlist.stop();
                axVLCPlugin21.playlist.items.clear();
                axVLCPlugin21.playlist.add(@"dvb-t://", null, vOptions);
                axVLCPlugin21.playlist.play();
                timer1.Enabled = false;
                timer1.Stop();
                bool isSuccessWriting = false;
                do
                {
                    try
                    {
                        File.WriteAllText(@"CurrentConfig.txt", currentChannel.ToString() + "`" + aspectRatio);
                        isSuccessWriting = true;
                    }
                    catch (Exception ex)
                    {

                    }
                } while (!isSuccessWriting);
            }
            else if(keyVal == 82)
            {
                if(aspectRatio == "4:3")
                {
                    aspectRatio = "16:9";
                }
                else if (aspectRatio == "16:9")
                {
                    aspectRatio = "16:10";
                }
                else if (aspectRatio == "16:10")
                {
                    aspectRatio = "4:3";
                }
                axVLCPlugin21.video.aspectRatio = aspectRatio;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool isSuccessWriting = false;
            if (label1.Text != "" && Convert.ToInt32(label1.Text) != currentChannel && Convert.ToInt32(label1.Text) < channelList.Count)
            {
                currentChannel = Convert.ToInt32(label1.Text);
                string[] vOptions = { @":dvb-frequency=" + channelList[currentChannel][0], @":dvb-bandwidth=6", @":program=" + channelList[currentChannel][1] };
                axVLCPlugin21.playlist.stop();
                axVLCPlugin21.playlist.items.clear();
                axVLCPlugin21.playlist.add(@"dvb-t://frequency=" + channelList[currentChannel][0], null, vOptions);
                axVLCPlugin21.playlist.play();
                label1.Text = "";
                do
                {
                    try
                    {
                        File.WriteAllText(@"CurrentConfig.txt", currentChannel.ToString() + "`" + aspectRatio);
                        isSuccessWriting = true;
                    }
                    catch (Exception ex)
                    {

                    }
                } while (!isSuccessWriting);
                timer1.Enabled = false;
                timer1.Stop();
            }
        }
    }
}
