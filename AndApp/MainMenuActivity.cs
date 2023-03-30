using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.AppCompat.App;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using AndroidX.Core.App;
using Android;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Net;
using System.Net.NetworkInformation;
using System.IO;
using Java.Lang;

namespace AndApp
{
    [Activity(Label = "Main Menu")]
    public class MainMenuActivity : AppCompatActivity
    {
        public static string path = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath + $"/m.txt";
        public static Socket client;
        private EditText editTextForPhone;
        private TextView textView;
        public static string ownPhone { get; private set; }
        private bool isFirst = true, isFirst2 = true;
        private LinkedList<Contact> contactss;
        private IEnumerable<Contact> contacts;
        private List<byte> dataList = new List<byte>();
        private LinkedListNode<Contact> contact;
        public static byte[] phoneContact { get; private set; }
        public static IPEndPoint iPEndPoint { get; private set; }
        public static List<KeyValuePair<byte[], byte[]>> messages { get; private set; }
        private static List<byte[]> bytes = new List<byte[]>();
        private static byte[] info;
        public static byte[] messagesForThis { get; set; }

        public static void GetInfoFromMessageFile()
        {
            messages = new List<KeyValuePair<byte[], byte[]>>();
            FileStream sourceStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read | FileAccess.Write, FileShare.ReadWrite);
            sourceStream.Position = 0;
            info = new byte[1000];
            sourceStream.Read(info, 0, info.Length);
            while (sourceStream.Position < sourceStream.Length)
            {
                byte[] temp = info;
                Array.Resize(ref info, info.Length + 100);
                Array.Copy(temp, info, temp.Length);
                sourceStream.Read(info, temp.Length, 100);
            }
            sourceStream.Flush();
            sourceStream.Close();

            List<byte> infos = Trimmer.TrimBytes(info).ToList();

            while (infos.Contains(246) && infos.Contains(247) && infos.IndexOf(246) != infos.LastIndexOf(246))
            {
                if (infos.IndexOf(247, 1) - infos.IndexOf(246) > 1 || infos.IndexOf(246) < infos.IndexOf(247))
                messages.Add(new KeyValuePair<byte[], byte[]>(infos.ToArray()[..infos.IndexOf(246)], infos.ToArray()[(infos.IndexOf(246)+1)..((infos.IndexOf(247, 1) > 0)? infos.IndexOf(247, 1) : ^0)]));
                infos = infos.ToArray()[((infos.IndexOf(247, 1) > 0) ? infos.IndexOf(247, 1) : ^0)..].ToList();
            }

            if (infos.Count > 0)
                messages.Add(new KeyValuePair<byte[], byte[]>(infos.ToArray()[1..21], infos.ToArray()[22..]));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mainmenu);
            editTextForPhone = FindViewById<EditText>(Resource.Id.ownPhone);
            textView = FindViewById<TextView>(Resource.Id.sug);

        }

        protected override void OnResume()
        {
            base.OnResume();

            if (client is null || !client.Connected)
            {
                string ipAddress = string.Empty;

                int port;

                if (MainActiv.IpIsNew)
                {
                    ipAddress = MainActiv.textVieww1.Text;
                }
                else
                {
                    ipAddress = MainActiv.spinner.SelectedItem.ToString();
                }

                port = 2460;

                client = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
                IPAddress iPadd = IPAddress.Parse(ipAddress);
                iPEndPoint = new IPEndPoint(iPadd, port);

                while (!client.Connected)
                try
                    {

                    client.Connect(iPEndPoint);

                    editTextForPhone.AfterTextChanged += delegate
                    {
                        if (editTextForPhone.Text.Count() == 13)
                        {
                            ownPhone = editTextForPhone.Text.ToString();
                            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadContacts }, 10);
                        }
                    };
                    }
                    catch (SocketException exc)
                    {
                        client.Connect(iPEndPoint);
                    }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
                {
                    case 10:
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                        Task t;
                            while (!(t = ConnectAndSend()).IsCompleted || (contact is null || contact.Next != null))
                        {
                        }
                        }
                        break;
                }
        }

        private async Task ConnectAndSend()
        {

            editTextForPhone.Visibility = ViewStates.Gone;
            textView.Visibility = ViewStates.Gone;

            if (isFirst)
            {
                dataList.Add(221);

                byte[] re = Encoding.UTF8.GetBytes(ownPhone);

                client.Send(re, SocketFlags.None);
            }

            contacts = await Contacts.GetAllAsync();

            if (isFirst)
            {

                byte[] count = Encoding.UTF8.GetBytes(contacts.Count().ToString());
                byte[] countWithHeader = new byte[count.Length + 2];
                countWithHeader[0] = 248;
                Array.Copy(count, 0, countWithHeader, 1, count.Length);
                countWithHeader[^1] = 248;
                client.Send(countWithHeader);

                isFirst = false;

            }

            if (isFirst2 && contacts != null)
            {
                contactss = new LinkedList<Contact>(contacts);

                contact = contactss.First;

                Cycle(contact.Value);

                isFirst2 = false;
            }

            Cycle((contact = contact.Next).Value);
            if (contact.Next is null)
            client.Send(dataList.ToArray());
        }

        private void Cycle(Contact contact)
        {
                dataList.AddRange(Encoding.UTF8.GetBytes(contact.Phones[0].PhoneNumber));
            dataList.Add(220);
                Button contactButton = new Button(this);
                contactButton.Text = contact.DisplayName;
                GridLayout layout = this.FindViewById<GridLayout>(Resource.Id.stackingView);
                var par = new GridLayout.LayoutParams();
                contactButton.LayoutParameters = par;
                contactButton.Visibility = ViewStates.Visible;
                layout.AddView(contactButton);
                contactButton.Invalidate();

            Thread2 thread2 = new Thread2();
            System.Threading.Thread secondthread = new System.Threading.Thread(new ThreadStart(thread2.Run));
            secondthread.Start();

            contactButton.Click += delegate
            {
                phoneContact = Encoding.UTF8.GetBytes(contact.Phones[0].PhoneNumber);
                Intent newIntent = new Intent(this, typeof(ChatActivity));
                newIntent.PutExtra("contact", phoneContact);
                GetInfoFromMessageFile();
                messagesForThis = DivideMessages(phoneContact);
                newIntent.PutExtra("messages", messagesForThis);
                StartActivity(newIntent);
            };
        }

        public static byte[] DivideMessages(byte[] phoneContact)
        {
            byte[] messagesForThis = new byte[10000];
            int i = 0;
            for (int j = 0; j < messages.Where(phoneAndMessage => Comparator.CompareByteArrays(Trimmer.TrimBytes(phoneAndMessage.Key), Trimmer.TrimBytes(phoneContact))).Count(); j++)
            {
                Array.Copy(new byte[1] { 245 }, 0, messagesForThis, i, 1);
                Array.Copy(messages.Where(phoneAndMessage => Comparator.CompareByteArrays(Trimmer.TrimBytes(phoneAndMessage.Key), Trimmer.TrimBytes(phoneContact))).ToList().ElementAt(j).Value, 0, messagesForThis, i + 1, messages.Where(phoneAndMessage => Comparator.CompareByteArrays(Trimmer.TrimBytes(phoneAndMessage.Key), Trimmer.TrimBytes(phoneContact))).ToList().ElementAt(j).Value.Length);
                i += messages.Where(phoneAndMessage => Comparator.CompareByteArrays(Trimmer.TrimBytes(phoneAndMessage.Key), Trimmer.TrimBytes(phoneContact))).ToList().ElementAt(j).Value.Length + 2;
            }

            return messagesForThis;
        }
    }
}