using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Projeto_Classes.Classes;
using Projeto_Classes.Classes.Gerencial;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Xml;
using System.Threading.Tasks;
using Escuta_Teltonika;


namespace Escuta_Teltonika
{
    class Program
    {
        #region Variáveis
        
        private const int CODEC_FMXXX = 0x08;
        private const int ACC = 1;
        private const int DOOR = 2;
        private const int Analog = 4;
        private const int GSM = 5;
        private const int SPEED = 6;
        private const int VOLTAGE = 7;
        private const int GPSPOWER = 8;
        private const int TEMPERATURE = 9;
        private const int ODOMETER = 16;
        private const int STOP = 20;
        private const int TRIP = 28;
        private const int IMMOBILIZER = 29;
        private const int AUTHORIZED = 30;
        private const int GREEDRIVING = 31;
        private const int OVERSPEED = 33;

        private static ArrayList contas = new ArrayList();
        static Socket s;

        #endregion

        private static void Main()
        {            
            #region Contas HERE

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("END_POINT");

            XmlNodeList coluna = xDoc.GetElementsByTagName("coluna");
            XmlNodeList app_id = xDoc.GetElementsByTagName("app_id");
            XmlNodeList app_code = xDoc.GetElementsByTagName("app_code");
            XmlNodeList inicio = xDoc.GetElementsByTagName("inicio");
            XmlNodeList fim = xDoc.GetElementsByTagName("fim");

            for (int i = 0; i < coluna.Count; i++)
            {
                ArrayList itens = new ArrayList();
                itens.Add(coluna[i].InnerText);
                itens.Add(app_id[i].InnerText);
                itens.Add(app_code[i].InnerText);
                itens.Add(inicio[i].InnerText);
                itens.Add(fim[i].InnerText);
                contas.Add(itens);
            }

            #endregion

            #region Socket
            TcpListener socket = new TcpListener(IPAddress.Any, 7040);

            try
            {
                Console.WriteLine("Conectado !");
                socket.Start();

                while (true)
                {
                    s = socket.AcceptSocket();

                    Thread nova_thread = new Thread(TraduzindoMensagem);
                    nova_thread.Start();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                Thread tcpListenThread = new Thread(Main);
                tcpListenThread.Start();
                socket.Stop();
            }
            #endregion
        }

        public static void TraduzindoMensagem()
        {

            try
            {
                #region recebendo IMEI
                byte[] b = new byte[s.ReceiveBufferSize];
                int k = s.Receive(b);

                string imei = "";
                for (int i = 0; i < k; i++)
                {
                    imei += Convert.ToChar(b[i]);
                }
                imei = imei.Substring(2);
                #endregion

                #region enviando Confirmação
                byte[] msg = new byte[] { 0x01 };
                s.Send(msg);
                #endregion

                #region tratando retorno
                //getting the gps data after acknowledgement
                byte[] ack = new byte[s.ReceiveBufferSize];
                int y = Program.s.Receive(ack);
                

                //ESSA PARTE PRINTA HEXADECIMAL
                //var ss = BitConverter.ToString(ack, 0, y).Replace("-", string.Empty);
                //Console.WriteLine(ss);

                var lista = ParseData(ack, y);

                //int ultimo = lista.Count - 1;

                #region todos campos
                //var imei = ParseHeartBeatData(ack, y);
                //Console.WriteLine("IMEI: " + imei);
                //Console.WriteLine("La: " + lista[ultimo].La);
                //Console.WriteLine("Lo: " + lista[ultimo].Lo);
                //Console.WriteLine("Speed: " + lista[ultimo].Speed);
                //Console.WriteLine("Status: " + lista[ultimo].Status);
                //Console.WriteLine("Direction: " + lista[ultimo].Direction);
                //Console.WriteLine("StopFlag: " + lista[ultimo].StopFlag);
                //Console.WriteLine("IsStop: " + lista[ultimo].IsStop);
                //Console.WriteLine("Mileage: " + lista[ultimo].Mileage);
                //Console.WriteLine("Alarm: " + lista[ultimo].Alarm);
                //Console.WriteLine("Time: " + lista[ultimo].Time);

                //Console.WriteLine("Id: " + lista[ultimo].Id);
                //Console.WriteLine("GpsTime: " + lista[ultimo].GpsTime);
                //Console.WriteLine("AlarmHandle: " + lista[ultimo].AlarmHandle);
                //Console.WriteLine("IsPointMsg: " + lista[ultimo].IsPointMsg);
                //Console.WriteLine("Pointed: " + lista[ultimo].Pointed);
                //Console.WriteLine("IsGetSetMsg: " + lista[ultimo].IsGetSetMsg);
                //Console.WriteLine("SettingStr: " + lista[ultimo].SettingStr);
                //Console.WriteLine("Temperature: " + lista[ultimo].Temperature);
                //Console.WriteLine("Fuel: " + lista[ultimo].Fuel);
                //Console.WriteLine("Input1: " + lista[ultimo].Input1);
                //Console.WriteLine("Input2: " + lista[ultimo].Input2);
                //Console.WriteLine("Input3: " + lista[ultimo].Input3);
                //Console.WriteLine("Input4: " + lista[ultimo].Input4);
                //Console.WriteLine("Input5: " + lista[ultimo].Input5);
                //Console.WriteLine("Output1: " + lista[ultimo].Output1);
                //Console.WriteLine("Output2: " + lista[ultimo].Output2);
                //Console.WriteLine("Output3: " + lista[ultimo].Output3);
                //Console.WriteLine("Output4: " + lista[ultimo].Output4);
                //Console.WriteLine("Output5: " + lista[ultimo].Output5);
                //Console.WriteLine("MNO: " + lista[ultimo].MNO);
                //Console.WriteLine("CarId: " + lista[ultimo].CarId);
                //Console.WriteLine("AlarmStatus: " + lista[ultimo].AlarmStatus);
                //Console.WriteLine("Prioridade: " + lista[ultimo].Prioridade);
                //Console.WriteLine("Altitude: " + lista[ultimo].Altitude);
                //Console.WriteLine("Ignicao: " + lista[ultimo].Ignicao);
                //Console.WriteLine("DataRastreador: " + lista[ultimo].DataRastreador);
                #endregion

                //for (int ultimo = 0; ultimo < lista.Count; ultimo++)
                //{
                int ultimo = 0;
                string mensagem = "FMB920;" +
                                    lista[ultimo].Id + ";" +
                                    imei + ";" +
                                    lista[ultimo].GpsTime + ";" +
                                    lista[ultimo].DataRastreador.ToString("yyyyMMdd;HH:mm:ss") + ";" +
                                    lista[ultimo].La.ToString().Replace(',', '.') + ";" +
                                    lista[ultimo].Lo.ToString().Replace(',', '.') + ";" +
                                    lista[ultimo].Ignicao + ";" +
                                    lista[ultimo].Speed + ";" +
                                    lista[ultimo].Direction + ";" +
                                    lista[ultimo].Prioridade + ";" +
                                    lista[ultimo].Altitude + ";" +
                                    lista[ultimo].Mileage;

                Console.WriteLine(mensagem);
                //}
                Console.WriteLine("");

                Gravar(lista, mensagem, imei);

                
                //myList.Stop();
                #endregion
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
            s.Close();
        }

        public static List<Position> ParseData(Byte[] byteBuffer, int size)
        {

            List<Position> result = new List<Position>();
            result = ParsePositions(byteBuffer, size);

            return result;
        }

        private static List<Position> ParsePositions(Byte[] byteBuffer, int linesNB)
        {
            int index = 0;
            index += 7;
            uint dataSize = byteBuffer[index];

            index++;
            uint codecID = byteBuffer[index];

            if (codecID == CODEC_FMXXX)
            {
                index++;
                uint NumberOfData = byteBuffer[index];

                //Console.WriteLine("{0} {1} {2} ", codecID, NumberOfData, dataSize);

                List<Position> result = new List<Position>();

                index++;
                for (int i = 0; i < NumberOfData; i++)
                {
                    Position position = new Position();

                    //timestamp
                    position.GpsTime = Convert.ToString(Int64.Parse(Parsebytes(byteBuffer, index, 8), System.Globalization.NumberStyles.HexNumber));
                    position.GpsTime = position.GpsTime.Substring(0, position.GpsTime.Length - 3);
                    //datetime
                    position.DataRastreador = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(position.GpsTime));
                    index += 8;

                    position.Time = DateTime.Now;

                    position.Prioridade = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                    index++;

                    position.Lo = Int32.Parse(Parsebytes(byteBuffer, index, 4), System.Globalization.NumberStyles.HexNumber) / 10000000.0;
                    index += 4;

                    position.La = Int32.Parse(Parsebytes(byteBuffer, index, 4), System.Globalization.NumberStyles.HexNumber) / 10000000.0;
                    index += 4;

                    position.Altitude = Int16.Parse(Parsebytes(byteBuffer, index, 2), System.Globalization.NumberStyles.HexNumber);
                    index += 2;

                    var dir = Int16.Parse(Parsebytes(byteBuffer, index, 2), System.Globalization.NumberStyles.HexNumber);

                    if (dir < 90) position.Direction = 1; //NOROESTE
                    else if (dir == 90) position.Direction = 2; //NORTE
                    else if (dir < 180) position.Direction = 3; //NORDESTE
                    else if (dir == 180) position.Direction = 4; //LESTE
                    else if (dir < 270) position.Direction = 5; //SUDESTE
                    else if (dir == 270) position.Direction = 6; //SUL
                    else if (dir > 270) position.Direction = 7; //SUDOESTE
                    else if (dir == 0) position.Direction = 8; //OESTE
                    index += 2;

                    var Satellite = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                    index++;

                    if (Satellite >= 3)
                        position.Status = "A";
                    else
                        position.Status = "L";

                    position.Speed = Int16.Parse(Parsebytes(byteBuffer, index, 2), System.Globalization.NumberStyles.HexNumber);
                    index += 2;

                    int ioEvent = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                    index++;
                    int ioCount = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                    index++;

                    #region read 1 byte
                    {
                        int cnt = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                        index++;
                        for (int j = 0; j < cnt; j++)
                        {
                            int id = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                            index++;
                            //Add output status
                            switch (id)
                            {
                                case ACC:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        position.Status += value == 1 ? ",ACC off" : ",ACC on";
                                        position.Ignicao = value;
                                        index++;
                                        break;
                                    }
                                case DOOR:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        position.Status += value == 1 ? ",door close" : ",door open";
                                        index++;
                                        break;
                                    }
                                case GSM:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        position.Status += string.Format(",GSM {0}", value);
                                        index++;
                                        break;
                                    }
                                case STOP:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        position.StopFlag = value == 1;
                                        position.IsStop = value == 1;

                                        index++;
                                        break;
                                    }
                                case IMMOBILIZER:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        position.Alarm = value == 0 ? "Activate Anti-carjacking success" : "Emergency release success";
                                        index++;
                                        break;
                                    }
                                case GREEDRIVING:
                                    {
                                        var value = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                                        switch (value)
                                        {
                                            case 1:
                                                {
                                                    position.Alarm = "acelaração intença!!";
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    position.Alarm = "freada brusca!!";
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    position.Alarm = "giro apertado!!";
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        index++;
                                        break;
                                    }
                                default:
                                    {
                                        index++;
                                        break;
                                    }
                            }

                        }
                    }
                    #endregion

                    #region read 2 byte
                    {
                        int cnt = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                        index++;
                        for (int j = 0; j < cnt; j++)
                        {
                            int id = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                            index++;



                            switch (id)
                            {
                                case Analog:
                                    {
                                        var value = Int16.Parse(Parsebytes(byteBuffer, index, 2), System.Globalization.NumberStyles.HexNumber);
                                        if (value < 12)
                                            position.Alarm += string.Format("Low voltage", value);
                                        index += 2;
                                        break;
                                    }
                                case SPEED:
                                    {
                                        var value = Int16.Parse(Parsebytes(byteBuffer, index, 2), System.Globalization.NumberStyles.HexNumber);
                                        position.Alarm += string.Format("Speed", value);
                                        index += 2;
                                        break;
                                    }
                                default:
                                    {
                                        index += 2;
                                        break;
                                    }

                            }
                        }
                    }
                    #endregion

                    #region read 4 byte
                    {
                        int cnt = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                        index++;
                        for (int j = 0; j < cnt; j++)
                        {
                            int id = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                            index++;

                            switch (id)
                            {
                                case TEMPERATURE:
                                    {
                                        var value = Int32.Parse(Parsebytes(byteBuffer, index, 4), System.Globalization.NumberStyles.HexNumber);
                                        position.Alarm += string.Format("Temperature {0}", value);
                                        index += 4;
                                        break;
                                    }
                                case ODOMETER:
                                    {
                                        var value = Int32.Parse(Parsebytes(byteBuffer, index, 4), System.Globalization.NumberStyles.HexNumber);
                                        position.Mileage = value;
                                        index += 4;
                                        break;
                                    }
                                default:
                                    {
                                        index += 4;
                                        break;
                                    }

                            }


                        }
                    }
                    #endregion

                    #region read 8 byte
                    {
                        int cnt = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                        index++;
                        for (int j = 0; j < cnt; j++)
                        {
                            int id = byte.Parse(Parsebytes(byteBuffer, index, 1), System.Globalization.NumberStyles.HexNumber);
                            index++;

                            var io = Int64.Parse(Parsebytes(byteBuffer, index, 8), System.Globalization.NumberStyles.HexNumber);
                            position.Status += string.Format(",{0} {1}", id, io);
                            index += 8;
                        }
                    }
                    #endregion

                    result.Add(position);
                    //Console.WriteLine(position.ToString());
                }

