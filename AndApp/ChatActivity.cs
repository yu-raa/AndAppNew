using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Android.Text;
using AndroidX.AppCompat.App;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using static Android.Icu.Text.IDNA;
using System.Linq;
using static Android.Provider.Telephony.Mms;
using System.IO;

namespace AndApp
{
    [Activity]
    public class ChatActivity : AppCompatActivity
    {
        public TextView listView { get; private set; }
        public EditText textView { get; private set; }
        public static byte[] contact = new byte[0];
        public Dictionary<byte[], bool> messagesDivided { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.chat);

            messagesDivided = new Dictionary<byte[], bool>();

            if (Intent.GetByteArrayExtra("contact") != null) {
                contact = Intent.GetByteArrayExtra("contact");
            }

            byte[] messages = Trimmer.TrimBytes(Intent.GetByteArrayExtra("messages"));
            
            List<byte> messagess = messages.ToList();

            while (messagess.Contains(245) || messagess.Contains(244))
            {
                if (messagess.Contains(245) && messagess.First() == 245)
                    messagesDivided.Add(messagess.ToArray()[(messagess.IndexOf(245)+1)..((messagess.IndexOf(245, 1) > messagess.IndexOf(244)) ? (messagess.IndexOf(244) > 0? messagess.IndexOf(244) : messagess.IndexOf(245, 1)) : (messagess.IndexOf(245, 1) > 0 ? messagess.IndexOf(245, 1) : (messagess.IndexOf(244) > 0? messagess.IndexOf(244) : ^0)))], false);
                else if (messagess.Contains(244) && messagess.First() == 244)
                    messagesDivided.Add(messagess.ToArray()[(messagess.IndexOf(244) + 1)..((messagess.IndexOf(244, 1) > messagess.IndexOf(245)) ? (messagess.IndexOf(245) > 0 ? messagess.IndexOf(245) : messagess.IndexOf(244, 1)) : (messagess.IndexOf(244, 1) > 0 ? messagess.IndexOf(244, 1) : (messagess.IndexOf(245) > 0 ? messagess.IndexOf(245) : ^0)))], true);

                messagess = messagess.ToArray()[(messagess.IndexOf(245) > messagess.IndexOf(244)? (messagess.IndexOf(245) > 0 ? messagess.IndexOf(245) : ^0) : (messagess.IndexOf(244) > 0 ? messagess.IndexOf(244) : (messagess.IndexOf(244, 1) > 0? messagess.IndexOf(244, 1) : (messagess.IndexOf(245, 1) > 0 ? messagess.IndexOf(245, 1) : ^0))))..].ToList();
            }

            if (messagess.Count > 0)
            {
                if (messagess.IndexOf(244) == 0)
                {
                    while (messagess.Count > 0)
                    {
                        messagesDivided.Add(messagess.ToArray()[(messagess.IndexOf(244) + 1)..(messagess.IndexOf(244,1) > 0? messagess.IndexOf(244, 1) : ^0)], true);
                        messagess = messagess.ToArray()[(messagess.IndexOf(244, 1) > 0 ? messagess.IndexOf(244, 1) : ^0)..].ToList();
                    }
                }
                else
                {
                    while (messagess.Count > 0)
                    {
                        messagesDivided.Add(messagess.ToArray()[(messagess.IndexOf(245) + 1)..(messagess.IndexOf(245, 1) > 0 ? messagess.IndexOf(245, 1) : ^0)], false);
                        messagess = messagess.ToArray()[(messagess.IndexOf(245, 1) > 0 ? messagess.IndexOf(245, 1) : ^0)..].ToList();
                    }
                }
            }

            foreach (var message in messagesDivided)
            {
                TextView textView = new TextView(this);
                GridLayout layout = this.FindViewById<GridLayout>(Resource.Id.stackingViewChat);
                var par = new GridLayout.LayoutParams();
                par.Width = 1000;
                par.LeftMargin = 10;
                textView.TextAlignment = (message.Value.CompareTo(false) == 0) ? TextAlignment.ViewStart : TextAlignment.ViewEnd;

                textView.LayoutParameters = par;
                textView.Visibility = ViewStates.Visible;
                layout.AddView(textView);
                textView.Invalidate();
                textView.Text = Thread2.Button2_Click(message.Key) + "\n";
            }

            textView = this.FindViewById<EditText>(Resource.Id.message);
            textView.AfterTextChanged += TextView_TextChanged;
        }

        private void TextView_TextChanged(object sender, AfterTextChangedEventArgs e)
        {
                byte[] data1 = new byte[10000];
                Array.Copy(contact, data1, contact.Length);

                Array.Copy(Encoding.UTF8.GetBytes(MainMenuActivity.ownPhone), 0, data1, 21, Encoding.UTF8.GetBytes(MainMenuActivity.ownPhone).Length);

                data1[41] = 255;


                if (textView.Text.Length != 0 && textView.Text.EndsWith('\n'))
                {
                    if (MainMenuActivity.client != null)
                    {
                        try
                        {
                            byte[] iv = new byte[16];
                            byte[] key = new byte[16];
                            Array.Copy(Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()), iv, Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()).Length);
                            Array.Copy(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length % 16)], key, Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length % 16)].Length);
                            for (int i = Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()).Length; i < iv.Length; i++)
                            {
                                iv[i] = (byte)((i + 3) * 9);
                            }

                            for (int i = Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length % 16)].Length; i < key.Length; i++)
                            {
                                key[i] = (byte)((i + 10) * 9);
                            }

                            Array.Copy(Encryption.Encrypt(Encoding.UTF8.GetBytes('\u0239' + textView.Text), key, iv), 0, data1, 42, Encryption.Encrypt(Encoding.UTF8.GetBytes('\u0239' + textView.Text), key, iv).Length);
                            try
                            {

                                MainMenuActivity.client.Send(data1);
                            }
                            catch (SocketException)
                            {
                                MainMenuActivity.client.Close();
                                MainMenuActivity.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                MainMenuActivity.client.Connect(MainMenuActivity.iPEndPoint);
                                MainMenuActivity.client.Send(data1);

                            }
                            textView.Text = string.Empty;

                        FileStream sourceStream = File.Open(MainMenuActivity.path, FileMode.Open, FileAccess.Read | FileAccess.Write, FileShare.ReadWrite);

                        sourceStream.Position = sourceStream.Length;

                        if (data1[42..].Length != 0)
                        {
                            sourceStream.Write(new byte[1] { 247 });
                            sourceStream.Write(data1[..21]);
                            sourceStream.Write(new byte[1] { 246 });
                            sourceStream.Write(Trimmer.TrimBytes(data1[42..]));
                            sourceStream.Flush();
                        }

                        sourceStream.Close();
                    }
                        catch (Exception exc)
                        {
                            listView.Text = exc.Message;
                            listView.Visibility = ViewStates.Visible;
                        }
                }
                }
                else
                {
                    try
                    {

                        MainMenuActivity.client.Send(data1);
                    }
                    catch (SocketException)
                    {
                        MainMenuActivity.client.Close();
                        MainMenuActivity.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        MainMenuActivity.client.Connect(MainMenuActivity.iPEndPoint);
                        MainMenuActivity.client.Send(data1);

                    }
                }
            
        }
    }
}