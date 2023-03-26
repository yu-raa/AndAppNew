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
using System.Linq;
using System.Threading;

namespace AndApp
{
    [Activity]
    public class ChatActivity : AppCompatActivity
    {
        public TextView listView { get; private set; }
        public EditText textView { get; private set; }
        public EditText textVieww1 { get; private set; }
        public EditText textVieww2 { get; private set; }
        public static byte[] contact = new byte[0];

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.chat);

            if (Intent.GetByteArrayExtra("contact") != null) {
                contact = Intent.GetByteArrayExtra("contact");
            }

            listView = this.FindViewById<TextView>(Resource.Id.successOrNot);
            listView.Visibility = ViewStates.Visible;
            listView.Text = Thread2.messager;

            textView = this.FindViewById<EditText>(Resource.Id.message);
            textView.AfterTextChanged += TextView_TextChanged;
        }

        private void TextView_TextChanged(object sender, AfterTextChangedEventArgs e)
        {
            byte[] data1 = new byte[10000];
            Array.Copy(contact, data1, contact.Length);

            data1[21] = 255;


            if (textView.Text.Length != 0 && textView.Text.EndsWith('\n'))
            {
                if (MainMenuActivity.client != null)
                {
                    try
                    {
                        byte[] iv = new byte[16];
                        byte[] key = new byte[16];
                        Array.Copy(Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()), iv, Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()).Length);
                        Array.Copy(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length% 16)], key, Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length % 16)].Length);
                        for (int i = Encoding.UTF8.GetBytes(DateTime.UtcNow.Hour.ToString()).Length; i < iv.Length; i++)
                        {
                            iv[i] = (byte)((i + 3)*9);
                        }

                        for (int i = Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString())[..(Encoding.UTF8.GetBytes(Encoding.UTF8.GetBytes(DateTime.UtcNow.Minute.ToString()).Length.ToString()).Length % 16)].Length; i < key.Length; i++)
                        {
                            key[i] = (byte)((i + 10) * 9);
                        }

                        Array.Copy(Encryption.Encrypt(Encoding.UTF8.GetBytes('\u0239'+textView.Text), key, iv), 0, data1, 22, Encryption.Encrypt(Encoding.UTF8.GetBytes('\u0239' + textView.Text), key, iv).Length);
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
            return;
        }
    }
}