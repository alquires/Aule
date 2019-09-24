using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Aule
{
    class Enigma
    {
        public void TryplaCripto(string sInputFilename, string sOutputFilename)
        {
            try
            {
                sOutputFilename = sOutputFilename.Replace("\"", "_");

                System.IO.FileStream fsInput = new System.IO.FileStream(sInputFilename,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read);

                System.IO.FileStream fsEncrypted = new System.IO.FileStream(sOutputFilename,
                                System.IO.FileMode.Create,
                                System.IO.FileAccess.Write);

                //criptografia segundo tripleDES
                TripleDESCryptoServiceProvider triDes = new TripleDESCryptoServiceProvider();


                triDes.Key = ASCIIEncoding.ASCII.GetBytes("**********************");
                triDes.IV = ASCIIEncoding.ASCII.GetBytes("**********");

                ICryptoTransform desencrypt = triDes.CreateEncryptor();
                CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

                byte[] bytearrayinput = new byte[fsInput.Length - 1];
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);

                cryptostream.Close();
                fsInput.Close();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        public void TryplaDescripto(string sInputFilename, string sOutputFilename)
        {
            try
            {
                TripleDESCryptoServiceProvider TriDES = new TripleDESCryptoServiceProvider();

                TriDES.Key = ASCIIEncoding.ASCII.GetBytes("*************************");
                TriDES.IV = ASCIIEncoding.ASCII.GetBytes("********");

                //Create a file stream to read the encrypted file back.
                System.IO.FileStream fsread = new System.IO.FileStream(sInputFilename,
                                               System.IO.FileMode.Open,
                                               System.IO.FileAccess.Read);
                //Create a DES decryptor from the DES instance.
                ICryptoTransform desdecrypt = TriDES.CreateDecryptor();
                //Create crypto stream set to read and do a 
                //DES decryption transform on incoming bytes.
                CryptoStream cryptostreamDecr = new CryptoStream(fsread,
                                                             desdecrypt,
                                                             CryptoStreamMode.Read);
                //Print the contents of the decrypted file.
                System.IO.StreamWriter fsDecrypted = new System.IO.StreamWriter(sOutputFilename);
                fsDecrypted.Write(new System.IO.StreamReader(cryptostreamDecr).ReadToEnd());

                fsDecrypted.Close();
                cryptostreamDecr.Close();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
    }
}
