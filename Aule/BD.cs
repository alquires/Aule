using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Devart.Data;
using Devart.Data.PostgreSql;


namespace Aule
{
    public class BD
    {
        public BD()
        {
        }

        //=====================================
        //             METODOS ACCESS
        //=====================================
        /// <summary>
        /// Faz a conexão com um banco de dados Access
        /// </summary>
        /// <param name="NomeArq">Nome do arquivo access</param>
        /// <param name="NomeTabela">Nome da tabela dentro do BD Access</param>
        /// <param name="senha">Senha de acesso ao BD</param>
        /// <param name="ClauSelect">Sintaxe de seleção das colunas (SELECT)</param>
        /// <param name="ClauWhere">Sintaxe de seleção de registros (WHERE)</param>
        /// <returns>Tabela Access</returns>
        public DataTable DB_Access(string NomeArq, string NomeTabela, string senha, string ClauSelect, string ClauWhere)
        {
            DataSet ds = new DataSet();

            try
            {
                //montagem da string de conexao com parte dos dados obtidos na chamada do metodo Provider=Microsoft.Jet.OLEDB.4.0;
                string strConexao = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + NomeArq;

                if (senha != "")
                {
                    strConexao += ";Jet OLEDB:Database Password=" + senha;
                    //MessageBox.Show("Incluir aqui a configuracao da senha");
                }

                //chamada dos objetos de banco de dados
                OleDbConnection OleDBconn = new OleDbConnection(strConexao);

                string SintaxeWHERE = "";
                if (ClauWhere != "")
                {
                    SintaxeWHERE = " WHERE " + ClauWhere;
                }

                OleDbDataAdapter OleDBda = new OleDbDataAdapter("SELECT " + ClauSelect + " FROM " + NomeTabela + SintaxeWHERE, OleDBconn);
                OleDBda.Fill(ds);
                OleDBconn.Open();
                //MessageBox.Show("Afetados: " + OleDBcomm.ExecuteNonQuery().ToString());
                OleDBconn.Close();
            }
            catch (OleDbException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Access.\r\n" + Ex.Message + "\r\nErro número: " +
                    Ex.ErrorCode.ToString());
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message);
                ds.Tables.Add();
            }

