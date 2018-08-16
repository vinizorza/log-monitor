using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Text;

namespace VZLogMonitor {

    class Program {

        static string path;
        const string pathRegras = "regras.txt";
        const string pathDestinos = "dest.txt";

        static string[] regras;
        static string[] emails;

        static void Main(string[] args) {

            Console.WriteLine("CAMINHO DA PASTA DE LOG: ");
            path = Console.ReadLine();
            path = path.Replace("\\","/");

            regras = LerRegras(pathRegras);
            emails = LerEmails(pathDestinos);
            
            Watchfile();
        }

        public static void Watchfile() {

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.log";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Digite \'e\' para encerrar o programa.");
            while (Console.Read() != 'e') ;
        }


        private static void OnChanged(object source, FileSystemEventArgs e) {

            // Abrindo arquivo de log
            string lastLine = String.Empty;
            using (FileStream logFile = new FileStream(@path + "\\" + e.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                using (StreamReader sr = new StreamReader (logFile)) {
                    string line = sr.ReadLine();

                    // Pegando ultima linha do arquivo de log
                    while (line != null) {
                        lastLine = line;
                        line = sr.ReadLine();
                    }
                }
            }            

            // Verificando regras
            List<string> regrasEncontradas = VerificaRegras(lastLine);
            if (regrasEncontradas.Count() > 0) {

                EnviarNotificacao(emails, lastLine, regrasEncontradas, e.Name);

            }
        }

        private static List<string> VerificaRegras(string lastLine) {
            List<string> regrasEncontradas = new List<string>();
            foreach (string regra in regras) {
                if (lastLine.Contains(regra)) {
                    regrasEncontradas.Add(regra);
                }
            }
            return regrasEncontradas;
        }

        private static void EnviarNotificacao(string[] emails, string lastLine, List<string> regrasEncontradas, string nomeArquivo) {

            // Montando email
            string corpoEmail = "\n=== [LOG MONITOR REPORT] ===\n" +
                                "ARQUIVO: \t" + nomeArquivo + "\n" +
                                "LINHA: \t\t" + lastLine + "\n" +
                                "KEYWORDS DETECTADAS:\n";
            foreach (string regra in regrasEncontradas) {
                corpoEmail += "\t\t" + regra + "\n";
            }
            string assuntoEmail = "LOG MONITOR - KEYWORD DETECTADA";

            EnviarEmail(emails, assuntoEmail, corpoEmail);
        }

        private static string[] LerRegras(string pathRegras) {
            return File.ReadAllLines(@pathRegras);
        }

        private static string[] LerEmails(string pathDestinos) {
            return File.ReadAllLines(pathDestinos);
        }

        private static void EnviarEmail(string[] destinos, string assunto, string corpo) {

            Console.WriteLine("________________________________");
            Console.WriteLine(corpo);
            
            // Configurando servidor SMTP.            
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("email (gmail)", "senha");

            // Criando email
            MailMessage objeto_mail = new MailMessage();
            objeto_mail.From = new MailAddress("email (gmail)");

            Console.WriteLine("EMAILS ENVIADOS:\n");
            foreach (string email in destinos) {
                objeto_mail.To.Add(new MailAddress(email));
                Console.WriteLine("\t\t" + email);
            }            
            objeto_mail.Subject = assunto;
            objeto_mail.Body = corpo;
            objeto_mail.BodyEncoding = UTF8Encoding.UTF8;

            // Enviando
            client.Send(objeto_mail);

        }
    }
}
