using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.PublisherControls;
using System.Threading;



namespace Aule
{
    public partial class frmPrincipal : Form
    {
        
        //=========================================
        //         Variaveis e declaracoes
        //=========================================
        //variaveis de propriedades
        /// <summary>
        /// variavel da propriedade ParaPSQ;
        /// </summary>
        public string[] strParPesq = new string[16];
        public string valorAtributos = "";
        string strLayers = "";

        //variavel de recebimento de propriedade
        /// <summary>
        /// Variavel que recebe os valores da propriedade ParaPSQ (strParPesq) e o junta na matriz 2D
        /// </summary>
        private string[,] ParametrosPesquisa = new string[10,16];
        /// <summary>
        /// guardará os atributos de pesquisa
        /// </summary>
        private string[] Atributos = new string[10];
        
        /// <summary>
        /// Verifica se o programa está em modo de alteração ou se é um novo registro
        /// </summary>
        public bool blAlteracao;
        private string projeto = "";
        //=========================================
        //              construtor
        //=========================================
        public frmPrincipal()
        {
            InitializeComponent();
        }

        //=========================================
        //                Métodos
        //=========================================
        private string Abre(string TipoArquivo, string Titulo)
        {
            
            string tmpAbre = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = TipoArquivo;
            ofd.Title = Titulo;
            //ofd.InitialDirectory = pastainicial;
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName != "")
            {
                tmpAbre = ofd.FileName;
            }
            return tmpAbre;
        
        }
        private void PegaLayer(IARLayer Camada, string nivel)
        {
            if (Camada.IsGroupLayer)
            {
                //gruposLaiers = nivel + "$" + Camada.Name;
                for (int i = 0; i < Camada.ARLayerCount; i++)
                {
                    ARLayer Leier = Camada.get_ChildARLayer(i);
                    PegaLayer(Leier, nivel + "$" + Camada.Name);
                } 
            }
            else
            {
                // um erro ocorre aqui quando o pi está na raiz do projeto.
                // O string Nivel permanece vazio, de modo que não é possivel retirar o "substring(1)"
                // mas isso fica pra amanha, pois já é 17h
                if (nivel != "")
                {
                    strLayers += nivel.Substring(1) + "$" + Camada.Name + "*";
                }
                else
                {
                    strLayers += Camada.Name + "*";
                }
            }
        }
        private void CarregaLayers(string PathPMF)
        {
            try
            {
                ARC.LoadDocument(PathPMF, textBox13.Text);
                for (int i = 0; i < ARC.ARPageLayout.FocusARMap.ARLayerCount; i++)
                {
                    //ArcReaderSearchDef arcReaderSearchDef = new ArcReaderSearchDefClass();
                    PegaLayer(ARC.ARPageLayout.FocusARMap.get_ARLayer(i), "");
                }

                string[] NomesCamadas = strLayers.Split('*');
                for (int a = 0; a < NomesCamadas.Length - 1; a++)
                {
                    comboBox4.Items.Add(NomesCamadas[a]);
                    comboBox3.Items.Add(NomesCamadas[a]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void RecebeConfTabela(string[] Parametros)
        {
            if (blAlteracao)
            {
                int intTmp001 = listBox1.SelectedIndex;
                for (int i = 0; i < 16; i++)
                {
                    ParametrosPesquisa[intTmp001, i] = Parametros[i];
                }
                if (Parametros[13] == "False")
                {
                    int posicao = listBox1.SelectedIndex;
                    listBox1.Items[posicao] = Parametros[3];
                }
                else if (Parametros[13] == "True")
                {
                    int posicao = listBox1.SelectedIndex;
                    string tmp001 = "*" + Parametros[3];
                    listBox1.Items[posicao] = tmp001;
                }
            }
            else
            {
                if (Parametros[13] == "False")
                {
                    listBox1.Items.Add(Parametros[3]);
                }
                else if (Parametros[13] == "True")
                {
                    string tmp001 = "*" + Parametros[3];
                    listBox1.Items.Add(tmp001);
                }
                int intTmp001 = listBox1.Items.Count - 1;

                for (int i = 0; i < 16; i++)
                {
                    ParametrosPesquisa[intTmp001, i] = Parametros[i];
                }
                if (listBox1.Items.Count == 10)
                {
                    button1.Enabled = false;
                }

            }
        }

        //retorna ou o texto ou a estrutura numerica do shapefile de pesquisa/area de estudo
        //importante em função da estrutura com "Agrupadores de camadas"
        private string pegaSeqTabelas(string Shape)
        {
            string[] strColunasLeier = Shape.Split('$');


            string vRetorno = "";
            int vPosicao = -1;
            IARLayer Leier = ARC.ARPageLayout.FocusARMap.get_ARLayer(0);

            for (int a = 0; a < strColunasLeier.Length; a++)
            {
                if (a == 0)
                {
                    vPosicao = vasculhaShape(ARC.ARPageLayout.get_ARMap(0), strColunasLeier[a]);
                    Leier = ARC.ARPageLayout.FocusARMap.get_ARLayer(vPosicao);
                }
                else
                {
                    vPosicao = vasculhaShape(Leier, strColunasLeier[a]);
                    if (a < strColunasLeier.Length - 1)
                    {
                        // é aqui q a magica tem que acontecer!
                        Leier = Leier.get_ChildARLayer(vPosicao);
                    }
                    else
                    {
                        Leier = Leier.get_ChildARLayer(vPosicao);
                        if (Leier.IsGroupLayer == false & Leier.Searchable)
                        {
                            ArcReaderSearchDef arcReaderSearchDef = new ArcReaderSearchDefClass();
                            ARFeatureSet feicoes = Leier.QueryARFeatures(arcReaderSearchDef);
                            //**************************************************

                            for (int b = 0; b < feicoes.get_ARFeature(0).FieldCount; b++)
                            {
                                vRetorno += feicoes.get_ARFeature(0).get_FieldName(b) + "*";
                            }
                            return vRetorno;
                        }
                        else
                        { MessageBox.Show("O Plano de Informação selecionado não é um formato pesquisável."); }
                    }
                }
            }
            return vRetorno;
        }
        private string pegaSeqNumerica(string Shape)
        {
            string[] strColunasLeier = Shape.Split('$');
            string vRetorno = "";
            int vPosicao = -1;
            IARLayer Leier = ARC.ARPageLayout.FocusARMap.get_ARLayer(0);



            for (int a = 0; a < strColunasLeier.Length; a++)
            {
                if (a == 0)
                {
                    vPosicao = vasculhaShape(ARC.ARPageLayout.get_ARMap(0), strColunasLeier[a]);
                    Leier = ARC.ARPageLayout.FocusARMap.get_ARLayer(vPosicao);
                }
                else
                {
                    vPosicao = vasculhaShape(Leier, strColunasLeier[a]);
                    if (a < strColunasLeier.Length - 1)
                    {Leier = Leier.get_ChildARLayer(vPosicao);}
                }
                    vRetorno += vPosicao + "*";
            }

            return vRetorno;
        }
        private int vasculhaShape(IARLayer camada, string nomecamada)
        {
            int Valorindice = -1;
            for (int a = 0; a < camada.ARLayerCount; a++)
            {
                if (camada.get_ChildARLayer(a).Name == nomecamada)
                {
                    Valorindice = a;
                    return Valorindice;
                }
            }
            return Valorindice;
        }
        private int vasculhaShape(IARMap Mapa, string nomecamada)
        {
            int Valorindice = -1;
            for (int a = 0; a < Mapa.ARLayerCount; a++)
            {
                if (Mapa.get_ARLayer(a).Name == nomecamada)
                {
                    Valorindice = a;
                    return Valorindice;
                }
            }
            return Valorindice;
        }
        private bool Verifica()
        {
            bool BoleanoBoleada = true;

            /*************************
             *       ABA GERAL
             ************************/

            //confere o título
            if (textBox11.Text == "")
            {
                MessageBox.Show("O projeto não possui título.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }


            //verifica o nome síntese
            if (textBox9.Text == "")
            {
                MessageBox.Show("Você não escreveu o nome síntese do projeto." +
                  "\r\nO nome síntese é importante para a nomenclatura das pastas e dos arquivos");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }


            //verifica a pasta de trabalho
            if (textBox7.Text != "")
            {
                try
                { string strTmp = Path.GetDirectoryName(textBox7.Text); }
                catch
                {
                    MessageBox.Show("O endereço da pasta de trabalho escolhida é inválido ou inexistente.");
                    BoleanoBoleada = false;
                    return BoleanoBoleada;
                }
            }
            else
            {
                MessageBox.Show("O endereço da pasta de trabalho escolhida é inválida ou inexistente.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }


            //ajuda
            if (textBox5.Text == "")
            {
                MessageBox.Show("Não há arquivo ou página de ajuda selecionada. Verifique o nome do arquivo ou o endereço.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }
            else
            {
                switch (cbbTipoAjuda.SelectedIndex)
                {
                    case 0:
                        try
                        { string strTmp = Path.GetFullPath(textBox5.Text); }
                        catch
                        {
                            MessageBox.Show("O arquivo escolhido é inválido ou inexistente.");
                            BoleanoBoleada = false;
                            return BoleanoBoleada;
                        }
                        break;

                    case 1:
                        try
                        {
                            Uri Yuri = new Uri(textBox5.Text);
                        }
                        catch
                        {
                            MessageBox.Show("O endereço da página de internet escolhida como serviço de ajuda e suporte é inválida");
                            BoleanoBoleada = false;
                            return BoleanoBoleada;
                        }                        
                        break;

                    case 2:
                        try
                        { string strTmp = Path.GetFullPath(textBox5.Text); }
                        catch
                        {
                            MessageBox.Show("O arquivo escolhido é inválido ou inexistente.");
                            BoleanoBoleada = false;
                            return BoleanoBoleada;
                        }
                        break;

                    default:
                        MessageBox.Show("Não há arquivo ou página de ajuda selecionada.");
                        BoleanoBoleada = false;
                        return BoleanoBoleada;
                       
                }
            }


            //verifica imagens
            if (textBox10.Text == "" | textBox1.Text == "" | textBox2.Text == "")
            {
                MessageBox.Show("Não foi selecionado nenhum arquivo de imagem síntese, rodapé ou cabeçalho.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }
            else
            {
                try
                {
                    string strTmp1 = Path.GetFullPath(textBox10.Text);
                    Image img1 = Image.FromFile(strTmp1);
                    if (img1.Height != 235 | img1.Width != 440)
                    {
                        MessageBox.Show("A imagem síntese deve ter 440 x 235 pixels. " +
                            " A imagem selecionada não possui essas características.");
                        BoleanoBoleada = false;
                        return BoleanoBoleada;
                    }

                    string strTmp2 = Path.GetFullPath(textBox1.Text);
                    Image img2 = Image.FromFile(strTmp2);
                    string strTmp3 = Path.GetFullPath(textBox2.Text);
                    Image img3 = Image.FromFile(strTmp3);
                }
                catch
                {
                    MessageBox.Show("O arquivo de imagem (síntese, rodapé ou cabeçalho) escolhido é inválido ou inexistente.");
                    BoleanoBoleada = false;
                    return BoleanoBoleada;
                }

            }
            
            //sobre o projeto
            if (textBox8.Text == "")
            {
                MessageBox.Show("A descrição o projeto está em branco." +
                    " Embora seja uma informação opcional, é relevante que sejam colocadas informações complementares.");
            }


            /*************************
             *       ABA VISUAL
             ************************/
            if (textBox12.Text != "")
            {
                try
                {
                    string strTmp = Path.GetExtension(textBox12.Text);
                    if (strTmp != ".pmf")
                    {
                        MessageBox.Show("O projeto visual do ArcGis Publish escolhido é inválido ou está num formato incorreto.");
                        BoleanoBoleada = false;
                        return BoleanoBoleada;
                    }
                }
                catch
                {
                    MessageBox.Show("O arquivo .pmf escolhido é inválido ou inexistente.");
                    BoleanoBoleada = false;
                    return BoleanoBoleada;
                }
            }
            else
            {
                MessageBox.Show("O arquivo .pmf escolhido é inválido ou inexistente.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }

            //colunas e shapes de pesquisa e seleção
            if (comboBox1.SelectedIndex == -1 | comboBox2.SelectedIndex == -1 |
                comboBox3.SelectedIndex == -1 | comboBox4.SelectedIndex == -1)
            {
                MessageBox.Show("É necessário que você escolha a camada de pesquisa e área de estudo," + 
                    " bem como as colunas que fazem a ligação com a tabela base.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }

            /*************************
             *       ABA Pesquisa
             ************************/
            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("A unidade de area não foi definida.");
                BoleanoBoleada = false;
                return BoleanoBoleada;
            }


            return BoleanoBoleada;
        }
        private void CarregaPMF()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();


            if (textBox12.Text != "")
            {
                bool boolTmp001 = ARC.CheckDocument(textBox12.Text);
                if (boolTmp001 == true)
                {
                    CarregaLayers(textBox12.Text);
                }
                else
                {
                    MessageBox.Show("O projeto *.pmf selecionado não possui permissão para abertura em" +
                      " aplicações personalizadas.");
                }
            }
        }
        private void SalvaProjeto(string ArquivoAule)
        {
            //------------
            //  tabGeral
            //------------
            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 10;
            //+++++++++++++++++++++++++++++++++

            toolStripStatusLabel2.Text = "Salvando arquivo Aulë";
            toolStripProgressBar2.Value = 0;

            string strTmp000 = textBox8.Text.Replace("\n", "*");
            string strTextoGeral =
                textBox11.Text + "\n" + //0
                textBox9.Text + "\n" + //1
                textBox7.Text + "\n" + //2
                cbbTipoAjuda.SelectedIndex + "\n" + //3
                textBox5.Text + "\n" + //4
                textBox10.Text + "\n" + //5
                textBox1.Text + "\n" + //6
                textBox2.Text + "\n" + //7
                strTmp000 + "\n" + //8
                textBox4.Text + "\n" + //9
                listBox3.SelectedIndex.ToString()+ "\n" + //10
                listBox2.SelectedIndex.ToString(); //11


            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 20;
            //+++++++++++++++++++++++++++++++++

            //------------
            //  tabVisual
            //------------
            string strTmpVisual =
                textBox12.Text + "\n" + //0
                textBox13.Text + "\n" + //1
                comboBox4.SelectedIndex.ToString() + "\n" + //2
                comboBox3.SelectedIndex.ToString() + "\n" + //3
                comboBox2.SelectedIndex.ToString() + "\n" + //4
                comboBox1.SelectedIndex.ToString() + "\n" + //5
                label19.Text + "\n" + //6
                comboBox5.SelectedIndex.ToString() + "\n" + //7
                checkBox1.Checked.ToString() + "\n" + // 8
                checkBox2.Checked.ToString() + "\n" + //9
                textBox6.Text; //10
              

            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 40;
            //+++++++++++++++++++++++++++++++++

            //------------
            //tabPesquisa
            //------------
            string strTmpPesquisa = "";
            for (int i = 0; i < 10; i++)
            {
                for (int a = 0; a < 16; a++)
                {
                    strTmpPesquisa = strTmpPesquisa + ParametrosPesquisa[i, a] + "¢";
                }
                strTmpPesquisa = strTmpPesquisa + "\n";
            }

            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 60;
            //+++++++++++++++++++++++++++++++++

            //------------
            //junta e salva!
            //------------
            string strTmpProjeto =
                strTextoGeral + "\n$" +
                strTmpVisual + "\n$" +
                strTmpPesquisa;

            StreamWriter Leitor = new StreamWriter(ArquivoAule);
            Leitor.Write(strTmpProjeto);
            Leitor.Close();
            projeto = ArquivoAule;


            toolStripStatusLabel2.Text = "";
            toolStripProgressBar2.Value = 0;

        }
        private int retornoValor(string strValor)
        {
            int V = -1;

            try
            {
               V = Int32.Parse(strValor);
            }
            catch
            {}
            return V;
        }
        private void AbreProjeto(string ArquivoAule)
        {
            toolStripStatusLabel2.Text = "Abrindo arquivo Aulë";
            
            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 5;
            //+++++++++++++++++++++++++++++++++

            StreamReader sr = new StreamReader(ArquivoAule);
            string proj = sr.ReadToEnd();
            projeto = ArquivoAule;
            string[] strAbas = proj.Split('$');

            //remove as informações da Aba Geral
            string[] strAbaGeral = strAbas[0].Split('\n');
            textBox11.Text = strAbaGeral[0];
            textBox9.Text = strAbaGeral[1];
            textBox7.Text = strAbaGeral[2];
            cbbTipoAjuda.SelectedIndex = Int32.Parse(strAbaGeral[3]);
            textBox5.Text = strAbaGeral[4];
            textBox10.Text = strAbaGeral[5];
            textBox1.Text = strAbaGeral[6];
            textBox2.Text = strAbaGeral[7];
            textBox8.Text = strAbaGeral[8].Replace("*", "\n");
            textBox4.Text = strAbaGeral[9];

            if (strAbaGeral.Length > 11)
            {
                
                listBox3.SelectedIndex = Int32.Parse(strAbaGeral[10]);
                listBox2.SelectedIndex = Int32.Parse(strAbaGeral[11]);
            }
            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 10;
            //+++++++++++++++++++++++++++++++++

            //remove as informações da Aba Visual
            string[] strAbaVisual = strAbas[1].Split('\n');
            textBox12.Text = strAbaVisual[0];
            textBox13.Text = strAbaVisual[1];
            CarregaPMF();

            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 50;
            //+++++++++++++++++++++++++++++++++
            try
            {
                comboBox4.SelectedIndex = retornoValor(strAbaVisual[2]);
                comboBox3.SelectedIndex = retornoValor(strAbaVisual[3]);
                comboBox2.SelectedIndex = retornoValor(strAbaVisual[4]);
                comboBox1.SelectedIndex = retornoValor(strAbaVisual[5]);
                label19.Text = strAbaVisual[6];
                comboBox5.SelectedIndex = retornoValor(strAbaVisual[7]);
                checkBox1.Checked = Boolean.Parse(strAbaVisual[8]);
                checkBox2.Checked = Boolean.Parse(strAbaVisual[9]);
                textBox6.Text = strAbaVisual[10];
            }
            catch
            { }
            //+++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 70;
            //+++++++++++++++++++++++++++++++++


            //remove as informações da Aba Pesquisa
            string[] strAbaPesquisa = strAbas[2].Split('\n');
            for (int i = 0; i < 10; i++)
            {
                string[] strAbaPesqLinha = strAbaPesquisa[i].Split('¢');
                for (int a = 0; a < 16; a++)
                {
                    ParametrosPesquisa[i, a] = strAbaPesqLinha[a];
                }
                if (strAbaPesqLinha[1] != "")
                {
                    RecebeConfTabela(strAbaPesqLinha);
                }
            }
            sr.Close();

            toolStripStatusLabel2.Text = "";
            toolStripProgressBar2.Value = 0;
        }
        private string TabelaToString(DataTable tabela)
        {
            string strTabela = "";
            int nCol = tabela.Columns.Count;
            int nLin = tabela.Rows.Count;

            toolStripProgressBar1.Maximum = nLin;
            


            //adiciona as colunas
            for (int i = 0; i < nCol; i++)
            {
                strTabela += tabela.Columns[i].ColumnName + "\t";

            }
            strTabela += "\r\n";

            //adiciona as linhas com os registros
            
            for (int a = 0; a < nLin; a++)
            {
                for (int b = 0; b < nCol; b++)
                {
                    strTabela += tabela.Rows[a].ItemArray[b].ToString();
                }
                strTabela += "\r\n";
                toolStripProgressBar1.Value = a;
            }
            toolStripProgressBar1.Maximum = 100;

            return strTabela;
        }
        private void copiaTabela()
        {
            DataTable Tabela = new DataTable();
            int list = listBox1.Items.Count;
            if (list > 1)
            {

                Dados dd = new Dados();
                BD ClasseDB = new BD();

                for (int i = 0; i < list - 1; i++)
                {
                    if (i == 0)
                    {

                        DataTable dt1 =
                            ClasseDB.DB_Access(ParametrosPesquisa[i, 1], ParametrosPesquisa[i, 3], ParametrosPesquisa[i, 8],
                            ParametrosPesquisa[i, 10], ParametrosPesquisa[i, 11]);
                        DataColumn dc1 = dt1.Columns[ParametrosPesquisa[i + 1, 5]];


                        DataTable dt2 =
                            ClasseDB.DB_Access(ParametrosPesquisa[i + 1, 1], ParametrosPesquisa[i + 1, 3], ParametrosPesquisa[i + 1, 8],
                            ParametrosPesquisa[i + 1, 10], ParametrosPesquisa[i + 1, 11]);
                        DataColumn dc2 = dt2.Columns[ParametrosPesquisa[i + 1, 4]];


                        //dataGridView1.DataSource = dt;
                        //dataGridView1.Refresh();
                        string AliasT1 = ParametrosPesquisa[i, 2];
                        string AliasT2 = ParametrosPesquisa[i + 1, 2];
                        Tabela = dd.JuntarTabela(dt1, dc1, AliasT1, dt2, dc2, AliasT2);

                    }
                    else
                    {

                        DataTable dt2 =
                            ClasseDB.DB_Access(ParametrosPesquisa[i + 1, 1], ParametrosPesquisa[i + 1, 3], ParametrosPesquisa[i + 1, 8],
                            ParametrosPesquisa[i + 1, 10], ParametrosPesquisa[i + 1, 11]);
                        DataColumn dc2 = dt2.Columns[ParametrosPesquisa[i + 1, 4]];

                        string ColunaT1 = ParametrosPesquisa[i + 1, 5] + "_" + ParametrosPesquisa[0, 2];
                        DataColumn ColunaTab1 = Tabela.Columns[ColunaT1];

                        string AliasT2 = ParametrosPesquisa[i + 1, 2];

                        Tabela = dd.JuntarTabela2(Tabela, ColunaTab1, dt2, dc2, AliasT2);
                    }
                }
                Clipboard.SetText(TabelaToString(Tabela));
            }

            MessageBox.Show("Pronto: foi pra área de transferencia!");

        }
        private static void DirectoryCopy(
string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {

                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        private void MontaArquivos()
        {
            /*
             * copia os arquivos para a pasta de projeto
             */

            toolStripStatusLabel2.Text = "Gerando projeto para ViZon";
            toolStripProgressBar2.Value = 0;


            //string[] strTmp000;
            
            //cria o diretório e a estrutura de diretorios do projeto
            string pathProjeto = textBox7.Text + "\\" + textBox9.Text;
            Directory.CreateDirectory(pathProjeto);
            Directory.CreateDirectory(pathProjeto + "\\pesq");
            Directory.CreateDirectory(pathProjeto + "\\projpmf");
            Directory.CreateDirectory(pathProjeto + "\\usuario");
            Directory.CreateDirectory(pathProjeto + "\\recursos");

            //++++++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 10;
            //++++++++++++++++++++++++++++++++++++

            //copia o projeto PMF (com DATA)
            if (checkBox1.Checked)
            {
                //copia os arquivos e a estrutura do Publish
                string strTmp000 = textBox12.Text;
                strTmp000 = Path.GetDirectoryName(textBox12.Text);

                strTmp000 = strTmp000.Substring(0, strTmp000.Length - 4);
                Directory.CreateDirectory(pathProjeto + "\\projpmf\\pmf");
                Directory.CreateDirectory(pathProjeto + "\\projpmf\\data");
                DirectoryCopy(strTmp000 + "\\pmf\\", pathProjeto + "\\projpmf\\pmf", true);
                DirectoryCopy(strTmp000 + "\\data\\", pathProjeto + "\\projpmf\\data", true);
                //CopiaPastaEArquivos(strTmp000 + "\\pmf\\",  pathProjeto + "\\projpmf\\pmf", false);
                //CopiaPastaEArquivos(strTmp000 + "\\data\\", pathProjeto + "\\projpmf\\data", false);
            }
            else
            {
                Directory.CreateDirectory(pathProjeto + "\\projpmf\\pmf");
                string destino = pathProjeto + "\\projpmf\\pmf" + Path.GetFileName(textBox12.Text);
                File.Copy(textBox12.Text, destino);
            }

            //copia, se houver, os extras do pmf

            if (checkBox2.Checked && textBox6.Text != "")
            {
                try
                {
                    string DirDestino = pathProjeto + "\\projpmf\\" + Path.GetDirectoryName(textBox6.Text);
                    Directory.CreateDirectory(DirDestino);
                    DirectoryCopy(textBox6.Text, DirDestino,true);
                }
                catch
                { }

            }

            //++++++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 20;
            //++++++++++++++++++++++++++++++++++++


            //copia o arquivo de capa, rodapé e cabeçalho
            File.Copy(textBox10.Text, pathProjeto + "\\recursos\\" + Path.GetFileName(textBox10.Text), true);
            File.Copy(textBox1.Text, pathProjeto + "\\recursos\\" + Path.GetFileName(textBox1.Text), true);
            File.Copy(textBox2.Text, pathProjeto + "\\recursos\\" + Path.GetFileName(textBox2.Text), true);

            //copia o arquivo de ajuda, se ele for pdf ou hlp
            if (cbbTipoAjuda.SelectedIndex != 1)
            {
                File.Copy(textBox5.Text, pathProjeto + "\\recursos\\" + Path.GetFileName(textBox5.Text), true);
            }
            //++++++++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 30;
            //++++++++++++++++++++++++++++++++++++

            //adiciona ao projetos os bancos de dados access
            for (int i = 0; i < 10; i++)
            {
                if (ParametrosPesquisa[i, 0] == "1" & 
                    ParametrosPesquisa[i, 14] == "True")
                {
                    File.Copy(ParametrosPesquisa[i, 1], pathProjeto + "\\pesq\\" + Path.GetFileName(ParametrosPesquisa[i, 1]), true);
                }
            }
            toolStripProgressBar2.Value = 40;

            //copia o conteúdo Extra
            if (checkBox2.Checked)
            {
                string strTmp000 = textBox12.Text;
                strTmp000 = Path.GetDirectoryName(textBox6.Text);
                string destino = pathProjeto + "\\projpmf\\Extras\\";
                Directory.CreateDirectory(destino);
                DirectoryCopy(textBox6.Text, destino, true);
            }

        }
        private void MontaZON()
        {
            /*
             * aqui é montado o arquivo *.zon
             * o valor entre as [] indicam o numero do parametro
             * segundo as especificações do arquivo *.zon
             */
            
            //+++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 40;
            //+++++++++++++++++++++++++++++++

            string TextoZon = "";

            //[0]
            TextoZon += textBox11.Text + "\r\n";

            //[1]
            string strTmp001 = Path.GetFileName(textBox12.Text);
            TextoZon += "projpmf\\pmf\\" + strTmp001 + "\r\n";
            
            //[2]
            TextoZon += "pesq\\" + textBox9.Text + ".psq" + "\r\n";

            //[3]
            string Versao = listBox2.SelectedItem.ToString(); //são usados de verdade no #4 mas foram 
            string Revisao = listBox3.SelectedItem.ToString(); //colocados aqui pra serem usados no #3

            strTmp001 = textBox8.Text + "\r\nVersão do projeto:" + Versao + "." + Revisao;
            strTmp001 = strTmp001.Replace("\r", "");
            strTmp001 = strTmp001.Replace("\n", "*");
            TextoZon += strTmp001 + "\r\n";

            //[4]
            // variaveis definidas no #3, para serem usadas antes.
            TextoZon += Versao +"."+ Revisao + "\r\n";

            //[5]
            int intTmp001 = cbbTipoAjuda.SelectedIndex + 1;
            TextoZon += intTmp001.ToString() + "\r\n";

            //[6]
            if (intTmp001 != 2)
            {
                strTmp001 = Path.GetFileName(textBox5.Text);
                TextoZon += "recursos\\" + strTmp001 + "\r\n";
            }
            else
            {
                TextoZon += textBox5.Text + "\r\n";
            }

            //[7]
            strTmp001 = Path.GetFileName(textBox10.Text);
            TextoZon += "recursos\\" + strTmp001 + "\r\n";

            //[8]
            TextoZon += textBox13.Text + "\r\n";

            //[9] !!! Agora é que é !!!
            TextoZon += pegaSeqNumerica(comboBox4.SelectedItem.ToString()) + "\r\n";

            //[10]
            TextoZon += comboBox1.SelectedItem + "\r\n";

            //[11] !!! Agora é que é !!!
            TextoZon += pegaSeqNumerica(comboBox3.SelectedItem.ToString()) + "\r\n";

            //[12]
            TextoZon += comboBox2.SelectedItem + "\r\n";

            //[13]
            TextoZon += label19.Text + "\r\n";
            
            //[14]
            strTmp001 = Path.GetFileName(textBox1.Text);
            TextoZon += "recursos\\" + strTmp001 + "\r\n";

            //[15]
            strTmp001 = Path.GetFileName(textBox2.Text);
            TextoZon += "recursos\\" + strTmp001 + "\r\n";

            //[16]
            TextoZon += comboBox5.SelectedIndex + "\r\n";

            //[17]
            TextoZon += textBox4.Text + "*"; //o '*' no final é pq a criptografia come 1 caractere no final


            //+++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 50;
            //+++++++++++++++++++++++++++++++


            //salva o arquivo para a criptografia
            string PathTempFile = textBox7.Text + "\\" + textBox9.Text + "\\proj.txt";
            SalvaArquivo(PathTempFile, TextoZon);

            //gera a criptografia
            string PathZON = textBox7.Text + "\\" + textBox9.Text + "\\" + textBox9.Text + ".zon";
            Enigma En = new Enigma();
            En.TryplaCripto(PathTempFile, PathZON);

            //deleta o arquivo temporario
            DeletaArquivo(PathTempFile);


            //+++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 70;
            //+++++++++++++++++++++++++++++++

        }
        private void MontaPSQ()
        {
            /*
             * aqui é montado o arquivo *.psq
             * o valor entre as [] indicam o numero do parametro
             * segundo as especificações do arquivo *.psq
             */

            //+++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 70;
            //+++++++++++++++++++++++++++++++



            string TextoPsq = "";

            for (int a = 0; a < listBox1.Items.Count; a++)
            {
                //[n,0]
                TextoPsq += ParametrosPesquisa[a, 0] + "#";

                //[n,1]
                if (ParametrosPesquisa[a, 0] == "1" & ParametrosPesquisa[a, 14] == "True")
                {
                    string strTmp001 = Path.GetFileName(ParametrosPesquisa[a, 1]);
                    TextoPsq += strTmp001 + "#";
                }
                else
                {
                    TextoPsq += ParametrosPesquisa[a, 1] + "#";
                }

                //[n,2]
                TextoPsq += ParametrosPesquisa[a, 2] + "#";

                //[n,3]
                TextoPsq += ParametrosPesquisa[a, 3] + "#";

                //[n,4]
                TextoPsq += ParametrosPesquisa[a, 4] + "#";

                //[n,5]
                TextoPsq += ParametrosPesquisa[a, 5] + "#";

                //[n,6]
                TextoPsq += ParametrosPesquisa[a, 6] + "#";

                //[n,7]
                TextoPsq += ParametrosPesquisa[a, 7] + "#";

                //[n,8]
                TextoPsq += ParametrosPesquisa[a, 8] + "#";

                //[n,9]
                TextoPsq += ParametrosPesquisa[a, 9] + "#";

                //[n,10]
                TextoPsq += ParametrosPesquisa[a, 10] + "#";

                //[n,11]
                TextoPsq += ParametrosPesquisa[a, 11] + "#";


                //[n,12]
                TextoPsq += ParametrosPesquisa[a, 12] + "#";


                //gera aqui o arquivo de atributo de pesquisa
                string PathAtributos = textBox7.Text + "\\" + textBox9.Text + "\\pesq\\" + ParametrosPesquisa[a, 12];
                string tmpTextoAtributos = MontaAtributos(ParametrosPesquisa[a, 15], ParametrosPesquisa[a, 2]);

                if (tmpTextoAtributos == "")
                {
                    tmpTextoAtributos = "X";
                }


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //criptografia dos arquivos de atributos
                //salva o arquivo para a criptografia
                string TempPathFile = textBox7.Text + "\\" + textBox9.Text + "T.txt";
                
                SalvaArquivo(TempPathFile, tmpTextoAtributos);

                //gera a criptografia
                //string PathPSQ = textBox7.Text + "\\" + textBox9.Text + "\\pesq\\" + textBox9.Text + ".psq";
                Enigma En = new Enigma();
                En.TryplaCripto(TempPathFile, PathAtributos);

                //deleta o arquivo temporario
                DeletaArquivo(TempPathFile);
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //SalvaArquivo(PathAtributos, tmpTextoAtributos);


                //final de linha
                TextoPsq += "\r\n";
            }

            //+++++++++++++++++++++++++++++++
            toolStripProgressBar2.Value = 95;
            //+++++++++++++++++++++++++++++++

            //salva o arquivo para a criptografia
            string PathTempFile = textBox7.Text + "\\" + textBox9.Text + ".txt";
            SalvaArquivo(PathTempFile, TextoPsq);

            //gera a criptografia
            string PathPSQ = textBox7.Text + "\\" + textBox9.Text + "\\pesq\\" + textBox9.Text + ".psq";
            Enigma En2 = new Enigma();
            En2.TryplaCripto(PathTempFile, PathPSQ);

            //deleta o arquivo temporario
            DeletaArquivo(PathTempFile);

            toolStripStatusLabel2.Text = "";
            toolStripProgressBar2.Value = 0;

        }
        private string MontaAtributos(string conteudo, string Tabela)
        {
            string[] valores = conteudo.Split('¬');
            string textofinal = "";
            for (int a = 0; a < valores.Length -1; a++)
            {
                string[] strTemp = valores[a].Split('#');
                bool inclui = Boolean.Parse(strTemp[0]);
               
                if(inclui)
                {
                    textofinal += strTemp[2] + "#" + strTemp[1] + "_" + Tabela + "#\r";
                }
            }

            return textofinal;

        }
        private void SalvaArquivo(string caminho, string conteudo)
        {
            //salva o arquivo temporário
            caminho = caminho.Replace("\"", "_");
            StreamWriter swArquivo = new StreamWriter(caminho);
            swArquivo.Write(conteudo);
            swArquivo.Close();
        }
        private void DeletaArquivo(string caminho)
        {
            FileInfo fiArquivo = new FileInfo(caminho);
            fiArquivo.Delete();
        }
        private DataTable TransformaTabela(string nomeArquivo)
        {

            StreamReader sr = new StreamReader(nomeArquivo,Encoding.UTF7);
            
            string texto = sr.ReadToEnd();
            char caract = Char.Parse("\n");
            texto = texto.Replace("\n", "");
            //texto.Trim(caract);
            string[] linhas = texto.Split('\r');

            DataTable dt = new DataTable();
            for (int a = 0; a < linhas.Length; a++)
            {
                if (a != 0)
                {
                    string[] celulas = linhas[a].Split('\t');
                    dt.Rows.Add(celulas);
                }
                else
                {
                    string[] celulas = linhas[a].Split('\t');
                    for (int b = 0; b < celulas.Length; b++)
                    {
                        dt.Columns.Add(celulas[b]);
                    }
                }
            }
            return dt;
        }
        
        //=========================================
        //             Propriedades
        //=========================================
        /// <summary>
        /// Parametros de pesquisa (*.psq)
        /// </summary>
        public string[] ParaPSQ //GetSet para parametros do arquivo psq 
        {
            get { return this.strParPesq; }
            set { this.strParPesq = value; }
        }
        
        //=========================================
        //      Eventos Barra de Ferramentas
        //=========================================
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //este é o botão de acionamento da engrenagem!
            if (Verifica())
            {
                MontaArquivos();
                MontaZON();
                MontaPSQ();
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (projeto == "")
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Arquivo Aulë|*.aule";
                saveFileDialog1.Title = "Salvar projeto Aulë";
                //saveFileDialog1.ShowDialog();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK & saveFileDialog1.FileName != "")
                {
                    SalvaProjeto(saveFileDialog1.FileName);
                }
            }
            else
            {
                SalvaProjeto(projeto);
            }
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Verifica().ToString());
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Verifica();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Arquivo Aulë|*.aule";
            ofd.Title = "Abre projeto Aulë";

            if (ofd.ShowDialog() == DialogResult.OK & ofd.FileName != "")
            {
                AbreProjeto(ofd.FileName);
            }
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Arquivo Aulë|*.aule";
            saveFileDialog1.Title = "Salvar projeto Aulë";
            //saveFileDialog1.ShowDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK & saveFileDialog1.FileName != "")
            {
                SalvaProjeto(saveFileDialog1.FileName);
            }
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string Versao = listBox2.SelectedItem.ToString();
            string Revisao = listBox3.SelectedItem.ToString();
            frmXML FormXML = new frmXML(Versao + "." + Revisao);
            FormXML.ShowDialog();
        }
       
        
        //=========================================
        //             Eventos Diversos
        //=========================================
        //aba geral
        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = fbd.SelectedPath;
            }
        }
        private void cbbTipoAjuda_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbbTipoAjuda.SelectedIndex)
            {
                case 0:
                    button3.Enabled = true;
                    break;

                case 1:
                    button3.Enabled = false;
                    break;

                case 2:
                    button3.Enabled = true;
                    break;
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            switch (cbbTipoAjuda.SelectedIndex)
            {
                case 0:
                    textBox5.Text = Abre("Todo tipo de documento|*.*", "Abrir documento de ajuda");

                    break;

                case 1:
                    button3.Enabled = false;
                    break;

                case 2:
                    textBox5.Text = Abre("Arquivo de Ajuda |*.hlp", "Abrir documento de ajuda");
                    break;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            blAlteracao = true;
            string CaminhoImagem = Abre("Figuras |*.jpg;*.bmp;*.gif", "Abre figura");
            if (CaminhoImagem != "")
            {
                try
                {
                    Image img = System.Drawing.Bitmap.FromFile(CaminhoImagem);

                    if (img.Height != 235 | img.Width != 440)
                    {
                        MessageBox.Show("A dimensão da imagem está fora dos padrões. Ela deve ter 440 x 235 pixels.");
                    }
                    else
                    {
                        textBox10.Text = CaminhoImagem;
                        //pictureBox1.Image = img;
                    }
                }
                catch (Exception i)
                {
                    MessageBox.Show(i.Message);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            blAlteracao = true;
            string CaminhoImagem = Abre("Figuras |*.jpg;*.bmp;*.gif", "Abre figura");
            if (CaminhoImagem != "")
            {
                try
                {
                    Image img = System.Drawing.Bitmap.FromFile(CaminhoImagem);

                    if (img.Height > 100 | img.Width > 700)
                    {
                        MessageBox.Show("A dimensão da imagem está fora dos padrões. Ela deve ter no máximo 700 pixels de largura.");
                    }
                    else
                    {
                        textBox1.Text = CaminhoImagem;
                        //pictureBox1.Image = img;
                    }
                }
                catch (Exception i)
                {
                    MessageBox.Show(i.Message);
                }
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            blAlteracao = true;
            string CaminhoImagem = Abre("Figuras |*.jpg;*.bmp;*.gif", "Abre figura");
                        if (CaminhoImagem != "")
            {
                try
                {
                    Image img = System.Drawing.Bitmap.FromFile(CaminhoImagem);

                    if (img.Height > 100 | img.Width > 700)
                    {
                        MessageBox.Show("A dimensão da imagem está fora dos padrões. Ela deve ter no máximo 700 pixels de largura.");
                    }
                    else
                    {
                        textBox2.Text = CaminhoImagem;
                        //pictureBox1.Image = img;
                    }
                }
                catch (Exception i)
                {
                    MessageBox.Show(i.Message);
                }
            }

        }
        //aba visual
        private void button6_Click(object sender, EventArgs e)
        {
            textBox12.Text = Abre("PMF Files|*.pmf|All Files|*.*", "Abrir projeto PMF");

        }
        private void cbbTipoAjuda_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbbTipoAjuda.SelectedIndex == 0 | cbbTipoAjuda.SelectedIndex == 2)
            {
                button3.Enabled = true;
            }
            else
            { button3.Enabled = false; }
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string[] strColunas = CarregaColunas(comboBox4.SelectedItem.ToString());
            string colunas = pegaSeqTabelas(comboBox4.SelectedItem.ToString());
            string[] strColunas = colunas.Split('*');
            comboBox1.Items.Clear();
            for (int i = 0; i < strColunas.Length-1; i++)
            {
                comboBox1.Items.Add(strColunas[i]);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string colunas = pegaSeqTabelas(comboBox3.SelectedItem.ToString());
            string[] strColunas = colunas.Split('*');
            comboBox2.Items.Clear();
            for (int i = 0; i < strColunas.Length; i++)
            {
                comboBox2.Items.Add(strColunas[i]);
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            CarregaPMF();
        }
        //aba pesquisa
        private void button1_Click_1(object sender, EventArgs e)
        {
            blAlteracao = false;

            if (listBox1.Items.Count < 10)
            {
                string valTabBase = "";
                for (int i = 0; i < 10; i++)
                {
                    strParPesq[i] = "";
                    if (ParametrosPesquisa[i, 13] == "True")
                    {
                        valTabBase = ParametrosPesquisa[i, 15];
                    }
                }

                frmTabela FormTabela = new frmTabela(valTabBase);
                FormTabela.prpStrBasica = this.ParaPSQ;
                FormTabela.Show(this);
            }
            else 
            {
                MessageBox.Show("Não é possivel adicionar uma nova tabela de pesquisa. O projeto atingiu o limite.");
                button1.Enabled = false;
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog()== DialogResult.OK)
            {
                int c = ColorTranslator.ToOle(cd.Color);
                label19.Text = c.ToString();
                label19.BackColor = cd.Color;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            int intTmp000 = listBox1.SelectedIndex;
            if (intTmp000 == -1) intTmp000 = 0;
            blAlteracao = true;
            string valTabBase = "";
            
            strParPesq[0] = ParametrosPesquisa[intTmp000, 0];
            strParPesq[1] = ParametrosPesquisa[intTmp000, 1];
            strParPesq[2] = ParametrosPesquisa[intTmp000, 2];
            strParPesq[3] = ParametrosPesquisa[intTmp000, 3];
            strParPesq[4] = ParametrosPesquisa[intTmp000, 4];
            strParPesq[5] = ParametrosPesquisa[intTmp000, 5];
            strParPesq[6] = ParametrosPesquisa[intTmp000, 6];
            strParPesq[7] = ParametrosPesquisa[intTmp000, 7];
            strParPesq[8] = ParametrosPesquisa[intTmp000, 8];
            strParPesq[9] = ParametrosPesquisa[intTmp000, 9];
            strParPesq[10] = ParametrosPesquisa[intTmp000, 10];
            strParPesq[11] = ParametrosPesquisa[intTmp000, 11];
            strParPesq[12] = ParametrosPesquisa[intTmp000, 12];
            strParPesq[13] = ParametrosPesquisa[intTmp000, 13];
            strParPesq[14] = ParametrosPesquisa[intTmp000, 14];
            strParPesq[15] = ParametrosPesquisa[intTmp000, 15];

            for (int i = 0; i < 10; i++)
            {
                if (ParametrosPesquisa[i, 13] == "True") // é +1 mesmo?
                {
                    valTabBase = ParametrosPesquisa[i, 15];
                }
            }            

            frmTabela FormTabela = new frmTabela(valTabBase);
            FormTabela.prpStrBasica = this.ParaPSQ;
            FormTabela.Show(this);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int selecionado = listBox1.SelectedIndex;
            int ciclos = listBox1.Items.Count - selecionado;
            listBox1.Items.Remove(listBox1.SelectedItem);

            for (int i = 0; i < ciclos; i++)
            {
                ParametrosPesquisa[selecionado, 0] = ParametrosPesquisa[selecionado + 1, 0];
                ParametrosPesquisa[selecionado, 1] = ParametrosPesquisa[selecionado + 1, 1];
                ParametrosPesquisa[selecionado, 2] = ParametrosPesquisa[selecionado + 1, 2];
                ParametrosPesquisa[selecionado, 3] = ParametrosPesquisa[selecionado + 1, 3];
                ParametrosPesquisa[selecionado, 4] = ParametrosPesquisa[selecionado + 1, 4];
                ParametrosPesquisa[selecionado, 5] = ParametrosPesquisa[selecionado + 1, 5];
                ParametrosPesquisa[selecionado, 6] = ParametrosPesquisa[selecionado + 1, 6];
                ParametrosPesquisa[selecionado, 7] = ParametrosPesquisa[selecionado + 1, 7];
                ParametrosPesquisa[selecionado, 8] = ParametrosPesquisa[selecionado + 1, 8];
                ParametrosPesquisa[selecionado, 9] = ParametrosPesquisa[selecionado + 1, 9];
                ParametrosPesquisa[selecionado, 10] = ParametrosPesquisa[selecionado + 1, 10];
                ParametrosPesquisa[selecionado, 11] = ParametrosPesquisa[selecionado + 1, 11];
                ParametrosPesquisa[selecionado, 12] = ParametrosPesquisa[selecionado + 1, 12];
                ParametrosPesquisa[selecionado, 13] = ParametrosPesquisa[selecionado + 1, 13];
                ParametrosPesquisa[selecionado, 14] = ParametrosPesquisa[selecionado + 1, 14];
                ParametrosPesquisa[selecionado, 15] = ParametrosPesquisa[selecionado + 1, 15];



                selecionado += 1;
            }


            if (listBox1.Items.Count == 10)
            {
                button1.Enabled = false;
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int list = listBox1.SelectedIndex;

            if (list >= 0)
            {
                button7.Enabled = true;
                button2.Enabled = true;

                textBox3.Text = "Tipo de banco de dados: " + ParametrosPesquisa[list, 0] +
                    "\r\nNome do Arquivo ou do BD: " + ParametrosPesquisa[list, 1] +
                    "\r\nNome da Tabela: " + ParametrosPesquisa[list, 3] +
                    "\r\nAlias da Tabela: " + ParametrosPesquisa[list, 2] +
                    "\r\nColuna de ligação/shapefile: " + ParametrosPesquisa[list, 4] +
                    "\r\nColuna de ligação base/area: " + ParametrosPesquisa[list, 5] +
                    "\r\nEndereço servidor: " + ParametrosPesquisa[list, 6] +
                    "\r\nLogin: " + ParametrosPesquisa[list, 7] +
                    "\r\nSenha: " + ParametrosPesquisa[list, 8] +
                    "\r\nPorta: " + ParametrosPesquisa[list, 9] +
                    "\r\nSintaxe 'SELECT': " + ParametrosPesquisa[list, 10] +
                    "\r\nSintaxe 'WHERE': " + ParametrosPesquisa[list, 11] +
                    "\r\nNome do arquivo de parametro de pesquisa: " + ParametrosPesquisa[list, 12] +
                    "\r\nEsta tabela base?: " + ParametrosPesquisa[list, 13];

                BD ClasseDB = new BD();
                DataTable dt =
                    ClasseDB.DB_Access(ParametrosPesquisa[list, 1], ParametrosPesquisa[list, 3], ParametrosPesquisa[list, 8],
                    ParametrosPesquisa[list, 10], ParametrosPesquisa[list, 11]);
                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
            }

        }
        private void button14_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Arquivo|*.txt";
            ofd.Title = "Abre arquivo";
            ofd.InitialDirectory = @"C:\Users\Embrapa\Desktop";

            DataTable tabela = new DataTable();

            if (ofd.ShowDialog() == DialogResult.OK & ofd.FileName != "")
            {
                tabela = TransformaTabela(ofd.FileName);
            }

            ofd.Filter = "Arquivo|*.accdb";
            ofd.Title = "Abre arquivo";
            ofd.InitialDirectory = @"C:\Users\Embrapa\Desktop\ZAAL_Aule\Pre_projeto";

            if (ofd.ShowDialog() == DialogResult.OK & ofd.FileName != "")
            {
                BD bd = new BD();
                bd.DB_Access2(ofd.FileName, "TabPedoclima", "33255988", tabela);
            }

        }

        //eventos genéricos
        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            listBox3.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            new Thread(copiaTabela).Start();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK && fbd.SelectedPath != "")
            {
                textBox6.Text = fbd.SelectedPath;
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.Enabled = checkBox2.Checked;
        }



    }//fim da classe
}//fim do namespace
