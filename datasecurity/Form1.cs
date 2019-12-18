using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
namespace datasecurity
{
    public partial class Form1 : Form
    {
        List<string> passwords = new List<string>();
        List<string> encryptpass = new List<string>();
        List<string> decrypass = new List<string>();
        List<string> id = new List<string>();
        RijndaelManaged myRijndael = new RijndaelManaged();
        Aes encryptor = Aes.Create();
        byte[] key;
        Stopwatch _ProcessTimer = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }
      
        private void button1_Click(object sender, EventArgs e)
        {

            if(comboBox1.SelectedItem.ToString()=="AES")
            {
                _ProcessTimer.Start();
                encryptor.Mode = CipherMode.ECB;               
                key = Encoding.UTF8.GetBytes(@"myRijndael.GenerateKey()");
                encryptor.Key = key;
                //Encoding.UTF8.GetBytes(@"QQsaw!257()%%ert");
                //encryptor.IV = Encoding.UTF8.GetBytes(@"myRijndael.GenerateIV()");
        
                AESEncrypt(passwords, encryptor.Key);
                
            }
            if(comboBox1.SelectedItem.ToString()=="DES")
            {

            }



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("AES");
            comboBox1.Items.Add("DES");

            DbOku();
        }
        public void AESEncrypt(List<string>Passwords,byte[] key)
        {
            int say = 0;
            foreach (string index in passwords)
            {
                MemoryStream memoryStream = new MemoryStream();
                ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
                byte[] plainBytes = Encoding.ASCII.GetBytes(index);
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);

                cryptoStream.FlushFinalBlock();

                byte[] cipherBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                encryptpass.Add(Convert.ToBase64String(cipherBytes,0, cipherBytes.Length));
                say++;
            }
            writeDb(encryptpass);
            _ProcessTimer.Stop();
            textBox1.Text+= "Aes Encryp: " + _ProcessTimer.Elapsed.ToString(@"m\:ss\.ff")+"\n"; 
        }
        public void writeDb(List<string> encryptpass)
        {
            int i = 1;
            SqlCommand cmd = new SqlCommand();
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "Data Source=DESKTOP-SIMI68N;Initial Catalog=data_security;Persist Security Info=True;User ID=sa;Password=bedir123456;";
            cn.Open();
            foreach (var item in encryptpass)
            {
               
                cmd.Connection = cn;
                string komut = "update login set password="+"'"+item+"'"+ "where id=" +"'"+id[i-1]+"'";
                if(i==1001)
                {
                    break;
                }
                cmd.CommandText = komut;
                cmd.ExecuteNonQuery();             
                i++;
            }
            cn.Close();

        }
        public void AESDecrypt(List<string>encryptpass,byte[] key)
        {
           
           foreach (string index in encryptpass)
            {
                encryptor.Mode = CipherMode.ECB;
                encryptor.Key = key;

                MemoryStream memoryStream = new MemoryStream();
                byte[] cipherBytes = Convert.FromBase64String(index);
                ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
                byte[] hedef = aesDecryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
               // CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
                //
                
                //    cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                
                  //  cryptoStream.FlushFinalBlock();

                    //byte[] plainBytes = memoryStream.ToArray();
                   // memoryStream.Close();
                 //   cryptoStream.Close();
                    decrypass.Add(Encoding.UTF8.GetString(hedef));
                
               

           }
            writeDb(decrypass);
            _ProcessTimer.Stop();
            textBox1.Text += "Aes Decry: " + _ProcessTimer.Elapsed.ToString(@"m\:ss\.ff")+"\n";


        }

        public void DbOku()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "Data Source=DESKTOP-SIMI68N;Initial Catalog=data_security;Persist Security Info=True;User ID=sa;Password=bedir123456;";
            string komut = "select * from login";            
            SqlCommand cmd = new SqlCommand(komut,cn);
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                passwords.Add(dr["password"].ToString());
                id.Add(dr["id"].ToString());
            }
            cn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "AES")
            {
                _ProcessTimer.Start();
                AESDecrypt(encryptpass, key);
                writeDb(decrypass);

            }
            if (comboBox1.SelectedItem.ToString() == "DES")
            {

            }
        }
    }
}
