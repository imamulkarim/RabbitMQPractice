using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel;
using System.Text;
using System.Threading.Channels;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RabbitChat
{
    public partial class Form1 : Form
    {

        string host = "192.168.1.24";
        string user = "root";
        string password = "123456";
        int port = 5672;

        string ChaneelNameUserBroadCast = "tonmoy.userbroadcast";
        ConnectionFactory factory;
        IConnection connection;

        string ChaneelNameMessageRequest = "tonmoy.user.message.channel.request";


        string _chatUser;
        BindingList<UserClass> userClasses;

        public Form1()
        {
            InitializeComponent();
            userClasses = new();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frmNameAdd frm = new frmNameAdd();
            frm.ShowDialog();

            if (string.IsNullOrEmpty(frm._name))
            {
                Application.Exit();
                return;
            }

            _chatUser = Guid.NewGuid().ToString().Substring(0,5) + "-" + frm._name;
            lblUser.Text = "Welcome " + _chatUser;

            try
            {
                factory = new ConnectionFactory() { HostName = host, UserName = user, Password = password, Port = port };

                connection = factory.CreateConnection();


                if (channelUserBroadCast == null)
                {

                    channelUserBroadCast = connection.CreateModel();

                    channelUserBroadCast.ExchangeDeclare(exchange: ChaneelNameUserBroadCast, type: ExchangeType.Fanout);

                    //PublishUserName();
                    Thread thread = new Thread(PublishUserName);
                    thread.Start();
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        userClasses.Add(new UserClass { UserName = "demo1", Messages = new() });
                        userClasses.Add(new UserClass { UserName = "demo2", Messages = new() });
                        UserDataBind();
                    }
                }


                {

                    channelUserReceiveer = connection.CreateModel();


                    channelUserReceiveer.ExchangeDeclare(exchange: ChaneelNameUserBroadCast, type: ExchangeType.Fanout);

                    var queueName = channelUserReceiveer.QueueDeclare().QueueName;
                    channelUserReceiveer.QueueBind(queue: queueName,
                                      exchange: ChaneelNameUserBroadCast,
                                      routingKey: "");

                    
                    #region reciveing


                    var consumer = new EventingBasicConsumer(channelUserReceiveer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        if (message != _chatUser)
                            if (userClasses.Where(m => m.UserName == message).FirstOrDefault() == null)
                            {
                                userClasses.Add(new UserClass { UserName = message, Messages = new List<Message>() });
                                UserDataBind();
                                PublishUserName();
                            }


                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channelUserReceiveer.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    #endregion

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection fail");
            }


        }

        private void PublishUserName()
        {
            var body = Encoding.UTF8.GetBytes(_chatUser);
            Thread.Sleep(2000);

            channelUserBroadCast.BasicPublish(exchange: ChaneelNameUserBroadCast,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
        }

        IModel channelUserBroadCast;
        IModel channelUserReceiveer;
       

        private void UserDataBind()
        {
            

            listBox1.Invoke((MethodInvoker)(() => {
                listBox1.DataSource = userClasses;
                listBox1.DisplayMember = "UserName";
                listBox1.ValueMember = "UserName";
            }));
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is null)
                return;
            var data = listBox1.SelectedItem as UserClass;

            if (data is null)
                return;

            if (data.Messages is null)
                return;


            richTextBox1.ResetText();

            foreach (var item in data.Messages)
            {
                if (item.sender is not null)
                    richTextBox1.AppendText($"{item.sender} : {item.message} " );

                if (item.receiver is not null)
                    richTextBox1.AppendText($"{item.receiver} : {item.message} ");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            if (listBox1.SelectedItem is null)
            {
                MessageBox.Show("Select a user");
                return;
            }
            var data = listBox1.SelectedItem as UserClass;

            if (data is null)
            {
                MessageBox.Show("Select a user");
                return;
            }
        }
    }
}
