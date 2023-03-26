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

namespace AndApp
{
    [Activity(Label = "Main Menu")]
    public class MainMenuActivity : AppCompatActivity
    {
        public static Socket client;
        private EditText editTextForPhone;
        private TextView textView;
        private string str;
        private bool isFirst = true, isFirst2 = true;
        private LinkedList<Contact> contactss;
        private IEnumerable<Contact> contacts;
        private List<byte> dataList = new List<byte>();
        private LinkedListNode<Contact> contact;
        public static byte[] phoneContact { get; private set; }
        public static IPEndPoint iPEndPoint { get; private set; }

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
                            str = editTextForPhone.Text.ToString();
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

                byte[] re = Encoding.UTF8.GetBytes(str);

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
            Thread secondthread = new Thread(new ThreadStart(thread2.Run));
            secondthread.Start();

            contactButton.Click += delegate
            {
                phoneContact = Encoding.UTF8.GetBytes(contact.Phones[0].PhoneNumber);
                Intent newIntent = new Intent(this, typeof(ChatActivity));
                newIntent.PutExtra("contact", phoneContact);
                newIntent.PutExtra("message", "");
                StartActivity(newIntent);
            };
        }
    }
}