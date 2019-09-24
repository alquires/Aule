using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aule
{
    public partial class frmXML : Form
    {
        string VersaoPrj = "";
        public frmXML(string Versao)
        {
            InitializeComponent();
            VersaoPrj = Versao;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Arquivo xml|*.xml";
            SFD.Title = "Salva arquivo xml";

            if (SFD.ShowDialog() == DialogResult.OK & SFD.FileName != "")
            {
                string strXML = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" +
                    "<info>\r\n" +
                    "<Titulo>" + textBox2.Text + "</Titulo>\r\n" +
                    "<Versao>" + VersaoPrj + "</Versao>\r\n" +
                    "<Mensagem>" + textBox3.Text + "</Mensagem>\r\n" +
                    "<Resposta>" + checkBox1.Checked.ToString() + "</Resposta>\r\n" +
                    "<Apontamento>" + textBox1.Text + "</Apontamento>\r\n" + 
                    "</info>";

                System.IO.StreamWriter swArquivo = new System.IO.StreamWriter(SFD.FileName);
                swArquivo.Write(strXML);
                swArquivo.Close();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
                textBox1.Enabled = checkBox1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Arquivo xml|*.xml";
            ofd.Title = "Salva arquivo xml";

            if (ofd.ShowDialog() == DialogResult.OK & ofd.FileName != "")
            {
                System.IO.StreamReader srArquivo = new System.IO.StreamReader(ofd.FileName);
                DataSet ds = new DataSet();
                ds.ReadXml(ofd.FileName);
                string titulo = ds.Tables[0].Rows[0].ItemArray.GetValue(0).ToString();
                string mensagem = ds.Tables[0].Rows[0].ItemArray.GetValue(2).ToString();

                string strOpcaoMSG = ds.Tables[0].Rows[0].ItemArray.GetValue(3).ToString();
                bool boolOpcaoMSG = Boolean.Parse(strOpcaoMSG);

                string strLink = ds.Tables[0].Rows[0].ItemArray.GetValue(4).ToString();


                textBox2.Text = titulo;
                textBox3.Text = mensagem;
                textBox1.Text = strLink;
                checkBox1.Checked = boolOpcaoMSG;
                srArquivo.Close();
            }
        }
    }
}
