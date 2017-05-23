using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;

namespace SportsStore.Domain.Concrete
{
    public class EmailSetting
    {
        public string MailToAddress = "23837880@qq.com";
        public string MailFromAddress = "2542729322@qq.com";
        public bool UseSsl = true;
        public string Username = "zsq";
        public string Password = "zsq";
        public string ServerName = "smtp.qq.com";
        public int ServerPort = 587;
        public bool WriteAsFile = true;//没有邮件服务器，可以设置为true，写入到某个文件夹中
        public string FileLocation = @"d:\emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSetting emailSetting;

        public EmailOrderProcessor(EmailSetting setting)
        {
            this.emailSetting = setting;
        }

        public void ProcessOrder(Cart cart, ShippingDetail shippingDetail)
        {
            using (var smtpClient=new SmtpClient())
            {
                smtpClient.EnableSsl = emailSetting.UseSsl;
                smtpClient.Host = emailSetting.ServerName;
                smtpClient.Port = emailSetting.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSetting.Username, emailSetting.Password);

                if (emailSetting.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSetting.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                //处理发送的下单内容
                StringBuilder sb = new StringBuilder()
                    .AppendLine("你的下单请求已提交")
                    .AppendLine("---")
                    .AppendLine("物品有：");

                foreach (var item in cart.Lines)
                {
                    var subTotal = item.Product.Price * item.Quantity;
                    sb.AppendFormat("{0} x {1}  总额：{2:c}", item.Quantity, item.Product.Price, subTotal);
                }
                sb.AppendFormat("所有物品总金额为：{0:c}", cart.ComputeTotalValue());


                MailMessage msg = new MailMessage(emailSetting.MailFromAddress, emailSetting.MailToAddress, "下单已提交", sb.ToString());

                if (emailSetting.WriteAsFile)
                {
                    msg.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(msg);
            }
        }
    }
}
