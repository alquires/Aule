using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace Aule
{
    class Dados
    {
        public Dados()
        {
        }
        //junção da primeira dupla de tabelas 1->n
        /// <summary>Junta duas tabelas em uma única grande tabela (primeira dupla de tabelas)
        /// <param name="tabela1">Primeira tabela a ser juntada, e que servirá de base para a seguinte</param>
        /// <param name="colunatabela1">Coluna da primeira tabela, que servirá de base para a junção da outra tabela</param>
        /// <param name="tabela2">Segunda tabela a ser juntada</param>
        /// <param name="colunatabela2">Coluna da segunda tabela que servirá de base para a junção da primeira tabela</param>
        /// </summary>
        public DataTable JuntarTabela(DataTable tabela1, DataColumn colunatabela1, string AliasTabela1,
                                        DataTable tabela2, DataColumn colunatabela2, string AliasTabela2)
        {

            DataTable dt = new DataTable();
            try
            {
                //frmPrincipal frm1 = new frmPrincipal();

                int ContLinhas = 0;
                int intContT1 = tabela1.Columns.Count;
                int intContT2 = tabela2.Columns.Count;

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //         adiciona as colunas
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                for (int i4 = 0; i4 < intContT1; i4++)
                {
                    dt.Columns.Add();
                    dt.Columns[i4].ColumnName = tabela1.Columns[i4].ColumnName + "_" + AliasTabela1;
                    dt.Columns[i4].DataType = tabela1.Columns[i4].DataType;

                    DataGridViewColumn dgvc = new DataGridViewColumn();

                    dgvc.Name = tabela1.Columns[i4].ColumnName + "_" + AliasTabela1;
                    dgvc.ValueType = tabela1.Columns[i4].DataType;
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Style.BackColor = System.Drawing.Color.Wheat;
                    dgvc.CellTemplate = cell;
                }

                for (int i4 = 0; i4 < intContT2; i4++)
                {
                    int i4plus = i4 + tabela1.Columns.Count;
                    dt.Columns.Add();
                    dt.Columns[i4plus].ColumnName = tabela2.Columns[i4].ColumnName + "_" + AliasTabela2;
                    dt.Columns[i4plus].DataType = tabela2.Columns[i4].DataType;

                    DataGridViewColumn dgvc = new DataGridViewColumn();

                    dgvc.ValueType = tabela2.Columns[i4].DataType;
                    dgvc.Name = tabela2.Columns[i4].ColumnName + "_" + AliasTabela2;
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Style.BackColor = System.Drawing.Color.White;
                    dgvc.CellTemplate = cell;
                }

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //XXXXXXXXXXX ADICIONA AS LINHAS XXXXXXXXXX
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                //ando por cada linha da tabela1 e encontro os correspondentes na tabela2
                //valor da celula de coluna1 da tabela 1
                for (int Lin = 0; Lin < tabela1.Rows.Count; Lin++)
                {
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //XXX Importante: preciso alterar valor XXX
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                    int intVC1; // intVC1 informa a posição no array da linha qual o valor da coluna para a pesquisa
                    intVC1 = tabela1.Columns.IndexOf(colunatabela1.ColumnName);

                    string strValorCelTab1 = tabela1.Rows[Lin].ItemArray.GetValue(intVC1).ToString(); //valor da celula de coluna1 
                    //da tabela1
                    string strSintaxe = colunatabela2.ColumnName + " = '" + strValorCelTab1 + "'"; //sintaxe de pesq do metodo 
                    //select()

                    int q2 = tabela2.Select(strSintaxe).Length; //comprimento da seleção na Tabela2 (quantos itens na tabela 2
                    // são iguais ao item da tabela1?)
                    DataRow[] drowT2 = new DataRow[q2]; //cria o DataRow de comprimento q
                    drowT2 = tabela2.Select(strSintaxe); //faz a consulta em função do primeiro registro encontrado em Tabela1

                    for (int i = 0; i < q2; i++)
                    {
                        object[] teste = new object[intContT1 + intContT2];

                        for (int i2 = 0; i2 < intContT1 + intContT2; i2++)
                        {

                            if (i2 >= intContT1)
                            {
                                teste.SetValue(drowT2[i].ItemArray.GetValue(i2 - intContT1), i2);
                            }
                            else
                            {
                                teste.SetValue(tabela1.Rows[Lin].ItemArray.GetValue(i2), i2);
                            }
                        }

                        dt.Rows.Add(teste);
                        //dt.Rows[ContLinhas].
                        ContLinhas = ContLinhas + 1;
                    }
                }
            }
            catch 
            {

            }


            return dt;

        }

        //junção das demais duplas de tabelas 1->n
        /// <summary>Junta duas tabelas em uma única grande tabela (demais duplas de tabelas)
        /// <param name="tabela1">Primeira tabela a ser juntada, e que servirá de base para a seguinte</param>
        /// <param name="colunatabela1">Coluna da primeira tabela, que servirá de base para a junção da outra tabela</param>
        /// <param name="tabela2">Segunda tabela a ser juntada</param>
        /// <param name="colunatabela2">Coluna da segunda tabela que servirá de base para a junção da primeira tabela</param>
        /// </summary>
        public DataTable JuntarTabela2(DataTable tabela1, DataColumn colunatabela1,
                                         DataTable tabela2, DataColumn colunatabela2, string AliasTabela2)
        {

            DataTable dt = new DataTable();
            //Form1 frm1 = new Form1();
            try
            {
                int ContLinhas = 0;
                int intContT1 = tabela1.Columns.Count;
                int intContT2 = tabela2.Columns.Count;

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //     adiciona as colunas tabela 1
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                for (int i4 = 0; i4 < intContT1; i4++)
                {
                    dt.Columns.Add();
                    dt.Columns[i4].ColumnName = tabela1.Columns[i4].ColumnName;
                    dt.Columns[i4].DataType = tabela1.Columns[i4].DataType;

                    DataGridViewColumn dgvc = new DataGridViewColumn();

                    dgvc.Name = tabela1.Columns[i4].ColumnName;
                    dgvc.ValueType = tabela1.Columns[i4].DataType;
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Style.BackColor = System.Drawing.Color.Wheat;
                    dgvc.CellTemplate = cell;
                }
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //     adiciona as colunas tabela 2
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                for (int i4 = 0; i4 < intContT2; i4++)
                {
                    int i4plus = i4 + tabela1.Columns.Count;
                    dt.Columns.Add();
                    dt.Columns[i4plus].ColumnName = tabela2.Columns[i4].ColumnName + "_" + AliasTabela2;
                    dt.Columns[i4plus].DataType = tabela2.Columns[i4].DataType;

                    DataGridViewColumn dgvc = new DataGridViewColumn();

                    dgvc.ValueType = tabela2.Columns[i4].DataType;
                    dgvc.Name = tabela2.Columns[i4].ColumnName + "_" + AliasTabela2;
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Style.BackColor = System.Drawing.Color.White;
                    dgvc.CellTemplate = cell;
                }

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //XXXXXXXXXXX ADICIONA AS LINHAS XXXXXXXXXX
                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                //ando por cada linha da tabela1 e encontro os correspondentes na tabela2
                //valor da celula de coluna1 da tabela 1
                for (int Lin = 0; Lin < tabela1.Rows.Count; Lin++)
                {
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //XXX Importante: preciso alterar valor XXX
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                    int intVC1; // intVC1 informa a posição no array da linha qual o valor da coluna para a pesquisa
                    intVC1 = tabela1.Columns.IndexOf(colunatabela1.ColumnName);

                    string strValorCelTab1 = tabela1.Rows[Lin].ItemArray.GetValue(intVC1).ToString(); //valor da celula de coluna1 
                    //da tabela1
                    string strSintaxe = colunatabela2.ColumnName + " = '" + strValorCelTab1 + "'"; //sintaxe de pesq do metodo 
                    //select()

                    int q2 = tabela2.Select(strSintaxe).Length; //comprimento da seleção na Tabela2 (quantos itens na tabela 2
                    // são iguais ao item da tabela1?)
                    DataRow[] drowT2 = new DataRow[q2]; //cria o DataRow de comprimento q
                    drowT2 = tabela2.Select(strSintaxe); //faz a consulta em função do primeiro registro encontrado em Tabela1

                    for (int i = 0; i < q2; i++)
                    {
                        object[] teste = new object[intContT1 + intContT2];

                        for (int i2 = 0; i2 < intContT1 + intContT2; i2++)
                        {

                            if (i2 >= intContT1)
                            {
                                teste.SetValue(drowT2[i].ItemArray.GetValue(i2 - intContT1), i2);
                            }
                            else
                            {
                                teste.SetValue(tabela1.Rows[Lin].ItemArray.GetValue(i2), i2);
                            }
                        }

                        dt.Rows.Add(teste);
                        //dt.Rows[ContLinhas].
                        ContLinhas = ContLinhas + 1;
                    }
                }
            }
            catch 
            {

            }



            return dt;

        }






        /// <summary>Retorna valores unicos e ordenados
        /// <param name="array">conjunto de dados a serem ordenados</param>
        /// </summary>
        public T[] GetDistinctValues<T>(T[] array)
        {
            List<T> tmp = new List<T>();
            //LOG dispensado (valor insignificante)

            try
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (tmp.Contains(array[i]))
                        continue;
                    tmp.Add(array[i]);
                }
            }
            catch
            {
            }
            return tmp.ToArray();
        }

        /// <summary>Retorna o valor da unidade de area utilizada no projeto
        /// <param name="tipo">Refere-se ao tipo de retorno da unidade: 0 = extenso; 1 = simbolo</param>
        /// <param name="unidade">Refere-se ao tipo de unidade: 0 = m²; 1 = km²; 2 = ha </param>
        /// </summary>
        public string RetornaUnidade(int tipo, int unidade)
        {
            //LOG dispensado (valor insignificante)

            string u_area = "";
            try
            {
                switch (tipo)
                {

                    case 0:
                        switch (unidade)
                        {
                            case 0:
                                u_area = "metros quadrados";
                                //u_areas = "metros quadrados";
                                break;
                            case 1:
                                u_area = "quilômetros quadrados";
                                //u_areas = "quilômetros quadrados";
                                break;
                            case 2:
                                u_area = "hectares";
                                //u_areas = "hectares";
                                break;
                            case 3:
                                u_area = "acres";
                                //u_areas = "hectares";
                                break;
                            default:
                                u_area = "unidade desconhecidas";
                                //u_areas = "unidade desconhecida";
                                break;
                        }
                        break;



                    case 1:

                        switch (unidade)
                        {
                            case 0:
                                u_area = "m²";
                                //u_areas = "metros quadrados";
                                break;
                            case 1:
                                u_area = "km²";
                                //u_areas = "quilômetros quadrados";
                                break;
                            case 2:
                                u_area = "ha";
                                //u_areas = "hectares";
                                break;
                            case 3:
                                u_area = "ac";
                                //u_areas = "hectares";
                                break;
                            default:
                                u_area = "?";
                                //u_areas = "unidade desconhecida";
                                break;
                        }
                        break;
                }
            }
            catch 
            {

            }

            return u_area;
        }


    }//fim da classe
}//fim do namespace
