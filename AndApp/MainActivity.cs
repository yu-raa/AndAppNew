using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using System.Text;
using AndroidX.Core.App;
using Android;
using System.Threading.Tasks;
using Android.Content.PM;
using static Android.Provider.DocumentsContract;
using static Android.Icu.Text.IDNA;

namespace AndApp
{
    static class Trimmer
    {
        public static byte[] TrimBytes(byte[] bytes)
        {
            byte[] newBytes = bytes;

            while (newBytes.Length > 0 && newBytes[0] == 0)
            {
                newBytes = newBytes[1..];
            }

            while (newBytes.Length > 0 && newBytes[^1] == 0)
            {
                newBytes = newBytes[..^1];
            }

            return newBytes;
        }
    }

    class Thread2
    {
        static byte[] data;
        static int g;

        public async Task Run()
        {
            while (MainMenuActivity.client != null)
            {
                try
                {
                    while (true)
                    {
                        byte[] data2 = new byte[10000];
                        try
                        {
                            g = await MainMenuActivity.client.ReceiveAsync(data2, SocketFlags.None).ConfigureAwait(false);

                            if (g >= 0 && Trimmer.TrimBytes(data2).Length >= 0)
                            {
                                data = Trimmer.TrimBytes(data2);

                                FileStream sourceStream = File.Open(MainMenuActivity.path, FileMode.Open, FileAccess.Read | FileAccess.Write, FileShare.ReadWrite);

                                if (data.Length > 42)
                                {
                                    sourceStream.Position = sourceStream.Length;
                                    sourceStream.Write(new byte[1] { 247 });
                                    sourceStream.Write(data[21..41]);
                                    sourceStream.Write(new byte[1] { 246 });
                                    sourceStream.Write(data[42..]);
                                    sourceStream.Flush();
                                }

                                sourceStream.Close();
                            }

                        }
                        catch (Exception exc) when (exc is SocketException) {
                            MainMenuActivity.client.Close();
                                MainMenuActivity.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            MainMenuActivity.client.Connect(MainMenuActivity.iPEndPoint);
                               g = MainMenuActivity.client.Receive(data2);
                            if (g >= 0 && Trimmer.TrimBytes(data2).Length >= 0)
                            {
                                data = Trimmer.TrimBytes(data2);

                                FileStream sourceStream = File.Open(MainMenuActivity.path, FileMode.Open, FileAccess.Read | FileAccess.Write, FileShare.ReadWrite);

                                if (data.Length > 42)
                                {
                                    sourceStream.Position = sourceStream.Length;
                                    sourceStream.Write(new byte[1] { 247 });
                                    sourceStream.Write(data[21..41]);
                                    sourceStream.Write(new byte[1] { 246 });
                                    sourceStream.Write(data[42..]);
                                    sourceStream.Flush();
                                }

                                sourceStream.Close();

                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            MainMenuActivity.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            MainMenuActivity.client.Connect(MainMenuActivity.iPEndPoint);
                            g = MainMenuActivity.client.Receive(data2);
                            if (g >= 0 && Trimmer.TrimBytes(data2).Length >= 0)
                            {
                                data = Trimmer.TrimBytes(data2);

                                FileStream sourceStream = File.Open(MainMenuActivity.path, FileMode.Open, FileAccess.Read | FileAccess.Write, FileShare.ReadWrite);

                                if (data.Length > 42)
                                {
                                    sourceStream.Position = sourceStream.Length;
                                    sourceStream.Write(new byte[1] { 247 });
                                    sourceStream.Write(data[21..41]);
                                    sourceStream.Write(new byte[1] { 246 });
                                    sourceStream.Write(data[42..]);
                                    sourceStream.Flush();
                                }

                                sourceStream.Close();
                            }
                        }
                    }
                }
                catch (Exception exc) when (exc is OutOfMemoryException || exc is StackOverflowException)
                { 
                    MainMenuActivity.client.Close();
                }
            }
        }

        public static string Button2_Click(byte[] data)
        {
            if (Trimmer.TrimBytes(data).Length > 16 && Trimmer.TrimBytes(data).Length % 8 == 0)
            {
                string message = string.Empty;
                string messageWithMarker = " ";
                int i = 0;
                while (!messageWithMarker.StartsWith('\u0239'))
                {
                    byte[] iv = new byte[16];
                    byte[] key = new byte[16];
                    Array.Copy(Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString())[..(Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString()).Length % 16)], key, Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString())[..(Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString()).Length % 16)].Length);
                    Array.Copy(Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Hour.ToString()), iv, Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Hour.ToString()).Length);
                    for (int j = Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Hour.ToString()).Length; j < iv.Length; j++)
                    {
                        iv[j] = (byte)((j + 3) * 9);
                    }

                    for (int j = Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString())[..(Encoding.UTF8.GetBytes(DateTime.UtcNow.AddMinutes(i).Minute.ToString().Length.ToString()).Length % 16)].Length; j < key.Length; j++)
                    {
                        key[j] = (byte)((j + 10) * 9);
                    }

                    try
                    {
                        messageWithMarker = Encryption.Decrypt(Trimmer.TrimBytes(data)[..^16], iv, key);
                    }
                    catch (CryptographicException)
                    {
                        i--;
                        continue;
                    }
                    
                    message = "<" + messageWithMarker[1..^1];
                    i--;
                }

                int hours = Math.Abs(i + 1) / 60;
                int minutes = Math.Abs(i + 1) % 60;

                message += $"> was sent {hours} hour{((hours.ToString().EndsWith('1') && !hours.ToString().EndsWith("11"))? "" : "s")} {minutes} minute{((minutes.ToString().EndsWith('1') && (minutes.ToString().Length < 2 || !minutes.ToString().EndsWith("11"))) ? "" : "s")} ago";

                return message;
            }

            return "user is typing...";
        }
    }

    public static class Encryption
    {
        private static Aes aes;

        public static byte[] Encrypt(byte[] text, byte[] key, byte[] IV)
        {
            aes = Aes.Create();
            aes.IV = IV;
            aes.Key = key;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
            MemoryStream memory = new MemoryStream();
            CryptoStream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write);
            stream.Write(text);
            stream.Flush();
            stream.FlushFinalBlock();
            byte[] encText = new byte[memory.ToArray().Length + aes.IV.Length];
                for (int i = 0; i < memory.ToArray().Length; i++)
                {
                    encText[i] = memory.ToArray()[i];
                }

                for (int i = 0; i < aes.IV.Length; i++)
                {
                    encText[i + memory.ToArray().Length] = (byte)new Random().Next();
                }

                memory.Close();
                stream.Close();
            aes.Dispose();
                return encText;
        }

        public static string Decrypt(byte[] text, byte[] initV, byte[] key)
        {
            aes = Aes.Create();
            aes.IV = initV;
            aes.Key = key;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            MemoryStream memory = new MemoryStream(text);
            memory.Position = 0;
            CryptoStream stream = new CryptoStream(memory, transform, CryptoStreamMode.Read);
            byte[] decText = new byte[text.Length];
            int decByteCount = stream.Read(decText);
            memory.Close();
            stream.Close();
            return Encoding.UTF8.GetString(decText, 0, decByteCount);
        }
    }

    static class Comparator
    {
        public static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a != b)
            {
                if (a.Length != b.Length) return false;
                for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            }
            return true;
        }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActiv : AppCompatActivity
    {
        public static ISharedPreferences sharedPreferences { get; set; }
        public Button button { get; private set; }
        public static EditText textVieww1 { get; private set; }
        public static Spinner spinner { get; private set; }
        public static bool IpIsNew { get; private set; }

        private void Button_Click(object sender, View.LongClickEventArgs e)
        {
            Intent newIntent = new Intent(this, typeof(MainMenuActivity));
            StartActivity(newIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            spinner = FindViewById<Spinner>(Resource.Id.ip);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.serv_ips, Android.Resource.Layout.SimpleSpinnerItem);


            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            button = FindViewById<Button>(Resource.Id.button1);
            textVieww1 = FindViewById<EditText>(Resource.Id.textView1);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string server = spinner.GetItemAtPosition(e.Position).ToString();
            if (server.Contains("new ip address"))
            {
                IpIsNew = true;
                textVieww1.Visibility = ViewStates.Visible;
                spinner.Visibility = ViewStates.Gone;
            }
            else
            {
                IpIsNew= false;
            }

            button.LongClick += Button_Click;
        }
    }
}