            return ds.Tables[0];
        }


        //=====================================
        //             METODOS MYSQL
        //=====================================
        public DataTable DB_MySql(string EndServidor, string NomeBD, string NomeTabela, string Login, string Senha,
            string ClauSelect, string ClauWhere)
        {
            DataSet ds = new DataSet();

            try
            {
                string strConexao = "server=" + EndServidor + ";User Id=" + Login + ";Password=" + Senha + ";database=" + NomeBD;
                MySqlConnection MySqlconn = new MySqlConnection(strConexao);
                MySqlDataAdapter MySqlda = new MySqlDataAdapter("SELECT " + ClauSelect + " FROM " + NomeTabela + " WHERE " +
                    ClauWhere, MySqlconn);
                MySqlda.Fill(ds);
            }
            catch (MySql.Data.MySqlClient.MySqlException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados MySQL.\r\n" + Ex.Message + "\r\n Erro número: " +
                    Ex.Number.ToString());
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message);
                ds.Tables.Add();
            }

            return ds.Tables[0];
        }


        //=====================================
        //          METODOS PostgreSQL
        //=====================================
        public DataTable BD_PostgreSQL(string EndServidor, string NomeBD, string NomeTabela, string Login, string Senha,
            string Porta, string ClauSelect, string ClauWhere)
        {
            DataSet ds = new DataSet();

            try
            {
                string strConexao = "Server=" + EndServidor + ";Port=" + Porta + ";Database=" + NomeBD + ";User Id=" + Login +
                    ";Password=" + Senha;
                PgSqlConnection Postgreconn = new PgSqlConnection(strConexao);
                PgSqlDataAdapter Postgreda = new PgSqlDataAdapter("SELECT " + ClauSelect + " FROM " + NomeTabela + " WHERE " +
                    ClauWhere + ";", Postgreconn);
                Postgreda.Fill(ds);
            }
            catch (PgSqlException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados PostgreSQL.\r\n" + Ex.Message + "\r\n" +
                    Ex.DetailMessage + "\r\n" + Ex.ErrorCode.ToString());
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados PostgreSQL.\r\n" + Ex.Message);
                ds.Tables.Add();
            }

            return ds.Tables[0];
        }


        //=====================================
        //          METODOS SQL-SERVER
        //=====================================
        public DataTable DB_SQL()
        {
            DataSet ds = new DataSet();

            try
            {
                //string strConexao = @"Source=.\SQLEXPRESS;attaachDbFilenameServer=";
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados SQL Server.\r\n" + Ex.Message + "\r\n" + Ex.Source);
                ds.Tables.Add();
            }
            /*catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message);
                ds.Tables.Add();
            }*/
            return ds.Tables[0];
        }

        //=====================================
        //          METODOS ORACLE
        //=====================================
        public DataTable DB_ORACLE()
        {
            DataSet ds = new DataSet();

            try
            {
                string strConexao = "Data Source=MyOracleDB;User Id=myUsername;Password=myPassword;Integrated Security=no;";
                OracleConnection Oracleconn = new OracleConnection(strConexao);
                OracleDataAdapter Oracleda = new OracleDataAdapter("", Oracleconn);
                Oracleda.Fill(ds);

            }
            catch (OracleException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message + "\r\nNumber: " +
                    Ex.Number.ToString() + "\r\nErrorCode: " + Ex.ErrorCode.ToString());
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message);
                ds.Tables.Add();
            }
            return ds.Tables[0];
        }

        //=====================================
        //          METODOS ORACLE
        //=====================================

        /*public DataTable DB_EXCEL(string NomeArq)
        {
            DataSet ds = new DataSet();

            try
            {
                //montagem da string de conexao com parte dos dados obtidos na chamada do metodo Provider=Microsoft.Jet.OLEDB.4.0;
                //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\myFolder\myExcel2007file.xlsx; 
                //Extended Properties = "Excel 12.0 Xml;HDR=YES";
                string strConexao = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + NomeArq;

                if (senha != "")
                {
                    strConexao += ";Jet OLEDB:Database Password=" + senha;
                    //MessageBox.Show("Incluir aqui a configuracao da senha");
                }

                //chamada dos objetos de banco de dados
                OleDbConnection OleDBconn = new OleDbConnection(strConexao);

                string SintaxeWHERE = "";
                if (ClauWhere != "")
                {
                    SintaxeWHERE = " WHERE " + ClauWhere;
                }

                OleDbDataAdapter OleDBda = new OleDbDataAdapter("SELECT " + ClauSelect + " FROM " + NomeTabela + SintaxeWHERE, OleDBconn);
                OleDBda.Fill(ds);
            }
            catch (OleDbException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Access.\r\n" + Ex.Message + "\r\nErro número: " +
                    Ex.ErrorCode.ToString());
                ds.Tables.Add();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Oracle.\r\n" + Ex.Message);
                ds.Tables.Add();
            }

            return ds.Tables[0];
        }*/

        public void DB_Access2(string NomeArq, string NomeTabela, string senha, DataTable tab)
        {

            try
            {
                //montagem da string de conexao com parte dos dados obtidos na chamada do metodo Provider=Microsoft.Jet.OLEDB.4.0;
                string strConexao = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + NomeArq;

                if (senha != "")
                {
                    strConexao += ";Jet OLEDB:Database Password=" + senha;
                    //MessageBox.Show("Incluir aqui a configuracao da senha");
                }
                OleDbConnection OleDBconn = new OleDbConnection(strConexao);
                OleDbCommand OleDBcomm = new OleDbCommand();
                OleDBcomm.Connection = OleDBconn;

                string ColunaKey = "numSequ";
                string Tabela = "TabPedoclima";
                int colunaP = tab.Columns.IndexOf(ColunaKey);

                OleDBconn.Open();

                /*string comandoCriar = "CREATE TABLE TabPedoclima (numSequ integer, SoloClima text, " +
                    "PC_B_S_ALG text, PC_C_S_ALG text, PC_B_R_ALG text, PC_C_R_ALG text, PC_B_C_ALG text, PC_C_C_ALG text, " +
                    "PC_B_S_CAN text, PC_C_S_CAN text, PC_B_R_CAN text, PC_C_R_CAN text, PC_B_C_CAN text, PC_C_C_CAN text, " +
                    "PC_B_S_CAU text, PC_C_S_CAU text, PC_B_R_CAU text, PC_C_R_CAU text, PC_B_C_CAU text, PC_C_C_CAU text, " +
                    "PC_B_S_MAM text, PC_C_S_MAM text, PC_B_R_MAM text, PC_C_R_MAM text, PC_B_C_MAM text, PC_C_C_MAM text, " +
                    "PC_B_S_MAN text, PC_C_S_MAN text, PC_B_R_MAN text, PC_C_R_MAN text, PC_B_C_MAN text, PC_C_C_MAN text, " +
                    "PC_B_S_MIL text, PC_C_S_MIL text, PC_B_R_MIL text, PC_C_R_MIL text, PC_B_C_MIL text, PC_C_C_MIL text, " +
                    "PC_B_S_PHA text, PC_C_S_PHA text, PC_B_R_PHA text, PC_C_R_PHA text, PC_B_C_PHA text, PC_C_C_PHA text, " +
                    "PC_B_S_SOR text, PC_C_S_SOR text, PC_B_R_SOR text, PC_C_R_SOR text, PC_B_C_SOR text, PC_C_C_SOR text, " +
                    "CAN_B text, CAN_C text, MIL_B text, MIL_C text, SOR_B text, SOR_C text, CAU_B text, CAU_C text, " +
                    "PHA_B text, PHA_C text, MAM_B text, MAM_C text, MAN_B text, MAN_C text, ALG_B text, ALG_C text, " +
                    "CLI_PHA_S text, CLI_PHA_R text, CLI_PHA_C text, " +
                    "CLI_SOR_C text, CLI_SOR_R text, CLI_SOR_S text, " +
                    "CLI_ALG_C text, CLI_ALG_R text, CLI_ALG_S text, " +
                    "CLI_CAN_C text, CLI_CAN_S text, CLI_CAN_R text, " +
                    "CLI_CAU_C text, CLI_CAU_R text, CLI_CAU_S text, " +
                    "CLI_MAM_C text, CLI_MAM_R text, CLI_MAM_S text, " +
                    "CLI_MAN_C text, CLI_MAN_R text, CLI_MAN_S text, " +
                    "CLI_MIL_C text, CLI_MIL_R text, CLI_MIL_S text)";*/
                //OleDBcomm.CommandText = comandoCriar;
                //OleDBcomm.ExecuteNonQuery();

                for (int a = 0; a < tab.Rows.Count; a++)
                {
                    string celulaP = tab.Rows[a].ItemArray[colunaP].ToString();
                    string comandoLinha = "INSERT INTO " + Tabela + " (" + ColunaKey + ") VALUES (" + celulaP + ")";
                    OleDBcomm.CommandText = comandoLinha;
                    OleDBcomm.ExecuteNonQuery();

                    for (int b = 0; b < tab.Columns.Count; b++)
                    {
                        if (b != colunaP)
                        {
                            string NomeColuna = tab.Columns[b].ColumnName;
                            string ValorCell = tab.Rows[a].ItemArray[b].ToString();
                            //OleDBcomm.CommandText = "UPDATE TabPedoclima SET SoloClima = 'CXbe1.Clim_1694' WHERE OBJECTID = 4055";
                            string comando = "UPDATE " + Tabela + " SET " + NomeColuna + " = '" + ValorCell +
                                "' WHERE " + ColunaKey + " = " + celulaP;
                            OleDBcomm.CommandText = comando;
                            OleDBcomm.ExecuteNonQuery();
                        }
                    }
                }
                OleDBconn.Close();
                MessageBox.Show("Pronto! Veja lá.");



                /*chamada dos objetos de banco de dados
                OleDbConnection OleDBconn = new OleDbConnection(strConexao);
                OleDbCommand OleDBcomm = new OleDbCommand("INSERT INTO TabPedoclima (OBJECTID) VALUES (4055)", OleDBconn);

                OleDBconn.Open();
                OleDBcomm.ExecuteNonQuery();

                OleDBcomm.CommandText = "UPDATE TabPedoclima SET SoloClima = 'CXbe1.Clim_1694' WHERE OBJECTID = 4055";
                OleDBcomm.ExecuteNonQuery();
                OleDBcomm.CommandText = "UPDATE TabPedoclima SET PC_B_S_ALG = 'B1 - forte limitação de solo (S4) e, ou, de clima (C1)' WHERE OBJECTID = 4055";
                OleDBcomm.ExecuteNonQuery();
                OleDBconn.Close();*/

            }
            catch (OleDbException Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados Access.\r\n" + Ex.Message + "\r\nErro número: " +
                    Ex.ErrorCode.ToString());
               
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Não foi possível carregar o banco de dados.\r\n" + Ex.Message);
                
            }
        }



    }
}
