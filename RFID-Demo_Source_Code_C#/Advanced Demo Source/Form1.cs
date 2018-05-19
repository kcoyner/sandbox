using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket MainSocket = null;
        List<RFIDTag> tags = new List<RFIDTag>();
        public Form1()
        {
            InitializeComponent();
        }
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (this.buttonConnect.Text == "Start")
            {
                if (string.IsNullOrEmpty(this.textBoxReaderIP.Text) || string.IsNullOrEmpty(this.textBoxPort.Text))
                    return;
                try
                {
                    IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8900);
                    IPAddress IP = IPAddress.Parse(textBoxReaderIP.Text);
                    int Port = Convert.ToInt32(textBoxPort.Text);
                    Reader_Config(this.textBoxReaderIP.Text, Convert.ToInt32(this.textBoxPort.Text), 1);
                    Thread.Sleep(100);
                    MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    MainSocket.Connect(IP, Port);
                    StateObject state = new StateObject();
                    MainSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnDataReceived), state);
                    this.buttonConnect.Text = "Stop";
                    this.textBoxReaderIP.Enabled = false;
                    this.textBoxPort.Enabled = false;
                }

                catch (SocketException ConnectE)
                {
                    MessageBox.Show(ConnectE.Message);
                }
            }
            else
            {
                this.buttonConnect.Text = "Start";
                this.textBoxReaderIP.Enabled = true;
                this.textBoxPort.Enabled = true;
                try
                {
                    MainSocket.Disconnect(true);
                }
                catch { }
            }
        }
        private void OnDataReceived(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                int bytesRead = MainSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] recvBytes = state.buffer;
                    //Judge the header info
                    string print_data = null;
                    for (int index = 0; index < bytesRead; index++)
                    {
                        print_data += recvBytes[index].ToString("X2");
                        if ( recvBytes[index] == 0x0D && recvBytes[index + 1] == 0x0D) //header confirmed
                        {
                            //NOTE: there is NO CRC check in here. it is better to have a CRC check by using XOR calculation, for instance, for a whole data of "0D0D10FFFF00430000000001507981FA",
                            //the CRC value would be the last byte (15th) which is "FA" and it could be got through 0D (xor) 0D(xor)10(xor)FF(xor)FF(xor)00(xor)43(xor)00(xor)00(xor)00(xor)00(xor)01(xor)50(xor)79(xor) 81
                            byte[] tagBytes = new byte[9]; //Declare a new byte array to store the tag ID
                            for (int i = 0; i < 9; i++)
                            {
                                //get the Tag ID, a tag id contains 8 bytes (0 ~ 7) in total, and the 8th bytes stands for the status
                                tagBytes[i] = recvBytes[i + index + 6];
                            }
                            RFIDTag temp = ConvertTag(tagBytes);
                            for (int j = 0; j < tags.Count; j++)
                            {
                                if (tags[j].ID == temp.ID)
                                {
                                    tags.RemoveAt(j);
                                    break;
                                }
                            }
                            tags.Add(ConvertTag(tagBytes));
                            if (tags.Count > 0)
                            {
                                List<ListViewItem> items = new List<ListViewItem>();
                                foreach (RFIDTag tag in tags)
                                {
                                    IInsertListViewItem(this, listView1, tag);
                                    //tags.Remove(tag);
                                }
                            }
                        }
                    }
                    //print out the raw data
                    Console.WriteLine("---" + print_data);
                    // if the Get Gain response from the reader to the PC
                  //  GetGainValue(recvBytes, bytesRead);
                }
                MainSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnDataReceived), state);
            }
            catch (Exception ex)
            {

            }
        }
        private class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 2048;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();

        }
        public class GUIControl
        {
            //add an item to a ListView
            public delegate void ListViewAddItem(ListView l, string s);
            public static void ListViewAddItem1(ListView l, string s)
            {

                l.Items.Add(new ListViewItem(s));
            }
            public static void IListViewAddItem(Control form, ListView l, string s)
            {

                if (l.InvokeRequired)
                {
                    ListViewAddItem LVAI = new ListViewAddItem(GUIControl.ListViewAddItem1);
                    form.Invoke(LVAI, new Object[] { l, s });
                }
                else
                {
                    l.Items.Add(new ListViewItem(s));
                }
            }

            //Insert an item to a ListView

            public delegate void ListViewInsertItem(ListView l, int i, string s);
            public static void ListViewInsertItem1(ListView l, int i, string s)
            {
                l.Items.Insert(i, new ListViewItem(s));
            }
            public static void IListViewInsertItem(Control form, ListView l, int i, string s)
            {

                if (l.InvokeRequired)
                {
                    ListViewInsertItem LVAI = new ListViewInsertItem(GUIControl.ListViewInsertItem1);
                    form.Invoke(LVAI, new Object[] { l, i, s });
                }
                else
                {
                    l.Items.Insert(i, new ListViewItem(s));
                }
            }

            //enable or disable a control
            public delegate void ControlEnable(Control c, bool enable);
            public static void ControlEnable1(Control c, bool enable)
            {
                c.Enabled = enable;
            }
            public static void IControlEnable(Control form, Control c, bool enable)
            {

                if (c.InvokeRequired)
                {
                    ControlEnable ce = new ControlEnable(GUIControl.ControlEnable1);
                    form.Invoke(ce, new Object[] { c, enable });
                }
                else
                {
                    c.Enabled = enable;
                }
            }
            //update a control's text
            public delegate void ControlSetText(Control c, string s);
            public static void ControlSetText1(Control c, string s)
            {
                c.Text = s;
            }
            public static void IControlSetText(Control form, Control c, string s)
            {

                if (c.InvokeRequired)
                {
                    ControlSetText cst = new ControlSetText(GUIControl.ControlSetText1);
                    form.Invoke(cst, new Object[] { c, s });
                }
                else
                {
                    c.Text = s;
                }
            }

            //get count
            public delegate int ListViewItemCount(ListView l);
            public static int ListViewItemCount1(ListView l)
            {
                return l.Items.Count;
            }
            public static int IListViewItemCount(Control form, ListView l)
            {

                if (l.InvokeRequired)
                {
                    ListViewItemCount LVIC = new ListViewItemCount(GUIControl.ListViewItemCount1);
                    return (int)(form.Invoke(LVIC, new Object[] { l }));
                }
                else
                {
                    return l.Items.Count;
                }
            }

            //add an item to a ListBox
            public delegate void ListBoxAddItem(ListBox l, string s);
            public static void ListBoxAddItem1(ListBox l, string s)
            {
                l.Items.Add(s);
            }
            public static void IListBoxAddItem(Control form, ListBox l, string s)
            {

                if (l.InvokeRequired)
                {
                    ListBoxAddItem LBAI = new ListBoxAddItem(GUIControl.ListBoxAddItem1);
                    form.Invoke(LBAI, new Object[] { l, s });
                }
                else
                {
                    l.Items.Add(s);
                }
            }

        }
        private static RFIDTag ConvertTag(byte[] record)
        {
            RFIDTag tag = new RFIDTag();
            tag.Battery = (record[8] < 128) ? "Low" : "High";
            switch (Convert.ToInt16(record[8] & 0x000F))
            {
                //Configurable Card Tag
                case 0:
                    tag.ID = GetTagID(record, 7);
                    tag.Type = "127002";
                    tag.Buckle = "";
                    tag.ButtonPress = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    break;

                //Configurable Strip Tag
                case 1:
                    tag.ID = GetTagID(record, 7);
                    tag.Type = "127001";
                    tag.Buckle = "";
                    tag.ButtonPress = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    break;

                // Temperature Tag
                case 2:
                    //RFIDTagMemory temperature = new RFIDTagMemory();
                    tag.Temperature = GetTempData(record) + " °C";
                    //temperature.MemoryType = RFIDTag_MemoryType.Temperature;
                    tag.ID = GetTagID(record, 5);
                    tag.Type = "127003";
                    tag.Buckle = "";
                    tag.ButtonPress = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Vibration = "";
                    //tag.AddTagMemory(temperature);

                    break;

                //Vibration Tag
                case 3:
                    //RFIDTagMemory vibration = new RFIDTagMemory();
                    tag.Vibration = (Convert.ToInt16(record[8] & 0x0040) > 0) ? "Yes" : "No";
                    //vibration.MemoryType = RFIDTag_MemoryType.Vibration;
                    tag.Type = "127004";
                    tag.ID = GetTagID(record, 7);
                    tag.Buckle = "";
                    tag.ButtonPress = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Temperature = "";
                    //tag.AddTagMemory(vibration);
                    break;

                //Key-fob Tag
                case 8:
                    //RFIDTagMemory keyfob = new RFIDTagMemory();
                    if (record[8] == 0xA8)
                    {
                        tag.ButtonPress = "Button#1 Pressed";
                    }
                    else if (record[8] == 0x98)
                    {
                        tag.ButtonPress = "Button#2 Pressed";
                    }
                    else
                    {
                        tag.ButtonPress = "";
                    }

                    //keyfob.MemoryType = RFIDTag_MemoryType.Button;
                    tag.Type = "127012";
                    tag.ID = GetTagID(record, 7);
                    tag.Buckle = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    //tag.AddTagMemory(keyfob);
                    break;

                //Wristband Tag
                case 5:
                    //RFIDTagMemory callMem = new RFIDTagMemory();
                    //callMem.MemoryType = RFIDTag_MemoryType.CallButton;
                    tag.Call = ((record[8] & 0x0020) > 0) ? "Button Pressed" : "";

                    //RFIDTagMemory buckleMem = new RFIDTagMemory();
                    //buckleMem.MemoryType = RFIDTag_MemoryType.Buckle;
                    tag.Buckle = ((record[8] & 0x0010) > 0) ? "Buckle Opened" : "Buckle Secured";
                    tag.Type = "127006";
                    tag.ID = GetTagID(record, 7);
                    tag.Mount = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    tag.ButtonPress = "";
                    //tag.AddTagMemory(callMem);
                    //tag.AddTagMemory(buckleMem);
                    break;

                //Beaconing Tag
                case 6:
                    //RFIDTagMemory mount = new RFIDTagMemory();
                    tag.Mount = ((record[8] & 0x0010) > 0) ? "Mounted" : "Un-mounted";
                    //mount.MemoryType = RFIDTag_MemoryType.Mount;
                    tag.Type = "127005";
                    tag.ID = GetTagID(record, 7);
                    tag.Buckle = "";
                    tag.Call = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    tag.ButtonPress = "";
                    //tag.AddTagMemory(mount);
                    break;
                default:
                    tag.ButtonPress = "";
                    //keyfob.MemoryType = RFIDTag_MemoryType.Button;
                    tag.Type = "";
                    tag.ID = GetTagID(record, 7);
                    tag.Buckle = "";
                    tag.Call = "";
                    tag.Mount = "";
                    tag.Temperature = "";
                    tag.Vibration = "";
                    break;

            }
            return tag;
        }
        private static string GetTempData(byte[] package)
        {
            string integerPart = int.Parse(package[6].ToString("X2"), System.Globalization.NumberStyles.HexNumber).ToString();
            string fractional = Convert.ToInt16(package[7].ToString("X2").Substring(1), 16).ToString();
            string sign = (package[7] < 0x10) ? "" : "-";

            return sign + integerPart + "." + fractional;
        }
        private static string GetTagID(byte[] package, int idLastIndex)
        {
            string tagID = "";
            for (int i = 0; i <= idLastIndex; i++)
            {
                tagID += package[i].ToString("X2");
            }
            // Print out the whole received data string
           // Console.WriteLine(tagID);
            return tagID;
        }
        public delegate void InsertListViewItem(ListView t, RFIDTag tag);
        public static void InsertListViewItem1(ListView t, RFIDTag tag)
        {
            updateListView(t, tag);
        }
        public static void IInsertListViewItem(Form form, ListView t, RFIDTag tag)
        {
            if (t.InvokeRequired)
            {
                InsertListViewItem a = new InsertListViewItem(InsertListViewItem1);
                form.Invoke(a, new Object[] { t, tag});
            }
            else
            {
                updateListView(t, tag);
            }
        }
        public static void updateListView(ListView t, RFIDTag tag)
        {
            lock (t)
            {
                string dataTime = DateTime.Now.ToString();
                for (int i = 0; i < t.Items.Count; i++)
                {
                    if (t.Items[i].SubItems[2].Text == tag.ID)
                    {
                        t.Items[i].SubItems[4].Text = dataTime;
                        int count = Convert.ToInt32(t.Items[i].SubItems[3].Text.ToString()) + 1;
                        t.Items[i].SubItems[3].Text = count.ToString();
                        t.Items[i].SubItems[6].Text = tag.Temperature;
                        t.Items[i].SubItems[5].Text = tag.Battery;
                        t.Items[i].SubItems[7].Text = tag.Buckle;
                        t.Items[i].SubItems[8].Text = tag.Vibration;
                        if (tag.Call != "")
                        {
                            t.Items[i].SubItems[9].Text = "Button Pressed";
                            t.Items[i].BackColor = Color.OrangeRed;
                        }
                        else if (tag.ButtonPress != "")
                        {
                            t.Items[i].SubItems[9].Text = tag.ButtonPress;
                            t.Items[i].BackColor = Color.OrangeRed;
                        }
                        else
                        {
                            t.Items[i].SubItems[9].Text = "";
                            t.Items[i].BackColor = Color.White;
                        }
                        if (tag.Mount == "Mounted")
                        {
                            t.Items[i].SubItems[10].Text = tag.Mount;
                            t.Items[i].BackColor = Color.Green;
                        }
                        else if (tag.Mount == "Un-mounted")
                        {
                            t.Items[i].SubItems[10].Text = tag.Mount;
                            t.Items[i].BackColor = Color.White;
                        }
                        return;
                    }
                }
                // total tags number
                ListViewItem item = new ListViewItem();
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = (t.Items.Count + 1).ToString();
                item.SubItems.Insert(0, subItem);

                // tag type
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Type;
                item.SubItems.Insert(1, subItem);

                //tag id
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.ID;
                item.SubItems.Insert(2, subItem);

                //count
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = Convert.ToString(1);
                item.SubItems.Insert(3, subItem);

                //timestamp
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = dataTime;
                item.SubItems.Insert(4, subItem);

                //Battery
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Battery;
                item.SubItems.Insert(5, subItem);

                //Temperature
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Temperature;
                item.SubItems.Insert(6, subItem);

                //Buckle
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Buckle;
                item.SubItems.Insert(7, subItem);

                //Vibration
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Vibration;
                item.SubItems.Insert(8, subItem);

                //Panic button
                subItem = new ListViewItem.ListViewSubItem();
                if (tag.Call != "")
                {
                    subItem.Text = "Button Pressed";
                }
                else if (tag.ButtonPress != "")
                {
                    subItem.Text = tag.ButtonPress;
                }
                else
                {
                    subItem.Text = "";
                }
                item.SubItems.Insert(9, subItem);

                //Mounted
                subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = tag.Mount;
                item.SubItems.Insert(10, subItem);
                t.Items.Add(item);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            this.Close();
            Application.Exit();
        }
        private void textBoxReaderIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ("0123456789.".IndexOf(Char.ToUpper(e.KeyChar)) < 0);
            if ((e.KeyChar == (char)Keys.Return))
            {
                textBoxReaderIP.Focus();
            }
        }
        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ("0123456789".IndexOf(Char.ToUpper(e.KeyChar)) < 0);
            if ((e.KeyChar == (char)Keys.Return))
            {
                textBoxPort.Focus();
            }
        }
        private void Reader_Config(string IP, int PORT, int value)
        {
            IPAddress ipaddr = IPAddress.Parse(IP);
            int port = Convert.ToInt32(PORT);
            Socket init = null;
            try
            {
                init = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                init.Connect(ipaddr, port);
                byte Mode;
                if (value == 1)
                {
                    Mode = 255;
                }
                else
                {
                    Mode = 0;
                }
                byte[] Result = new byte[8];
                string deviceaddr = "FFFF";
                //Header
                Result[0] = 13;
                Result[1] = 13;

                //Length
                Result[2] = 8;

                //Command
                Result[3] = 160;

                //Address
                Result[4] = Convert.ToByte(deviceaddr.Substring(0, 2), 16);
                Result[5] = Convert.ToByte(deviceaddr.Substring(2, 2), 16);

                //Mode
                Result[6] = Mode;

                //XOR
                Result[7] = (byte)(Result[0] ^ Result[1] ^ Result[2] ^ Result[3] ^ Result[4] ^ Result[5] ^ Result[6]);

                init.Send(Result);
                Thread.Sleep(100);
                init.Disconnect(true);
                init.Close();
            }
            catch{}
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBoxReaderIP.Focus();
        }

    }  
}