                return result;
            }
            return null;
        }

        private static string Parsebytes(Byte[] byteBuffer, int index, int Size)
        {
            return BitConverter.ToString(byteBuffer, index, Size).Replace("-", string.Empty);
        }

        public static void Gravar(List<Position> objeto, string mensagem, string id)
        {
            try
            {
                var m = new Mensagens();
                var r = new Rastreador();
                r.PorId(id);

                m.Data_Rastreador = objeto[0].DataRastreador.ToString("yyyyMMdd HH:mm:ss");
                m.Data_Gps = objeto[0].DataRastreador.ToString("yyyy-MM-dd HH:mm:ss");
                m.Data_Recebida = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                m.ID_Rastreador = id;
                m.Mensagem = mensagem;
                m.Ras_codigo = r.Codigo;
                m.Tipo_Mensagem = "STT";
                m.Latitude = objeto[0].La.ToString().Replace(',', '.');
                m.Longitude = "-0" + objeto[0].Lo.ToString().Substring(1).Replace(',', '.');
                m.Tipo_Alerta = "";
                m.Velocidade = objeto[0].Speed.ToString();
                m.Vei_codigo = r.Vei_codigo != 0 ? r.Vei_codigo : 0;
                m.Ignicao = Convert.ToBoolean(objeto[0].Ignicao);
                m.Hodometro = objeto[0].Mileage.ToString();
                m.Bloqueio = false;
                m.Sirene = false;
                m.Tensao = "0";
                m.Horimetro = 0;
                m.CodAlerta = 0;
                m.Endereco = Util.BuscarEndereco(m.Latitude, m.Longitude, contas);

                #region Gravar
                if (m.Gravar())
                {
                    m.Tipo_Mensagem = "EMG";

                    if (r.veiculo != null)
                    {
                        //Verifica Area de Risco/Cerca
                        Mensagens.EventoAreaCerca(m);

                        //Evento Por E-mail
                        var corpoEmail = m.Tipo_Alerta + "<br /> Endereço: " + m.Endereco;
                        Mensagens.EventoPorEmail(m.Vei_codigo, m.CodAlerta, corpoEmail);
                    }

                    #region Tensão

                    /*string voltagem = r.veiculo.voltagem.ToString().Replace(",00", "");
                            /*voltagem = voltagem.Length == 3 ? "0" + voltagem : voltagem;
                            string voltagem_correta = voltagem.Substring(0, 2) + "." + voltagem.Substring(2, 2);
                            decimal voltagem_cadastro = Convert.ToDecimal(voltagem_correta);
                            */
                    /*string total = (Convert.ToDecimal(voltagem_correta) + 2).ToString();

                    StreamWriter txt = new StreamWriter("tensao.txt", true);
                    txt.WriteLine("Tensão: " + total);
                    txt.Close();*/

                    //var tet = r.rastreador_evento.Where(x => x.te_codigo.Equals(26)).ToList().ForEach(x => { x.te_codigo });


                    /*var a = r.rastreador_evento.Select(tet => tet.te_codigo = 26);


                    Console.WriteLine("----------------------------");
                    Console.WriteLine(a.ToString());
                    Console.WriteLine("----------------------------");

                    var gravar_evento = true;
                    r.rastreador_evento.ForEach(x => { 
                        if(x.te_codigo == 26){
                            gravar_evento = false;
                        }
                    });

                    if (gravar_evento)
                    {*/
                    /*if ((Convert.ToDecimal(voltagem_correta) + 200) < Convert.ToDecimal(m.Tensao))
                    {
                        m.Tipo_Mensagem = "EVT";
                        m.Tipo_Alerta = "Tensão Acima do Ideal";
                        m.CodAlerta = 26;
                        m.GravarEvento();
                    }*/
                    //}

                    /*StreamWriter txt = new StreamWriter("teste_bloqueio_evento.txt", true);
                    txt.WriteLine(tet);
                    txt.Close();*/

                    /*if (!r.rastreador_evento.Where(x => x.te_codigo.Equals(26)))
                    {*/

                    /*decimal porcentagem_alta = voltagem_cadastro + (voltagem_cadastro * Convert.ToDecimal(0.25));
                    /*decimal porcentagem_baixa = voltagem_cadastro - (voltagem_cadastro * Convert.ToDecimal(0.20)); ;

                    if (porcentagem_alta < Convert.ToDecimal(m.Tensao))
                    {
                        m.Tipo_Mensagem = "EVT";
                        m.Tipo_Alerta = "Tensão Acima do Ideal";
                        m.CodAlerta = 26;
                        m.GravarEvento();
                    }*/
                    /*}

                     if (!r.rastreador_evento.Where(x => x.te_codigo.Equals(25)))
                     {
                         StreamWriter txt = new StreamWriter("teste_bloqueio_evento.txt", true);
                         txt.WriteLine("NICE");
                         txt.Close();
                     */
                    /*if (porcentagem_baixa > Convert.ToDecimal(m.Tensao))
                    {
                        m.Tipo_Mensagem = "EVT";
                        m.Tipo_Alerta = "Tensão Abaixo do Ideal";
                        m.CodAlerta = 25;
                        m.GravarEvento();
                    }*/
                    /*}
                    else
                    {
                        StreamWriter txt = new StreamWriter("teste_bloqueio_evento.txt", true);
                        txt.WriteLine("NOT_NICE");
                        txt.Close();
                    }*/


                    #endregion

                    #region Velocidade
                    if (r.Vei_codigo != 0)
                    {
                        var veiculo = Veiculo.BuscarVeiculoVelocidade(m.Vei_codigo);
                        var velocidade_nova = Convert.ToDecimal(veiculo.vei_velocidade);
                        if (velocidade_nova < Convert.ToDecimal(m.Velocidade) && velocidade_nova > 0)
                        {
                            m.Tipo_Mensagem = "EVT";
                            m.Tipo_Alerta = "Veículo Ultrapassou a Velocidade";
                            m.CodAlerta = 23;
                            m.GravarEvento();

                            //Evento Por E-mail
                            var corpoEmail = m.Tipo_Alerta + "<br /> Velocidade: " + m.Velocidade + "<br /> Endereço: " + m.Endereco;
                            Mensagens.EventoPorEmail(m.Vei_codigo, m.CodAlerta, corpoEmail);
                        }
                    }
                    #endregion

                }
                #endregion
            }
            catch (Exception e)
            {
                //LogException.GravarException("Erro: " + ex.Message.ToString() + " - Mensagem: " + (ex.InnerException != null ? ex.InnerException.ToString() : " Valor nulo na mensagem "), 12, "Escuta Suntech Novo - Método " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                StreamWriter txt = new StreamWriter("erros_01.txt", true);
                txt.WriteLine("ERRO: " + e.Message.ToString());
                txt.Close();
            }
        }



        #region Métodos Não Usados
        private static string parseIMEI(Byte[] byteBuffer, int size)
        {
            int index = 0;
            var result = Parsebytes(byteBuffer, index, 2);
            return result;
        }

        private static bool checkIMEI(string data)
        {
            //Console.WriteLine(data.Length);
            if (data.Length == 15)
                return true;

            return false;
        }

        public static Byte[] DealingWithHeartBeat(string data)
        {

            Byte[] result = { 1 };
            if (checkIMEI(data))
            {
                return result;
            }
            return null;
        }

        public static string ParseHeartBeatData(Byte[] byteBuffer, int size)
        {
            var IMEI = parseIMEI(byteBuffer, size);
            if (checkIMEI(IMEI))
            {
                return IMEI;
            }
            else
            {
                int index = 0;
                index += 7;
                uint dataSize = byteBuffer[index];

                index++;
                uint codecID = byteBuffer[index];

                if (codecID == CODEC_FMXXX)
                {
                    index++;
                    uint NumberOfData = byteBuffer[index];

                    return NumberOfData.ToString();
                }

            }
            return string.Empty;
        }
        #endregion
    }
}
