using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AndApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActiv : AppCompatActivity
    {
        private Socket client;
        public EditText textView { get; private set; }
        public EditText textVieww1 { get; private set; }
        public EditText textVieww2 { get; private set; }
        public TextView listView { get; private set; }
        public Button button { get; private set; }
        private string server = string.Empty, port = string.Empty;
        private bool isFirst = true;

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (isFirst)
            {
                button = FindViewById<Button>(Resource.Id.button1);
                button.LongClick += Button_Click;
                textView = this.FindViewById<EditText>(Resource.Id.message);
                textView.AfterTextChanged += TextView_TextChanged;
                isFirst= false;
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            client.Close();
            this.Dispose();
        }

        private void TextView_TextChanged(object sender, AfterTextChangedEventArgs e)
        {
            if (textView.Text.Length != 0 && textView.Text.EndsWith('@'))
            {
                if (client != null)
                {
                    listView = this.FindViewById<TextView>(Resource.Id.successOrNot);
                    try
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(textView.Text);
                        client.Send(data);
                        int i = 0;
                        listView.Text = "No response";

                        while (i++ < 200)
                        {
                            try
                            {

                                client.Receive(data, 0, 1, SocketFlags.Partial);
                            
                            if (data.Contains((byte)225))
                            {
                                listView.Text = "Success";
                                break;
                            }
                            }
                            catch (SocketException) { }
                        }
                        listView.Visibility = ViewStates.Visible;
                        textView.Text = string.Empty;
                    }
                    catch (Exception exc)
                    {
                        listView.Text = exc.Message;
                        listView.Visibility = ViewStates.Visible;
                    }
                }
            }
            return;
        }

        private void Button_Click(object sender, View.LongClickEventArgs e)
        {
            if (e.Handled)
            {
                textVieww1 = this.FindViewById<EditText>(Resource.Id.textView1);
                textVieww2 = this.FindViewById<EditText>(Resource.Id.textView2);
                server = (textVieww1.Text.Length > 0) ? textVieww1.Text : string.Empty;
                port = (textVieww2.Text.Length > 0) ? textVieww2.Text : string.Empty;
                TextView textView2 = this.FindViewById<TextView>(Resource.Id.listView1);
                if (server != string.Empty && port != string.Empty)
                    try
                    {
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        client.ReceiveTimeout = 5000;
                        byte[] data = { 0 };
                        long ipaddr = IPAddress.Parse(server).MapToIPv4().Address;
                        IPEndPoint endPoint = new IPEndPoint(ipaddr, int.Parse(port));
                        client.Connect(endPoint);
                        client.Send(data);
                        textVieww1.Visibility = ViewStates.Gone;
                        textVieww2.Visibility = ViewStates.Gone;
                        FindViewById<Button>(Resource.Id.button1).Visibility = ViewStates.Gone;
                        textView2.Visibility = ViewStates.Gone;
                        textView.Visibility = ViewStates.Visible;
                    }
                    catch (Exception ex)
                    {
                        textView2.Text = ex.Message;
                    }
                e.Handled = false;
                return;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}