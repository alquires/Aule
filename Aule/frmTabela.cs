using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Aule
{
    public partial class frmTabela : Form
    {
        //=========================================
        //         Variaveis e declaracoes
        //=========================================
        //propriedades psq
        public string[] ValoresTabela = new string[16];

        string[] ColTabBase;
        string SobeDesce = "";
        //=========================================
        //              construtor
        //=========================================
        public frmTabela(string tabela)
        {
            InitializeComponent();


            if (tabela != "")
            {
                string[] strTabela = tabela.Split('¬'); //aqui fica a tabela com os dados das colunas

                ColTabBase = new string[strTabela.Length];
                for (int a = 0; a < strTabela.Length; a++)
                {
                    string[] strTemp = strTabela[a].Split('#');
                    if (1 < strTemp.Length)
                    { ColTabBase[a] = strTemp[1]; }
                }
            }
        }

        //=========================================
        //                Métodos
        //=========================================
        /// <summary>
        /// Pega os campos da tabela para preencher o DataGridView
        /// </summary>
        /// <param name="Tabela">tabela cujos campos serão retirados</param>
        private void GetCampos(DataTable Tabela)
        {
            int intNumCol = Tabela.Columns.Count;

            for (int i = 0; i < intNumCol; i++)
            {
                DataGridViewCell dgvcCB = new DataGridViewCheckBoxCell();
                dgvcCB.Value = true;

                DataGridViewCell dgvcTB1 = new DataGridViewTextBoxCell();
                dgvcTB1.Value = Tabela.Columns[i].ColumnName;

                DataGridViewCell dgvcTB2 = new DataGridViewTextBoxCell();
                dgvcTB2.Value = Tabela.Columns[i].ColumnName;

                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.Cells.Add(dgvcCB);
                dgvr.Cells.Add(dgvcTB1);
                dgvr.Cells.Add(dgvcTB2);
                dataGridView1.Rows.Add(dgvr);
                
            }//fim do laço FOR
        }//Fim do metodo GetCampos
        /*private void GetCampos(string[] valores)
        {
            int intNumCol = valores.Length;

            for (int i = 0; i < intNumCol - 1; i++)
            {
                string[] subValor = valores[i].Split('#');
                DataGridViewCell dgvcCB = new DataGridViewCheckBoxCell();
                dgvcCB.Value = Boolean.Parse(subValor[0]);

                DataGridViewCell dgvcTB1 = new DataGridViewTextBoxCell();
                dgvcTB1.Value = subValor[1];

                DataGridViewCell dgvcTB2 = new DataGridViewTextBoxCell();
                dgvcTB2.Value = subValor[2];

                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.Cells.Add(dgvcCB);
                dgvr.Cells.Add(dgvcTB1);
                dgvr.Cells.Add(dgvcTB2);
                dataGridView1.Rows.Add(dgvr);

            } //fim do laço FOR
        
        }*/
        /// <summary>
        /// <para>Preenche os ComboBoxes com os nomes das colunas da tabela selecionada.</para>
        /// <para>Metodo para tabela base.</para>
        /// </summary>
        /// <param name="Tabela">tabela de onde será retirada as colunas</param>
        private void PreencheComboBox(DataTable Tabela)
        {
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            int intNumCol = Tabela.Columns.Count;
            for (int i = 0; i < intNumCol; i++)
            {

                if (Tabela.Columns[i].DataType.ToString() == "System.Int32" |
                    Tabela.Columns[i].DataType.ToString() == "System.Double")
                {
                    comboBox4.Items.Add(Tabela.Columns[i].ColumnName);
                }
                comboBox5.Items.Add(Tabela.Columns[i].ColumnName);
            }
        }

        /// <summary>
        /// <para>Preenche os ComboBoxes com os nomes das colunas da tabela selecionada.</para>
        /// <para>Metodo para tabela base.</para>
        /// </summary>
        /// <param name="Tabela">tabela de onde será retirada as colunas</param>
        /// <param name="TabelaBase">Tabela base onde será retirada as colunas</param>
        private void PreencheComboBox(DataTable Tabela, string[] colunasBase)
        {
            int intNumCol = Tabela.Columns.Count;
            comboBox5.Items.Clear();
            for (int i = 0; i < intNumCol; i++)
            {
                comboBox5.Items.Add(Tabela.Columns[i].ColumnName);
            }

            if (colunasBase != null)
            {
                intNumCol = colunasBase.Length;
            }
            else
            {
                intNumCol = 0;
            }
            comboBox4.Items.Clear();
            for (int i = 0; i < intNumCol; i++)
            {
                if (colunasBase[i] != null)
                { comboBox4.Items.Add(colunasBase[i]); }
            }
        }

        /// <summary>
        /// Faz a verificação se os parametros estão todos preenchidos e corretos
        /// </summary>
        /// <returns>Retorna um valor boleano, informando se o está tudo certo</returns>
        private bool Verifica()
        {
            bool BoleanoBoleada = true;


            return BoleanoBoleada;
        }
        private void CarregaTabela()
        {
            BD ClasseDB = new BD();
            DataTable dt = new DataTable();
            //DataTable dtBase = new DataTable();
            dataGridView1.Rows.Clear();

            if (checkBox1.Checked)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        dt = ClasseDB.DB_Access(textBox1.Text, textBox6.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        PreencheComboBox(dt);
                        break;

                    case 1:
                        dt = ClasseDB.DB_MySql(textBox5.Text, textBox1.Text, textBox6.Text, 
                            textBox2.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        PreencheComboBox(dt);
                        break;

                    case 2:
                        dt = ClasseDB.BD_PostgreSQL(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox4.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        PreencheComboBox(dt);
                        break;

                }
            }
            else
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        dt = ClasseDB.DB_Access(textBox1.Text, textBox6.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        //dtBase = ClasseDB.DB_Access(textBox1.Text, textBox6.Text, "", textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        PreencheComboBox(dt, ColTabBase);
                        break;

                    case 1:
                        dt = ClasseDB.DB_MySql(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox7.Text, textBox8.Text);

                        //dtBase = ClasseDB.DB_MySql(textBox5.Text, textBox1.Text, textBox6.Text,
                            //textBox2.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        
                            GetCampos(dt);
                            PreencheComboBox(dt, ColTabBase);
                        break;

                    case 2:
                            dt = ClasseDB.BD_PostgreSQL(textBox5.Text, textBox1.Text, textBox6.Text,
                                textBox2.Text, textBox3.Text, textBox4.Text, textBox7.Text, textBox8.Text);
                        
                            //dtBase = ClasseDB.BD_PostgreSQL(textBox5.Text, textBox1.Text, textBox6.Text,
                                //Text, textBox3.Text, textBox4.Text, textBox7.Text, textBox8.Text);

                            GetCampos(dt);
                            PreencheComboBox(dt, ColTabBase);
                        break;

                }
            }

        }
        private void CarregaTabela(string valorTabela)
        {
            BD ClasseDB = new BD();
            DataTable dt = new DataTable();

            string[] tabela = valorTabela.Split('¬');

            if (checkBox1.Checked)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        dt = ClasseDB.DB_Access(textBox1.Text, textBox6.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        CarregaAtribTabela(tabela);
                        //GetCampos(tabela);
                        PreencheComboBox(dt);
                        break;

                    case 1:
                        dt = ClasseDB.DB_MySql(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        CarregaAtribTabela(tabela);
                        //GetCampos(tabela);
                        PreencheComboBox(dt);
                        break;

                    case 2:
                        dt = ClasseDB.BD_PostgreSQL(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox4.Text, textBox7.Text, textBox8.Text);

                        GetCampos(dt);
                        CarregaAtribTabela(tabela);

                        //GetCampos(tabela);
                        PreencheComboBox(dt);
                        break;

                }
            }
            else
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        dt = ClasseDB.DB_Access(textBox1.Text, textBox6.Text, textBox3.Text, textBox7.Text, textBox8.Text);
                        GetCampos(dt);
                        CarregaAtribTabela(tabela);
                        //GetCampos(tabela);
                        PreencheComboBox(dt, ColTabBase);
                        break;

                    case 1:
                        dt = ClasseDB.DB_MySql(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox7.Text, textBox8.Text);

                        GetCampos(dt);
                        CarregaAtribTabela(tabela);
                        //GetCampos(tabela);
                        PreencheComboBox(dt, ColTabBase);
                        break;

                    case 2:
                        dt = ClasseDB.BD_PostgreSQL(textBox5.Text, textBox1.Text, textBox6.Text,
                            textBox2.Text, textBox3.Text, textBox4.Text, textBox7.Text, textBox8.Text);

                        GetCampos(dt);
                        CarregaAtribTabela(tabela);
                        //GetCampos(tabela);
                        PreencheComboBox(dt, ColTabBase);
                        break;

                }
            }

        }
        private void CarregaAtribTabela(string[] valores)
        {
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            { 
                for (int b = 0; b < valores.Length-1; b++)
                {
                    string[] subValor = valores[b].Split('#');
                    if (dataGridView1.Rows[a].Cells[1].Value.ToString() == subValor[1])
                    {
                        dataGridView1.Rows[a].Cells[0].Value = Boolean.Parse(subValor[0]);

                        dataGridView1.Rows[a].Cells[2].Value = subValor[2];
                    }
                }
            }
        }
        private void MudaTexto()
        {
            switch (checkBox1.Checked)
            {
                case true:
                    label9.Text = "Coluna shapefile";
                    label8.Text = "Coluna de área";
                    break;

                case false:
                    label9.Text = "Coluna de Ligação (tabela)";
                    label8.Text = "Coluna de Ligação (base)";
                    //PegaColunaLigacao();
                    break;
            }
        }
        private void sobe()
        {
            try
            {
                
                int totalRows = dataGridView1.Rows.Count;
                int idx = dataGridView1.SelectedCells[0].OwningRow.Index;
                if (idx == 0)
                    return;
                int col = dataGridView1.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = dataGridView1.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx - 1, row);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[idx - 1].Cells[col].Selected = true;
            }
            catch { }

        }
        private void desce()
        {
            try
            {
                int totalRows = dataGridView1.Rows.Count;
                int idx = dataGridView1.SelectedCells[0].OwningRow.Index;
                if (idx == totalRows - 2)
                    return;
                int col = dataGridView1.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = dataGridView1.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx + 1, row);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[idx + 1].Cells[col].Selected = true;
            }
            catch { }
        }
        private void primeiro()
        { }
        private void ultimo()
        { }
        //=========================================
        //|            Propriedades               |
        //=========================================
        public string[] prpStrBasica
        {
            get { return this.ValoresTabela; }
            set { this.ValoresTabela = value; }
        }

        //=========================================
        //             Eventos Diversos
        //=========================================
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmTabela_Load(object sender, EventArgs e)
        {
            if (ValoresTabela[0] == "")
            {
                comboBox1.SelectedIndex = 0;

            }
            else
            {
                comboBox1.SelectedIndex = Int32.Parse(ValoresTabela[0])-1;
                textBox1.Text = ValoresTabela[1];
                textBox6.Text = ValoresTabela[3];
                textBox5.Text = ValoresTabela[6];
                textBox2.Text = ValoresTabela[7];
                textBox3.Text = ValoresTabela[8];
                textBox4.Text = ValoresTabela[9];
                textBox7.Text = ValoresTabela[10];
                textBox8.Text = ValoresTabela[11];
                checkBox1.Checked = Boolean.Parse(ValoresTabela[13]);

                string[] strColunas = ValoresTabela[15].Split('#');
                CarregaTabela(ValoresTabela[15]);

                int ValorCboBox = 0;

                for (int i = 0; i < comboBox5.Items.Count; i++)
                {
                    string valorComboBox = comboBox5.Items[i].ToString();
                    string valorColLigacao = ValoresTabela[4];

                    if (valorColLigacao == valorComboBox)
                    {
                        ValorCboBox = i;
                        i = comboBox5.Items.Count;
                    }
                }//fim do laço for


                comboBox5.SelectedIndex = ValorCboBox;

                ValorCboBox = 0;
                for (int i = 0; i < comboBox4.Items.Count; i++)
                {
                    string valorComboBox = comboBox4.Items[i].ToString();
                    string valorColLigacao = ValoresTabela[5];

                    if (valorColLigacao == valorComboBox)
                    { 
                        ValorCboBox = i;
                        i = comboBox4.Items.Count;
                    }
                }//fim do laço for
                comboBox4.SelectedIndex = ValorCboBox;


            }// fim do Else
        }//fim do evento frmTabela_Load
        private void button6_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "All Files|*.*";
                    ofd.Title = "Abre banco de dados";
                    ofd.InitialDirectory = "C:\\Geoprocessamento\\Visualizador\\aprendizado\\Proj_exemplo\\" + 
                        "proj009_zaal001\\pesq";
                    ofd.Filter = "All Files|*.*";
                    ofd.Title = "Abre banco de dados";
                    ofd.InitialDirectory = "C:\\Geoprocessamento\\Visualizador\\aprendizado\\Proj_exemplo\\proj009_zaal001\\pesq";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = ofd.FileName;
                    }//fim do "if (ofd.ShowDialog() == DialogResult.OK)"

           }//fim do "if(comboBox1.SelectedIndex == 0)"
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            CarregaTabela();
            //MessageBox.Show(ColTabBase.Length.ToString());
            button3.Enabled = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //string parametros = "";
            if (Verifica() == true)
            {
                //parametro [n,0] de *.psq
                int intTmp000 = comboBox1.SelectedIndex + 1;
                prpStrBasica[0] = intTmp000.ToString();
                
                //parametro [n,1] de *.psq
                prpStrBasica[1] = textBox1.Text;
                
                //parametro [n,2] de *.psq
                prpStrBasica[2] = textBox9.Text;
                
                //parametro [n,3] de *.psq
                prpStrBasica[3] = textBox6.Text;
                
                //parametro [n,4] de *.psq
                prpStrBasica[4] = comboBox5.Text;
                
                //parametro [n,5] de *.psq
                prpStrBasica[5] = comboBox4.Text;
                
                //parametro [n,6] de *.psq
                prpStrBasica[6] = textBox5.Text;
                
                //parametro [n,7] de *.psq
                prpStrBasica[7] = textBox2.Text;
                
                //parametro [n,8] de *.psq
                prpStrBasica[8] = textBox3.Text;
                
                //parametro [n,9] de *.psq
                prpStrBasica[9] = textBox4.Text;
                
                //parametro [n,10] de *.psq
                prpStrBasica[10] = textBox7.Text;
                
                //parametro [n,11] de *.psq
                prpStrBasica[11] = textBox8.Text;
                
                //parametro [n,12] de *.psq
                prpStrBasica[12] = textBox9.Text +".txt";
                
                //é tabela base?
                prpStrBasica[13] = checkBox1.Checked.ToString();
                
                //Incluir a tabela no projeto?
                prpStrBasica[14] = checkBox2.Checked.ToString();

                //estrutura da tabela
                string tmp001 = "";
                for (int i = 0; i < dataGridView1.RowCount; i++) //linhas
                {
                    tmp001 += dataGridView1.Rows[i].Cells[0].Value.ToString() + "#" +
                        dataGridView1.Rows[i].Cells[1].Value.ToString() + "#" +
                        dataGridView1.Rows[i].Cells[2].Value.ToString() + "¬";
                }
                prpStrBasica[15] = tmp001;

            }//fim do if (Verifica() == true)

            if (this.Owner != null && this.Owner is frmPrincipal)
            {
                ((frmPrincipal)this.Owner).RecebeConfTabela(this.prpStrBasica);
            }
                


            this.Close();
        }//fim do evento button1_Click
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MudaTexto();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBox2.Enabled = false;
                    textBox4.Enabled = false;
                    textBox5.Enabled = false;
                    button6.Enabled = true;
                    checkBox2.Enabled = true;
                    break;

                case 1:
                    textBox2.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    button6.Enabled = false;
                    checkBox2.Enabled = false;
                    checkBox2.Checked = false;
                    break;

                case 2:
                    textBox2.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    button6.Enabled = false;
                    checkBox2.Enabled = false;
                    checkBox2.Checked = false;
                break;
            }
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox9.Text = textBox6.Text;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Arquivo de texto|*.txt";
            saveFileDialog1.Title = "Salvar projeto Aulë";
            //saveFileDialog1.ShowDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK & saveFileDialog1.FileName != "")
            {
                string strTmpatributos = "";

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    strTmpatributos += dataGridView1.Rows[i].Cells[0].Value.ToString() + "#" +
                        dataGridView1.Rows[i].Cells[1].Value.ToString() + "#" +
                        dataGridView1.Rows[i].Cells[2].Value.ToString() + "\r\n"; //inclui o \r
                }

                StreamWriter Leitor = new StreamWriter(saveFileDialog1.FileName);
                Leitor.Write(strTmpatributos);
                Leitor.Close();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Arquivo de texto|*.txt";
            ofd.Title = "Abre atributos";
            if (ofd.ShowDialog() == DialogResult.OK & ofd.FileName != "")
            {
                StreamReader Leitor = new StreamReader(ofd.FileName);
                string strTmpatributos = Leitor.ReadToEnd();
                Leitor.Close();
                strTmpatributos = strTmpatributos.Replace("\r","");
                string[] strValAtributos = strTmpatributos.Split('\n');

                //dataGridView1.Rows.Add(strValAtributos.Length);

                for (int i = 0; i < strValAtributos.Length - 1; i++)
                {
                    string[] valLinhas = strValAtributos[i].Split('#');

                    int Linhas = dataGridView1.Rows.Count;

                    for (int a = 0; a < Linhas; a++)
                    {
                        string valores = dataGridView1.Rows[a].Cells[1].Value.ToString();
                        if (valores == valLinhas[1])
                        {

                            dataGridView1.Rows[a].Cells[0].Value = Boolean.Parse(valLinhas[0]);
                            dataGridView1.Rows[a].Cells[2].Value = valLinhas[2];
                            a = Linhas;
                        }
                    }                    
                }
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            desce();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            sobe();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (SobeDesce)
            {
                case "desce":
                    desce();
                    break;
                case "sobe":
                    sobe();
                    break;
            }
        }
        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            SobeDesce = "desce";
            System.Threading.Thread.Sleep(250);
            timer1.Enabled = true;
            timer1.Interval = 100;
        }
        private void button9_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
        }
        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            SobeDesce = "sobe";
            System.Threading.Thread.Sleep(250);
            timer1.Enabled = true;
            timer1.Interval = 100;
        }
        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
        }
    }//fim da classe
}//fim do namespace
